using UnityEditor;
using UnityEngine;

namespace Heyworks.PocketShooter.PropertyAttributesAndDrawers
{
    [CustomPropertyDrawer(typeof(SpritePathAttribute))]
    public class SpritePathDrawer : PropertyDrawer
    {
        private Sprite sprite;
        private string value;

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            if (prop.type == "string")
            {
                if (!string.IsNullOrEmpty(prop.stringValue))
                {
                    if (sprite == null || prop.stringValue != value)
                    {
                        sprite = (Sprite)AssetDatabase.LoadAssetAtPath(prop.stringValue, typeof(Sprite));
                    }
                }
                else
                {
                    sprite = null;
                }

                sprite = (Sprite)EditorGUI.ObjectField(position, label, sprite, typeof(Sprite), false);

                if (sprite == null)
                {
                    prop.stringValue = null;
                }
                else
                {
                    prop.stringValue = AssetDatabase.GetAssetPath(sprite);
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