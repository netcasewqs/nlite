using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite.Linq
{
    #if SDK4
    enum CustomExpressionType
    {
        DoWhileExpression,
        ForEachExpression,
        ForExpression,
        UsingExpression,
        WhileExpression,
    }
#endif
}
