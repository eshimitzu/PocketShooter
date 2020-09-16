using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Heyworks.PocketShooter.Serialization
{
    /// <summary>
    /// Used by <see cref="JsonSerializer"/> to resolves a <see cref="JsonContract"/> for a given <see cref="Type"/>.
    /// Creates a <see cref="JsonProperty"/> only for type property that can be written to (have a set accessor).
    /// </summary>
    internal class CanWritePropertiesOnlyResolver : DefaultContractResolver
    {
        /// <summary>
        /// Creates a <see cref="JsonProperty"/> for the given <see cref="MemberInfo"/>.
        /// </summary>
        /// <param name="member">The member to create a <see cref="JsonProperty"/> for.</param>
        /// <param name="memberSerialization">The member's parent <see cref="MemberInfo"/>.</param>
        /// <returns>A created <see cref="MemberInfo"/> for the given <see cref="MemberSerialization"/>.</returns>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);

            if (!prop.Writable)
            {
                PropertyInfo propInfo = member as PropertyInfo;
                prop.Writable = propInfo != null && propInfo.CanWrite;
            }

            return prop;
        }

        /// <summary>
        /// Creates properties for the given <see cref="JsonContract"/>.
        /// </summary>
        /// <param name="type">The type to create properties for.</param>
        /// <param name="memberSerialization">The member serialization mode for the type.</param>
        /// <returns>Properties for the given <see cref="JsonContract"/>.</returns>
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> props = base.CreateProperties(type, memberSerialization);
            var writable = props.Where(p => p.Writable).ToList();

            return writable;
        }
    }
}