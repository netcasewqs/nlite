using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLite.Internal;
using NLite.Validation.DataAnnotations;


namespace NLite.Validation
{
    /// <summary>
    /// 
    /// </summary>
    public static class Validator
    {
        /// <summary>
        /// 缺省检验器
        /// </summary>
        public static IValidator Default = new EntityValidator();

        /// <summary>
        /// 注册运行时动态绑定校验
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="entityValidatorType"></param>
        public static void Register(Type entityType, Type entityValidatorType)
        {
            Guard.NotNull(entityType, "entityType");
            Guard.NotNull(entityValidatorType, "entityValidatorType");
            Default.Register(entityType, entityValidatorType);
        }

        /// <summary>
        /// 注册运行时动态绑定校验
        /// </summary>
        public static void Register<TEntity,TEntityValidator>()
        {
            Default.Register(typeof(TEntity), typeof(TEntityValidator));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static IErrorState Validate(object entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            return Default.Validate(entity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static IErrorState Validate<TEntity>(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            return Default.Validate(entity);
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="TEntity"></typeparam>
        ///// <param name="map"></param>
        ///// <returns></returns>
        //public static IErrorState Validate<TEntity>(IDictionary<string, string> map)
        //{
        //    if (map == null)
        //        throw new ArgumentNullException("map");
        //    return Default.Validate(typeof(TEntity), map);
        //}

       
    }
}
