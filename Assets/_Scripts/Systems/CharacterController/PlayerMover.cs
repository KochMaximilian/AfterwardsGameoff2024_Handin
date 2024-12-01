using UnityEngine;
using UnityUtils;

namespace AdvancedController {
    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
    public class PlayerMover : MonoBehaviour {
        #region Fields
        [Header("Collider Settings:")]
        [Range(0f, 1f)] [SerializeField] private float stepHeightRatio = 0.1f;
        [SerializeField] private float colliderHeight = 2f;
        [SerializeField] private float colliderThickness = 1f;
        [SerializeField] private Vector3 colliderOffset = Vector3.zero;

        private Rigidbody _rb;
        private Transform _tr;
        private CapsuleCollider _col;
        private RaycastSensor _sensor;

        private bool _isGrounded;
        private float _baseSensorRange;
        private Vector3 _currentGroundAdjustmentVelocity; // Velocity to adjust player position to maintain ground contact
        private int _currentLayer;
        
        [Header("Sensor Settings:")]
        [SerializeField] bool isInDebugMode;

        private bool _isUsingExtendedSensorRange = true; // Use extended range for smoother ground transitions
        #endregion

        void Awake() {
            Setup();
            RecalculateColliderDimensions();
        }

        void OnValidate() {
            if (gameObject.activeInHierarchy) {
                RecalculateColliderDimensions();
            }
        }
        
        void Update() {
            if (isInDebugMode) {
                _sensor.DrawDebug();
            }
        }

        public void CheckForGround() {
            if (_currentLayer != gameObject.layer) {
                RecalculateSensorLayerMask();
            }
            
            _currentGroundAdjustmentVelocity = Vector3.zero;
            _sensor.castLength = _isUsingExtendedSensorRange 
                ? _baseSensorRange + colliderHeight * _tr.localScale.x * stepHeightRatio
                : _baseSensorRange;

            _sensor.Cast();
            
            _isGrounded = _sensor.HasDetectedHit();
        }
        
        public void HandleFeetPosition(Vector3 velocity, bool isGrounded) {
            if (isGrounded)
            {
                _sensor.Cast(velocity.Multiply(Time.fixedDeltaTime, 0, Time.fixedDeltaTime));
                _isGrounded = _sensor.HasDetectedHit();
            }

            if (!_isGrounded) return;

            float distance = _sensor.GetDistance();
            float upperLimit = colliderHeight * _tr.localScale.x * (1f - stepHeightRatio) * 0.5f;
            
            float middle = upperLimit + colliderHeight * _tr.localScale.x * stepHeightRatio;
            float distanceToGo = middle - distance;
            
            _currentGroundAdjustmentVelocity = _tr.up * (distanceToGo / Time.fixedDeltaTime);
        }
        
        public bool IsGrounded() => _isGrounded;
        public Vector3 GetGroundNormal() => _sensor.GetNormal();
        
        public void SetVelocity(Vector3 velocity)
        {
            _rb.linearVelocity = velocity + _currentGroundAdjustmentVelocity;
        }

        public void SetExtendSensorRange(bool isExtended) => _isUsingExtendedSensorRange = isExtended;

        void Setup() {
            _tr = transform;
            _rb = GetComponent<Rigidbody>();
            _col = GetComponent<CapsuleCollider>();
            
            _rb.freezeRotation = true;
            _rb.useGravity = false;
        }

        void RecalculateColliderDimensions() {
            if (_col == null) {
                Setup();
            }
            
            _col.height = colliderHeight * (1f - stepHeightRatio);
            _col.radius = colliderThickness / 2f;
            _col.center = colliderOffset * colliderHeight + new Vector3(0f, stepHeightRatio * _col.height / 2f, 0f);

            if (_col.height / 2f < _col.radius) {
                _col.radius = _col.height / 2f;
            }
            
            RecalibrateSensor();
        }

        void RecalibrateSensor() {
            _sensor ??= new RaycastSensor(_tr);
            
            _sensor.SetCastOrigin(_col.bounds.center);
            _sensor.SetCastDirection(RaycastSensor.CastDirection.Down);
            RecalculateSensorLayerMask();
            
            const float safetyDistanceFactor = 0.001f; // Small factor added to prevent clipping issues when the sensor range is calculated
            
            float length = colliderHeight * (1f - stepHeightRatio) * 0.5f + colliderHeight * stepHeightRatio;
            _baseSensorRange = length * (1f + safetyDistanceFactor) * _tr.localScale.x;
            _sensor.castLength = length * _tr.localScale.x;
        }

        void RecalculateSensorLayerMask() {
            int objectLayer = gameObject.layer;
            int layerMask = Physics.AllLayers;

            for (int i = 0; i < 32; i++) {
                if (Physics.GetIgnoreLayerCollision(objectLayer, i)) {
                    layerMask &= ~(1 << i);
                }
            }
            
            int ignoreRaycastLayer = LayerMask.NameToLayer("Ignore Raycast");
            layerMask &= ~(1 << ignoreRaycastLayer);
            
            _sensor.layermask = layerMask;
            _currentLayer = objectLayer;
        }
        
        public void SetStepHeightRatio(float ratio) {
            stepHeightRatio = Mathf.Clamp(ratio, 0f, 1f);
            RecalculateColliderDimensions();
        }

        public float GetStepHeightRatio()
        {
            return stepHeightRatio;
        }
    }
}