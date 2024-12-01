using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PasswordData", menuName = "Word Combining/Password", order = 1)]
public class PasswordData : ScriptableObject
{
    [SerializeField] private string passwordName;
    [SerializeField] private PasswordFragment[] fragments;
    
    public IReadOnlyList<PasswordFragment> Fragments => fragments;
    public string Name => passwordName;
}