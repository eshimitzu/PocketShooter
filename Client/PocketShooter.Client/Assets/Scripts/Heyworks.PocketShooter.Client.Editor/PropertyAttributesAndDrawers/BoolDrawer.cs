using UnityEditor;
using UnityEngine;

namespace Heyworks.PocketShooter.PropertyAttributesAndDrawers
{
    [CustomPropertyDrawer(typeof(BoolAttribute))]
    public class BoolDrawer : PropertyDrawer
    {
        private bool isOn;

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            if (prop.type == "bool")
            {
                float originalValue = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 250;
                isOn = EditorGUI.PropertyField(position, prop, label);
                EditorGUIUtility.labelWidth = originalValue;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 20;
        }

        public override bool CanCacheInspectorGUI(SerializedProperty property)
        {
            return false;
        }
    }
}