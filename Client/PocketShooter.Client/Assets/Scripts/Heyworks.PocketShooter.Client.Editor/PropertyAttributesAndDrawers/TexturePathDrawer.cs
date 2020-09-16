using UnityEditor;
using UnityEngine;

namespace Heyworks.PocketShooter.PropertyAttributesAndDrawers
{
    [CustomPropertyDrawer(typeof(TexturePathAttribute))]
    public class TexturePathDrawer : PropertyDrawer
    {
        private Texture2D texture;
        private string value;

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            if (prop.type == "string")
            {
                if (!string.IsNullOrEmpty(prop.stringValue))
                {
                    if (texture == null || prop.stringValue != value)
                    {
                        texture = (Texture2D)AssetDatabase.LoadAssetAtPath(prop.stringValue, typeof(Texture2D));
                    }
                }
                else
                {
                    texture = null;
                }

                texture = (Texture2D)EditorGUI.ObjectField(position, label, texture, typeof(Texture2D), false);

                if (texture == null)
                {
                    prop.stringValue = null;
                }
                else
                {
                    prop.stringValue = AssetDatabase.GetAssetPath(texture);
                }

                value = prop.stringValue;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 60;
        }

        public override bool CanCacheInspectorGUI(SerializedProperty property)
        {
            return false;
        }
    }
}