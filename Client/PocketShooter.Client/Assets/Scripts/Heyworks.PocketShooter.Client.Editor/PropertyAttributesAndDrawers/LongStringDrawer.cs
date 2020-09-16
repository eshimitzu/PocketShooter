using UnityEditor;
using UnityEngine;

namespace Heyworks.PocketShooter.PropertyAttributesAndDrawers
{
    [CustomPropertyDrawer(typeof(LongStringAttribute))]
    public class LongStringDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            var height = (attribute as LongStringAttribute).height;
            float labelHeight = 20;

            // Adjust height of the text field
            Rect textFieldPosition = position;
            textFieldPosition.height = labelHeight;
            EditorGUI.LabelField(textFieldPosition, label);

            // make a rect of fixed height for text area.
            Rect textareaPos = position;
            textareaPos.y += labelHeight;
            textareaPos.height = height - labelHeight;
            EditorGUI.BeginProperty(textareaPos, label, prop);

            // add word wrap to style.
            GUIStyle style = new GUIStyle(EditorStyles.textField);
            style.wordWrap = true;
            style.fixedHeight = height - labelHeight;

            // show the text area.
            EditorGUI.BeginChangeCheck();
            string input = EditorGUI.TextArea(textareaPos, prop.stringValue, style);
            if (EditorGUI.EndChangeCheck())
            {
                prop.stringValue = input;
            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = (attribute as LongStringAttribute).height;
            return height;
        }
    }
}