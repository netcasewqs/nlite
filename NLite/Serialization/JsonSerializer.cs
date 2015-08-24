using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite.Serialization
{
    /// <summary>
    /// Json序列化器
    /// </summary>
    public class JsonSerializer : ISerializer
    {
        private static ISerializer defaultInstance = new JsonSerializer();

        /// <summary>
        /// 得到当前的序列化器
        /// </summary>
        public static ISerializer Current { get { return defaultInstance; } }
        /// <summary>
        /// 设置序列化器为当前的
        /// </summary>
        /// <param name="serializer"></param>
        public static void SetSerializer(ISerializer serializer)
        {
            if (serializer == null)
                throw new ArgumentNullException("serializer");

            defaultInstance = serializer;
        }

        /// <summary>
        /// 把一个对象序列化到一个串种
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public string Serialize(object o)
        {
            return SimpleJson.SerializeObject(o);
        }

        /// <summary>
        /// 把一个串中的内容反序列化到特定类型的对象中
        /// </summary>
        /// <param name="str"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public object Deserialize(string str, Type type)
        {
            return SimpleJson.DeserializeObject(str, type);
        }
    }

}
