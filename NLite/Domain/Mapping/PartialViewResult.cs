using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite.Domain.Mapping
{
    [Serializable]
    public class PartialViewResult:IPartialViewResult
    {
        public string ViewName { get; set; }
        public object Model { get; set; }

        public string Type
        {
            get { return ActionResultTypes.PartialView; }
        }
    }
}
