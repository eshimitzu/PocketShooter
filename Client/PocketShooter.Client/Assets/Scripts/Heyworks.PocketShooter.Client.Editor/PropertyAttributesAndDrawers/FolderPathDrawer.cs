using UnityEditor;
using UnityEngine;

namespace Heyworks.PocketShooter.PropertyAttributesAndDrawers
{
    [CustomPropertyDrawer(typeof(FolderPathAttribute))]
    public class FolderPathDrawer : PropertyDrawer
    {
        private string folderPath;
        private Object pathObject;

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            if (prop.type == "string")
            {
                if (!string.IsNullOrEmpty(prop.stringValue))
                {
                    if (pathObject == null || prop.stringValue != folderPath)
                    {
                        pathObject = (Object)AssetDatabase.LoadAssetAtPath(prop.stringValue, typeof(Object));
                    }
                }
                else
                {
                    pathObject = null;
                }

                float originalValue = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 250;
                pathObject = (Object)EditorGUI.ObjectField(position, label, pathObject, typeof(Object), false);
                EditorGUIUtility.labelWidth = originalValue;

                if (pathObject == null)
                {
                    prop.stringValue = null;
                }
                else
                {
                    prop.stringValue = AssetDatabase.GetAssetPath(pathObject);
                }

                folderPath = prop.stringValue;
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