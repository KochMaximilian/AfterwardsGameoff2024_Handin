using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif
#if !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

namespace UnityUtils {
    public static class Helpers {
        public static WaitForSeconds GetWaitForSeconds(float seconds) {
            return WaitFor.Seconds(seconds);
        }
        
        private static Camera _camera;
        public static Camera Camera
        {
            get
            {
                if(_camera == null) _camera = Camera.main;
                return _camera;
            }
        }
        
        private static Camera _uiCamera;
        public static Camera UICamera
        {
            get
            {
                if(_uiCamera == null) _uiCamera = GameObject.FindWithTag("UICamera").GetComponent<Camera>();
                return _camera;
            }
        }
    
        private static PointerEventData _eventDataCurrentPosition;
        private static List<RaycastResult> _results;
        
#if ENABLE_LEGACY_INPUT_MANAGER
        /// <summary>
        /// returns true if the pointer is over a UI element
        /// </summary>
        public static bool PointerIsOverUi()
        {
            _eventDataCurrentPosition = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
            _results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(_eventDataCurrentPosition, _results);
            return _results.Count > 0;
        }
#else
        /// <summary>
        /// returns true if the pointer is over a UI element
        /// </summary>
        public static bool PointerIsOverUi()
        {
            if (UnityEngine.InputSystem.Pointer.current != null)
            {
                _eventDataCurrentPosition = new PointerEventData(EventSystem.current)
                {
                    position = UnityEngine.InputSystem.Pointer.current.position.ReadValue()
                };
                _results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(_eventDataCurrentPosition, _results);
                return _results.Count > 0;
            }
            return false;
        }
#endif

        public static Vector2 GetWorldPositionOfCanvasElement(RectTransform element)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(element, element.position, Camera, out var result);
            return result;
        }
        
        public static float GetRandomSign()
        {
            return Random.value < 0.5f ? -1 : 1;
        }

        /// <summary>
        /// Clears the console log in the Unity Editor.
        /// </summary
#if UNITY_EDITOR        
        public static void ClearConsole() {
            var assembly = Assembly.GetAssembly(typeof(SceneView));
            var type = assembly.GetType("UnityEditor.LogEntries");
            var method = type.GetMethod("Clear");
            method?.Invoke(new object(), null);
        }
#endif        
    }
}
