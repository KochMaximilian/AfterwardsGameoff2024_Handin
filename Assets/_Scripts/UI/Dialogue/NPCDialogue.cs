using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue/Dialogue")]
public class NPCDialogue : ScriptableObject
{
    [Header("Dialogue")]
    public Actor actor;
    [SerializeField, TextArea] private string[] dialogue;
    public WordData[] keyWords;
    
    [Header("Options")]
    public DialogueOption[] Options;
    
    public virtual string[] Dialogue => dialogue;
    public virtual EmotionalDialogue[] EmotionalDialogues => null;
    
    [Header("Extra Dialogue")]
    public NPCDialogue onEndDialogue;
    
    [HideInInspector] public string path;

#if UNITY_EDITOR
    private void OnValidate()
    {
        string localPath = UnityEditor.AssetDatabase.GetAssetPath(this);
        localPath = localPath.Substring(17);
        localPath = localPath.Substring(0, localPath.Length - 6);
        
        if (string.IsNullOrEmpty(path) || !path.Equals(localPath))
        {
            path = localPath;
            Debug.Log("path: " + path);
        }
    }

    private void OnEnable() 
    {
        string localPath = UnityEditor.AssetDatabase.GetAssetPath(this);
        localPath = localPath.Substring(17);
        localPath = localPath.Substring(0, localPath.Length - 6);

        if (string.IsNullOrEmpty(path) || !path.Equals(localPath))
        {
            path = localPath;
            Debug.Log("path: " + path);
        }
    }
#endif
}

[System.Serializable]
public class DialogueOption
{
    public string text;
    public NPCDialogue dialogue;
    [FormerlySerializedAs("OnSelected")] public Events[] onSelected;
    
    public void Select()
    {
        if(onSelected == null) return;
        foreach (var e in onSelected)
        {
            EventFactory.Raise(e);
        }
    }
}

[System.Serializable]
public class EmotionalDialogue
{
    public Emotion emotion;
    [TextArea] public string text;
}

public enum Emotion
{
    Neutral,
    Angry,
    Sad,
    Indifferent,
    Happy
}