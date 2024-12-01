using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(PasswordFragment))]
public class PasswordFragmentPropertyDrawer : PropertyDrawer
{
    private SerializedProperty _useText;
    private SerializedProperty _wordText;
    private SerializedProperty _word;
    
    private bool _initialized;
    
    private void OnEnable(SerializedProperty property)
    {
        _useText = property.FindPropertyRelative("useText");
        _wordText = property.FindPropertyRelative("wordText");
        _word = property.FindPropertyRelative("word");
    }
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (!_initialized) OnEnable(property);
        
        EditorGUI.BeginProperty(position, label, property);
        
        Rect foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        _useText.boolValue = EditorGUI.Toggle(foldoutRect, "Use Text",_useText.boolValue);
        
        if (_useText.boolValue)
        {
            Rect textRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(textRect, _wordText, new GUIContent("Word Text"));
        }
        else
        {
            Rect wordRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(wordRect, _word, new GUIContent("Word"));
        }
        
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 2;
    }
}