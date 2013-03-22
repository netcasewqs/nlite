using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NLite;
using NLite.Reflection;
using NLite.Collections;
using System.Collections;
using NLite.Mapping;
using NLite.Mapping.Internal;

namespace NLite.Binding
{
    /// <summary>
    /// 缺省模型绑定器
    /// </summary>
    public class DefaultModelBinder : IModelBinder
    {
        /// <summary>
        /// 执行模型绑定
        /// </summary>
        /// <param name="bindingInfo"></param>
        /// <param name="valueProvider"></param>
        /// <returns></returns>
        public object BindModel(BindingInfo bindingInfo, IDictionary<string, object> valueProvider)
        {
            if (bindingInfo == null)
                throw new ArgumentNullException("bindingInfo");
            if (valueProvider == null)
                throw new ArgumentNullException("valueProvider");

            bindingInfo.FilterValueProvider(valueProvider);

            var modelType = bindingInfo.Type;
            if (Converter.IsPrimitiveType(modelType))
                return BindPrimitiveModel(bindingInfo, valueProvider);
            if (!Types.IEnumerable.IsAssignableFrom(modelType))
                return BindSimpleModel(modelType, bindingInfo, valueProvider);
            if (modelType.IsDictionaryType())
                return BindDictionaryModel(modelType, bindingInfo.Name, valueProvider);
            return BindCollectionModel(modelType, bindingInfo.Name, valueProvider);
        }

        private static object BindCollectionModel(Type collectionType, string prefix, IDictionary<string, object> iDictionary)
        {
            Type elementType = TypeHelper.GetElementType(collectionType);

            //确保集合必须是数组或泛型集合类型
            if (elementType == null)
                return null;

            IList coll = null;

            if (iDictionary.ContainsKey(prefix))//判断字典中是否完全匹配参数名
            {
                var from = iDictionary[prefix];
                coll = Mapper.Map(from, null, collectionType) as IList;
            }
            else
            {
                coll = ObjectCreator.CreateList(collectionType, elementType, 2);
                var parameterNamePrefix = prefix + "[";

                int index;
                var expectedColl = iDictionary.Keys
                    .Where(p => p.StartsWith(parameterNamePrefix, StringComparison.InvariantCultureIgnoreCase))//确保字典Key必须是xxx[ 格式
                    .Select(p => new { Parts = p.Split('.'), Value = iDictionary[p] })//把Key进行句号分隔
                    .Where(p => p.Parts.Length > 1)
                    .Select(p =>
                    {
                        var parts = p.Parts.ToList();
                        parts.RemoveAt(0);
                        return new
                        {
                            Index = p.Parts[0].Matches("[", "]")//数组索引号
                            ,
                            Name = parts.ToCSV(".")
                            ,
                            Value = p.Value
                        };
                    })
                        .Where(p => p.Index.Length == 1 && int.TryParse(p.Index[0], out index))//目前仅仅支持一维数组，确保索引号是Int类型
                        .OrderBy(p => p.Index[0])//
                        .Select(p => new { Index = p.Index[0], Name = p.Name, Value = p.Value })
                        .ToLookup(p => p.Index)
                        .ToArray();

                if (expectedColl.Length > 0)
                {
                    foreach (var group in expectedColl)
                    {
                        var item = ObjectCreator.Create(elementType);
                        if (item == null)
                            continue;
                        coll.Add(item);
                        foreach (var prop in group)
                            item.SetProperty(prop.Name, prop.Value, null);
                    }
                }
            }

            if (collectionType.IsArray)
            {
                Array array = Array.CreateInstance(elementType, coll.Count);
                coll.CopyTo(array, 0);
                coll = array;
            }
            return coll;
        }

        private object BindDictionaryModel(Type dictType, string prefix, IDictionary<string, object> iDictionary)
        {
            var elementType = DictionaryInfo.Get(dictType);

            //确保字典必须是强类型
            if (elementType.Key == null)
                return null;

            var targetDict = ObjectCreator.CreateDictionary(dictType, elementType.Key, elementType.Value);//TODO:


            var parameterNamePrefix = prefix + "[";
            var addMethod = elementType.Type.GetMethod("Add", new Type[] { elementType.Key, elementType.Value }).GetProc();

            int index;
            var expectedColl = iDictionary.Keys
                .Where(p => p.StartsWith(parameterNamePrefix, StringComparison.InvariantCultureIgnoreCase))//确保字典Key必须是xxx[ 格式
                .Select(p => new { Parts = p.Split('.'), Value = iDictionary[p] })//把Key进行句号分隔
                .Where(p => p.Parts.Length > 1)
                .Select(p =>
                {
                    var parts = p.Parts.ToList();
                    parts.RemoveAt(0);
                    return new
                    {
                        Index = p.Parts[0].Matches("[", "]")//数组索引号
                        ,
                        Name = parts.ToCSV(".")
                        ,
                        Value = p.Value
                    };
                })
                    .Where(p => p.Index.Length == 1 && int.TryParse(p.Index[0], out index))//目前仅仅支持一维数组，确保索引号是Int类型
                    .OrderBy(p => p.Index[0])//
                    .Select(p => new { Index = p.Index[0], Name = p.Name, Value = p.Value })
                    .ToLookup(p => p.Index)
                    .ToArray();

            if (expectedColl.Length < 1)
                return targetDict;


            foreach (var group in expectedColl)
            {
                var keyItem = group.FirstOrDefault(p => p.Name.Equals("key", StringComparison.InvariantCultureIgnoreCase));
                if (keyItem == null
                    || keyItem.Name.Split('.').Length != 1//确保key类型必须是基本类型
                    || keyItem.Value == null)//Key 的值为null没有任何意义
                    continue;

                var valueItems = group
                    .Where(p => p.Name.StartsWith("value", StringComparison.InvariantCultureIgnoreCase))
                    .Select(p => new { Name = p.Name.Remove(0, 6), Value = p.Value })
                    .ToArray();


                var key = Mapper.Map(keyItem.Value, keyItem.Value.GetType(), elementType.Key);
                if (key == null)
                    continue;
                if (valueItems.Length > 1)
                {
                    var value = ObjectCreator.Create(elementType.Value);
                    addMethod(targetDict, key, value);

                    foreach (var item in valueItems)
                    {
                        value.SetProperty(item.Name, item.Value);
                    }
                }
                else if (valueItems.Length == 1)
                {
                    //TODO:
                }
                else
                {
                    //TODO:
                }
            }

            return targetDict;
        }

        internal struct DictionaryInfo
        {
            /// <summary>
            /// Dictionary Type
            /// </summary>
            public Type Type;
            public Type Key;
            public Type Value;
            public Type Kvp;


            public bool IsGeneric
            {
                get { return Type != null && typeof(DictionaryEntry) != Kvp; }
            }

            static readonly Type KvpType = typeof(KeyValuePair<,>);
            public static DictionaryInfo Get(Type type)
            {
                DictionaryInfo info;
                if (TypeHelper.IsGenericDictionaryType(type))
                {
                    info.Type = type.GetGenericDictionaryType();
                    info.Key = info.Type.GetGenericArguments()[0];
                    info.Value = info.Type.GetGenericArguments()[1];
                    info.Kvp = KvpType.MakeGenericType(info.Key, info.Value);
                }
                else
                {
                    info.Type = type;
                    info.Key = Types.Object;
                    info.Value = Types.Object;
                    info.Kvp = typeof(DictionaryEntry);
                }

                return info;
            }

        }



        private object BindPrimitiveModel(BindingInfo bindingInfo, IDictionary<string, object> valueProvider)
        {
            try
            {
                var tmpValue = valueProvider
                    .FirstOrDefault(p => string.Equals(p.Key, bindingInfo.Name, StringComparison.InvariantCultureIgnoreCase))
                    .Value;

                if (tmpValue != null)
                    return BindSimpleModel(tmpValue, bindingInfo.Type);

                return bindingInfo.DefaultValue.Value;
            }
            catch
            {
                throw;
            }

        }
        private static readonly MethodInfo MapModel_Method = typeof(DefaultModelBinder).GetMethod("MapModel", BindingFlags.NonPublic | BindingFlags.Instance);

        private T MapModel<T>(BindingInfo bindingInfo, IDictionary<string, object> valueProvider)
        {
            var mapper = Mapper.CreateMapper<IDictionary<string, object>, T>();

            var tmpValueProvider = new Dictionary<string, object>(valueProvider, StringComparer.InvariantCultureIgnoreCase);
            bindingInfo.FilterValueProvider(tmpValueProvider);
            var o = mapper.Map(tmpValueProvider);
            return o;
        }

        private object BindSimpleModel(Type modelType, BindingInfo bindingInfo, IDictionary<string, object> valueProvider)
        {
            try
            {
                return MapModel_Method.MakeGenericMethod(modelType).FastFuncInvoke(this, bindingInfo, valueProvider);
            }
            catch
            {
                throw;
            }
        }


        static object BindSimpleModel(object from, Type toType)
        {
            var to = Mapper.Map(from, null, toType);
            return to;
        }

    }
}
