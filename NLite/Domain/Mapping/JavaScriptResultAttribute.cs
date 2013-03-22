using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite.Domain.Mapping
{
    [Serializable]
    public class JavaScriptResultAttribute : NavigationResultAttribute, IJavaScriptResult
    {
        string IJavaScriptResult.Script { get { return null; } }
        public JavaScriptResultAttribute() : base(ActionResultTypes.JavaScript) { }
    }
}
