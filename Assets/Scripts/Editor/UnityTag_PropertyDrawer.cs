using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(UnityTag))]
public class UnityTag_PropertyDrawer : PropertyDrawer
{
  public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
  {
    EditorGUI.BeginProperty(_position, _label, _property);
    SerializedProperty tagName = _property.FindPropertyRelative("m_TagName");
    if (tagName != null)
    {
        tagName.stringValue = EditorGUI.TagField(_position, _label, tagName.stringValue);
    }
    EditorGUI.EndProperty();
  }
}
