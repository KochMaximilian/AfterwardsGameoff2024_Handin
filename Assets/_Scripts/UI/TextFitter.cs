using UnityEngine;
using TMPro;

public class TextFitter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private TextMeshProUGUI textMeshPro;

    [Header("Settings")]
    [SerializeField] private float padding = 10f;
    [SerializeField] private float maxWidth = 400f;
    [SerializeField] private float preferredFontSize = 40f;

    public void AdjustSize()
    {
        textMeshPro.fontSize = preferredFontSize;
        textMeshPro.ForceMeshUpdate();
        
        float preferredWidth = textMeshPro.preferredWidth + padding;

        if (preferredWidth > maxWidth)
        {
            // Reduce font size to fit within maxWidth
            float scaleFactor = maxWidth / preferredWidth;
            textMeshPro.fontSize *= scaleFactor;

            // Recalculate preferred width with new font size
            textMeshPro.ForceMeshUpdate(); 
            preferredWidth = textMeshPro.preferredWidth + padding;
        }

        rectTransform.sizeDelta = new Vector2(Mathf.Min(preferredWidth, maxWidth), rectTransform.sizeDelta.y);
    }
    
    public void AdjustSize(float targetWidth, float targetHeight)
    {
        textMeshPro.fontSize = preferredFontSize;
        textMeshPro.ForceMeshUpdate();
        
        float preferredWidth = textMeshPro.preferredWidth + padding;
        float scaleFactor = 1f;

        if (preferredWidth > targetWidth)
        {
             scaleFactor = targetWidth / preferredWidth;
        }
        
        textMeshPro.fontSize *= scaleFactor;
        textMeshPro.ForceMeshUpdate(); 

        rectTransform.sizeDelta = new Vector2(targetWidth, targetHeight);
    }
}

public class PlayerPositioner : MonoBehaviour, ISceneInitializer
{
    [Header("References")]
    [SerializeField] private Transform playerTransform;

    public void OnStartScene()
    {

    }
}