using UnityEngine;

namespace UnityUtils {
    public static class CameraExtensions {
        /// <summary>
        /// Calculates and returns viewport extents with an optional margin. Useful for calculating a frustum for culling.
        /// </summary>
        /// <param name="camera">The camera object this method extends.</param>
        /// <param name="viewportMargin">Optional margin to be applied to viewport extents. Default is 0.2, 0.2.</param>
        /// <returns>Viewport extents as a Vector2 after applying the margin.</returns>
        public static Vector2 GetViewportExtentsWithMargin(this Camera camera, Vector2? viewportMargin = null) {
            Vector2 margin = viewportMargin ?? new Vector2(0.2f, 0.2f);

            Vector2 result;
            float halfFieldOfView = camera.fieldOfView * 0.5f * Mathf.Deg2Rad;
            result.y = camera.nearClipPlane * Mathf.Tan(halfFieldOfView);
            result.x = result.y * camera.aspect + margin.x;
            result.y += margin.y;
            return result;
        }
        
        /// <summary><para>Returns a vector with the 4 corners of the screen</para>
        /// <param name="targetObjectScale"> The scale of the object we want to be at the point</param>
        /// <param name="borderModification"> Multiplier for the borders to be bigger or smaller</param>
        /// <param name="targetObjectDistance"> (ONLY USE IN PERSPECTIVE MODE) The distance of the object we want to be at the point</param>
        /// </summary>
        public static Vector3[] GetBounds(this Camera cam, float targetObjectScale = 1, float borderModification = 1, float targetObjectDistance = 0)
        {
            Vector3 dist = cam.WorldToScreenPoint(cam.transform.forward * (cam.nearClipPlane + targetObjectScale + targetObjectDistance));
            dist = cam.ScreenToWorldPoint(dist);
        
            if (cam.orthographic) 
            {
                var rightTopBounds = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0)) + dist;
                rightTopBounds *= borderModification;
                var leftTopBounds = cam.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)) + dist;
                leftTopBounds *= borderModification;
                var rightBotBounds = cam.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)) + dist;
                rightBotBounds *= borderModification;
                var leftBotBounds = cam.ScreenToWorldPoint(new Vector3(0, 0, 0)) + dist;
                leftBotBounds *= borderModification;
                return new Vector3[] { leftBotBounds, rightBotBounds, rightTopBounds, leftTopBounds };
            }
        
            Vector3[] res = new Vector3[4];
            Ray ray = cam.ScreenPointToRay(new Vector3(0, 0, 0)+new Vector3(-Screen.width, -Screen.height, 0)* (borderModification-1));
            Plane plane = new Plane(cam.transform.forward, dist);
            plane.Raycast(ray, out var distance);
            res[0] = ray.GetPoint(distance);
                    
            ray = cam.ScreenPointToRay(new Vector3(Screen.width, 0, 0)+new Vector3(Screen.width, -Screen.height, 0)* (borderModification-1));
            plane.Raycast(ray, out distance);
            res[1] = ray.GetPoint(distance);
                    
            ray = cam.ScreenPointToRay(new Vector3(Screen.width, Screen.height, 0)* borderModification);
            plane.Raycast(ray, out distance);
            res[2] = ray.GetPoint(distance);
                    
            ray = cam.ScreenPointToRay(new Vector3(0, Screen.height, 0)+new Vector3(-Screen.width, Screen.height, 0)* (borderModification-1));
            plane.Raycast(ray, out distance);
            res[3] = ray.GetPoint(distance);
            return res;
        }
        
        private struct MyVector
        {
            public Vector3 startPoint;
            public Vector3 direction;
            public float distance;
        }
        private static readonly MyVector[] _vectors = new MyVector[4];
        /// <summary><para>Returns a random point in the bounds of the screen</para>
        /// <param name="targetObjectScale"> The scale of the object we want to be at the point</param>
        /// <param name="borderModification"> Multiplier for the borders to be bigger or smaller</param>
        /// <param name="distanceToTarget"> (ONLY USE IN PERSPECTIVE MODE) The distance of the object we want to be at the point</param>
        /// </summary>
        public static Vector3 GetRandomPointInBounds(this Camera cam, float targetObjectScale = 1, float borderModification = 1, float distanceToTarget = 0)
        {
            Vector3[] bounds = cam.GetBounds(targetObjectScale, borderModification, distanceToTarget);
            for (var i = 0; i < bounds.Length; i++)
            {
                _vectors[i] = new MyVector
                {
                    startPoint = bounds[i],
                    direction = (bounds[(i+1)%bounds.Length] - bounds[i]).normalized,
                    distance = Vector3.Distance(bounds[i], bounds[(i+1)%bounds.Length])
                };
            }
            MyVector vector = _vectors[Random.Range(0, 4)];
            return vector.startPoint + vector.direction * (vector.distance * Random.value);
        }
    }
}