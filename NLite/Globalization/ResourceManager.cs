using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Resources;
using NLite.Reflection;

namespace NLite.Globalization
{
    /// <summary>
    /// 资源管理器
    /// </summary>
    /// <typeparam name="TResource"></typeparam>
     #if !SILVERLIGHT
    [Serializable]
    #endif
    public abstract class ResourceManager<TResource> : BooleanDisposable, IResourceLocator<TResource>, IResourceRegistry
    {
        string resourceDirectory;

        /// <summary>
        /// 资源集合
        /// </summary>
        protected Dictionary<string, TResource> Resources { get; private set; }
        /// <summary>
        /// 得到缺省资源名称
        /// </summary>
        protected abstract string DefaultResourceName { get; }

        /// <summary>
        /// 文件资源列表
        /// </summary>
        protected HashSet<string> FileResources { get; private set; }

        /// <summary>
        /// 程序集资源列表
        /// </summary>
        protected HashSet<AssemblyResource> AssemblyResources { get; private set; }

        /// <summary>
        /// 程序集资源
        /// </summary>
        protected struct AssemblyResource : IEqualityComparer<AssemblyResource>
        {
            public string BaseResourceName;
            public Assembly Assembly;



            #region IEqualityComparer<AssemblyResource> Members

            public bool Equals(AssemblyResource x, AssemblyResource y)
            {
                return x.Assembly == y.Assembly && string.Equals(x.BaseResourceName, y.BaseResourceName, StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode(AssemblyResource obj)
            {
                throw new NotImplementedException();
            }

            #endregion
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceDirectory"></param>
        protected ResourceManager(string resourceDirectory)
        {
            if (resourceDirectory == null)
                throw new ArgumentNullException("resourceDirectory");

            this.resourceDirectory = resourceDirectory;

            Resources = new Dictionary<string, TResource>(StringComparer.OrdinalIgnoreCase);
            FileResources = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            AssemblyResources = new HashSet<AssemblyResource>();

            LoadLocalResources();
        }

        private Stream PopulateResourceStream(string baseResourceName, Assembly assembly, bool isStringResourceType)
        {
            var names = assembly.GetManifestResourceNames();
            var index = Array.FindIndex<string>(names, (item) => item.Equals(baseResourceName, StringComparison.OrdinalIgnoreCase));

            var resurceName = baseResourceName;

            if (index == -1)
                resurceName = baseResourceName + "_" + LanguageManager.Instance.Language + ".resources";
            index = Array.FindIndex<string>(names, (item) => item.Equals(resurceName, StringComparison.OrdinalIgnoreCase));


            if (index == -1)
                resurceName = baseResourceName + "." + DefaultResourceName + "_" + LanguageManager.Instance.Language + ".resources";
            index = Array.FindIndex<string>(names, (item) => item.Equals(resurceName, StringComparison.OrdinalIgnoreCase));

            if (index == -1)
                return null;

            var res = new AssemblyResource { BaseResourceName = baseResourceName, Assembly = assembly };
            AssemblyResources.Add(res);
            return assembly.GetManifestResourceStream(names[index]);

        }

        /// <summary>
        /// 注册文件资源
        /// </summary>
        /// <param name="fileName"></param>
        public void Register(string fileName)
        {
            var map = Load<TResource>(fileName);
            if (map != null && map.Count > 0)
            {
                FileResources.Add(fileName);
                foreach (var pair in map)
                    if (!Resources.ContainsKey(pair.Key))
                        Resources[pair.Key] = pair.Value;
            }
        }
        /// <summary>
        /// 注册流资源
        /// </summary>
        /// <param name="fileName"></param>
        public void Register(Stream stream)
        {
            var map = Load<TResource>(stream);
            if (map != null && map.Count > 0)
                foreach (var pair in map)
                    if (!Resources.ContainsKey(pair.Key))
                        Resources[pair.Key] = pair.Value;
        }

        /// <summary>
        /// 注册Assembly资源
        /// </summary>
        /// <param name="baseResourceName"></param>
        /// <param name="assembly"></param>
        public void Register(string baseResourceName, Assembly assembly)
        {
            Register(PopulateResourceStream(baseResourceName, assembly, true));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <param name="asm"></param>
        public void Register(IEnumerable<IResourceItem> items, Assembly asm)
        {
            if (items == null) return;
            foreach (var item in items)
                Register(item, asm);
        }

        /// <summary>
        /// 根据ResourceItem 决定是注册文件资源或者程序集资源
        /// </summary>
        /// <param name="item"></param>
        /// <param name="asm"></param>
        public void Register(IResourceItem item, Assembly asm)
        {
            if (item == null) return;

            if (!string.IsNullOrEmpty(item.AssemblyFile))
                Register(item.BaseResourceName, AssemblyLoader.Load(item.AssemblyFile));
            else if (!string.IsNullOrEmpty(item.ResourceFile))
                Register(item.ResourceFile);
            else
                Register(item.BaseResourceName, asm);
        }

       /// <summary>
       /// 刷新资源
       /// </summary>
        public void RefreshResource()
        {
            LoadLocalResources();
            LoadResources();
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void ClearResources()
        {
            Resources.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void LoadLocalResources()
        {
            ClearResources();

            var language = LanguageManager.Instance.Language;
            

            Register(resourceDirectory + Path.DirectorySeparatorChar + DefaultResourceName + "_" + language + ".Resources");
        }

        private void LoadResources()
        {
            var tmpFileResources = new HashSet<string>(FileResources);
            FileResources.Clear();
            foreach (var item in tmpFileResources)
                Register(item);

            var tmpAssemblyResources = new HashSet<AssemblyResource>(AssemblyResources);
            AssemblyResources.Clear();
            foreach (var item in tmpAssemblyResources)
                Register(item.BaseResourceName, item.Assembly);
        }



        static Dictionary<string, TValue> Load<TValue>(string fileName)
        {
            if (!File.Exists(fileName))
                fileName = fileName + "_" + LanguageManager.Instance.Language + ".Resources";

            if (File.Exists(fileName))
                using (var stream = new FileStream(fileName, FileMode.Open))
                    return Load<TValue>(stream);
            return null;
        }

        static Dictionary<string, TValue> Load<TValue>(Stream stream)
        {
            var resources = new Dictionary<string, TValue>();
            if (stream != null && stream.CanRead)
                using (ResourceReader rr = new ResourceReader(stream))
                    foreach (DictionaryEntry entry in rr)
                        resources[entry.Key as string] = (TValue)entry.Value;
            return resources;
        }


        /// <summary>
        /// 通过资源名称得到指定的资源对象
        /// </summary>
        /// <param name="name">资源名称</param>
        /// <returns></returns>
        public virtual TResource Get(string name)
        {
            if (Resources != null && Resources.ContainsKey(name))
                return Resources[name];
            return default(TResource);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            ClearResources();
        }
    }
}
