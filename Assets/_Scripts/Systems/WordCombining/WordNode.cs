using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordNode
{
    private readonly WordNode[] _parentWords = new WordNode[2];
        
    public WordData WordData { get; }
    public IList<WordNode> ParentWords => _parentWords;
    public int Depth => Mathf.Max(ParentWord0Depth, ParentWord1Depth);

    private int ParentWord0Depth => _parentWords[0] != null ? _parentWords[0].Depth + 1 : 0;
    private int ParentWord1Depth => _parentWords[1] != null ? _parentWords[1].Depth + 1 : 0;
    
    public WordNode(WordData wordData)
    {
        this.WordData = wordData;
    }

    public WordNode(WordData wordData, WordNode parentWord1, WordNode parentWord2)
    {
        this.WordData = wordData;
        _parentWords[0] = parentWord1;
        _parentWords[1] = parentWord2;
    }

    public override bool Equals(object obj)
    {
        return obj is WordNode wordNode && WordData.Text == wordNode.WordData.Text;
    }
    
    public override int GetHashCode()
    {
        return WordData.Text.GetHashCode();
    }
}