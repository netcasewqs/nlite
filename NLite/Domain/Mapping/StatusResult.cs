using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite.Domain.Mapping
{
    public class StatusResult:IStatusResult
    {
        public int StatusCode { get; set; }
        public string Body { get; set; }

        public string Type
        {
            get { return ActionResultTypes.Status; }
        }
    }

    [Serializable]
    public class StatusResultAttribute : NavigationResultAttribute, IStatusResult
    {
         public int StatusCode { get; set; }
        public string Body { get; set; }
        public StatusResultAttribute(uint statusCode)
            : base(ActionResultTypes.Status)
        {
            this.StatusCode = (int)statusCode;
        }
    }
}
