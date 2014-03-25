#if !SILVERLIGHT
using System;
using System.Drawing;
using System.Linq;
using NLite.Globalization;

namespace NLite.Globalization.Internal
{

    [Serializable]
    class ImageResourceManager : ResourceManager<Image>
    {
        const string stringResources = "ImageResources";
        public ImageResourceManager(string resourceDirectory) : base(resourceDirectory) { }

        protected override string DefaultResourceName
        {
            get { return stringResources; }
        }

        protected override void ClearResources()
        {
            var items = Resources.Values.ToArray();
            foreach (var item in items)
                if (item != null)
                    item.Dispose();
            Resources.Clear();
        }
    }

}
#endif