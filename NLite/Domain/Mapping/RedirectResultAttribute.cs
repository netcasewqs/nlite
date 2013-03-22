using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite.Domain.Mapping
{
    [Serializable]
    public class RedirectResultAttribute : NavigationResultAttribute, IRedirectResult
    {
        public string Url { get; set; }
        public RedirectResultAttribute() : base(ActionResultTypes.Redirect) { }
    }
}
