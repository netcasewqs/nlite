using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite.Domain.Mapping
{
    [Serializable]
    public class JavaScriptResult:IJavaScriptResult
    {
        public string Script { get; set; }
        string INavigationResult.Type
        {
            get { return ActionResultTypes.JavaScript; }
        }
    }
}
