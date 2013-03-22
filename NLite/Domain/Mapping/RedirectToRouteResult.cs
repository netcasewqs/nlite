using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite.Domain.Mapping
{
    [Serializable]
    public class RedirectToRouteResult:IRedirectToRouteResult
    {
        public string RouteName { get; set; }
        public object RouteValues { get; set; }

        public string Type
        {
            get { return ActionResultTypes.RedirectToRoute; }
        }
    }
}
