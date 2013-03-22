using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite.Domain.Mapping
{
    [Serializable]
    public class ViewResult:IViewResult
    {
        public string ViewName { get; set; }
        public string MasterName { get; set; }
        public object Model { get; set; }

        public string Type
        {
            get { return ActionResultTypes.View; }
        }
    }
}
