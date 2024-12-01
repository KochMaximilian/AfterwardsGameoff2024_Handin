using System.Collections.Generic;
using UnityEngine;

namespace UnityUtils
{
    public static class RectTransformExtensions
    {
        /// <summary>
        /// Finds all RectTransforms that overlap with the target RectTransform within a specified list of RectTransforms.
        /// </summary>
        /// <param name="target">The target RectTransform to check for overlaps.</param>
        /// <param name="rectsToCheck">A list of RectTransforms to check for overlapping with the target.</param>
        /// <returns>A list of RectTransforms that overlap with the target RectTransform.</returns>
        public static List<RectTransform> GetOverlappingRects(this RectTransform target, List<RectTransform> rectsToCheck)
        {
            List<RectTransform> overlappingRects = new List<RectTransform>();

            // Get the world-space rectangle of the target RectTransform.
            Rect targetRect = GetWorldRect(target);

            foreach (var rectTransform in rectsToCheck)
            {
                // Skip the target RectTransform itself.
                if (rectTransform != target)
                {
                    // Get the world-space rectangle of the current RectTransform.
                    Rect rect = GetWorldRect(rectTransform);
                    
                    // Check if the target's rectangle overlaps with the current rectangle.
                    if (targetRect.Overlaps(rect))
                    {
                        overlappingRects.Add(rectTransform);
                    }
                }
            }

            return overlappingRects;
        }

        /// <summary>
        /// Calculates the world-space Rect for a given RectTransform.
        /// </summary>
        /// <param name="rectTransform">The RectTransform to get the world-space Rect for.</param>
        /// <returns>The world-space Rect of the RectTransform.</returns>
        private static Rect GetWorldRect(RectTransform rectTransform)
        {
            Vector3[] corners = new Vector3[4];
            
            // Get the four corners of the RectTransform in world space.
            rectTransform.GetWorldCorners(corners);

            // Calculate the size of the RectTransform based on the distance between corners.
            Vector2 size = new Vector2(
                Vector2.Distance(corners[0], corners[3]),
                Vector2.Distance(corners[0], corners[1])
            );

            // Return the world-space Rect starting from the bottom-left corner.
            return new Rect((Vector2)corners[0], size);
        }
    }
}