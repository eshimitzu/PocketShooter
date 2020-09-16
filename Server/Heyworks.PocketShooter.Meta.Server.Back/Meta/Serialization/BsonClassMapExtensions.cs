using System;
using System.Linq;
using System.Reflection;
using MongoDB.Bson.Serialization;

namespace Heyworks.PocketShooter.Meta.Serialization
{
    /// <summary>
    /// Contains an extension methods for the <see cref="BsonClassMap"/> class.
    /// </summary>
    internal static class BsonClassMapExtensions
    {
        /// <summary>
        /// AutoMap the specified type and all subtypes if no class map is registered.
        /// </summary>
        /// <param name="baseType">The base type.</param>
        public static void AutoMapWithSubtypes(Type baseType)
        {
            var knownTypes = Assembly.GetAssembly(baseType).GetTypes().Where(t => !t.IsInterface && baseType.IsAssignableFrom(t)).ToList();

            foreach (var type in knownTypes)
            {
                BsonClassMap.LookupClassMap(type);
            }
        }
    }
}
