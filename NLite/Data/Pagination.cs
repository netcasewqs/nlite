/*
 * Created by SharpDevelop.
 * User: netcasewqs@gmail.com
 * Date: 2010-9-20
 * Time: 18:08
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using NLite;
using NLite.Dynamic;
using NLite.Binding;
using NLite.Reflection;
using System.Linq.Expressions;
using System.Reflection;
using NLite.Internal;
using System.Data;
using System.ComponentModel;

namespace NLite
{
    /// <summary>
    /// 排序标志
    /// </summary>
    public enum SortOrder
    {
        /// <summary>
        /// 升序
        /// </summary>
        Ascending,
        /// <summary>
        /// 降序
        /// </summary>
        Descending,
    }
}
namespace NLite.Data
{
    /// <summary>
    /// 分页记录集接口
    /// </summary>
    public interface IPagination : IEnumerable
    {
        /// <summary>
        /// 总记录数
        /// </summary>
        int TotalRowCount { get; }
        /// <summary>
        /// 页面大小
        /// </summary>
        int PageSize { get; }
        /// <summary>
        /// 起始行索引号（从0开始）
        /// </summary>
        int StartRowIndex { get; }
        /// <summary>
        /// 页面索引号（从0开始）
        /// </summary>
        int PageIndex { get; }
        /// <summary>
        /// 页面编号（从1开始）
        /// </summary>
        int PageNumber { get; }
        /// <summary>
        /// 当前页面记录数
        /// </summary>
        int Length { get; }
        /// <summary>
        /// 总页面数 
        /// </summary>
        int PageCount { get; }
        /// <summary>
        /// 是否可以进行分页（分页条件是PageCount > 1)
        /// </summary>
        bool HasPagination { get; }
        /// <summary>
        /// 是否支持上一页
        /// </summary>
        bool HasPrevious { get; }
        /// <summary>
        /// 是否支持下一页
        /// </summary>
        bool HasNext { get; }
        /// <summary>
        /// 是否首页
        /// </summary>
        bool HasFirst { get; }
        /// <summary>
        /// 是否末页
        /// </summary>
        bool HasLast { get; }
    }

    /// <summary>
    /// 泛型分页记录集接口
    /// </summary>
    public interface IPagination<T> : IPagination, IEnumerable<T>
    {

    }

    public class Pagination : IPagination
    {
        protected readonly object[] _collection;

        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalRowCount { get; private set; }
        /// <summary>
        /// 起始行索引号（从0开始）
        /// </summary>
        public int StartRowIndex { get; private set; }
        /// <summary>
        /// 页面大小
        /// </summary>
        public int PageSize { get; private set; }
        /// <summary>
        /// 页面索引号（从0开始）
        /// </summary>
        public int PageIndex { get; private set; }
        /// <summary>
        /// 页面编号（从1开始）
        /// </summary>
        public int PageNumber { get { return PageIndex + 1; } }
        /// <summary>
        /// 总页面数 
        /// </summary>
        public int PageCount { get; private set; }
        /// <summary>
        /// 当前页面记录数
        /// </summary>
        public int Length { get; private set; }
        /// <summary>
        /// 是否可以进行分页（分页条件是PageCount > 1)
        /// </summary>
        public bool HasPagination { get; private set; }
        /// <summary>
        /// 是否支持上一页
        /// </summary>
        public bool HasPrevious
        {
            get
            {
                if (!HasPagination || PageCount <= 2) return false;
                return PageIndex != 0 && PageIndex != 1;
            }
        }
        /// <summary>
        /// 是否支持下一页
        /// </summary>
        public bool HasNext { get { return HasPagination && PageIndex < PageCount - 2; } }
        /// <summary>
        /// 是否首页
        /// </summary>
        public bool HasFirst
        {
            get
            {
                //return HasPagination && PageIndex > 0 && PageIndex <= PageCount - 1;
                return HasPagination && PageCount - PageIndex < PageCount;
            }
        }
        /// <summary>
        /// 是否末页
        /// </summary>
        public bool HasLast
        {
            get
            {
                if (!HasPagination)
                    return false;
                return PageCount - PageIndex != 1;
            }
        }


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <modelExp name="collection">Local 数据源</modelExp>
        /// <modelExp name="pageIndex">页面索引号，下标从0开始</modelExp>
        /// <modelExp name="pageSize">页面大小</modelExp>
        public Pagination(IEnumerable dataSource, int pageIndex, int pageSize, int rowCount)
        {
            Guard.NotNull(dataSource, "collection");
            if (pageIndex < 0)
                throw new ArgumentException("Page index must be greater than or equals 0 at least.");
            if (pageSize <= 0)
                throw new ArgumentException("Page size must be greater than 1 at least.");


            PageSize = pageSize;
            PageIndex = pageIndex;

            PageSize = pageSize;
            PageIndex = pageIndex;
            StartRowIndex = pageIndex * pageSize;
            TotalRowCount = rowCount;
            PageCount = TotalRowCount / pageSize;
            if (TotalRowCount % pageSize != 0)
                PageCount = PageCount + 1;
            HasPagination = PageCount > 1;
            
            _collection = dataSource.Cast<object>().ToArray();

            Length = _collection.Length;
        }

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        #endregion
    }

    class DataTableAdapter : IEnumerable
    {
        private IListSource Target;

        public DataTableAdapter(DataTable table)
        {
            Target = table as IListSource;
        }
        public IEnumerator GetEnumerator()
        {
            return Target.GetList().GetEnumerator();
        }
    }

    /// <summary>
    /// 泛型分页记录集对象
    /// </summary>
    [Serializable]
    public class Pagination<T> : Pagination,IPagination<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataSource">Remote 数据源</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        public Pagination(IQueryable<T> dataSource, int pageIndex, int pageSize) : base(dataSource.Skip(pageIndex * pageSize).Take(pageSize).ToArray(), pageIndex, pageSize, dataSource.Count()) { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <modelExp name="collection">Local 数据源</modelExp>
        /// <modelExp name="pageIndex">页面索引号，下标从0开始</modelExp>
        /// <modelExp name="pageSize">页面大小</modelExp>
        public Pagination(IEnumerable<T> dataSource, int pageIndex, int pageSize, int rowCount)
            : base(dataSource, pageIndex, pageSize, rowCount)
        {
        }

        #region IEnumerable<T> Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _collection.Cast<T>().GetEnumerator();
        }

        #endregion

        /// <summary>
        /// 空记录集对象
        /// </summary>
        public static readonly Pagination<T> Empty = new Pagination<T>(Enumerable.Empty<T>().AsQueryable(), 0, 10,0);
    }

    /// <summary>
    /// 集合的扩展类
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        /// 将数据源转会为分页记录集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <modelExp name="dataSource">Remote数据源</modelExp>
        /// <modelExp name="pageIndex">页面索引号（下标从0开始）</modelExp>
        /// <modelExp name="pageSize">页面大小</modelExp>
        /// <returns></returns>
        public static IPagination<T> ToPagination<T>(this IQueryable<T> dataSource, int pageIndex, int pageSize)
        {
            if (dataSource == null)
                throw new ArgumentNullException("dataSource");
            return new Pagination<T>(dataSource, pageIndex, pageSize);
        }

        /// <summary>
        /// 将数据源转会为分页记录集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <modelExp name="dataSource">Local数据源</modelExp>
        /// <modelExp name="pageIndex">页面索引号（下标从0开始）</modelExp>
        /// <modelExp name="pageSize">页面大小</modelExp>
        /// <returns></returns>
        public static IPagination<T> ToPagination<T>(this IEnumerable<T> dataSource, int pageIndex, int pageSize, int rowCount)
        {
            if (dataSource == null)
                throw new ArgumentNullException("dataSource");
            return new Pagination<T>(dataSource, pageIndex, pageSize,rowCount);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataTable">local datatable</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="rowCount"></param>
        /// <returns></returns>
        public static Pagination ToPaination(this DataTable dataTable, int pageIndex, int pageSize, int rowCount)
        {
            Guard.NotNull(dataTable, "dataTable");
            return new Pagination(new DataTableAdapter(dataTable), pageIndex, pageSize, rowCount);
        }

       

        /// <summary>
        /// 对数据源进行排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <modelExp name="dataSource">Remote数据源</modelExp>
        /// <modelExp name="propertyName">排序列</modelExp>
        /// <modelExp name="sortOrder">排序方向</modelExp>
        /// <returns></returns>
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> dataSource, string propertyName, SortOrder sortOrder)
        {
            if (dataSource == null)
                throw new ArgumentNullException("dataSource");
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException("propertyName");

            var type = typeof(T);
            MemberInfo member = type.GetProperty(propertyName);
            if (member == null)
                member = type.GetField(propertyName);
            if (member == null)
                member = type.GetProperties().Where(p => string.Equals(propertyName, p.Name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            if (member == null)
                member = type.GetFields().Where(p => string.Equals(propertyName, p.Name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            if (member == null)
                throw new NotSupportedException(type.FullName + " not contains property or field :" + propertyName);

            var param = Expression.Parameter(type, "p");
            Expression propertyAccessExpression = Expression.MakeMemberAccess(param, member);
            var orderByExpression = Expression.Lambda(propertyAccessExpression, param);
            var methodName = sortOrder == SortOrder.Ascending ? "OrderBy" : "OrderByDescending";
            var resultExp = Expression.Call(typeof(Queryable), methodName, new Type[] { type, member.GetMemberType() },
                                            dataSource.Expression, Expression.Quote(orderByExpression));
            return dataSource.Provider.CreateQuery<T>(resultExp);
        }

    

        /// <summary>
        /// 使IQueryable支持动态查询
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <modelExp name="dataSource">Remote 数据源</modelExp>
        /// <modelExp name="filters">filters对象</modelExp>
        /// <returns></returns>
        public static IQueryable<TEntity> Where<TEntity>(this IQueryable<TEntity> dataSource, IEnumerable<Filter> filters)
        {
            return Where<TEntity>(dataSource, filters, "");
        }


        /// <summary>
        /// 使IQueryable支持动态查询
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <modelExp name="dataSource">Remote 数据源</modelExp>
        /// <modelExp name="filters">filters对象</modelExp>
        /// <modelExp name="prefix">使用前缀区分查询条件</modelExp>
        /// <returns></returns>
        public static IQueryable<TEntity> Where<TEntity>(this IQueryable<TEntity> dataSource, IEnumerable<Filter> filters, string prefix)
        {
            if (dataSource == null)
                throw new ArgumentNullException("dataSource");

            if (filters == null)
                throw new ArgumentNullException("filters");

            var filterItems =
                string.IsNullOrEmpty(prefix)
                    ? filters.Where(c => string.IsNullOrEmpty(c.Prefix)).ToArray()
                    : filters.Where(c => c.Prefix == prefix).ToArray();
            if (filterItems.Length == 0) return dataSource;
            return new QueryableSearcher<TEntity>(dataSource, filterItems).Search();
        }

      


        /// <summary>
        /// 将数据源转会为分页记录集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <modelExp name="dataSource">Remote数据源</modelExp>
        /// <modelExp name="ctx">查询上下文对象</modelExp>
        /// <returns></returns>
        public static IPagination<T> ToPagination<T>(this IQueryable<T> dataSource, QueryContext ctx)
        {
            var list = dataSource;
            if (ctx.Data != null && ctx.Data.Count > 0)
                list = list.Where(ctx.Data);
            if (ctx.Property.HasValue())
                list = list.OrderBy(ctx.Property, ctx.SortOrder);

            var pageSize = ctx.PageSize ?? 10;
            var pageIndex = ctx.PageIndex ?? 0;
            return list.ToPagination(pageIndex, pageSize);
        }
    }





    /// <summary>
    /// 查询上下文
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    [Serializable]
    public class QueryContext<TData>
    {
        /// <summary>
        /// 页面大小
        /// </summary>
        public int? PageSize { get; set; }
        /// <summary>
        /// 页面索引号
        /// </summary>
        public int? PageIndex { get; set; }
        /// <summary>
        /// 排序属性
        /// </summary>
        public string Property { get; set; }
        /// <summary>
        /// 排序方向
        /// </summary>
        public SortOrder SortOrder { get; set; }
        /// <summary>
        /// 查询条件数据
        /// </summary>
        public TData Data { get; set; }
    }

    /// <summary>
    /// 查询上下文
    /// </summary>
    [Serializable]
    public class QueryContext : QueryContext<List<Filter>>
    {
        public QueryContext()
        {
            Data = new List<Filter>();
        }
    }

    /// <summary>
    /// 缺省查询参数设置
    /// </summary>
    public class DefaultQuerySettings
    {
        /// <summary>
        /// 配置查询的缺省参数设置
        /// </summary>
        public static void Configure()
        {
            Configure(new DefaultQuerySettings());
        }
        /// <summary>
        /// 配置查询的缺省参数设置
        /// </summary>
        /// <modelExp name="setting"></modelExp>
        public static void Configure(DefaultQuerySettings setting)
        {
            if (setting == null)
                throw new ArgumentNullException("setting");
            if (string.IsNullOrEmpty(setting.PageSizeName))
                throw new ArgumentNullException("setting.PageSizeName");
            if (string.IsNullOrEmpty(setting.PageIndexName))
                throw new ArgumentNullException("setting.PageIndexName");
            if (string.IsNullOrEmpty(setting.SortPropertyName))
                throw new ArgumentNullException("setting.SortPropertyName");
            if (string.IsNullOrEmpty(setting.SortOrderName))
                throw new ArgumentNullException("setting.SortOrderName");
            if (string.IsNullOrEmpty(setting.AscendingValue))
                throw new ArgumentNullException("setting.AscendingValue");
            if (string.IsNullOrEmpty(setting.DescendingValue))
                throw new ArgumentNullException("setting.DescendingValue");
            if (setting.PageSize == 0)
                throw new ArgumentException("setting.PageSize 必须大于0");

            var binder = new QueryContextBinder(setting);
            ModelBinders.RegisterBinder<QueryContext, QueryContextBinder>(binder);
        }

        /// <summary>
        ///  设置缺省页面大小为，默认为10
        /// </summary>
        public uint PageSize = 10;
        /// <summary>
        /// 缺省升序值，默认为"asc"
        /// </summary>
        public string AscendingValue = "asc";
        /// <summary>
        /// 缺省降序值，默认为"asc"
        /// </summary>
        public string DescendingValue = "desc";
        /// <summary>
        /// 页面大小参数名称，默认ps
        /// </summary>
        public string PageSizeName = "ps";
        /// <summary>
        /// 页面索引号参数名称，默认pi
        /// </summary>
        public string PageIndexName = "pi";
        /// <summary>
        /// 排序列参数名称，默认sp
        /// </summary>
        public string SortPropertyName = "sp";
        /// <summary>
        /// 排序方向参数名称，默认so
        /// </summary>
        public string SortOrderName = "so";

        /// <summary>
        /// 页面索引号是否从0开始，默认是true
        /// </summary>
        public bool PageIndexIsFromZero = true;
    }
    /// <summary>
    /// 查询上下文模型绑定器
    /// </summary>
    class QueryContextBinder : IModelBinder
    {
        private readonly DefaultQuerySettings Setting;

        /// <summary>
        /// 构造函数
        /// </summary>
        internal QueryContextBinder(DefaultQuerySettings setting)
        {
            Setting = setting;
        }


        void ParsePageSize(IDictionary<string, object> valueProvider, QueryContext ctx)
        {
            var strValue = valueProvider
               .FirstOrDefault(p => string.Equals(p.Key, Setting.PageSizeName, StringComparison.InvariantCultureIgnoreCase))
               .Value as string;

            int i;
            if (int.TryParse(strValue, out i))
                ctx.PageSize = i;
            if (!ctx.PageSize.HasValue)
                ctx.PageSize =(int)Setting.PageSize;

        }

        void ParsePageIndex(IDictionary<string, object> valueProvider, QueryContext ctx)
        {
            var strValue = valueProvider
               .FirstOrDefault(p => string.Equals(p.Key, Setting.PageIndexName, StringComparison.InvariantCultureIgnoreCase))
               .Value as string;

            int i;
            if (int.TryParse(strValue, out i))
                ctx.PageIndex = i;

            //if (!Setting.PageIndexIsFromZero && i > 1)
            //    ctx.PageIndex--;
            if (!Setting.PageIndexIsFromZero)//如果从1开始那么那么页面索引号-1
                ctx.PageIndex -= 1;
        }

        /// <summary>
        /// 模型绑定
        /// </summary>
        /// <modelExp name="bindingInfo"></modelExp>
        /// <modelExp name="valueProvider"></modelExp>
        /// <returns></returns>
        object IModelBinder.BindModel(BindingInfo info, IDictionary<string, object> valueProvider)
        {
            var ctx = new QueryContext();

            ParsePageSize(valueProvider, ctx);
            ParsePageIndex(valueProvider, ctx);
            ParseSortProperty(valueProvider, ctx);

            ctx.Data = new QueryModelBinder().BindModel(info, valueProvider) as QueryModel;

            return ctx;
        }

        /// <summary>
        /// 解析排序列参数
        /// </summary>
        /// <modelExp name="valueProvider"></modelExp>
        /// <modelExp name="ctx"></modelExp>
        void ParseSortProperty(IDictionary<string, object> valueProvider, QueryContext ctx)
        {
            var strValue = valueProvider
               .FirstOrDefault(p => string.Equals(p.Key, Setting.SortPropertyName, StringComparison.InvariantCultureIgnoreCase))
               .Value as string;
            ctx.Property = strValue as string;
            if (ctx.Property.HasValue())
            {
                strValue = valueProvider
              .FirstOrDefault(p => string.Equals(p.Key, Setting.SortOrderName, StringComparison.InvariantCultureIgnoreCase))
              .Value as string;
                ParseSortOrder(ctx, strValue);
            }
        }

        /// <summary>
        /// 解析排序方向
        /// </summary>
        /// <modelExp name="ctx"></modelExp>
        /// <modelExp name="strValue"></modelExp>
        void ParseSortOrder(QueryContext ctx, string strValue)
        {
            if (string.Equals(strValue, Setting.AscendingValue, StringComparison.InvariantCultureIgnoreCase))
                ctx.SortOrder = SortOrder.Ascending;
            else if (string.Equals(strValue, Setting.DescendingValue, StringComparison.InvariantCultureIgnoreCase))
                ctx.SortOrder = SortOrder.Descending;
        }
    }

}

