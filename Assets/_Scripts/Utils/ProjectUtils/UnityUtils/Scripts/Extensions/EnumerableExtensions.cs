using System;
using System.Collections.Generic;

namespace UnityUtils {
    public static class EnumerableExtensions {
        /// <summary>
        /// Performs an action on each element in the sequence.
        /// </summary>
        /// <typeparam name="T">The type of elements in the sequence.</typeparam>
        /// <param name="sequence">The sequence to iterate over.</param>
        /// <param name="action">The action to perform on each element.</param>    
        public static void ForEach<T>(this IEnumerable<T> sequence, Action<T> action) {
            foreach (var item in sequence) {
                action(item);
            }
        }
        
        public static void DestroyAllGameObjects<T>(this IEnumerable<T> sequence) where T : UnityEngine.MonoBehaviour {
            foreach (var item in sequence) {
                UnityEngine.Object.Destroy(item.gameObject);
            }
        }
        
        public static void DestroyAll<T>(this IEnumerable<T> sequence, float delay) where T : UnityEngine.Object {
            foreach (var item in sequence) {
                UnityEngine.Object.Destroy(item, delay);
            }
        }
    }
}