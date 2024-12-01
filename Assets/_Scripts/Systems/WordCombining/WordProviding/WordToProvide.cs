using System;
using System.Collections;
using UnityEngine;
using UnityUtils;

[Serializable]
public class WordToProvide
{
    public WordData wordData;
    public int amount = 1;
    public bool provideOnStart;
    
    private bool _hasBeenProvided;
    private int _providedAmount;
    
    public bool Provide()
    {
        if (_hasBeenProvided) return false;
        
        for (int i = 0; i < amount; i++)
        {
            WordNode wordNode = wordData.CreateNode();
            _= WordManager.Instance.CreateWordAsync(wordNode);
        }

        _hasBeenProvided = true;
        return true;
    }
    
    public int Provide(int amount)
    {
        if (_hasBeenProvided) return 0;
        amount = Mathf.Max(0, this.amount - _providedAmount);
        _providedAmount += amount;
        
        for (int i = 0; i < amount; i++)
        {
            WordNode wordNode = wordData.CreateNode();
            _= WordManager.Instance.CreateWordAsync(wordNode);
        }
        
        if (_providedAmount == this.amount)
        {
            _hasBeenProvided = true;
        }
        return this.amount;
    }
    
}