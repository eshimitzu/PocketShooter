using UnityEditor;
using UnityEngine;

namespace Heyworks.PocketShooter.CharacterSetupUtility
{
    [CustomPropertyDrawer(typeof(OverrideControllerAttribute))]
    public class OverrideControllerDrawer : PropertyDrawer
    {
        private AnimatorOverrideController overrideController;
        private string value;

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            if (prop.type == "string")
            {
                if (!string.IsNullOrEmpty(prop.stringValue))
                {
                    if (overrideController == null || prop.stringValue != value)
                    {
                        overrideController = (AnimatorOverrideController)AssetDatabase.LoadAssetAtPath(prop.stringValue, typeof(AnimatorOverrideController));
                    }
                }
                else
                {
                    overrideController = null;
                }

                float originalValue = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 250;
                overrideController = (AnimatorOverrideController)EditorGUI.ObjectField(position, label, overrideController, typeof(AnimatorOverrideController), false);
                EditorGUIUtility.labelWidth = originalValue;

                if (overrideController == null)
                {
                    prop.stringValue = null;
                }
                else
                {
                    prop.stringValue = AssetDatabase.GetAssetPath(overrideController);
                }

                value = prop.stringValue;
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