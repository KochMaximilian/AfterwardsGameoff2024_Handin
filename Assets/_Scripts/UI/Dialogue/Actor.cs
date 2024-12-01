using UnityEngine;

[CreateAssetMenu(fileName = "Actor", menuName = "Dialogue/Actor")]
public class Actor : ScriptableObject
{
    public string actorName;
    public AudioClip[] voice;
    public int charactersPerSound = 5;
}