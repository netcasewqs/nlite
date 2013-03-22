using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite.Domain.Mapping
{
    [Serializable]
    public class ContentResultAttribute : NavigationResultAttribute, IContentResult
    {
        string IContentResult.Content { get { return null; } }
        public string ContentType { get; set; }
        public Encoding ContentEncoding { get; set; }
        public ContentResultAttribute() : base(ActionResultTypes.Content) { }
    }
}
