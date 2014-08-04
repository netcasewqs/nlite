using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using NLite.Collections;
using NLite.Internal;
using NLite.Mini;
using NLite.Reflection;
using NLite.Threading.Internal;
using System.IO;
using NLite.Cache;

namespace NLite
{
    /// <summary>
    /// NLite 环境信息
    /// </summary>
    public static partial class NLiteEnvironment
    {
        private static Lazy<bool> _isMono = new Lazy<bool>(() => Type.GetType("Mono.Runtime") != null);
        /// <summary>
        /// 得到当前操作系统的平台（Win32、Win64或其他） 
        /// </summary>
        public static string Platform { get; private set; }

        /// <summary>
        /// 得到当前.Net SDK路径
        /// </summary>
        public static string SDK_Path { get; private set; }

        /// <summary>
        /// 得到当前程序的物理路径
        /// </summary>
        public static string ApplicationPhysicalPath { get; private set; }

        /// <summary>
        /// 得到或设置当前应用程序的路径
        /// </summary>
        public static string ApplicationPath { get; set; }

        /// <summary>
        /// 得到当前程序的数据文件路径夹
        /// </summary>
        public static string App_Data { get; private set; }

        /// <summary>
        /// 得到当前程序的Local资源文件夹路径
        /// </summary>
        public static string App_LocalResources { get; private set; }

        /// <summary>
        /// 得到当前程序的产品名称
        /// </summary>
        public static string ProductName { get; private set; }

        /// <summary>
        /// 得到一个值用来指示当前运行环境是否是Web环境
        /// </summary>
        public static bool IsWeb
        {
            get
            {
                return HttpContextWrapper.RealContext != null;
            }
        }

       
        /// <summary>
        /// 得到一个值用来指示是否是Mono环境
        /// </summary>
        public static bool IsMono
        {
            get
            {
                return _isMono.Value;
            }
        }
        /// <summary>
        /// 得到或设置DI 容器的类型
        /// </summary>
        public static string ContainerType { get; set; }

       
        internal static string PropertiesFile;

        static NLiteEnvironment()
        {
            switch (IntPtr.Size)
            {
                case 4: Platform = "Win32"; break;
                case 8: Platform = "Win64"; break;
                default: Platform = "Other"; break;
            }



            var fullPath = typeof(object).Assembly.Location;
            var fileName = System.IO.Path.GetFileName(fullPath);
            SDK_Path = fullPath.Replace(fileName, "");

            var asm = Assembly.GetEntryAssembly();
            if (asm != null)
            {
                AssemblyProductAttribute attr = Assembly.GetEntryAssembly().GetAttribute<AssemblyProductAttribute>(false);
                if (attr != null)
                    ProductName = attr.Product;
            }

            ApplicationPhysicalPath = AppDomain.CurrentDomain.BaseDirectory;
           
            var WebAssembly = (from a in AppDomain.CurrentDomain.GetAssemblies()
                               where a.GetName().Name.Equals("System.Web")
                               select a).FirstOrDefault();

            if (WebAssembly != null)
            {
                HttpContextWrapper.HttpContextType = WebAssembly.GetType("System.Web.HttpContext");
                Trace.Assert(HttpContextWrapper.HttpContextType != null);
          

                HttpContextWrapper.CurrentProperty = HttpContextWrapper.HttpContextType.GetProperty("Current", BindingFlags.Public | BindingFlags.Static).GetGetMethod().GetFunc();
                Trace.Assert(HttpContextWrapper.CurrentProperty != null);

                HttpContextWrapper.ItemsProperty = HttpContextWrapper.HttpContextType.GetProperty("Items").GetGetMethod().GetFunc();
                Trace.Assert(HttpContextWrapper.ItemsProperty != null);

                var applicationProperty = HttpContextWrapper.HttpContextType.GetProperty("Application").GetGetMethod();
                Trace.Assert(applicationProperty != null);
                HttpContextWrapper.ApplicationProperty = applicationProperty.GetFunc();
                HttpContextWrapper.ApplicationType = applicationProperty.ReturnType;

                var cacheProperty = HttpContextWrapper.HttpContextType.GetProperty("Cache").GetGetMethod();
                Trace.Assert(cacheProperty != null);
                HttpContextWrapper.CacheType = cacheProperty.ReturnType;
                HttpContextWrapper.CacheProperty = cacheProperty.GetFunc();

                var sessionProperty = HttpContextWrapper.HttpContextType.GetProperty("Session").GetGetMethod();
                Trace.Assert(sessionProperty != null);
                HttpContextWrapper.SessionType = sessionProperty.ReturnType;
                HttpContextWrapper.SessionProperty = sessionProperty.GetFunc();
               


                var ctx = HttpContextWrapper.RealContext;
                if (ctx != null)
                {
                    try
                    {
                        var uri = ctx.GetProperty("Request").GetProperty<Uri>("Url");
                        if (uri.Port != 80)
                            ApplicationPath = string.Format("http://{0}:{1}", uri.Host, uri.Port);
                        else
                            ApplicationPath = string.Format("http://{0}", uri.Host);
                    }
                    catch
                    {
                    }
                }
                else
                    ApplicationPath = ApplicationPhysicalPath;
            }

            App_Data = ApplicationPhysicalPath + @"\App_Data\";
            if (!Directory.Exists(App_Data))
                Directory.CreateDirectory(App_Data);
            PropertiesFile = App_Data + @"NLite.Properties.xml";
            App_LocalResources = App_Data + @"\App_LocalResources\";

            //Refresh();
        }

        /// <summary>
        /// 重置DI容器
        /// </summary>
        public static void Refresh()
        {
            if (ServiceLocator.Current != null)
                ServiceLocator.Current.Dispose();

            IKernel kernel = null;
            if (string.IsNullOrEmpty(ContainerType))
                kernel = new Kernel();
            else
            {
                var classLoader = new SimpleClassLoader();
                var type = classLoader.Load(ContainerType);
                if (type != null)
                    kernel = (IKernel)Activator.CreateInstance(type);
            }

            ServiceLocator.Current = kernel;
            ServiceRegistry.Current = kernel;
        }

        /// <summary>
        /// 通过资源名称得到对应的资源值
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        public static string StringResource(this string resourceName)
        {
            return StringFormatter.Format("${res:" + resourceName + "}");
        }

       

        static LocalState localState = new LocalState();
        static LocalCacheState localCache = new LocalCacheState();

        /// <summary>
        /// 得到ApplicationState状态对象
        /// </summary>
        public static IApplication Application
        {
            get
            {
                return IsWeb ? HttpContextWrapper.ApplicationState : localState;
            }
        }

        /// <summary>
        /// 得到Session状态对象
        /// </summary>
        public static ISession Session
        {
            get { return IsWeb ? HttpContextWrapper.SessionState : localState; }
        }

        /// <summary>
        /// 得到Cache状态对象
        /// </summary>
        public static ICache Cache
        {
            get { return IsWeb ? HttpContextWrapper.CacheState : localCache; }
        }
    }

   
}
