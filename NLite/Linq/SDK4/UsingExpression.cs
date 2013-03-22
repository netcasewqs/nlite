using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace NLite.Linq
{
#if SDK4
    class UsingExpression : CustomExpression
    {
        readonly ParameterExpression variable;
        readonly Expression disposable;
        readonly Expression body;

        public new ParameterExpression Variable
        {
            get { return variable; }
        }

        public Expression Disposable
        {
            get { return disposable; }
        }

        public Expression Body
        {
            get { return body; }
        }

        public override Type Type
        {
            get { return body.Type; }
        }

        public override CustomExpressionType CustomNodeType
        {
            get { return CustomExpressionType.UsingExpression; }
        }

        internal UsingExpression(ParameterExpression variable, Expression disposable, Expression body)
        {
            this.variable = variable;
            this.disposable = disposable;
            this.body = body;
        }

        public UsingExpression Update(ParameterExpression variable, Expression disposable, Expression body)
        {
            if (this.variable == variable && this.disposable == disposable && this.body == body)
                return this;

            return CustomExpression.Using(variable, disposable, body);
        }

        public override Expression Reduce()
        {
            var end_finally = Expression.Label("end_finally");

            return Expression.Block(
                new[] { variable },
                variable.Assign(disposable),
                Expression.TryFinally(
                    body,
                    Expression.Block(
                        variable.NotEqual(Expression.Constant(null)).Condition(
                            Expression.Block(
                                Expression.Call(
                                    variable.Convert(typeof(IDisposable)),
                                    typeof(IDisposable).GetMethod("Dispose")),
                                Expression.Goto(end_finally)),
                            Expression.Goto(end_finally)),
                        Expression.Label(end_finally))));
        }

        protected override Expression VisitChildren(System.Linq.Expressions.ExpressionVisitor visitor)
        {
            return Update(
                (ParameterExpression)visitor.Visit(variable),
                visitor.Visit(disposable),
                visitor.Visit(body));
        }

        public override Expression Accept(CustomExpressionVisitor visitor)
        {
            return visitor.VisitUsingExpression(this);
        }
    }

    abstract partial class CustomExpression
    {

        public static UsingExpression Using(Expression disposable, Expression body)
        {
            return Using(null, disposable, body);
        }

        public static UsingExpression Using(ParameterExpression variable, Expression disposable, Expression body)
        {
            if (disposable == null)
                throw new ArgumentNullException("disposable");
            if (body == null)
                throw new ArgumentNullException("body");

            if (!typeof(IDisposable).IsAssignableFrom(disposable.Type))
                throw new ArgumentException("The disposable must implement IDisposable", "disposable");

            if (variable == null)
                variable = Expression.Parameter(disposable.Type);

            return new UsingExpression(variable, disposable, body);
        }
    }
#endif
}
