using System;
using UnityEngine;

public class NubiFaceController : FaceControllerBase
{
    [SerializeField] private Animator animator;
    [SerializeField] private Emotion defaultEmotion;

    private void Start()
    {
        SetToDefaultEmotion();
    }

    public override void SetEmotion(Emotion emotion)
    {
        animator.SetInteger("State", (int)emotion);
    }

    public override void SetToDefaultEmotion()
    {
        SetEmotion(defaultEmotion);
    }
}