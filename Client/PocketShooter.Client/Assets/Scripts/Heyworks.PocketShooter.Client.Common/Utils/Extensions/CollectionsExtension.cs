using System.Collections.Generic;
using UnityEngine;

namespace Heyworks.PocketShooter.Utils.Extensions
{
    public static class CollectionsExtension
    {
        public static List<T> RemoveDuplicates<T>(this List<T> source)
        {
            List<T> result = new List<T>();
            for (int i = 0; i < source.Count - 1; i++)
            {
                if (!source[i].Equals(source[i + 1]))
                {
                    result.Add(source[i]);
                }
            }
            result.Add(source[source.Count - 1]);

            return result;
        }

        public static T RandomObject<T>(this IList<T> list)
        {
            return (list.Count > 0) ? list[Random.Range(0, list.Count)] : default(T);
        }
    }
}