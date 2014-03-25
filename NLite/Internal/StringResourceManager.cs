using System;
using System.Reflection;
using NLite.Globalization;

namespace NLite.Globalization.Internal
{
    #if !SILVERLIGHT
    [Serializable]
    #endif
    class StringResourceManager : ResourceManager<string>, IResourceLocator<string>
    {
        const string stringResources = "StringResources";
        public StringResourceManager(string resourceDirectory)
            : base(resourceDirectory)
        {
            Register("NLite.Resources", Assembly.GetExecutingAssembly());
        }

        protected override string DefaultResourceName
        {
            get { return stringResources; }
        }
    }
}
