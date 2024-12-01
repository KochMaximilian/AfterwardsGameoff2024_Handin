using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class GameStateSnapshot 
{
    public List<WordSnapshot> WordSnapshots;
    public List<SentenceSnapshot> CollectedSentences;
    public int currentLevel;
    [SerializeField] private float[] playerPosition = new float[3];
    [SerializeField] private float[] playerRotation = new float[3];
    public float time;
    public List<InteractableObjectSnapshot> interactableObjects;
    
    public Vector3 PlayerPosition 
    {
        get => new Vector3(playerPosition[0], playerPosition[1], playerPosition[2]);
        set
        {
            playerPosition[0] = value.x;
            playerPosition[1] = value.y;
            playerPosition[2] = value.z;
        }
    }
    
    public Vector3 PlayerRotation
    {
        get => new Vector3(playerRotation[0], playerRotation[1], playerRotation[2]);
        set
        {
            playerRotation[0] = value.x;
            playerRotation[1] = value.y;
            playerRotation[2] = value.z;
        }
    }
    
    public GameStateSnapshot()
    {
        WordSnapshots = new List<WordSnapshot>();
        CollectedSentences = new List<SentenceSnapshot>();
    }
}

[Serializable]
public class WordSnapshot
{
    public WordNodeSnapshot wordNodeSnapshot;
    [SerializeField] private float[] position = new float[3];
    
    public Vector3 Position
    {
        get => new Vector3(position[0], position[1], position[2]);
        set
        {
            position[0] = value.x;
            position[1] = value.y;
            position[2] = value.z;
        }
    }

    public WordSnapshot()
    {
    }
    
    public WordSnapshot(WordNodeSnapshot wordNodeSnapshot, Vector3 position)
    {
        this.wordNodeSnapshot = wordNodeSnapshot;
        this.position = new float[] {position.x, position.y, position.z};
    }
    
    public WordNode GetWordNode()
    {
        return wordNodeSnapshot.GetWordNode();
    }
}

[Serializable]
public class WordDataSnapshot
{
    public string Text;
    public bool AllowCombining;
    public bool AllowLiteralCombining;
    public WordDataSnapshot Parent1;
    public WordDataSnapshot Parent2;
    
    public WordDataSnapshot()
    {
    }
    
    public WordDataSnapshot(WordData wordData)
    {
        Text = wordData.Text;
        AllowCombining = wordData.AllowCombining;
        AllowLiteralCombining = wordData.AllowLiteralCombining;
        Parent1 = wordData.Parent1 == null ? null : new WordDataSnapshot(wordData.Parent1);
        Parent2 = wordData.Parent2 == null ? null : new WordDataSnapshot(wordData.Parent2);
    }
    
    public WordData GetWordData()
    {
        WordData wordData = ScriptableObject.CreateInstance<WordData>();
        wordData.Text = Text;
        wordData.AllowCombining = AllowCombining;
        wordData.AllowLiteralCombining = AllowLiteralCombining;
        wordData.Parent1 = Parent1?.GetWordData();
        wordData.Parent2 = Parent2?.GetWordData();
        return wordData;
    }
}

[Serializable]
public class WordNodeSnapshot
{
    public WordNodeSnapshot[] ParentWords = new WordNodeSnapshot[2];
    public WordDataSnapshot WordData;
    
    public WordNodeSnapshot()
    {
    }
    
    public WordNodeSnapshot(WordDataSnapshot wordData, WordNodeSnapshot parentWord1, WordNodeSnapshot parentWord2)
    {
        WordData = wordData;
        ParentWords[0] = parentWord1;
        ParentWords[1] = parentWord2;
    }
    
    public WordNode GetWordNode()
    {
        return new WordNode(WordData.GetWordData(), ParentWords[0]?.GetWordNode(), ParentWords[1]?.GetWordNode());
    }
}

[Serializable]
public class SentenceSnapshot
{
    public string text;
    public List<KeyWordSnapshot> keyWords;
    public string formattedText;
    
    public SentenceSnapshot()
    {
    }
    
    public SentenceSnapshot(Sentence sentence)
    {
        text = sentence.OriginalText;
        formattedText = sentence.FormattedText;
        keyWords = new List<KeyWordSnapshot>();
        foreach (KeyWord keyWord in sentence.keyWords)
        {
            keyWords.Add(new KeyWordSnapshot(keyWord));
        }
    }
    
    public Sentence GetSentence()
    {
        List<KeyWord> keyWords = new List<KeyWord>();
        foreach (KeyWordSnapshot keyWordSnapshot in this.keyWords)
        {
            keyWords.Add(keyWordSnapshot.GetKeyWord());
        }
        return new Sentence(text,formattedText, keyWords);
    }
}

[Serializable]
public class KeyWordSnapshot
{
    public readonly int leftMargin;
    public readonly int rightMargin;
    
    public readonly string text;
    public readonly int index;
    public int noTagsIndex;
    
    public WordDataSnapshot wordData;
    public bool collected;
   
    public KeyWordSnapshot()
    {
    }
    
    public KeyWordSnapshot(KeyWord keyWord)
    {
        leftMargin = keyWord.leftMargin;
        rightMargin = keyWord.rightMargin;
        text = keyWord.text;
        index = keyWord.index;
        wordData = new WordDataSnapshot(keyWord.WordData);
        collected = keyWord.Collected;
        noTagsIndex = keyWord.noTagsIndex;
    }
    
    public KeyWord GetKeyWord()
    {
        KeyWord keyWord = new KeyWord(text, index, leftMargin, rightMargin, true, true, wordData.GetWordData());
        keyWord.noTagsIndex = noTagsIndex;
        keyWord.Collected = collected;
        return keyWord;
    }
}

[Serializable]
public class InteractableObjectSnapshot
{
    public string interactText;
    public bool hasFirstInteracted;
    public bool hasFirstStoppedInteraction;
    public bool canInteract;
    public string dialoguePath;
    public long hash;

    public InteractableObjectSnapshot()
    {
    }
}
