//using System;

//namespace NLite.Mini.Fluent
//{
//    /// <summary>
//    /// 绑定表达式
//    /// </summary>
//    public interface IBindingExpression : IContractExpression
//    {
//        /// <summary>
//        /// 设置Id
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        INamedExpression Named(string id);

//        /// <summary>
//        /// 设置契约类型
//        /// </summary>
//        /// <param name="contract"></param>
//        /// <returns></returns>
//        IContractExpression Bind(Type contract);

//        /// <summary>
//        /// 设置契约类型
//        /// </summary>
//        /// <typeparam name="TContract"></typeparam>
//        /// <returns></returns>
//        IContractExpression Bind<TContract>();

//        /// <summary>
//        /// 设置契约类型和Id
//        /// </summary>
//        /// <param name="contract"></param>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        IContractExpression Bind(Type contract,string id);

//        /// <summary>
//        /// 设置契约类型和Id
//        /// </summary>
//        /// <typeparam name="TContract"></typeparam>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        IContractExpression Bind<TContract>(string id);
//    }
//}
