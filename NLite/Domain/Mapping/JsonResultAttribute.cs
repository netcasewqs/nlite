using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite.Domain.Mapping
{
    [Serializable]
    public class JsonResultAttribute : NavigationResultAttribute, IJsonResult
    {
        object IJsonResult.Data { get { return null; } }
        public string ContentType { get; set; }
        public Encoding ContentEncoding { get; set; }
        public bool AllowGet { get; set; }
        public JsonResultAttribute() : base(ActionResultTypes.Json) { }
    }
}
