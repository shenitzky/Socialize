using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Socialize.Logic
{
    public static class Extensions
    {
        public static bool IsDefault<TKey, TValue>(this KeyValuePair<TKey, TValue>  value) 
            where TValue : class

        {
            bool isDefault = value.Equals(default(KeyValuePair<TKey, TValue>));
            return isDefault;
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}