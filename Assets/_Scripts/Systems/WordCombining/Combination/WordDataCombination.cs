using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WordDataCombination : Combination
{
    [SerializeField] private bool isCommutative = true;

    [Header("Input")]
    [SerializeField] private WordData inputWord1;
    [SerializeField] private WordData inputWord2;
    
    [Header("Output")]
    [SerializeField] private WordData outputWord;
    
    public override WordData InputWord1 => inputWord1;
    public override WordData InputWord2 => inputWord2;
    public override WordData OutputWord => outputWord;
    protected override bool IsCommutative => isCommutative;
    
    public WordDataCombination()
    {
        isCommutative = true;
    }
    
    public WordDataCombination(WordData inputWord1, WordData inputWord2, WordData outputWord)
    {
        this.inputWord1 = inputWord1;
        this.inputWord2 = inputWord2;
        this.outputWord = outputWord;
        
        isCommutative = true;
    }
}