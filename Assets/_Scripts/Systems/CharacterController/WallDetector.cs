using UnityEngine;

namespace AdvancedController {
    public class WallDetector : MonoBehaviour {
        public float ceilingAngleLimit = 10f;
        public float wallAngleLimit = 85f;
        public bool isInDebugMode;

        private bool _ceilingWasHit;
        private bool _wallWasHit;
        private Vector3 _wallNormal;

        private const float DebugDrawDuration = 2.0f;

        private Transform _tr;

        void Awake() {
            _tr = transform;
        }

        void OnCollisionEnter(Collision collision) => CheckFirstContact(collision);
        void OnCollisionStay(Collision collision) => CheckFirstContact(collision);

        void CheckFirstContact(Collision collision) {
            if (collision.contacts.Length == 0)
            {
                _ceilingWasHit = false;
                return;
            }

            float angle = Vector3.Angle(-_tr.up, collision.contacts[0].normal);

            _ceilingWasHit = angle < ceilingAngleLimit;
            _wallWasHit = angle > wallAngleLimit && angle < 180 - wallAngleLimit;
            _wallNormal = collision.contacts[0].normal;
            
            if (isInDebugMode) {
                Debug.DrawRay(collision.contacts[0].point, collision.contacts[0].normal, Color.red, DebugDrawDuration);
            }
        }

        public bool HitCeiling() => _ceilingWasHit;
        public bool HitWall() => _wallWasHit;
        public Vector3 GetWallNormal() => _wallNormal;
        public void ResetCeiling()
        {
            _ceilingWasHit = false;
        }
        
        public void ResetWall()
        {
            _wallWasHit = false;
        }
    }
}