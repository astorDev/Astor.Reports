using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using Astor.Linq;
using Newtonsoft.Json;

namespace Astor.Reports.Protocol
{
    public class HttpHelper
    {
        public static string GetQueryString(string url, object obj)
        {
            var dict = getKeyValueQueryParts(obj);
            if (!dict.Any())
            {
                return url;
            }

            var query = new StringBuilder($"{url}?{dict.First()}");
            foreach (var rec in dict.Skip(1))
            {
                query.Append($"&{rec}");
            }

            return query.ToString();
        }

        private static string toUrlValueString(object obj)
        {
            if (obj is DateTime dtObj)
            {
                var dtStr = dtObj.ToString("yyyy-MM-ddTHH:mm:ss");
                return dtStr;
            }

            if (obj.GetType().Name.Contains("AnonymousType"))
            {
                return JsonConvert.SerializeObject(obj);
            }

            var objStr = obj.ToString();
            var encodedString = WebUtility.UrlEncode(objStr);

            return encodedString;
        }

        private static IEnumerable<string> getKeyValueQueryParts(object obj)
        {
            var allProps = obj.GetType().GetTypeInfo().GetProperties();
            var notNullProps = from p in allProps
                where p.GetValue(obj) != null
                select new {p.Name, Value = p.GetValue(obj)};

            var enumerablesAndNot = notNullProps.Fork(p => p.Value is IEnumerable && !(p.Value is string));
            
            foreach (var prop in enumerablesAndNot.satisfying)
            {
                foreach (var v in (IEnumerable) prop.Value)
                {
                    yield return keyValueQueryPart(prop.Name, v);
                }
            }

            foreach (var prop in enumerablesAndNot.notSatisfying)
            {
                yield return keyValueQueryPart(prop.Name, prop.Value);
            }
        }

        private static string keyValueQueryPart(string key, object value)
        {
            var result = $"{key}={toUrlValueString(value)}";
            return result;
        }
    }
}