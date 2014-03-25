#if !SILVERLIGHT
using System.Drawing;
using System.Linq;
using NLite.Globalization;

namespace NLite.Globalization.Internal
{
    class IconResourceManager:ResourceManager<Icon>
    {
        const string stringResources = "IconResources";
        public IconResourceManager(string resourceDirectory) : base(resourceDirectory) { }

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