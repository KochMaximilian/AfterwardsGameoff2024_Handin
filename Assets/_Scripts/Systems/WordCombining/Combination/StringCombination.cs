using System;
using UnityEngine;

[Serializable]
public class StringCombination : Combination
{
    [SerializeField] private bool isCommutative = true;

    [Header("Input")]
    [SerializeField] private string inputWord1;
    [SerializeField] private string inputWord2;
    
    [Header("Output")]
    [SerializeField] private string outputWord;
    
    private WordData _inputWord1;
    private WordData _inputWord2;
    private WordData _outputWord;

    public override WordData InputWord1
    {
        get
        {
            if (_inputWord1 != null) return _inputWord1;
           
            _inputWord1 = ScriptableObject.CreateInstance<WordData>();
            _inputWord1.Text = inputWord1;
            _inputWord1.AllowCombining = true;
            _inputWord1.AllowLiteralCombining = true;
            return _inputWord1;
        }
    }
    
    public override WordData InputWord2
    {
        get
        {
            if (_inputWord2 != null) return _inputWord2;
            
            _inputWord2 = ScriptableObject.CreateInstance<WordData>();
            _inputWord2.Text = inputWord2;
            _inputWord2.AllowCombining = true;
            _inputWord2.AllowLiteralCombining = true;
            return _inputWord2;
        }
    }
    
    public override WordData OutputWord
    {
        get
        {
            if (_outputWord != null) return _outputWord;
            
            _outputWord = ScriptableObject.CreateInstance<WordData>();
            _outputWord.Text = outputWord;
            _outputWord.AllowCombining = true;
            _outputWord.AllowLiteralCombining = true;
            return _outputWord;
        }
    }
    
    protected override bool IsCommutative => isCommutative;
    
    public StringCombination()
    {
        isCommutative = true;
    }
    
    public StringCombination(string inputWord1, string inputWord2, string outputWord)
    {
        this.inputWord1 = inputWord1;
        this.inputWord2 = inputWord2;
        this.outputWord = outputWord;
        isCommutative = true;
    }
}