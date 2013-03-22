using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite.Domain.Mapping
{
    [Serializable]
    public class ViewResultAttribute : NavigationResultAttribute, IViewResult
    {
        public string ViewName { get; set; }
        public string MasterName { get; set; }
        object IViewResult.Model { get { return null; } }
        public ViewResultAttribute() : base(ActionResultTypes.View) { }
    }

}
