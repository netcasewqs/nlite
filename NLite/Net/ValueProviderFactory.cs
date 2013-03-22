using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using NLite.Internal;

namespace NLite.Net
{
    public interface IValueProviderFactory
    {
        IDictionary<string, object> GetValueProvider(IHttpContext ctx,IDictionary<string,object> routeValues);
    }

    public class DefaultValueProviderFactory : IValueProviderFactory
    {
        public IDictionary<string, object> GetValueProvider(IHttpContext ctx, IDictionary<string, object> routeValues)
        {
            Guard.NotNull(ctx, "ctx");
            Guard.NotNull(routeValues, "routeValues");
            Dictionary<string, object> valueProvider = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            PopulateValueProvider(ctx.Request, valueProvider, routeValues);
            return valueProvider;
        }

        static void PopulateValueProvider(IHttpRequest req, Dictionary<String, object> dict,IDictionary<string,object> routeValues)
        {

            var form = req.Form;
            if (form != null)
            {
                string[] keys = form.AllKeys;
                foreach (string key in keys)
                {
                    string[] rawValue = form.GetValues(key);
                    if (rawValue.Length > 1)
                        AddToDictionaryIfNotPresent(key, rawValue, dict);
                    else
                        AddToDictionaryIfNotPresent(key, form[key], dict);
                }

                if (dict.ContainsKey("_method"))
                    dict.Remove("_method");
            }


            var queryString = req.QueryString;
            if (queryString != null)
            {
                string[] keys = queryString.AllKeys;
                foreach (string key in keys)
                {
                    string[] rawValue = queryString.GetValues(key);
                    if (rawValue.Length > 1)
                        AddToDictionaryIfNotPresent(key, rawValue, dict);
                    else
                        AddToDictionaryIfNotPresent(key, queryString[key], dict);
                }
            }

            var httpPostFiles = req.Files;
            foreach (var key in httpPostFiles.AllKeys)
                AddToDictionaryIfNotPresent(key, httpPostFiles[key], dict);

            if (routeValues != null)
            {
                foreach(var key in routeValues.Keys)
                    AddToDictionaryIfNotPresent(key,routeValues[key],dict);
            }

            if (dict.ContainsKey("__VIEWSTATE"))
                dict.Remove("__VIEWSTATE");
            if (dict.ContainsKey("__EVENTVALIDATION"))
                dict.Remove("__EVENTVALIDATION");

            string verb = req.HttpMethod.ToUpper();
            string method = verb;
            if (verb == "POST" && form != null)
            {
                var s = form["_method"];
                if (!string.IsNullOrEmpty(s))
                {
                    method = s.ToUpper();
                }
            }

            dict["_method"] = method;


        }

        protected static void AddToDictionaryIfNotPresent(string key, object attemptedValue, IDictionary<string, object> dict)
        {
            if (string.IsNullOrEmpty(key))
                return;

            if (!dict.ContainsKey(key))
                dict.Add(key, attemptedValue);
        }

        private static readonly Dictionary<string, IHttpPostedFile[]> _emptyDictionary = new Dictionary<string, IHttpPostedFile[]>();

        private static Dictionary<string, IHttpPostedFile[]> GetHttpPostedFileDictionary(IHttpFileCollection files)
        {
            if (files.Count == 0)
                return _emptyDictionary;

            // build up the 1:many file mapping
            List<KeyValuePair<string, IHttpPostedFile>> mapping = new List<KeyValuePair<string, IHttpPostedFile>>();
            string[] allKeys = files.AllKeys;
            for (int i = 0; i < files.Count; i++)
            {
                string key = allKeys[i];
                if (key != null)
                {
                    mapping.Add(new KeyValuePair<string, IHttpPostedFile>(key, files[i]));
                }
            }

            // turn the mapping into a 1:many dictionary
            var grouped = mapping.GroupBy(el => el.Key, el => el.Value, StringComparer.OrdinalIgnoreCase);
            return grouped.ToDictionary(g => g.Key, g => g.ToArray(), StringComparer.OrdinalIgnoreCase);
        }

    }

    public static class ValueProviderFactory
    {
        public static IValueProviderFactory Current = new DefaultValueProviderFactory();
    }
}
