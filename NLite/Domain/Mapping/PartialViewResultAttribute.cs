using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite.Domain.Mapping
{
    [Serializable]
    public class PartialViewResultAttribute : NavigationResultAttribute, IPartialViewResult
    {
        object IPartialViewResult.Model { get { return null; } }
        public string ViewName { get; set; }
        public PartialViewResultAttribute() : base(ActionResultTypes.PartialView) { }
    }
}
