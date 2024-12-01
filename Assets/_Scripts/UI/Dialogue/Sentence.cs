using System.Collections.Generic;
using UnityEngine;

public class Sentence
{
    private string text;
    public List<KeyWord> keyWords;
    private string _formattedText;
    
    public bool HasWordsToCollect => keyWords.Exists(k => !k.Collected);
    public string Text => GetText();
    public string OriginalText => text;
    public string FormattedText => _formattedText;
    
    
    public Sentence(string text, string formattedText, List<KeyWord> keyWords)
    {
        this.text = text;
        this.keyWords = keyWords;
        this._formattedText = formattedText;
    }
    
    private string GetText()
    {
        string result = text;
        int offset = 0;

        foreach (KeyWord keyWord in keyWords)
        {
            string openingTag;
            string closingTag;

            if (keyWord.Collected)
            {
                openingTag = "<color=grey><b>";
                closingTag = "</color></b>";
            }
            else
            {
                openingTag = "<b>";
                closingTag = "</b>";
            }

            // Insert the opening tag
            result = result.Insert(keyWord.index + offset, openingTag);
            offset += openingTag.Length;

            // Insert the closing tag after the text
            result = result.Insert(keyWord.index + offset + keyWord.text.Length, closingTag);
            offset += closingTag.Length;
        }

        return result;
    }

    public bool TryCollectWord(int charIndex, out KeyWord keyWord)
    {
        foreach (KeyWord word in keyWords)
        {
            if (word.Collected) continue;
            if (charIndex >= word.noTagsIndex - word.leftMargin && charIndex <= word.noTagsIndex + word.text.Length + word.rightMargin)
            {
                word.Collected = true;
                keyWord = word;

                WordSounds.PlayCollectWordSound();
                WordManager.Instance.CreateWordAsync(keyWord.WordData.CreateNode());
                return true;
            }
        }

        keyWord = null;
        return false;
    }
   
}

public class KeyWord
{
    public readonly int leftMargin;
    public readonly int rightMargin;
    
    public readonly string text;
    public readonly int index;
    public int noTagsIndex;
    
    private WordData _wordData;
    private bool _collected;
    private bool _allowCombining;
    private bool _allowLiteralCombining;

    public bool Collected
    {
        get => _collected;
        set => _collected = value;
    }

    public WordData WordData
    {
        get
        {
            if(_wordData != null) return _wordData;
            _wordData = ScriptableObject.CreateInstance<WordData>();
            _wordData.Text = text;
            _wordData.AllowCombining = _allowCombining;
            _wordData.AllowLiteralCombining = _allowLiteralCombining;
            return _wordData;
        }
    }
    
    public KeyWord(string text, int index, int leftMargin, int rightMargin, bool allowCombining, bool allowLiteralCombining, WordData wordData)
    {
        this.text = text;
        this.index = index;
        this.leftMargin = leftMargin;
        this.rightMargin = rightMargin;
        _allowCombining = allowCombining;
        _allowLiteralCombining = allowLiteralCombining;
        _wordData = wordData;
    }
}