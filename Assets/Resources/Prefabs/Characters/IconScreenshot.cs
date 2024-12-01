using UnityEngine;
using System.IO;

public class IconScreenshot : MonoBehaviour
{
    public Camera iconCamera; 
    public RenderTexture renderTexture;
    public string fileName = "GameObjectIcon.png"; // Set your desired file name

    public void CaptureIcon()
    {
        // Set the camera target
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = renderTexture;

        // Render the camera
        iconCamera.Render();

        // Create a Texture2D from the RenderTexture
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        // Save the texture to a PNG
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(Path.Combine(Application.dataPath, fileName), bytes);

        // Clean up
        RenderTexture.active = currentRT;
        Destroy(texture);

        Debug.Log($"Screenshot saved to {Path.Combine(Application.dataPath, fileName)}");
    }
}