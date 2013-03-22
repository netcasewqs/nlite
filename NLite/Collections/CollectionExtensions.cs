using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace NLite.Collections
{
    /// <summary>
    /// 
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item1"></param>
        /// <param name="item2"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static T[] Combine<T>(this T item1, T item2, params T[] items)
        {
            if (item1 == null && item2 == null)
                throw new ArgumentNullException("item1 and item2");

            if (item1 == null)
                return Combine<T>(item2, items);

            if (item2 == null)
                return Combine<T>(item1, items);

            //if we reached here then item1 and item2 are not null
            if (items == null)
            {
                return new T[2] { item1, item2 };
            }
            else
            {
                T[] combination = new T[items.Length + 2];
                combination[0] = item1;
                combination[1] = item2;
                for (int i = 2; i < combination.Length; i++)
                {
                    combination[i] = items[i - 2];
                }
                return combination;
            }
        }
        /// <summary>
        /// combination helper method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static T[] Combine<T>(this T item, params T[] items)
        {
            if (items == null)
            {
                return new T[1] { item };
            }
            else
            {
                T[] combination = new T[items.Length + 1];
                combination[0] = item;
                for (int i = 1; i < combination.Length; i++)
                {
                    combination[i] = items[i - 1];
                }
                return combination;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public static bool Exists<T>(this IQueryable<T> queryable)
        {
            return queryable.Count() != 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="coll"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static bool TrueForAll<T>(this IEnumerable<T> coll, Predicate<T> predicate)
        {
            Trace.Assert(coll != null, "coll == null");
            Trace.Assert(predicate != null, "predicate == null");

            using(var it = coll.GetEnumerator())
            while (it.MoveNext())
            {
                if (!predicate(it.Current))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="coll"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this IEnumerable<T> coll, Action<T> action)
        {
            Trace.Assert(coll != null, "coll == null");
            Trace.Assert(action != null, "action == null");

            var it = coll.GetEnumerator();
            while (it.MoveNext())
                action(it.Current);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="coll"></param>
        /// <param name="converter"></param>
        /// <returns></returns>
        public static IEnumerable<TOutput> ConvertAll<TInput, TOutput>(this IEnumerable<TInput> coll, Converter<TInput, TOutput> converter)
        {
            Trace.Assert(coll != null, "coll == null");
            Trace.Assert(converter != null, "converter == null");

            return from input in coll
                   select converter(input);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="coll"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static bool Exist<T>(this IEnumerable<T> coll, Predicate<T> predicate)
        {
            Trace.Assert(coll != null, "coll == null");
            Trace.Assert(predicate != null, "predicate == null");

            var it = coll.GetEnumerator();
            while (it.MoveNext())
                if (predicate(it.Current))
                    return true;

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool In<T>(this T t, IEnumerable<T> c)
        {
            return c.Any(i => i.Equals(t));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="delim"></param>
        /// <returns></returns>
        public static string ToCSV<T>(this IEnumerable<T> collection, string delim)
        {
            if (collection == null)
            {
                return "";
            }

            StringBuilder result = new StringBuilder();
            foreach (T value in collection)
            {
                result.Append(value);
                result.Append(delim);
            }
            if (result.Length > 0)
            {
                result.Length -= delim.Length;
            }
            return result.ToString();
        }

       
    }
}
