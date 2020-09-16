using Heyworks.PocketShooter.Utils.Unity.EditorTools.Attributes;
using UnityEditor;
using UnityEngine;

namespace Heyworks.PocketShooter.Utils.Unity.EditorTools.Editor
{
    /// <summary>
    /// Represents class responsible for drawing of a field, which has <see cref="OptionalPropertyAttribute"/> applied to it.
    /// </summary>
    [CustomPropertyDrawer(typeof(OptionalPropertyAttribute))]
    public sealed class OptionalPropertyDrawer : PropertyDrawer
    {
        private const string OptionalPropertyNameFormat = "[{0}]";

        /// <summary>
        /// <para>
        /// Override this method to make your own GUI for the property.
        /// </para>
        /// </summary>
        /// <param name="position"> Rectangle on the screen to use for the property GUI. </param>
        /// <param name="property"> The SerializedProperty to make the custom GUI for. </param>
        /// <param name="label"> The label of this property. </param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            label.text = string.Format(OptionalPropertyNameFormat, label.text);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            EditorGUI.PropertyField(position, property, GUIContent.none);

            EditorGUI.EndProperty();
        }
    }
}
