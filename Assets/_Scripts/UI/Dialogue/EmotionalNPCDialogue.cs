using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "EmotionalDialogue", menuName = "Dialogue/EmotionalDialogue")]
public class EmotionalNPCDialogue : NPCDialogue
{
    public EmotionalDialogue[] emotionalDialogues;
    
    public override string[] Dialogue => emotionalDialogues.Select(x => x.text).ToArray();
    public override EmotionalDialogue[] EmotionalDialogues => emotionalDialogues;
}