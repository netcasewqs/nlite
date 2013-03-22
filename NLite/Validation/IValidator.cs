using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite.Validation
{
    /// <summary>
    /// 
    /// </summary>
     //[Contract]
    public interface IValidator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
         IErrorState Validate(object instance);

         ///// <summary>
         ///// 
         ///// </summary>
         ///// <param name="dic"></param>
         ///// <returns></returns>
         //IErrorState Validate(Type entityType, IDictionary<string, string> dic);

         /// <summary>
         /// 注册运行时动态绑定校验
         /// </summary>
         /// <param name="entityType"></param>
         /// <param name="entityValidatorType"></param>
         void Register(Type entityType, Type entityValidatorType);
    }


    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class EntityValidatorAttribute : Attribute
    {
        ///
        public readonly string StrEntityValidatorType;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strEntityValidatorType"></param>
        public EntityValidatorAttribute(string strEntityValidatorType)
        {
            if (string.IsNullOrEmpty(strEntityValidatorType))
                throw new ArgumentNullException(strEntityValidatorType);

            this.StrEntityValidatorType = strEntityValidatorType;
        }


        /// <summary>
        /// 
        /// </summary>
        public Type EntityValidatorType
        {
            get
            {
                Type type = null;

                type = Type.GetType(StrEntityValidatorType);

                if (type == null)
                    throw new ArgumentException("Invalid EntityValidatorType:"+ StrEntityValidatorType, "strEntityValidatorType");

                return type;
            }
        }
    }
}
