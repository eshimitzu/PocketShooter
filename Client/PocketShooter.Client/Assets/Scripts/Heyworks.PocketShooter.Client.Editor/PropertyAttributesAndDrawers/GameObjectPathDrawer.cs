using UnityEditor;
using UnityEngine;

namespace Heyworks.PocketShooter.PropertyAttributesAndDrawers
{
    [CustomPropertyDrawer(typeof(GameObjectPathAttribute))]
    public class GameObjectPathDrawer : PropertyDrawer
    {
        private GameObject gameObject;
        private string value;

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            if (prop.type == "string")
            {
                if (!string.IsNullOrEmpty(prop.stringValue))
                {
                    if (gameObject == null || prop.stringValue != value)
                    {
                        gameObject = (GameObject)AssetDatabase.LoadAssetAtPath(prop.stringValue, typeof(GameObject));
                    }
                }
                else
                {
                    gameObject = null;
                }

                gameObject = (GameObject)EditorGUI.ObjectField(position, label, gameObject, typeof(GameObject), false);

                if (gameObject == null)
                {
                    prop.stringValue = null;
                }
                else
                {
                    prop.stringValue = AssetDatabase.GetAssetPath(gameObject);
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