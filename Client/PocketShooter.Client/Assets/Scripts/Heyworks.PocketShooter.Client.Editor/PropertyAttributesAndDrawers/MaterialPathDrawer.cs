using UnityEditor;
using UnityEngine;

namespace Heyworks.PocketShooter.PropertyAttributesAndDrawers
{
    [CustomPropertyDrawer(typeof(MaterialPathAttribute))]
    public class MaterialPathDrawer : PropertyDrawer
    {
        private Material material;
        private string value;

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            if (prop.type == "string")
            {
                if (!string.IsNullOrEmpty(prop.stringValue))
                {
                    if (material == null || prop.stringValue != value)
                    {
                        material = (Material)AssetDatabase.LoadAssetAtPath(prop.stringValue, typeof(Material));
                    }
                }
                else
                {
                    material = null;
                }

                material = (Material)EditorGUI.ObjectField(position, label, material, typeof(Material), false);

                if (material == null)
                {
                    prop.stringValue = null;
                }
                else
                {
                    prop.stringValue = AssetDatabase.GetAssetPath(material);
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