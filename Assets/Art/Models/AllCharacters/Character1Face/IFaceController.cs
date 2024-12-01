using UnityEngine;

public interface IFaceController
{
    void SetTalking(bool talking);
    void SetEmotion(Emotion emotion);
    
    void SetToDefaultEmotion();
}

public class FaceControllerBase : MonoBehaviour, IFaceController
{
    public virtual void SetTalking(bool talking)
    {
    }

    public virtual void SetEmotion(Emotion emotion)
    {
    }
    
    public virtual void SetToDefaultEmotion()
    {
    }
}