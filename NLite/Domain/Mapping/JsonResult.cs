using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite.Domain.Mapping
{
    [Serializable]
    public class JsonResult:IJsonResult
    {
        public object Data { get; set; }

        public string ContentType { get; set; }

        public Encoding ContentEncoding { get; set; }

        public bool AllowGet { get; set; }

        public string Type
        {
            get { return ActionResultTypes.Json; }
        }
    }
}
