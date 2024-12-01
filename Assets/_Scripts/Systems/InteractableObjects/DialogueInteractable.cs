using System;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Outline))]
public class DialogueInteractable : InteractableObject
{
    [Header("Dialogue")]
    [SerializeField] private NPCDialogue dialogue;
    [SerializeField] private Transform optionsPosition;
    [SerializeField] private bool hideIfTooFar = true;
    
    [Header("Rotation")]
    [SerializeField] private bool lookAtPlayerWhileInteracting = true;
    [SerializeField] private bool rotateOnlyYAxis = true;
    [SerializeField] private Transform rotationTarget;
    
    private IFaceController _faceController;
    private Vector3 _originalRotation;

    public NPCDialogue Dialogue => dialogue;
    public Transform OptionsPosition => optionsPosition;

    protected override void Start()
    {
        base.Start();

        _faceController = GetComponent<IFaceController>();
        _originalRotation = transform.rotation.eulerAngles;
        
        if(rotationTarget == null) rotationTarget = transform;
    }

    public override void Interact()
    {
        base.Interact();
        if(Dialogue == null) return;
        
        _originalRotation = transform.rotation.eulerAngles;
        DialogueManager.OnSentenceStart += SetEmotion;
        
        DialogueManager.Instance.StartDialogue(this, hideIfTooFar, BreakInteraction, StopInteraction);
    }

    private void OnDestroy()
    {
        DialogueManager.OnSentenceStart -= SetEmotion;
    }

    public override void StopInteraction()
    {
        base.StopInteraction();
        
        _faceController?.SetToDefaultEmotion();
        DialogueManager.OnSentenceStart -= SetEmotion;
    }

    public override void BreakInteraction()
    {
        base.BreakInteraction();
        
        _faceController?.SetToDefaultEmotion();
        DialogueManager.OnSentenceStart -= SetEmotion;
    }

    private void Update()
    {
        if(isInteracting && lookAtPlayerWhileInteracting) LookAtPlayer();
        else LookAtOriginalRotation();
        
        _faceController?.SetTalking(DialogueManager.Instance.IsTalking);
    }
    
    private void LookAtPlayer()
    {
        Vector3 direction = (CameraController.Instance.transform.position - rotationTarget.position).normalized;
        
        Vector3 lookAt = new Vector3(direction.x, 0, direction.z);
        if(!rotateOnlyYAxis) lookAt = new Vector3(direction.x, direction.y, direction.z);
        
        Quaternion lookRotation = Quaternion.LookRotation(lookAt);
        rotationTarget.rotation = Quaternion.Slerp(rotationTarget.rotation, lookRotation, Time.deltaTime * 5f);
    }
    
    private void LookAtOriginalRotation()
    {
        rotationTarget.rotation = Quaternion.Slerp(rotationTarget.rotation, Quaternion.Euler(_originalRotation), Time.deltaTime * 5f);
    }

    public override bool CanInteract()
    {
        return base.CanInteract() && ConditionValidator.CanDisplayDialogue;
    }
    
    public void ChangeDialogue(NPCDialogue newDialogue)
    {
        dialogue = newDialogue;
    }
    
    private void SetEmotion(NPCDialogue dialogue, int emotionIndex)
    {
        if(_faceController == null || dialogue.EmotionalDialogues == null || dialogue.EmotionalDialogues.Length == 0) return;

        Emotion emotion = dialogue.EmotionalDialogues[emotionIndex].emotion;
        _faceController?.SetEmotion(emotion);
    }

    public override void LoadData(InteractableObjectSnapshot data)
    {
        base.LoadData(data);
        NPCDialogue newDialogue = Resources.Load<NPCDialogue>(data.dialoguePath);
        ChangeDialogue(newDialogue);
    }
    public override InteractableObjectSnapshot GetData()
    {
       InteractableObjectSnapshot res = base.GetData();
       res.dialoguePath = dialogue?.path;
       return res;
    }
}

