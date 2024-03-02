#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

public class NameAttribute : PropertyAttribute
{
    public string NewName { get; private set; }
    public NameAttribute(string name)
    {
        NewName = name;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(NameAttribute))]
public class NamePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        NameAttribute nameAttribute = (NameAttribute)this.attribute;
        label.text = nameAttribute.NewName;
        EditorGUI.PropertyField(position, property, label);
    }
}
#endif