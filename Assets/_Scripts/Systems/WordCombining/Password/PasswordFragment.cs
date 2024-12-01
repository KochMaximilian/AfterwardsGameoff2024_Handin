using System;
using UnityEngine;

[Serializable]
public class PasswordFragment
{
    [SerializeField] private bool useText;
    public string wordText;
    public WordData word;
    
    public bool UseText => useText;
}