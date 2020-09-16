using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Heyworks.PocketShooter.Realtime.Connection
{
    /// <summary>
    /// Represent class for getting not public field values. Uses cache for FieldInfos.
    /// </summary>
    /// <typeparam name="TThis">The type of the this.</typeparam>
    public class FieldInfoValueProvider<TThis>
    {
        private static readonly ConcurrentDictionary<string, FieldInfo> fieldInfos =
        new ConcurrentDictionary<string, FieldInfo>();

        /// <summary>
        /// Gets the field value.
        /// </summary>
        /// <typeparam name="T">Type of return value.</typeparam>
        /// <param name="instance">The object with field.</param>
        /// <param name="fieldName">Name of the field.</param>
        public static T GetFieldValue<T>(TThis instance, string fieldName)
        {
            if (!fieldInfos.TryGetValue(fieldName, out var fieldInfo))
            {
                    fieldInfo = typeof(TThis).GetField(
                        fieldName,
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                    fieldInfos[fieldName] = fieldInfo;
            }

            return (T)fieldInfo.NotNull().GetValue(instance);
        }
    }
}