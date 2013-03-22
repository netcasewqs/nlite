using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace NLite.Linq
{
#if SDK4
    abstract partial class CustomExpression : Expression
    {

        public abstract CustomExpressionType CustomNodeType { get; }

        public override ExpressionType NodeType
        {
            get { return ExpressionType.Extension; }
        }

        public override bool CanReduce
        {
            get { return true; }
        }

        public abstract Expression Accept(CustomExpressionVisitor visitor);
    }
#endif
}
