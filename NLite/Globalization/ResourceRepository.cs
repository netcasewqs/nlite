using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using NLite.Internal;
using NLite.Globalization.Internal;

namespace NLite.Globalization
{

    /// <summary>
    /// 
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public partial class ResourceRepository : IResourceRepository
    {
        
        #if !SILVERLIGHT
        [NonSerialized]
        private IDictionary<string, IResourceRegistry> resourceMgrs;
        [NonSerialized]
        private StringResourceManager StringResourceManager;
        [NonSerialized]
        private ImageResourceManager ImageResourceManager;
        [NonSerialized]
        private IconResourceManager IconResourceManager;
        #else
        private IDictionary<string, IResourceRegistry> resourceMgrs;
        private StringResourceManager StringResourceManager;
        #endif

        private static readonly object locker = new object();
        private static ManualResetEvent Task;

        /// <summary>
        /// 
        /// </summary>
        protected ResourceRepository()
        {
            Task = new ManualResetEvent(false);
            ThreadPool.QueueUserWorkItem((state) => Init());
        }

        private static void CheckInit()
        {
            lock (locker)
            {
                if (Task != null)
                {
                    Task.WaitOne();

                    Task.Close();
                    Task = null;
                }
            }
        }

        private void Init()
        {
            resourceMgrs = new Dictionary<string, IResourceRegistry>(StringComparer.OrdinalIgnoreCase);
            StringResourceManager = new StringResourceManager(NLiteEnvironment.App_LocalResources);
           
            #if !SILVERLIGHT
            ImageResourceManager = new ImageResourceManager(NLiteEnvironment.App_LocalResources);
            IconResourceManager = new IconResourceManager(NLiteEnvironment.App_LocalResources);
            resourceMgrs[Constance.ImageResources] = ImageResourceManager;
            resourceMgrs[Constance.IconResources] = IconResourceManager;            
            #endif
            resourceMgrs[Constance.StringResources] = StringResourceManager;
           

            Task.Set();
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly ResourceRepository Instance = new ResourceRepository();
        /// <summary>
        /// 
        /// </summary>
        public void RefreshResource()
        {
            if (resourceMgrs.Count > 0)
                foreach (var item in resourceMgrs)
                    if (item.Value != null)
                        item.Value.RefreshResource();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="resourceMgr"></param>
        public void Register(string key, IResourceRegistry resourceMgr)
        {
            CheckInit();
            if (!resourceMgrs.ContainsKey(key))
                resourceMgrs.Add(key, resourceMgr);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IResourceRegistry Get(string key)
        {
            CheckInit();
            if (resourceMgrs.ContainsKey(key))
                return resourceMgrs[key];
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public IResourceLocator<TResource> Get<TResource>(string key)
        {
            CheckInit();
            return Get(key) as IResourceLocator<TResource>;
        }
        /// <summary>
        /// 
        /// </summary>
        public static IResourceRegistry StringRegistry
        {
            get
            {
                CheckInit();
                return ResourceRepository.Instance.StringResourceManager;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public static IResourceLocator<string> Strings
        {
            get
            {
                CheckInit();
                return ResourceRepository.Instance.StringResourceManager;
            }
        }
        #if !SILVERLIGHT
        /// <summary>
        /// 
        /// </summary>
        public static IResourceRegistry ImageRegistry
        {
            get
            {
                CheckInit();
                return ResourceRepository.Instance.ImageResourceManager;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static IResourceRegistry IconRegistry
        {
            get
            {
                CheckInit();
                return ResourceRepository.Instance.IconResourceManager;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static IResourceLocator<System.Drawing.Image> Images
        {
            get
            {
                CheckInit();
                return ResourceRepository.Instance.ImageResourceManager;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static IResourceLocator<Icon> Icons
        {
            get
            {
                CheckInit();
                return ResourceRepository.Instance.IconResourceManager;
            }
        }

#endif
    }

}
