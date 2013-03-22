using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NLite.Domain.Mapping
{
    [Serializable]
    public class RedirectToRouteResultAttribute : NavigationResultAttribute, IRedirectToRouteResult
    {
        public string RouteName { get; set; }
        object IRedirectToRouteResult.RouteValues { get { return null; } }
        public RedirectToRouteResultAttribute() : base(ActionResultTypes.RedirectToRoute) { }
    }
}
