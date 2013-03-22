using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace NLite.Linq
{
#if SDK4
    abstract class CustomExpressionVisitor : System.Linq.Expressions.ExpressionVisitor
    {

        protected override Expression VisitExtension(Expression node)
        {
            var custom_node = node as CustomExpression;
            if (custom_node != null)
                return Visit(custom_node);

            return base.VisitExtension(node);
        }

        protected Expression Visit(CustomExpression node)
        {
            if (node == null)
                return null;

            return node.Accept(this);
        }

        protected internal virtual Expression VisitDoWhileExpression(DoWhileExpression node)
        {
            return node.Update(
                Visit(node.Test),
                Visit(node.Body),
                node.BreakTarget,
                node.ContinueTarget);
        }

        protected internal virtual Expression VisitForExpression(ForExpression node)
        {
            return node.Update(
                (ParameterExpression)Visit(node.Variable),
                Visit(node.Initializer),
                Visit(node.Test),
                Visit(node.Step),
                Visit(node.Body),
                node.BreakTarget,
                node.ContinueTarget);
        }

        protected internal virtual Expression VisitForEachExpression(ForEachExpression node)
        {
            return node.Update(
                (ParameterExpression)Visit(node.Variable),
                Visit(node.Enumerable),
                Visit(node.Body),
                node.BreakTarget,
                node.ContinueTarget);
        }

        protected internal virtual Expression VisitUsingExpression(UsingExpression node)
        {
            return node.Update(
                (ParameterExpression)Visit(node.Variable),
                Visit(node.Disposable),
                Visit(node.Body));
        }

        protected internal virtual Expression VisitWhileExpression(WhileExpression node)
        {
            return node.Update(
                Visit(node.Test),
                Visit(node.Body),
                node.BreakTarget,
                node.ContinueTarget);
        }
    }
#endif
}
