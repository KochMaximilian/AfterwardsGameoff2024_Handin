using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EmotionalNPCDialogue))]
public class EmotionalNPCDialogueEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var dialogue = target as EmotionalNPCDialogue;
        if (dialogue == null)
            return;

        SerializedProperty actor = serializedObject.FindProperty("actor");
        SerializedProperty emotionalDialogues = serializedObject.FindProperty("emotionalDialogues");
        SerializedProperty keyWords = serializedObject.FindProperty("keyWords");
        SerializedProperty options = serializedObject.FindProperty("Options");
        SerializedProperty onEndDialogue = serializedObject.FindProperty("onEndDialogue");

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(actor);
        EditorGUILayout.PropertyField(emotionalDialogues, true);
        EditorGUILayout.PropertyField(keyWords, true);

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(options, true);
        
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(onEndDialogue);

        serializedObject.ApplyModifiedProperties();
    }
}