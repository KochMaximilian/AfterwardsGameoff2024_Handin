using UnityEngine;
#if !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

namespace UnityUtils {
    public static class Vector2Extensions {
        /// <summary>
        /// Adds to any x y values of a Vector2
        /// </summary>
        public static Vector2 Add(this Vector2 vector2, float x = 0, float y = 0) {
            return new Vector2(vector2.x + x, vector2.y + y);
        }
        
        /// <summary>
        /// Adds a Vector2 to another Vector2
        /// </summary>
        public static Vector2 Add(this Vector2 vector2, Vector2 vectorToAdd) {
            return new Vector2(vector2.x + vectorToAdd.x, vector2.y + vectorToAdd.y);
        }
        
        /// <summary>
        /// Multiplies any x y values of a Vector2
        /// </summary>
        public static Vector2 Multiply(this Vector2 vector2, float x = 1, float y = 1) {
            return new Vector2(vector2.x * x, vector2.y * y);
        }
        
        /// <summary>
        /// Multiplies a Vector2 by another Vector2
        /// </summary>
        public static Vector2 Multiply(this Vector2 vector2, Vector2 vectorToMultiply) {
            return new Vector2(vector2.x * vectorToMultiply.x, vector2.y * vectorToMultiply.y);
        }
        
        /// <summary>
        /// Divides any x y values of a Vector2
        /// </summary>
        public static Vector2 Divide(this Vector2 vector2, float x = 1, float y = 1) {
            return new Vector2(vector2.x / x, vector2.y / y);
        }
        
        /// <summary>
        /// Divides a Vector2 by another Vector2
        /// </summary>
        public static Vector2 Divide(this Vector2 vector2, Vector2 vectorToDivide) {
            return new Vector2(vector2.x / vectorToDivide.x, vector2.y / vectorToDivide.y);
        }
        
        /// <summary>
        /// Sets any x y values of a Vector2
        /// </summary>
        public static Vector2 With(this Vector2 vector2, float? x = null, float? y = null) {
            return new Vector2(x ?? vector2.x, y ?? vector2.y);
        }

        /// <summary>
        /// Returns a Boolean indicating whether the current Vector2 is in a given range from another Vector2
        /// </summary>
        /// <param name="current">The current Vector2 position</param>
        /// <param name="target">The Vector2 position to compare against</param>
        /// <param name="range">The range value to compare against</param>
        /// <returns>True if the current Vector2 is in the given range from the target Vector2, false otherwise</returns>
        public static bool InRangeOf(this Vector2 current, Vector2 target, float range) {
            return (current - target).sqrMagnitude <= range * range;
        }
        
        /// <summary>
        /// Computes a random point in an annulus (a ring-shaped area) based on minimum and 
        /// maximum radius values around a central Vector2 point (origin).
        /// </summary>
        /// <param name="origin">The center Vector2 point of the annulus.</param>
        /// <param name="minRadius">Minimum radius of the annulus.</param>
        /// <param name="maxRadius">Maximum radius of the annulus.</param>
        /// <returns>A random Vector2 point within the specified annulus.</returns>
        public static Vector2 RandomPointInAnnulus(this Vector2 origin, float minRadius, float maxRadius) {
            float angle = Random.value * Mathf.PI * 2f;
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    
            // Squaring and then square-rooting radii to ensure uniform distribution within the annulus
            float minRadiusSquared = minRadius * minRadius;
            float maxRadiusSquared = maxRadius * maxRadius;
            float distance = Mathf.Sqrt(Random.value * (maxRadiusSquared - minRadiusSquared) + minRadiusSquared);
    
            // Calculate the position vector
            Vector2 position = direction * distance;
            return origin + position;
        }
        
        /// <summary>
        /// Calculates the signed angle between two vectors on a plane defined by a normal vector.
        /// </summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <param name="planeNormal">The normal vector of the plane on which to calculate the angle.</param>
        /// <returns>The signed angle between the vectors in degrees.</returns>
        public static float GetAngle(this Vector2 vector1, Vector2 vector2, Vector3 planeNormal) {
            var angle = Vector2.Angle(vector1, vector2);
            var sign = Mathf.Sign(Vector3.Dot(planeNormal, Vector3.Cross(vector1.ToVector3(), vector2.ToVector3())));
            return angle * sign;
        }

        /// <summary>
        /// Calculates the dot product of a vector and a normalized direction.
        /// </summary>
        /// <param name="vector">The vector to project.</param>
        /// <param name="direction">The direction vector to project onto.</param>
        /// <returns>The dot product of the vector and the direction.</returns>
        public static float GetDotProduct(this Vector2 vector, Vector2 direction) => Vector2.Dot(vector, direction.normalized);

        /// <summary>
        /// Removes the component of a vector that is in the direction of a given vector.
        /// </summary>
        /// <param name="vector">The vector from which to remove the component.</param>
        /// <param name="direction">The direction vector whose component should be removed.</param>
        /// <returns>The vector with the specified direction removed.</returns>
        public static Vector2 RemoveDotVector(this Vector2 vector, Vector2 direction) {
            direction.Normalize();
            return vector - direction * Vector2.Dot(vector, direction);
        }
        
        /// <summary>
        /// Extracts and returns the component of a vector that is in the direction of a given vector.
        /// </summary>
        /// <param name="vector">The vector from which to extract the component.</param>
        /// <param name="direction">The direction vector to extract along.</param>
        /// <returns>The component of the vector in the direction of the given vector.</returns>
        public static Vector2 ExtractDotVector(this Vector2 vector, Vector2 direction) {
            direction.Normalize();
            return direction * Vector2.Dot(vector, direction);
        }

        /// <summary>
        /// Rotates a vector onto a plane defined by a normal vector using a specified up direction.
        /// </summary>
        /// <param name="vector">The vector to be rotated onto the plane.</param>
        /// <param name="planeNormal">The normal vector of the target plane.</param>
        /// <param name="upDirection">The current 'up' direction used to determine the rotation.</param>
        /// <returns>The vector after being rotated onto the specified plane.</returns>
        public static Vector2 RotateVectorOntoPlane(this Vector2 vector, Vector3 planeNormal, Vector3 upDirection) {
            var rotation = Quaternion.FromToRotation(upDirection, planeNormal);
            var rotatedVector = rotation * vector.ToVector3();
            return new Vector2(rotatedVector.x, rotatedVector.y);
        }

        /// <summary>
        /// Projects a given point onto a line defined by a starting position and direction vector.
        /// </summary>
        /// <param name="point">The point to project onto the line.</param>
        /// <param name="lineStartPosition">The starting position of the line.</param>
        /// <param name="lineDirection">The direction vector of the line, which should be normalized.</param>
        /// <returns>The projected point on the line closest to the original point.</returns>
        public static Vector2 ProjectPointOntoLine(this Vector2 point, Vector2 lineStartPosition, Vector2 lineDirection) {
            var projectLine = point - lineStartPosition;
            var dotProduct = Vector2.Dot(projectLine, lineDirection);

            return lineStartPosition + lineDirection * dotProduct;
        }

        /// <summary>
        /// Increments a vector toward a target vector at a specified speed over a given time interval.
        /// </summary>
        /// <param name="currentVector">The current vector to be incremented.</param>
        /// <param name="speed">The speed at which to move towards the target vector.</param>
        /// <param name="deltaTime">The time interval over which to move.</param>
        /// <param name="targetVector">The target vector to approach.</param>
        /// <returns>The new vector incremented toward the target vector by the specified speed and time interval.</returns>
        public static Vector2 IncrementVectorTowardTargetVector(this Vector2 currentVector, float speed, float deltaTime, Vector2 targetVector) {
            return Vector2.MoveTowards(currentVector, targetVector, speed * deltaTime);
        }
        
        /// <summary>
        /// Returns a new vector maintaining only the axis that has the highest value in the vector
        /// </summary>
        /// <para name = "vector">The current vector</para>
        /// <para name = "compareAbsolutes">If true, the comparison will be made with the absolute values of the vector</para>
        /// <returns>A vector with value only in the axis with the highest value in the initial vector</returns>
        public static Vector2 GetPredominantAxis(this Vector2 vector, bool compareAbsolutes)
        {
            float x, y;
            if (compareAbsolutes)
            {
                x = Mathf.Abs(vector.x);
                y = Mathf.Abs(vector.y);
            }
            else
            {
                x = vector.x;
                y = vector.y;
            }

            if (x > y) return Vector2.right * vector.x;
            return Vector2.up * vector.y;
        }
        
        /// <summary>
        /// Returns a new vector with the value 1 in the axis with the highest value in the vector
        /// </summary>
        /// <para name = "vector">The current vector</para>
        /// <para name = "compareAbsolutes">If true, the comparison will be made with the absolute values of the vector</para>
        /// <returns>A vector with value 1 only in the axis with the highest value in the initial vector</returns>
        public static Vector2 GetPredominantAxisNormalized(this Vector2 vector, bool compareAbsolutes) {
            float x, y;
            if (compareAbsolutes)
            {
                x = Mathf.Abs(vector.x);
                y = Mathf.Abs(vector.y);
            }
            else
            {
                x = vector.x;
                y = vector.y;
            }

            if (x > y) return Vector2.right;
            return Vector2.up;
        }
        
#if ENABLE_LEGACY_INPUT_MANAGER
        /// <summary>
        /// Returns the angle in degrees between this position and the mouse position in a 2D space
        /// </summary>
        /// <para name="position">The point to get the angle between the pointer position</para>
        /// <returns>The angle in degrees between the position and the mouse pointer</returns>
        public static float GetAngleToPointer(this Vector2 position)
        {
            Vector3 pointerDirection = Input.mousePosition;
            pointerDirection = Helpers.Camera.ScreenToWorldPoint(pointerDirection);
            pointerDirection = pointerDirection.Add(-position.x, -position.y).normalized;

            float angle = Mathf.Atan2(pointerDirection.y, pointerDirection.x) * Mathf.Rad2Deg;
            while (angle<0) angle += 360;

            return angle;
        }
#else
        /// <summary>
        /// Returns the angle in degrees between this position and the mouse position in a 2D space
        /// </summary>
        /// <para name="position">The point to get the angle between the pointer position</para>
        /// <returns>The angle in degrees between the position and the mouse pointer</returns>
        public static float GetAngleToPointer(this Vector2 position)
        {
            Vector3 attackDirection = UnityEngine.InputSystem.Pointer.current.position.ReadValue();
            attackDirection = Helpers.Camera.ScreenToWorldPoint(attackDirection);
            attackDirection = (attackDirection.Add(-position.x, -position.y)).normalized;

            float angle = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg;
            while (angle<0) angle += 360;

            return angle;
        }
       
#endif
    }
}