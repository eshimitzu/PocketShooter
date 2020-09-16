using System;
using UnityEngine;

namespace Heyworks.PocketShooter.Utils.Unity.EditorTools.Attributes
{
    /// <summary>
    /// Represents an attribute, which marks the property as optional for setting in inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class OptionalPropertyAttribute : PropertyAttribute
    {
    }
}
