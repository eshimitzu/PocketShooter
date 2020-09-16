using UnityEditor;
using UnityEngine;

namespace Heyworks.PocketShooter.PropertyAttributesAndDrawers
{
    [CustomPropertyDrawer(typeof(MeshPathAttribute))]
    public class MeshPathDrawer : PropertyDrawer
    {
        private Mesh mesh;
        private string value;

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            if (prop.type == "string")
            {
                if (!string.IsNullOrEmpty(prop.stringValue))
                {
                    if (mesh == null || prop.stringValue != value)
                    {
                        mesh = (Mesh)AssetDatabase.LoadAssetAtPath(prop.stringValue, typeof(Mesh));
                    }
                }
                else
                {
                    mesh = null;
                }

                mesh = (Mesh)EditorGUI.ObjectField(position, label, mesh, typeof(Mesh), false);

                if (mesh == null)
                {
                    prop.stringValue = null;
                }
                else
                {
                    prop.stringValue = AssetDatabase.GetAssetPath(mesh);
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