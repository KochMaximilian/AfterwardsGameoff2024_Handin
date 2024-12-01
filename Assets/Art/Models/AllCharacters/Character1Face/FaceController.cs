using UnityEngine;

public class FaceController : FaceControllerBase
{
    public Material faceMaterial; 
    public Vector2[] blinkUVOffsets; 
    public Vector2[] talkUVOffsets;      
    public float blinkInterval = 5f;
    public float blinkDuration = 0.2f;
    public float talkInterval = 0.1f;

    private float timer = 0f;     
    private bool isBlinking = false;
    public bool isTalking = false;
    
    void Update()
    {
        timer += Time.deltaTime;

        if (isTalking)
        {
            HandleTalking();
        }
        else if (!isBlinking && timer >= blinkInterval)
        {
            StartCoroutine(Blink());
            timer = 0f;
        }
        else if (!isBlinking && !isTalking)
        {
            faceMaterial.mainTextureOffset = blinkUVOffsets[0];
        }
    }

    private System.Collections.IEnumerator Blink()
    {
        isBlinking = true;

        faceMaterial.mainTextureOffset = blinkUVOffsets[1];
        yield return new WaitForSeconds(blinkDuration);

        faceMaterial.mainTextureOffset = blinkUVOffsets[0];
        isBlinking = false;
    }

    private void HandleTalking()
    {
        if (timer >= talkInterval)
        {
            timer = 0f;

            int randomTalkFrame = Random.Range(0, talkUVOffsets.Length);
            faceMaterial.mainTextureOffset = talkUVOffsets[randomTalkFrame];
        }
    }

    public override void SetTalking(bool talking)
    {
        if (!isTalking && talking)
        {
            timer = 0f;
            faceMaterial.mainTextureOffset = blinkUVOffsets[0];
        }
        
        isTalking = talking;
    }
}