using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite.Domain.Mapping
{
    [Serializable]
    public class RedirectToActionAttribute : NavigationResultAttribute, IRedirectToActionResult
    {
        public RedirectToActionAttribute(string actionName)
            : base(ActionResultTypes.RedirectToAction)
        {
            if (string.IsNullOrEmpty(actionName))
                throw new ArgumentNullException("actionName");
            ActionName = actionName;
        }

        object IRedirectToActionResult.RouteValues { get { return null; } }
        public string ControllerName { get; set; }
        public string ActionName { get; private set; }
    }
}
