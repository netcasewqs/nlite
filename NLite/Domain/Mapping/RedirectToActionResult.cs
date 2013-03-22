using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite.Domain.Mapping
{
    [Serializable]
    public class RedirectToActionResult:IRedirectToActionResult
    {
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public object RouteValues { get; set; }

        public string Type
        {
            get { return ActionResultTypes.RedirectToAction; }
        }
    }
}
