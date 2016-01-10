using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace BigData.Net
{
    public static class Extensions
    {
        /// <summary>
        /// String hashing function
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static UInt64 GetBigHashCode(this object value)
        {
            const ulong p = 1099511628211; 
            ulong hash = 2166136261;
            byte[] data = System.Text.ASCIIEncoding.ASCII.GetBytes(value.ToString());
            foreach (byte b in data)
                hash = (hash ^ b) * p;
            hash += hash << 13;
            hash ^= hash >> 7;
            hash += hash << 3;
            hash ^= hash >> 17;
            hash += hash << 5;
            return hash;
        }

       public static T BinarySearch<T, TKey>(this IList<T> list, Func<T, TKey> keySelector, TKey key)
        where TKey : IComparable<TKey>
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            int min = 0;
            int max = list.Count - 1;
            int mid = 0;
            while (min < max && max > 1)
            {
                mid = (max + min) / 2;
                T midItem = list[mid];
                TKey midKey = keySelector(midItem);
                int comp = midKey.CompareTo(key);
                if (comp < 0)
                {
                    min = mid + 1;
                }
                else if (comp > 0)
                {
                    max = mid - 1; 
                }
                else
                {
                    break;
                }
            }

            T x = list[min];
            T y = list[mid];
            T z = list[max];
 
            return new[] { 
                    new key<T>{ hash = x, equality = keySelector(x).CompareTo(key) }, 
                    new key<T>{ hash = y, equality = keySelector(y).CompareTo(key) },
                    new key<T>{ hash = z, equality = keySelector(z).CompareTo(key) }
            }.Aggregate((aggregate,value)=>{

                return (value.equality >= 0 && (value.equality < aggregate.equality || aggregate.equality < 0)) ? value : aggregate;
               
            }).hash;
        }
        
        static IEnumerable<T> Descendants<T>(this T root) where T : IEnumerable<T>
        {
            var nodes = new Stack<T>(new[] { root });
            while (nodes.Any())
            {
                T node = nodes.Pop();
                yield return node;
                foreach (var n in node) nodes.Push(n);
            }
        }

    }
}
