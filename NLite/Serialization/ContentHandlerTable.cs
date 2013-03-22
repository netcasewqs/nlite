using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite.Serialization
{
    /// <summary>
    /// 内容类型处理器映射表
    /// </summary>
    public sealed class ContentHandlerTable
    {
        private static IDictionary<string, ISerializer> items;

        static ContentHandlerTable()
        {
            items = new Dictionary<string, ISerializer>(StringComparer.InvariantCultureIgnoreCase);

            //register default handlers
            items.Add("application/json", JsonSerializer.Current);
            items.Add("application/xml", XmlSerializer.Current);
            items.Add("text/json", JsonSerializer.Current);
            items.Add("text/x-json", JsonSerializer.Current);
            items.Add("text/javascript", JsonSerializer.Current);
            items.Add("text/xml", XmlSerializer.Current);
            items.Add("json", JsonSerializer.Current);
            items.Add("xml", XmlSerializer.Current);
            items.Add("*", JsonSerializer.Current);
        }

        /// <summary>
        /// 类型处理器字典
        /// </summary>
        public static IDictionary<string, ISerializer> Items { get { return items; } }

        /// <summary>
        /// 得到特定类型的处理器
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static ISerializer GetHandler(string contentType)
        {
            if (string.IsNullOrEmpty(contentType) && items.ContainsKey("*"))
            {
                return items["*"];
            }

            var semicolonIndex = contentType.IndexOf(';');
            if (semicolonIndex > -1) contentType = contentType.Substring(0, semicolonIndex);
            ISerializer handler = null;
            if (items.ContainsKey(contentType))
            {
                handler = items[contentType];
            }
            else if (items.ContainsKey("*"))
            {
                handler = items["*"];
            }
            if (handler == null)
                handler = XmlSerializer.Current;
            return handler;
        }

    }

   
   
}
