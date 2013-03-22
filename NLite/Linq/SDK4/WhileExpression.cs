using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace NLite.Linq
{
#if SDK4
    class WhileExpression : CustomExpression
    {

        readonly Expression test;
        readonly Expression body;

        readonly LabelTarget break_target;
        readonly LabelTarget continue_target;

        public Expression Test
        {
            get { return test; }
        }

        public Expression Body
        {
            get { return body; }
        }

        public LabelTarget BreakTarget
        {
            get { return break_target; }
        }

        public LabelTarget ContinueTarget
        {
            get { return continue_target; }
        }

        public override Type Type
        {
            get
            {
                if (break_target != null)
                    return break_target.Type;

                return typeof(void);
            }
        }

        public override CustomExpressionType CustomNodeType
        {
            get { return CustomExpressionType.WhileExpression; }
        }

        internal WhileExpression(Expression test, Expression body, LabelTarget breakTarget, LabelTarget continueTarget)
        {
            this.test = test;
            this.body = body;
            this.break_target = breakTarget;
            this.continue_target = continueTarget;
        }

        public WhileExpression Update(Expression test, Expression body, LabelTarget breakTarget, LabelTarget continueTarget)
        {
            if (this.test == test && this.body == body && this.break_target == breakTarget && this.continue_target == continueTarget)
                return this;

            return CustomExpression.While(test, body, breakTarget, continueTarget);
        }

        public override Expression Reduce()
        {
            var inner_loop_break = Expression.Label("inner_loop_break");
            var inner_loop_continue = Expression.Label("inner_loop_continue");

            var @continue = continue_target ?? Expression.Label("continue");
            var @break = break_target ?? Expression.Label("break");

            return Expression.Block(
                Expression.Loop(
                    Expression.Block(
                        Expression.Label(@continue),
                        test.Condition(
                            Expression.Block(
                                body,
                                Expression.Goto(inner_loop_continue)),
                            Expression.Goto(inner_loop_break))),
                    inner_loop_break,
                    inner_loop_continue),
                Expression.Label(@break));
        }

        protected override Expression VisitChildren(System.Linq.Expressions.ExpressionVisitor visitor)
        {
            return Update(
                visitor.Visit(test),
                visitor.Visit(body),
                continue_target,
                break_target);
        }

        public override Expression Accept(CustomExpressionVisitor visitor)
        {
            return visitor.VisitWhileExpression(this);
        }
    }

    abstract partial class CustomExpression
    {

        public static WhileExpression While(Expression test, Expression body)
        {
            return While(test, body, null);
        }

        public static WhileExpression While(Expression test, Expression body, LabelTarget breakTarget)
        {
            return While(test, body, breakTarget, null);
        }

        public static WhileExpression While(Expression test, Expression body, LabelTarget breakTarget, LabelTarget continueTarget)
        {
            if (test == null)
                throw new ArgumentNullException("test");
            if (body == null)
                throw new ArgumentNullException("body");

            if (test.Type != typeof(bool))
                throw new ArgumentException("test must be a boolean expression", "test");

            if (continueTarget != null && continueTarget.Type != typeof(void))
                throw new ArgumentException("Continue label target must be void ", "continueTarget");

            return new WhileExpression(test, body, breakTarget, continueTarget);
        }
    }
#endif
}
