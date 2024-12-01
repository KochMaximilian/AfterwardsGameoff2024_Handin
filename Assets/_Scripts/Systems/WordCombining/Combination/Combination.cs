using System;
using UnityEngine;

[Serializable]
public abstract class Combination
{
    public abstract WordData InputWord1 { get; }
    public abstract WordData InputWord2 { get; }
    public abstract WordData OutputWord { get; }
    protected abstract bool IsCommutative { get; }
    public bool CanCombine(string word1, string word2)
    {
        return (InputWord1.Text == word1 && InputWord2.Text == word2) || (IsCommutative && InputWord1.Text == word2 && InputWord2.Text == word1);
    }

    public override bool Equals(object obj)
    {
        return obj is Combination combination &&
               ((InputWord1.Text == combination.InputWord1.Text && InputWord2.Text == combination.InputWord2.Text)
                || (IsCommutative && InputWord1.Text == combination.InputWord2.Text && InputWord2.Text == combination.InputWord1.Text));
    }
    
    public override int GetHashCode()
    {
        if(IsCommutative)
            return InputWord1.Text.GetHashCode() ^ InputWord2.Text.GetHashCode();
        return (InputWord1.Text + InputWord2.Text).GetHashCode();
    }
    
    public override string ToString()
    {
        return $"{InputWord1.Text} + {InputWord2.Text} = {OutputWord.Text}";
    }
}