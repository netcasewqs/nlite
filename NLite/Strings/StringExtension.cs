using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Globalization;
using System.Linq;
using NLite.Collections;
using NLite.Internal;

namespace NLite
{
    /// <summary>
    /// 
    /// </summary>
    public static class StringExtensions
    {
        private const string AntExpressionPrefix = "${";
        private const string AntExpressionSuffix = "}";
        private const string RegMetadatas = @"$\.*+?|(){}^";
        private const string RegExpressionWithPrefix = @"(?<!\\{0}*)(?<={0})(?<exp>\w+)";
        private const string GroupName = "exp";

        /// <summary>
        /// Check that a string is not null or empty
        /// </summary>
        /// <param name="input">String to check</param>
        /// <returns>bool</returns>
        public static bool HasValue(this string input)
        {
            return !string.IsNullOrEmpty(input);
        }
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string input)
        {
            return string.IsNullOrEmpty(input);
        }

        /// <summary>
        /// Returns the name of an element with the namespace if specified
        /// </summary>
        /// <param name="name">Element name</param>
        /// <param name="namespace">XML Namespace</param>
        /// <returns></returns>
        internal static XName AsNamespaced(this string name, string @namespace)
        {
            XName xName = name;

            if (@namespace.HasValue())
                xName = XName.Get(name, @namespace);

            return xName;
        }

        internal static XName AsXName(this string name)
        {
            return XName.Get(name);
        }

        /// <summary>
        /// Remove underscores from a string
        /// </summary>
        /// <param name="input">String to process</param>
        /// <returns>string</returns>
        public static string RemoveUnderscoresAndDashes(this string input)
        {
            Guard.NotNullOrEmpty(input, "input");
            return input.Replace("_", "").Replace("-", ""); // avoiding regex
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="newType"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static object ChangeType(this object source, Type newType, CultureInfo culture)
        {
            Guard.NotNull(newType, "newType");
            return Convert.ChangeType(source, newType, culture);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static String Format(this String format, params object[] args)
        {
            if (format == null || args == null)
                throw new ArgumentNullException((format == null) ? "format" : "args");

            var capacity= format.Length + args.Where(p => p != null).Select(p => p.ToString()).Sum(p => p.Length);
            var stringBuilder = new StringBuilder(capacity);
            stringBuilder.AppendFormat( format, args);
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 字符串首字母小写
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static string FirstLower(this string src)
        {
            Guard.NotNullOrEmpty(src, "scr");
            return src[0].ToString().ToLower() + src.Remove(0, 1);
        }

        /// <summary>
        /// 字符串首字母大写
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static string FirstUpper(this string src)
        {
            Guard.NotNullOrEmpty(src, "scr");
            return src[0].ToString().ToUpper() + src.Remove(0, 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="target"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static string Surround(string prefix, string target, string suffix)
        {
            return string.Format("{0}{1}{2}", prefix, target, suffix);
        }

        /// <summary>
        /// 返回所有匹配指定前缀的字符串
        /// </summary>
        public static string[] Matches(this string src, char prefix)
        {

            Guard.NotNullOrEmpty(src, "scr");

            #region Regex

            var regexExp = string.Format(RegExpressionWithPrefix,
                RegMetadatas.IndexOf(prefix) != -1 ? "\\" + prefix : prefix.ToString());

            var matches = Regex.Matches(src, regexExp, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            var expressions = new List<string>();
            foreach (Match match in matches)
            {
                if (expressions.IndexOf(match.Groups["exp"].Value) < 0)
                {
                    expressions.Add(match.Groups[GroupName].Value);
                }
            }

            return expressions.ToArray();

            #endregion

        }

        static readonly string[] EmptyStringArray = new string[0];
        /// <summary>
        /// 返回所有匹配指定前缀与后缀的字符串
        /// </summary>
        /// <param name="src"></param>
        /// <param name="prefix"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static string[] Matches(this string src, string prefix, string suffix)
        {
            Guard.NotNullOrEmpty(src, "scr");
            Guard.NotNullOrEmpty(prefix, "prefix");

            var expressions = new List<string>();
            var start = src.IndexOf(prefix);

            while (start >= 0)
            {
                int end = src.IndexOf(suffix, start + prefix.Length);

                if (end == -1)
                {
                    //没有找到则退出循环。。。
                    start = -1;
                }
                else
                {
                    //求蚂蚁表达式
                    string exp = src.Substring(start + prefix.Length, end - start - prefix.Length);

                    if (String.IsNullOrEmpty(exp))
                    {
                        throw new FormatException(string.Format("Empty {0}{1} value found in text : '{2}'.",
                            prefix,
                            suffix,
                            src));
                    }

                    exp = exp.Trim();
                    if (expressions.IndexOf(exp) < 0)
                    {
                        expressions.Add(exp);
                    }
                    start = src.IndexOf(prefix, end);
                }
            }
            return expressions.ToArray();
        }

        /// <summary>
        /// 返回所有匹配前缀为'${'，后缀为'}'的蚂蚁表达式
        /// </summary>
        /// <exception cref="System.FormatException">
        /// 如果提供的表达式为空(<c>${}</c>).
        /// </exception>
        public static string[] MatchAntExpressions(this string src)
        {
            Guard.NotNullOrEmpty(src, "scr");

            return Matches(src, AntExpressionPrefix, AntExpressionSuffix);
        }


        /// <summary>
        /// 将指定字符串中的蚂蚁表达式替换成指定的内容
        /// </summary>
        /// <param name="src"></param>
        /// <param name="expression">蚂蚁表达式</param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public static string SetAntExpression(string src, string expression, object newValue)
        {
            Guard.NotNullOrEmpty(src, "scr");
            if (newValue == null)
            {
                newValue = String.Empty;
            }
            return src.Replace(Surround(AntExpressionPrefix, expression, AntExpressionSuffix), newValue.ToString());
        }

        /// <summary>
        ///返回匹配项在指定字符串中第一次出现位置左边的内容
        /// </summary>
        public static string LeftOf(this string src, string value)
        {
            Guard.NotNullOrEmpty(src, "scr");
            string ret = src;
            int idx = src.IndexOf(value);
            if (idx != -1)
            {
                ret = src.Substring(0, idx);
            }
            return ret;
        }


        /// <summary>
        ///返回匹配项在指定字符串中第一次出现位置左边的内容
        /// </summary>
        public static string LeftOf(this string src, char value)
        {
            Guard.NotNullOrEmpty(src, "scr");
            return LeftOf(src, value.ToString());
        }

        /// <summary>
        /// 返回匹配项在指定字符串中第‘n’次出现位置左边的内容
        /// </summary>
        public static string LeftOf(this string src, string value, int n)
        {
            Guard.NotNullOrEmpty(src, "scr");
            Guard.NotNullOrEmpty(value, "value");

            string ret = src;
            int idx = -1;
            while (n > 0)
            {
                idx = src.IndexOf(value, idx + 1);
                if (idx == -1)
                {
                    break;
                }
                --n;
            }
            if (idx != -1)
            {
                ret = src.Substring(0, idx);
            }
            return ret;
        }

        /// <summary>
        /// 返回匹配项在指定字符串中第‘n’次出现位置左边的内容
        /// </summary>
        public static string LeftOf(this string src, char value, int n)
        {
            Guard.NotNullOrEmpty(src, "scr");
            return LeftOf(src, value.ToString(), n);
        }

        /// <summary>
        /// 返回匹配项在指定字符串中第一次出现位置右边的内容
        /// </summary>
        public static string RightOf(this string src, string value)
        {
            Guard.NotNullOrEmpty(src, "scr");
            Guard.NotNullOrEmpty(value, "value");

            string ret = String.Empty;
            int idx = src.IndexOf(value);
            if (idx != -1)
            {
                idx += value.Length - 1;
                ret = src.Substring(idx + 1);
            }
            return ret;
        }

        /// <summary>
        /// 返回匹配项在指定字符串中第一次出现位置右边的内容
        /// </summary>
        public static string RightOf(this string src, char value)
        {
            Guard.NotNullOrEmpty(src, "scr");
            return RightOf(src, value.ToString());
        }

        /// <summary>
        /// 返回匹配项在指定字符串中第n次出现位置右边的内容
        /// </summary>
        public static string RightOf(this string src, string value, int n)
        {
            Guard.NotNullOrEmpty(src, "scr");

            string ret = String.Empty;
            int idx = -1;
            while (n > 0)
            {
                idx = src.IndexOf(value, idx + 1);
                if (idx == -1)
                {
                    break;
                }
                --n;
            }

            if (idx != -1)
            {
                ret = src.Substring(idx + 1);
            }

            return ret;
        }

        /// <summary>
        /// 返回字符在指定字符串中第n次出现位置右边的内容
        /// </summary>
        public static string RightOf(this string src, char value, int n)
        {
            Guard.NotNullOrEmpty(src, "scr");
            return RightOf(src, value.ToString(), n);
        }

        /// <summary>
        /// 返回字符串中两个匹配项（字符）间的子串
        /// </summary>
        public static string Between(this string src, char start, char end)
        {
            Guard.NotNullOrEmpty(src, "scr");
            return Between(src, start.ToString(), end.ToString());
        }

        /// <summary>
        /// 返回字符串中两个匹配项（字符串）间的子串
        /// </summary>
        public static string Between(this string src, string start, string end)
        {
            Guard.NotNullOrEmpty(src, "scr");
            Guard.NotNullOrEmpty(start, "start");
            Guard.NotNullOrEmpty(end, "end");

            string ret = String.Empty;
            if (String.IsNullOrEmpty(src))
                return ret;

            int idxStart = src.IndexOf(start);
            if (idxStart != -1)
            {
                ++idxStart;
                int idxEnd = src.IndexOf(end, idxStart);
                if (idxEnd != -1)
                {
                    ret = src.Substring(idxStart, idxEnd - idxStart);
                }
            }
            return ret;
        }

        /// <summary>
        /// 返回字符在字符串中出现的次数
        /// </summary>
        /// <param name="src"></param>
        /// <param name="c"></param>
        /// <param name="flag">是否处理转义字符</param>
        /// <returns></returns>
        private static int Count(this string src, char c, bool flag)
        {
            int n = 0;
            for (int i = 0; i < src.Length; i++)
            {
                if (src[i].Equals(c))
                {
                    if (flag) // 如果处理转义
                    {
                        if (0 != i && src[i - 1] != '\\')//判断找到字符的前一个字符不是转义字符则++
                        {
                            n++;
                            continue;
                        }
                        else //反之，判断找到字符紧邻右边的字符是不是一样的，如果是则过滤掉
                        {
                            for (i++; i < src.Length; i++)
                            {
                                if (!src[i].Equals(c)) break;
                            }
                        }
                    }
                    else n++;
                }
            }
            return n;
        }

        /// <summary>
        /// 返回字符在字符串中出现的次数
        /// </summary>
        public static int Count(this string src, char c)
        {
            Guard.NotNullOrEmpty(src, "scr");

            return Count(src, c, false);
        }

        /// <summary>
        /// 返回字符在指定字符串中最后一次次出现位置的左边内容
        /// </summary>
        public static string LastLeftOf(this string src, char c)
        {
            Guard.NotNullOrEmpty(src, "scr");

            string ret = src;
            int idx = src.LastIndexOf(c);
            if (idx != -1)
            {
                ret = src.Substring(0, idx);
            }
            return ret;
        }

        /// <summary>
        /// 返回字符在指定字符串中最后一次次出现位置的右边内容
        /// </summary>
        public static string LastRightOf(this string src, char c)
        {
            Guard.NotNullOrEmpty(src, "scr");
            string ret = String.Empty;
            int idx = src.LastIndexOf(c);
            if (idx != -1)
            {
                ret = src.Substring(idx + 1);
            }
            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] GetBytes(this string str)
        {
            Guard.NotNullOrEmpty(str, "str");
            return Encoding.Default.GetBytes(str);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] GetBytesUTF8(this string str)
        {
            Guard.NotNullOrEmpty(str, "str");
            return Encoding.UTF8.GetBytes(str);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] GetBytesASCII(this string str)
        {
            Guard.NotNullOrEmpty(str, "str");
            return Encoding.ASCII.GetBytes(str);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static string GetString(this byte[] arr)
        {
            Guard.NotNull(arr, "arr");
            return GetString(arr, 0, arr.Length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="offset"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string GetString(this byte[] arr, int offset, int len)
        {
            Guard.NotNull(arr, "arr");
            return Encoding.Default.GetString(arr, offset, len);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static string GetStringUTF8(this byte[] arr)
        {
            Guard.NotNull(arr, "arr");
            return GetStringUTF8(arr, 0, arr.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="offset"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string GetStringUTF8(this byte[] arr, int offset, int len)
        {
            Guard.NotNull(arr, "arr");
            return Encoding.UTF8.GetString(arr, offset, len);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="offset"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string GetStringASCII(this byte[] arr, int offset, int len)
        {
            Guard.NotNull(arr, "arr");
            return Encoding.ASCII.GetString(arr, offset, len);
        }
    }

}
