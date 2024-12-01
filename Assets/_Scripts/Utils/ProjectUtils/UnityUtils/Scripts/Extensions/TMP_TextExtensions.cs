using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityUtils;

public static class TMP_TextExtensions
{
    /// <summary>
    /// Gets the center position of a word based on its start and end character indexes.
    /// </summary>
    /// <param name="textMeshPro">The TMP_Text component.</param>
    /// <param name="startIndex">The starting index of the word.</param>
    /// <param name="endIndex">The ending index of the word.</param>
    /// <returns>The world position of the word's center.</returns>
    public static Vector3 GetWordCenter(this TMP_Text textMeshPro, int startIndex, int endIndex)
    {
        // Force an update to the text mesh
        textMeshPro.ForceMeshUpdate();

        TMP_TextInfo textInfo = textMeshPro.textInfo;

        if (startIndex < 0 || startIndex >= textInfo.characterCount || endIndex < 0 || endIndex >= textInfo.characterCount)
        {
            Debug.LogWarning("Invalid word index range.");
            return Vector3.zero;
        }

        // Get the character info for the start and end of the word
        TMP_CharacterInfo startCharInfo = textInfo.characterInfo[startIndex];
        TMP_CharacterInfo endCharInfo = textInfo.characterInfo[endIndex];

        // Check if the characters are visible (i.e., not whitespace)
        if (!startCharInfo.isVisible || !endCharInfo.isVisible)
        {
            Debug.LogWarning("Word contains invisible characters.");
            return Vector3.zero;
        }

        // Calculate the word's center by averaging the positions of the start and end characters' bounds
        Vector3 localStartCenter = (startCharInfo.bottomLeft + startCharInfo.topRight) / 2;
        Vector3 localEndCenter = (endCharInfo.bottomLeft + endCharInfo.topRight) / 2;

        // Transform the local positions to world positions
        Transform textTransform = textMeshPro.transform;
        Vector3 worldStartCenter = textTransform.TransformPoint(localStartCenter);
        Vector3 worldEndCenter = textTransform.TransformPoint(localEndCenter);

        // Return the average center in world space
        return (worldStartCenter + worldEndCenter) / 2;
    }

    
    /// <summary>
    /// Starts a coroutine to apply a shaking effect to all characters within a specified word for a given duration.
    /// </summary>
    /// <param name="textMeshPro">The TMP_Text component.</param>
    /// <param name="startIndex">The starting index of the word.</param>
    /// <param name="endIndex">The ending index of the word.</param>
    /// <param name="duration">The duration for which the shake effect should apply.</param>
    /// <param name="shakeIntensity">The intensity of the shake effect.</param>
    /// <param name="shakeSpeed">The speed of the shake effect.</param>
    public static void ApplyShakeEffect(this TMP_Text textMeshPro, int startIndex, int endIndex, float duration, float shakeIntensity = 0.5f, float shakeSpeed = 10f)
    {
        CoroutineController.Start(ShakeCoroutine(textMeshPro, startIndex, endIndex, shakeIntensity, shakeSpeed, duration));
    }


    /// <summary>
    /// Coroutine that applies a shaking effect to characters for a specified duration.
    /// </summary>
    private static IEnumerator ShakeCoroutine(TMP_Text textMeshPro, int startIndex, int endIndex, float shakeIntensity, float shakeSpeed, float duration)
    {
        // Force an update to the text mesh to initialize the character info
        textMeshPro.ForceMeshUpdate(true, true);

        TMP_TextInfo textInfo = textMeshPro.textInfo;
        string text = textMeshPro.text;

        if (startIndex < 0 || startIndex >= textInfo.characterCount || endIndex < 0 || endIndex >= textInfo.characterCount)
        {
            Debug.LogWarning("Invalid word index range.");
            yield break;
        }

        Mesh mesh = textMeshPro.mesh;
        Vector3[] originalVertices = mesh.vertices; // Original vertex positions
        Vector3[] vertices = new Vector3[originalVertices.Length];
        Array.Copy(originalVertices, vertices, originalVertices.Length);

        float timeElapsed = 0f;

        // Loop through the characters in the word and apply shake effect to their vertices
        while (timeElapsed < duration && textMeshPro.gameObject.activeInHierarchy && textMeshPro.text == text)
        {
            for (int i = startIndex; i <= endIndex; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible) continue; // Skip invisible characters (e.g., spaces)

                int vertexIndex = charInfo.vertexIndex;

                Vector3 offset = new Vector3(
                    Mathf.Sin(Time.time * shakeSpeed + i) * shakeIntensity,
                    Mathf.Cos(Time.time * shakeSpeed + i) * shakeIntensity, 0);

                // Reset vertices to original position and then apply the shake offset
                vertices[vertexIndex + 0] = originalVertices[vertexIndex + 0] + offset; // Bottom-left
                vertices[vertexIndex + 1] = originalVertices[vertexIndex + 1] + offset; // Top-left
                vertices[vertexIndex + 2] = originalVertices[vertexIndex + 2] + offset; // Top-right
                vertices[vertexIndex + 3] = originalVertices[vertexIndex + 3] + offset; // Bottom-right
            }

            // Apply the modified vertices back to the mesh
            mesh.vertices = vertices;
            textMeshPro.canvasRenderer.SetMesh(mesh);

            // Increment the time elapsed
            timeElapsed += Time.deltaTime;

            // Yield for the next frame
            yield return null;
        }

        if(textMeshPro.text != text) yield break;
        
        // Reset the text mesh to its original state after the shake duration is over
        mesh.vertices = originalVertices;
        textMeshPro.canvasRenderer.SetMesh(mesh);
    }
    
    /// <summary>
    /// Starts a coroutine to apply a size-changing effect to all characters within a specified word using an animation curve.
    /// </summary>
    /// <param name="textMeshPro">The TMP_Text component.</param>
    /// <param name="startIndex">The starting index of the word.</param>
    /// <param name="endIndex">The ending index of the word.</param>
    /// <param name="duration">The duration for which the size effect should apply.</param>
    /// <param name="sizeCurve">The animation curve that defines the size changes over time.</param>
    public static void ApplySizeEffect(this TMP_Text textMeshPro, int startIndex, int endIndex, float duration, AnimationCurve sizeCurve)
    {
        CoroutineController.Start(SizeCoroutine(textMeshPro, startIndex, endIndex, duration, sizeCurve));
    }

    /// <summary>
    /// Coroutine that applies a size-changing effect to characters for a specified duration.
    /// </summary>
    private static IEnumerator SizeCoroutine(TMP_Text textMeshPro, int startIndex, int endIndex, float duration, AnimationCurve sizeCurve)
    {
        // Force an update to the text mesh to initialize the character info
        textMeshPro.ForceMeshUpdate(true, true);

        TMP_TextInfo textInfo = textMeshPro.textInfo;
        string text = textMeshPro.text;

        if (startIndex < 0 || startIndex >= textInfo.characterCount || endIndex < 0 || endIndex >= textInfo.characterCount)
        {
            Debug.LogWarning("Invalid word index range.");
            yield break;
        }

        Mesh mesh = textMeshPro.mesh;
        Vector3[] originalVertices = mesh.vertices; // Original vertex positions
        Vector3[] vertices = new Vector3[originalVertices.Length];
        Array.Copy(originalVertices, vertices, originalVertices.Length);

        float timeElapsed = 0f;

        // Loop through the characters in the word and apply the size effect
        while (timeElapsed < duration && textMeshPro.gameObject.activeInHierarchy && textMeshPro.text == text)
        {
            float scale = sizeCurve.Evaluate(timeElapsed / duration); // Get scale factor from the curve

            for (int i = startIndex; i <= endIndex; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible) continue; // Skip invisible characters (e.g., spaces)

                int vertexIndex = charInfo.vertexIndex;

                // Calculate the center of the character's quad
                Vector3 charCenter = (originalVertices[vertexIndex + 0] + originalVertices[vertexIndex + 2]) / 2;

                // Scale each vertex around the center
                vertices[vertexIndex + 0] = charCenter + (originalVertices[vertexIndex + 0] - charCenter) * scale; // Bottom-left
                vertices[vertexIndex + 1] = charCenter + (originalVertices[vertexIndex + 1] - charCenter) * scale; // Top-left
                vertices[vertexIndex + 2] = charCenter + (originalVertices[vertexIndex + 2] - charCenter) * scale; // Top-right
                vertices[vertexIndex + 3] = charCenter + (originalVertices[vertexIndex + 3] - charCenter) * scale; // Bottom-right
            }

            // Apply the modified vertices back to the mesh
            mesh.vertices = vertices;
            textMeshPro.canvasRenderer.SetMesh(mesh);

            // Increment the time elapsed
            timeElapsed += Time.deltaTime;

            // Yield for the next frame
            yield return null;
        }

        if (textMeshPro.text != text) yield break;

        // Reset the text mesh to its original state after the effect duration is over
        mesh.vertices = originalVertices;
        textMeshPro.canvasRenderer.SetMesh(mesh);
    }


}
