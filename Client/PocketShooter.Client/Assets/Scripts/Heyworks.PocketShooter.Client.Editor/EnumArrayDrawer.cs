using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EnumArrayAttribute))]
public class EnumArrayDrawer : PropertyDrawer
{
    private Rect rect;

    private EnumArrayAttribute NamedAttribute => ((EnumArrayAttribute)attribute);

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float size = 16;

        if (property.isExpanded)
        {
            foreach (SerializedProperty p in property)
            {
                size += EditorGUI.GetPropertyHeight(p);
            }
        }

        return size;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        position.height = 16;

        SerializedProperty prop = property.FindPropertyRelative(NamedAttribute.PropertyName);
        if (prop != null)
        {
            label.text = prop.enumNames[prop.enumValueIndex];
        }

        property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);
        if (property.isExpanded)
        {
            rect = new Rect(position.xMin + 10, position.yMin, position.width - 10, 16);

            DrawPropertiesRecursively(rect, property);
        }
    }

    void DrawPropertiesRecursively(Rect position, SerializedProperty property)
    {
        rect = position;

        foreach (SerializedProperty p in property)
        {
            float height = EditorGUI.GetPropertyHeight(p);
            rect = new Rect(position.xMin + 10, rect.yMin + height, position.width - 10, height);
            EditorGUI.PropertyField(rect, p);

            if (p.hasChildren && p.isExpanded)
            {
                DrawPropertiesRecursively(rect, p);
            }
        }
    }
}
