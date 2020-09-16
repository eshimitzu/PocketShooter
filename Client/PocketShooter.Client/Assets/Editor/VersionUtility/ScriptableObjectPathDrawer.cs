using UnityEditor;
using UnityEngine;

namespace Heyworks.PocketShooter.VersionUtility.PropertyAttributesAndDrawers
{
    [CustomPropertyDrawer(typeof(ScriptableObjectPathAttribute))]
    public class ScriptableObjectPathDrawer : PropertyDrawer
    {
        private ScriptableObject scriptableObject;
        private string value;

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            if (prop.type == "string")
            {
                if (!string.IsNullOrEmpty(prop.stringValue))
                {
                    if (scriptableObject == null || prop.stringValue != value)
                    {
                        scriptableObject = (ScriptableObject)AssetDatabase.LoadAssetAtPath(prop.stringValue, typeof(ScriptableObject));
                    }
                }
                else
                {
                    scriptableObject = null;
                }

                scriptableObject = (ScriptableObject)EditorGUI.ObjectField(position, label, scriptableObject, typeof(ScriptableObject), false);

                if (scriptableObject == null)
                {
                    prop.stringValue = null;
                }
                else
                {
                    prop.stringValue = AssetDatabase.GetAssetPath(scriptableObject);
                }

                value = prop.stringValue;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 20;
        }
    }
}