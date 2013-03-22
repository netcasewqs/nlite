using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite.Domain.Mapping
{
    [Serializable]
    public class ContentResult:IContentResult
    {
        public string Content { get; set; }
        public string ContentType { get; set; }
        public Encoding ContentEncoding { get; set; }

        string INavigationResult.Type
        {
            get { return ActionResultTypes.Content; }
        }
    }
}
