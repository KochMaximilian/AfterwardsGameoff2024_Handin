using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Word Combining/Word")]
public class WordData : ScriptableObject
{
    [Header("Data")]
    [SerializeField] private string text;
    [SerializeField] private bool allowCombining = true;
    [SerializeField, Tooltip("If true, can be merged with any word even if they don't have a combination")]
    private bool allowLiteralCombining = true;
    
    [Header("Optional parent words")]
    [SerializeField] private WordData parent1;
    [SerializeField] private WordData parent2;
    
    public string Text { get => text; set => text = value; }
    public bool AllowCombining { get => allowCombining; set => allowCombining = value; }
    public bool AllowLiteralCombining { get => allowLiteralCombining; set => allowLiteralCombining = value; }
    public WordData Parent1 { get => parent1; set => parent1 = value; }
    public WordData Parent2 { get => parent2; set => parent2 = value; }

    public WordNode CreateNode()
    {
        if(parent1 == null || parent2 == null)
            return new WordNode(this);
        
        WordManager.Instance.CombinationTable.AddCombination(parent1, parent2, this);
        return new WordNode(this, parent1.CreateNode(), parent2.CreateNode());
    }
}