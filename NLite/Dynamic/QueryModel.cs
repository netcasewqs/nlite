using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using NLite.Collections;
using System.Linq.Expressions;
using System.Reflection;
using NLite.Binding;
using NLite.Reflection;
using NLite.Data;

namespace NLite.Dynamic
{
    /// <summary>
    /// 用户自动收集搜索条件的Model
    /// </summary>
    [Serializable]
    [QueryModelBinder]
    public class QueryModel:List<Filter>
    {
       
    }

    /// <summary>
    /// 查询模型绑定器
    /// </summary>
    public class QueryModelBinder : IModelBinder
    {
        /// <summary>
        /// 执行模型绑定
        /// </summary>
        /// <param name="info"></param>
        /// <param name="valueProvider"></param>
        /// <returns></returns>
        public object BindModel(BindingInfo info, IDictionary<string, object> valueProvider)
        {
           

            var model = new QueryModel();
            var keys = valueProvider.Keys.Where(c => c.TrimStart().StartsWith("[")).ToArray();//我们认为只有[开头的为需要处理的
            if (keys.Length != 0)
            {

                for(int i=0;i<keys.Length ;i++)
                {
                    var key = keys[i].Replace(" ", "");

                    if (!key.StartsWith("[")) 
                        continue;

                    var val = valueProvider[keys[i]];
                    if (val == null)
                        continue;
                    var typeCode = Type.GetTypeCode(val.GetType());
                    if (typeCode == TypeCode.String)
                    {
                        //处理无值的情况
                        if (string.IsNullOrEmpty(val as string))
                            continue;
                    }
                    AddSearchItem(model, key, val);
                }
            }
            return model;
        }

        /// <summary>
        /// 将一组key=value添加入QueryModel.Items
        /// </summary>
        /// <param name="model">QueryModel</param>
        /// <param name="key">当前项的HtmlName</param>
        /// <param name="val">当前项的值</param>
        static void AddSearchItem(QueryModel model, string key, object val)
        {
            string field = "", prefix = "", orGroup = "", method = "";
            var keywords = key.Split(']', ')', '}');
            //将Html中的name分割为我们想要的几个部分
            foreach (var keyword in keywords)
            {
                if (Char.IsLetterOrDigit(keyword[0]))
                    field = keyword;
                var last = keyword.Substring(1);
                if (keyword[0] == '(')
                    prefix = last;
                if (keyword[0] == '[')
                    method = last;
                if (keyword[0] == '{') 
                    orGroup = last;
            }
            if (string.IsNullOrEmpty(method)) 
                return;
            OperationType opType;
            if (!FilterSettings.Dict.TryGetValue(method, out opType))
                return;
            if (!string.IsNullOrEmpty(field))
            {
                var item = new Filter
                {
                    Field = field,
                    Value = val,
                    Prefix = prefix,
                    OrGroup = orGroup,
                    Operation = opType
                };
                model.Add(item);
            }
        }
    }

    class QueryModelBinderAttribute : CustomModelBinderAttribute
    {

        public override IModelBinder GetBinder()
        {
            return new QueryModelBinder();
        }
    }

    /// <summary>
    /// 查询条件
    /// </summary>
    [Serializable]
    public class Filter
    {
        /// <summary>
        /// 字段
        /// </summary>
        public string Field { get; set; }
        /// <summary>
        /// 操作
        /// </summary>
        public OperationType Operation { get; set; }
        /// <summary>
        /// Value
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// 前缀，用于标记作用域，HTML中使用()进行标识
        /// </summary>
        public string Prefix { get; set; }
        /// <summary>
        /// 如果使用Or组合，则此组组合为一个Or序列,HTML中使用{}进行标识
        /// </summary>
        public string OrGroup { get; set; }

      
    }

    /// <summary>
    /// 操作枚举
    /// </summary>
    public enum OperationType
    {
        /// <summary>
        /// 等于
        /// </summary>
        Equal = 0,

        /// <summary>
        /// 小于
        /// </summary>
        Less = 1,

        /// <summary>
        /// 大于
        /// </summary>
        Greater = 2,

        /// <summary>
        /// 小于等于
        /// </summary>
        LessOrEqual = 3,

        /// <summary>
        /// 大于等于
        /// </summary>
        GreaterOrEqual = 4,

        /// <summary>
        /// Like
        /// </summary>
        Like = 6,

        /// <summary>
        /// In
        /// </summary>
        In = 7,

        /// <summary>
        /// 输入一个时间获取当前天的时间块操作, ToSql未实现，仅实现了IQueryable
        /// </summary>
        DateBlock = 8,

        NotEqual = 9,


        // [GlobalCode("like", OnlyAttribute = true)]
        BeginWith = 10,

        // [GlobalCode("like", OnlyAttribute = true)]
        EndWith = 11,

        /// <summary>
        /// 处理Like的问题
        /// </summary>
        Contains = 12,

        /// <summary>
        /// 处理In的问题
        /// </summary>
        StdIn = 13,

        /// <summary>
        /// 处理Datetime小于+23h59m59s999f的问题
        /// </summary>
        DateTimeLessThanOrEqual = 14,

        NotBeginWith = 15,
        NotEndWith = 16,
        NotContains = 17,
        NotIn = 18,
    }

    /// <summary>
    /// 条件设置类
    /// </summary>
    public sealed class FilterSettings
    {
        internal static Dictionary<string, OperationType> Dict = new Dictionary<string, OperationType>(StringComparer.InvariantCultureIgnoreCase);
        private static FilterSettings instance;
        /// <summary>
        /// 设置确实操作名称
        /// </summary>
        /// <param name="op"></param>
        public static void Configure(FilterSettings op)
        {
            if (op == null)
                throw new ArgumentNullException("op");
            if (op.Contains.IsNullOrEmpty())
                throw new ArgumentNullException("op.Contains");
            if (op.EndWith.IsNullOrEmpty())
                throw new ArgumentNullException("op.EndWith");
            if (op.Equal.IsNullOrEmpty())
                throw new ArgumentNullException("op.Equal");
            if (op.Greater.IsNullOrEmpty())
                throw new ArgumentNullException("op.Greater");
            if (op.GreaterOrEqual.IsNullOrEmpty())
                throw new ArgumentNullException("op.GreaterOrEqual");
            if (op.In.IsNullOrEmpty())
                throw new ArgumentNullException("op.In");
            if (op.Less.IsNullOrEmpty())
                throw new ArgumentNullException("op.Less");
            if (op.LessOrEqual.IsNullOrEmpty())
                throw new ArgumentNullException("op.LessOrEqual");
            if (op.NotBeginWith.IsNullOrEmpty())
                throw new ArgumentNullException("op.NotStartsWith");
            if (op.NotContains.IsNullOrEmpty())
                throw new ArgumentNullException("op.NotContains");
            if (op.NotEndWith.IsNullOrEmpty())
                throw new ArgumentNullException("op.NotEndWith");
            if (op.NotEqual.IsNullOrEmpty())
                throw new ArgumentNullException("op.NotEqual");
            if (op.NotIn.IsNullOrEmpty())
                throw new ArgumentNullException("op.NotIn");
            if (op.BeginWith.IsNullOrEmpty())
                throw new ArgumentNullException("op.BeginWith");

            if (op.OperationBeginFlag.IsNullOrEmpty())
                throw new ArgumentNullException("op.OperationBeginFlag");
            if (op.PrefixBeginFlag.IsNullOrEmpty())
                throw new ArgumentNullException("op.PrefixBeginFlag");
            if (op.OrGroupBeginFlag.IsNullOrEmpty())
                throw new ArgumentNullException("op.OrGroupBeginFlag");
            if (op.OperationBeginFlag.Equals(op.PrefixBeginFlag))
                throw new ArgumentException("op.OperationBeginFlag same as op.PrefixBeginFlag");
            if (op.OperationBeginFlag.Equals(op.OrGroupBeginFlag))
                throw new ArgumentException("op.OperationBeginFlag same as op.OrGroupBeginFlag");
            if (op.PrefixBeginFlag.Equals(op.OrGroupBeginFlag))
                throw new ArgumentException("op.PrefixBeginFlag same as op.OrGroupBeginFlag");


            instance = op;

            Dict.Clear();
            Dict[op.Contains] = OperationType.Contains;
            Dict[op.EndWith] = OperationType.EndWith;
            Dict[op.Equal] = OperationType.Equal;
            Dict[op.Greater] = OperationType.Greater;
            Dict[op.GreaterOrEqual] = OperationType.GreaterOrEqual;
            Dict[op.In] = OperationType.In;
            Dict[op.Less] = OperationType.Less;
            Dict[op.LessOrEqual] = OperationType.LessOrEqual;
            Dict[op.NotContains] = OperationType.NotContains;
            Dict[op.NotEndWith] = OperationType.NotEndWith;
            Dict[op.NotEqual] = OperationType.NotEqual;
            Dict[op.NotIn] = OperationType.NotIn;
            Dict[op.NotBeginWith] = OperationType.NotBeginWith;
            Dict[op.BeginWith] = OperationType.BeginWith;
        }

        static FilterSettings()
        {
            Configure(new FilterSettings());
        }

        /// <summary>
        /// 操作符起始标识,默认是'['
        /// </summary>
        public string OperationBeginFlag = "[";
        /// <summary>
        /// 前缀起始标识，默认是'('
        /// </summary>
        public string PrefixBeginFlag = "(";
        /// <summary>
        /// Or组起始标识，默认是'{'
        /// </summary>
        public string OrGroupBeginFlag = "{";

        /// <summary>
        /// 返回当前实例
        /// </summary>
        public static FilterSettings Current
        {
            get { return instance; }
        }

        /// <summary>
        /// 等于,缺省值为eq
        /// </summary>
        public string Equal = "=";

        /// <summary>
        /// 小于,缺省值为lt
        /// </summary>
        public string Less = "lt";

        /// <summary>
        /// 大于,缺省值为gt
        /// </summary>
        public string Greater = "gt";

        /// <summary>
        /// 小于等于,缺省值为le
        /// </summary>
        public string LessOrEqual = "le";

        /// <summary>
        /// 大于等于,缺省值为ge
        /// </summary>
        public string GreaterOrEqual = "ge";

        /// <summary>
        /// In,缺省值为in
        /// </summary>
        public string In = "in";

        /// <summary>
        ///不等于 ,缺省值为ne
        /// </summary>
        public string NotEqual = "ne";

        /// <summary>
        ///BeginWith ,缺省值为bw
        /// </summary>
        public string BeginWith = "bw";
        /// <summary>
        /// EndWith,缺省值为ew
        /// </summary>
        public string EndWith = "ew";

        /// <summary>
        /// 处理Contains的问题,缺省值为cn
        /// </summary>
        public string Contains = "cn";

        /// <summary>
        ///NotBeginWith ,缺省值为nbw
        /// </summary>
        public string NotBeginWith = "nbw";
        /// <summary>
        ///NotEndWith ,缺省值为new
        /// </summary>
        public string NotEndWith = "new";
        /// <summary>
        /// NotContains,缺省值为nc
        /// </summary>
        public string NotContains = "nc";
        /// <summary>
        /// NotIn,缺省值为ni
        /// </summary>
        public string NotIn = "ni";

    }

    /// <summary>
    /// 
    /// </summary>
    public interface ITransformProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="binderType"></param>
        /// <returns></returns>
        bool Match(Filter item, Type type);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="binderType"></param>
        /// <returns></returns>
        IEnumerable<Filter> Transform(Filter item, Type type);
    }

    /// <summary>
    /// 
    /// </summary>
    public class InTransformProvider : ITransformProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="binderType"></param>
        /// <returns></returns>
        public bool Match(Filter item, Type type)
        {
            return item.Operation == OperationType.In;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="binderType"></param>
        /// <returns></returns>
        public IEnumerable<Filter> Transform(Filter item, Type type)
        {
            var arr = (item.Value as Array);
            if (arr == null)
            {
                var arrStr = item.Value.ToString();
                if (!string.IsNullOrEmpty(arrStr))
                {
                    arr = arrStr.Split(',');
                }
            }
            return new[] { new Filter { Field = item.Field, Operation = OperationType.StdIn, Value = arr} };
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class OrGroupTransformProvider : ITransformProvider
    {

        bool ITransformProvider.Match(Filter item, Type type)
        {
            return item.OrGroup.HasValue()
                && item.Value != null
                && item.Value.GetType().HasElementType;
        }

        IEnumerable<Filter> ITransformProvider.Transform(Filter item, Type type)
        {
            foreach (var v in item.Value as IEnumerable)
                yield return new Filter { Field = item.Field, Operation = item.Operation, Prefix = item.Prefix, Value = v, OrGroup = item.OrGroup };
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class TransformProviders
    {
        /// <summary>
        /// 
        /// </summary>
        public static List<ITransformProvider> Items { get; private set; }

        static TransformProviders()
        {
            Items = new List<ITransformProvider>
                                     {
                                         new LikeTransformProvider(),
                                         new DateBlockTransformProvider(),
                                         new InTransformProvider(),
                                         new UnixTimeTransformProvider(),
                                     };

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="providers"></param>
        public static void SetProviders(params ITransformProvider[] providers)
        {
            if (providers == null|| providers.Length == 0)
                throw new ArgumentNullException("providers");
            Items.Clear();
            Items.AddRange(providers);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DateBlockTransformProvider : ITransformProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="binderType"></param>
        /// <returns></returns>
        public bool Match(Filter item, Type type)
        {
            return item.Operation == OperationType.DateBlock;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="binderType"></param>
        /// <returns></returns>
        public IEnumerable<Filter> Transform(Filter item, Type type)
        {
            return new[]
                       {
                           new Filter { Field = item.Field, Operation = OperationType.GreaterOrEqual, Value = item.Value},
                           new Filter{ Field = item.Field, Operation = OperationType.Less, Value = item.Value}
                       };
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class UnixTimeTransformProvider : ITransformProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="binderType"></param>
        /// <returns></returns>
        public bool Match(Filter item, Type type)
        {
            var elementType = TypeUtil.GetUnNullableType(type);
            var valueType = item.Value.GetType();

            return ((elementType == Types.Int32 && !(valueType == Types.Int32))
                    || (elementType == typeof(long) && !(item.Value is long))
                    || (elementType == typeof(DateTime) && !(item.Value is DateTime))
                   )
                   && item.Value.ToString().Contains("-");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="binderType"></param>
        /// <returns></returns>
        public IEnumerable<Filter> Transform(Filter item, Type type)
        {
            DateTime willTime;
            if (DateTime.TryParse(item.Value.ToString(), out willTime))
            {
                var method = item.Operation;

                if (method == OperationType.Less || method == OperationType.LessOrEqual)
                {
                    method = OperationType.DateTimeLessThanOrEqual;
                    if (willTime.Hour == 0 && willTime.Minute == 0 && willTime.Second == 0)
                    {
                        willTime = willTime.AddDays(1).AddMilliseconds(-1);
                    }
                }
                object value = null;
                if (type == typeof(DateTime))
                {
                    value = willTime;
                }
                else if (type == typeof(int))
                {
                    value = (int)UnixTime.FromDateTime(willTime);
                }
                else if (type == typeof(long))
                {
                    value = UnixTime.FromDateTime(willTime);
                }
                return new[] { new Filter { Field = item.Field, Operation = method, Value = value} };
            }

            return new[] { new Filter{ Field = item.Field, Operation = item.Operation, Value =Convert.ChangeType(item.Value, type)} };
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class LikeTransformProvider : ITransformProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="binderType"></param>
        /// <returns></returns>
        public bool Match(Filter item, Type type)
        {
            return item.Operation == OperationType.Like;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="binderType"></param>
        /// <returns></returns>
        public IEnumerable<Filter> Transform(Filter item, Type type)
        {
            var str = item.Value.ToString();
            var keyWords = str.Split('*');
            if (keyWords.Length == 1)
            {
                return new[] { new Filter{ Field = item.Field, Operation = OperationType.Equal, Value = item.Value} };
            }
            var list = new List<Filter>();
            if (!string.IsNullOrEmpty(keyWords.First()))
                list.Add(new Filter { Field = item.Field, Operation = OperationType.BeginWith, Value = keyWords.First() });
            if (!string.IsNullOrEmpty(keyWords.Last()))
                list.Add(new Filter { Field = item.Field, Operation = OperationType.EndWith, Value = keyWords.Last() });
            for (int i = 1; i < keyWords.Length - 1; i++)
            {
                if (!string.IsNullOrEmpty(keyWords[i]))
                    list.Add(new Filter { Field = item.Field, Operation = OperationType.Contains, Value = keyWords[i] });
            }
            return list;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum ConjunctionOperator
    {
        /// <summary>
        /// 
        /// </summary>
        And,
        /// <summary>
        /// 
        /// </summary>
        Or
    }

    internal class QueryableSearcher<T>
    {
        public QueryableSearcher()
        {
        }
        public QueryableSearcher(IQueryable<T> table, IEnumerable<Filter> items)
        {
            Table = table;
            Items = items;
        }


        protected IEnumerable<Filter> Items { get; set; }

        protected IQueryable<T> Table { get; set; }

        public IQueryable<T> Search()
        {
            //构建 c=>Body中的c
            ParameterExpression param = Expression.Parameter(typeof(T), "c");
            //构建c=>Body中的Body
            var body = GetExpressoinBody(param, Items);
            //将二者拼为c=>Body
            var expression = Expression.Lambda<Func<T, bool>>(body, param);
            //传到Where中当做参数，类型为Expression<Func<T,bool>>
            return Table.Where(expression);
        }

        private Expression GetExpressoinBody(ParameterExpression param, IEnumerable<Filter> items)
        {
            var list = new List<Expression>();
            //OrGroup为空的情况下，即为And组合
            var andList = items.Where(c => string.IsNullOrEmpty(c.OrGroup));
            //将And的子Expression以AndAlso拼接
            if (andList.Count() != 0)
            {
                list.Add(GetGroupExpression(param, andList, Expression.AndAlso));
            }
            //其它的则为Or关系，不同Or组间以And分隔
            var orGroupByList = items.Where(c => !string.IsNullOrEmpty(c.OrGroup)).GroupBy(c => c.OrGroup);
            //拼接子Expression的Or关系
            foreach (IGrouping<string, Filter> group in orGroupByList)
            {
                if (group.Count() != 0)
                    list.Add(GetGroupExpression(param, group, Expression.OrElse));
            }
            //将这些Expression再以And相连
            return list.Aggregate(Expression.AndAlso);
        }

        private Expression GetGroupExpression(ParameterExpression param, IEnumerable<Filter> items, Func<Expression, Expression, Expression> func)
        {
            //获取最小的判断表达式
            var list = items.Select(item => GetExpression(param, item));
            //再以逻辑运算符相连
            return list.Aggregate(func);
        }

        private Expression GetExpression(ParameterExpression param, Filter item)
        {
            //属性表达式
            LambdaExpression exp = GetPropertyLambdaExpression(item, param);
            //如果有特殊类型处理，则进行处理，暂时不关注
            foreach (var provider in TransformProviders.Items)
            {
                if (provider.Match(item, exp.Body.Type))
                {
                    return GetGroupExpression(param, provider.Transform(item, exp.Body.Type), Expression.AndAlso);
                }


            }

            ITransformProvider orProvider = new OrGroupTransformProvider();
            if(orProvider.Match(item,exp.Body.Type))
                return GetGroupExpression(param, orProvider.Transform(item, exp.Body.Type), Expression.OrElse);
            //常量表达式
            var constant = ChangeTypeToExpression(item, exp.Body.Type);
            //以判断符或方法连接
            return ExpressionDict[item.Operation](exp.Body, constant);
        }

        private LambdaExpression GetPropertyLambdaExpression(Filter item, ParameterExpression param)
        {
            //获取每级属性如c.Users.Proiles.UserId
            var props = item.Field.Split('.');
            Expression memberAccess = param;
            var typeOfProp = typeof(T);
            int i = 0;
            do
            {
                MemberInfo member = typeOfProp.GetProperties().Where(p => string.Equals(props[i], p.Name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                if(member == null)
                   member = typeOfProp.GetFields().Where(p => string.Equals(props[i], p.Name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                
                if (member == null) throw new NotSupportedException(typeOfProp.FullName + " not contains property or field :" + props[i]);
                typeOfProp = member.GetMemberType();
                memberAccess = Expression.MakeMemberAccess(memberAccess, member);
                i++;
            } while (i < props.Length);

            return Expression.Lambda(memberAccess, param);
        }

        #region ChangeType

        /// <summary>
        /// 类型转换，支持非空类型与可空类型之间的转换
        /// </summary>
        /// <param name="value"></param>
        /// <param name="conversionType"></param>
        /// <returns></returns>
        public static object ChangeType(object value, Type conversionType)
        {
            if (value == null) return null;
            return Convert.ChangeType(value, TypeUtil.GetUnNullableType(conversionType));
        }

        /// <summary>
        /// 转换SearchItem中的Value的类型，为表达式树
        /// </summary>
        /// <param name="item"></param>
        /// <param name="conversionType">目标类型</param>
        public static Expression ChangeTypeToExpression(Filter item, Type conversionType)
        {
            if (item.Value == null) return Expression.Constant(item.Value, conversionType);
            #region 数组
            if (item.Operation == OperationType.StdIn)
            {
                var arr = (item.Value as Array);
                var expList = new List<Expression>();
                //确保可用
                if (arr != null)
                    for (var i = 0; i < arr.Length; i++)
                    {
                        //构造数组的单元Constant
                        var newValue = ChangeType(arr.GetValue(i), conversionType);
                        expList.Add(Expression.Constant(newValue, conversionType));
                    }
                //构造inType类型的数组表达式树，并为数组赋初值
                return Expression.NewArrayInit(conversionType, expList);
            }

            #endregion

            var elementType = TypeUtil.GetUnNullableType(conversionType);
            var value = Convert.ChangeType(item.Value, elementType);
            return Expression.Constant(value, conversionType);
        }

        #endregion

        #region SearchMethod 操作方法

        private static readonly Dictionary<OperationType, Func<Expression, Expression, Expression>> ExpressionDict;
        static QueryableSearcher()
        {
            ExpressionDict = new Dictionary<OperationType, Func<Expression, Expression, Expression>>();
            ExpressionDict[OperationType.Equal] = (left, right) => Expression.Equal(left, right);
            ExpressionDict[OperationType.Greater] = (left, right) => Expression.GreaterThan(left, right);
            ExpressionDict[OperationType.GreaterOrEqual] = (left, right) => Expression.GreaterThanOrEqual(left, right);
            ExpressionDict[OperationType.Less] = (left, right) => Expression.LessThan(left, right);
            ExpressionDict[OperationType.LessOrEqual] = (left, right) => Expression.LessThanOrEqual(left, right);
            ExpressionDict[OperationType.Contains] = (left, right) => Expression.Call(left, typeof(string).GetMethod("Contains"), right);
            ExpressionDict[OperationType.NotEqual] = (left, right) => Expression.NotEqual(left, right);
            ExpressionDict[OperationType.DateTimeLessThanOrEqual] = (left, right) => Expression.LessThanOrEqual(left, right);

            ExpressionDict[OperationType.NotBeginWith] = (left, right) => Expression.Not(ExpressionDict[OperationType.BeginWith](left, right));
            ExpressionDict[OperationType.NotEndWith] = (left, right) => Expression.Not(ExpressionDict[OperationType.EndWith](left, right));
            ExpressionDict[OperationType.NotContains] = (left, right) => Expression.Not(ExpressionDict[OperationType.Contains](left, right));
            ExpressionDict[OperationType.NotIn] = (left, right) => Expression.Not(ExpressionDict[OperationType.In](left, right));


            ExpressionDict[OperationType.StdIn] = (left, right) =>
            {
                if (!right.Type.IsArray) return null;
                //调用Enumerable.Contains扩展方法
                MethodCallExpression resultExp =
                    Expression.Call(
                        typeof(Enumerable),
                        "Contains",
                        new[] { left.Type },
                        right,
                        left);

                return resultExp;
            };

            ExpressionDict[OperationType.BeginWith] = (left, right) =>
            {
                if (left.Type != typeof(string))
                    return null;
                return Expression.Call(left, typeof(string).GetMethod("StartsWith", new[] { typeof(string) }), right);
            };
            ExpressionDict[OperationType.EndWith] = (left, right) =>
            {
                if (left.Type != typeof(string))
                    return null;
                return Expression.Call(left, typeof(string).GetMethod("EndsWith", new[] { typeof(string) }), right);
            };


        }


        #endregion
    }
    class TypeUtil
    {
        /// <summary>
        /// 获取可空类型的实际类型
        /// </summary>
        /// <param name="conversionType"></param>
        /// <returns></returns>
        public static Type GetUnNullableType(Type conversionType)
        {
            return conversionType.IsNullable() ? Nullable.GetUnderlyingType(conversionType) : conversionType;
        }
    }

    /// <summary>
    /// Unix 时间
    /// </summary>
    public class UnixTime
    {
        private static DateTime _baseTime = new DateTime(1970, 1, 1);

        /// <summary>
        /// 将unixtime转换为.NET的DateTime
        /// </summary>
        /// <param name="timeStamp">秒数</param>
        /// <returns>转换后的时间</returns>
        public static DateTime FromUnixTime(long timeStamp)
        {
            return new DateTime((timeStamp + 8 * 60 * 60) * 10000000 + _baseTime.Ticks);
            //return BaseTime.AddSeconds(timeStamp);
            //return new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(timeStamp);
        }

        /// <summary>
        /// 将.NET的DateTime转换为unix time
        /// </summary>
        /// <param name="dateTime">待转换的时间</param>
        /// <returns>转换后的unix time</returns>
        public static long FromDateTime(DateTime dateTime)
        {
            return (dateTime.Ticks - _baseTime.Ticks) / 10000000 - 8 * 60 * 60;
            //return (dateTime.Ticks - new DateTime(1970, 1, 1, 0, 0, 0).Ticks) / 10000000;
        }
    }
}
