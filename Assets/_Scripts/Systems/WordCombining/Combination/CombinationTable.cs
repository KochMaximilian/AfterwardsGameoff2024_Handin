using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Word Combining/Combination Table")]
public class CombinationTable : ScriptableObject
{
    [SerializeField] 
    [Tooltip("Combinations that are made of two WordData objects")]
    private List<WordDataCombination> combinations;
    [SerializeField] 
    [Tooltip("Combinations that are made of two strings, only used for special cases")]
    private List<StringCombination> stringCombinations;

    private HashSet<Combination> _combinations;

    private HashSet<Combination> Combinations
    {
        get
        {
            if (_combinations != null) return _combinations;
            
            _combinations = new HashSet<Combination>();
            _combinations.UnionWith(combinations);
            _combinations.UnionWith(stringCombinations);
            return _combinations;
        }
    }
    
    public Combination GetCombination(string word1, string word2)
    {
        return Combinations.FirstOrDefault(combination => combination.CanCombine(word1, word2));
    }
    
    public Combination GetCombination(WordData word1, WordData word2)
    {
        return Combinations.FirstOrDefault(combination => combination.CanCombine(word1.Text, word2.Text));
    }
    
    public void AddCombination(WordData parent1, WordData parent2, WordData outputWord)
    {
        WordDataCombination combination = new WordDataCombination(parent1, parent2, outputWord);
        Combinations.Add(combination);
    }
    
    public void AddCombination(string parent1, string parent2, string outputWord)
    {
        StringCombination combination = new StringCombination(parent1, parent2, outputWord);
        Combinations.Add(combination);
    }
}

