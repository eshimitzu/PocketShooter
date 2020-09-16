using System;
using UnityEngine;

public class EnumArrayAttribute : PropertyAttribute
{
    public string PropertyName { get; set; }

    public Type EnumType { get; set; }

    public EnumArrayAttribute(string propertyName, Type enumType)
    {
        PropertyName = propertyName;
        EnumType = enumType;
    }
}
