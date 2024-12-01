using System;
using System.Collections.Generic;
using UnityEngine;
using UnityUtils;

namespace AdvancedController {
    [RequireComponent(typeof(PlayerMover)), RequireComponent(typeof(WallDetector))]
    public class PlayerController : MonoBehaviour {
        #region Fields

        private Transform _tr;
        private PlayerMover _mover;
        private WallDetector _wallDetector;
        
        [Header("Movement Settings")]
        public float movementSpeed = 7f;
        public float timeToReachSpeed = 0.1f;
        public float maxSpeed = 30f;
        public float airControlRate = 2f;
        public float airFriction = 0.5f;
        public float groundFriction = 100f;
        public float gravity = 30f;
        public float forcedSlideGravity = 5f;
        public float slopeLimit = 30f;
        public bool useLocalMomentum;

        private StateMachine _stateMachine;
        
        [Header("Camera Binding")]
        [SerializeField] public Transform cameraTransform;

        private Vector3 _momentum, _savedVelocity, _savedMovementVelocity;
        private float _lastTimeMoved;
        public event Action<Vector3> OnLand = delegate { };
        
        private MovementMechanic[] _movementMechanics;
        public IEnumerable<MovementMechanic> MovementMechanics => _movementMechanics;
        
        #endregion

        #region Properties
        
        public Vector3 Momentum 
        {
            get => useLocalMomentum ? _tr.localToWorldMatrix * _momentum : _momentum;
            set
            {
                _momentum = value; 
                if(useLocalMomentum) _momentum = _tr.localToWorldMatrix * _momentum;
            }
        }
        
        public Vector3 Velocity => _savedVelocity;
        private bool IsRising => Vector3.Dot(Momentum, _tr.up) > 0f;
        private bool IsFalling => Vector3.Dot(Momentum, _tr.up) < 0f;
        public bool IsGroundTooSteep => !_mover.IsGrounded() || Vector3.Angle(_mover.GetGroundNormal(), _tr.up) > slopeLimit;
        private bool IsGrounded => _stateMachine.CurrentState is GroundedState;
        public Vector3 MovementVelocity => _savedMovementVelocity;
        public bool IsMoving => _direction != Vector3.zero && _stateMachine.CurrentState is GroundedState;

        #endregion

        private void Awake() {
            _tr = transform;
            _mover = GetComponent<PlayerMover>();
            _wallDetector = GetComponent<WallDetector>();
            _movementMechanics = GetComponents<MovementMechanic>();

            foreach (var movementMechanic in _movementMechanics)
            {
                movementMechanic.Initialize(this, _mover, _wallDetector);
            }
            
            SetupStateMachine();
        }

        private void SetupStateMachine() {
            _stateMachine = new StateMachine();
            
            var grounded = new GroundedState(this);
            var falling = new FallingState(this);
            var forcedSliding = new ForcedSlidingState(this);
            var rising = new RisingState(this);
            
            At(grounded, rising, () => IsRising && !_mover.IsGrounded());
            At(grounded, forcedSliding, () => _mover.IsGrounded() && IsGroundTooSteep);
            At(grounded, falling, () => !_mover.IsGrounded());
            
            At(falling, forcedSliding, () => _mover.IsGrounded() && IsGroundTooSteep);
            At(falling, grounded, () => _mover.IsGrounded() && !IsGroundTooSteep);
            
            At(forcedSliding, grounded, () => _mover.IsGrounded() && !IsGroundTooSteep);
            At(forcedSliding, falling, () => IsFalling && !_mover.IsGrounded());
            
            At(rising, grounded, () => _mover.IsGrounded() && !IsGroundTooSteep);
            At(rising, forcedSliding, () => _mover.IsGrounded() && IsGroundTooSteep);
            At(rising, falling, () => IsFalling);

            foreach (var movementMechanic in _movementMechanics)
            {
                movementMechanic.InitializeStateTransitions(_stateMachine, grounded, falling, forcedSliding, rising, _movementMechanics);
            }
            _stateMachine.SetState(falling); // Initial state
            
            void At(IState from, IState to, Func<bool> condition) => _stateMachine.At(from, to, new FuncPredicate(condition));
            void Any(IState to, Func<bool> condition) => _stateMachine.Any(to, new FuncPredicate(condition));
        }
        

        private Vector3 _direction;

        private void Update()
        {
            _stateMachine.Update();
            HandleInput();
        }

        private void HandleInput()
        {
            foreach (var mechanic in _movementMechanics)
            {
                mechanic.HandleInput();
            }
            
            Vector3 previousDirection = _direction;
            _direction = InputManager.Instance.MoveInput;
            if (_direction.magnitude > 1f) _direction.Normalize();
            
            if(previousDirection == Vector3.zero && _direction != Vector3.zero) _lastTimeMoved = Time.time;
        }
        
        private void FixedUpdate() {
            _stateMachine.FixedUpdate();
            _mover.CheckForGround();
            HandleMomentum();
            Vector3 velocity = _stateMachine.CurrentState is GroundedState ? CalculateMovementVelocity() : Vector3.zero;
            foreach (var movementMechanic in _movementMechanics)
            {
                movementMechanic.HandleVelocity(ref velocity, _stateMachine.CurrentState);
            }
            
            velocity += useLocalMomentum ? _tr.localToWorldMatrix * _momentum : _momentum;
            
            Momentum = Vector3.ClampMagnitude(Momentum, maxSpeed);
            velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
            
            _mover.SetExtendSensorRange(IsGrounded);
            _mover.HandleFeetPosition(velocity, _stateMachine.CurrentState is GroundedState);
            _mover.SetVelocity(velocity);
            
            _savedVelocity = velocity;
            _savedMovementVelocity = CalculateMovementVelocity();
            
            foreach (var movementMechanic in _movementMechanics)
            {
                movementMechanic.HandleFixedUpdate();
            }
        }

        private Vector3 CalculateMovementVelocity()
        {
            float speed = movementSpeed;
            speed = Mathf.Lerp(0, movementSpeed, Mathf.Clamp01((Time.time - _lastTimeMoved) / timeToReachSpeed));
            return CalculateMovementDirection() * speed;
        }

        private Vector3 CalculateMovementDirection() {
            Vector3 direction = cameraTransform == null 
                ? _tr.right * _direction.x + _tr.forward * _direction.y 
                : Vector3.ProjectOnPlane(cameraTransform.right, _tr.up).normalized * _direction.x + 
                  Vector3.ProjectOnPlane(cameraTransform.forward, _tr.up).normalized * _direction.y;
            
            return direction.magnitude > 1f ? direction.normalized : direction;
        }

        private void HandleMomentum() {
            if (useLocalMomentum) _momentum = _tr.localToWorldMatrix * _momentum;
            
            Vector3 verticalMomentum = _momentum.ExtractDotVector(_tr.up);
            Vector3 horizontalMomentum = _momentum - verticalMomentum;
            
            // Apply gravity
            verticalMomentum -= _tr.up * (gravity * Time.deltaTime);
            foreach (var movementMechanic in _movementMechanics)
            {
                movementMechanic.HandleGravity(ref verticalMomentum, _stateMachine.CurrentState);
            }
            
            // Reset vertical momentum if grounded and moving downwards
            if (_stateMachine.CurrentState is GroundedState && Vector3.Dot(verticalMomentum, _tr.up) < 0f) {
                verticalMomentum = Vector3.zero;
            }
            
            if (!IsGrounded) {
                AdjustHorizontalMomentum(ref horizontalMomentum, CalculateMovementVelocity());
            }

            if (_stateMachine.CurrentState is ForcedSlidingState) {
                HandleForcedSliding(ref horizontalMomentum);
            }
            
            horizontalMomentum = ApplyFriction(horizontalMomentum);
            _momentum = horizontalMomentum + verticalMomentum;

            foreach (var movementMechanic in _movementMechanics)
            {
                movementMechanic.HandleMomentum(ref _momentum, _stateMachine.CurrentState);
            }
            
            if (_stateMachine.CurrentState is ForcedSlidingState) {
                _momentum = Vector3.ProjectOnPlane(_momentum, _mover.GetGroundNormal());
                if (Vector3.Dot(_momentum, _tr.up) > 0f) {
                     _momentum = _momentum.RemoveDotVector(_tr.up);
                }
            
                Vector3 slideDirection = Vector3.ProjectOnPlane(-_tr.up, _mover.GetGroundNormal()).normalized;
                _momentum += slideDirection * (forcedSlideGravity * Time.deltaTime);
            }

            // Prevent from sticking to the ceiling
            if (_wallDetector.HitCeiling() && Vector3.Dot(_momentum, _tr.up) > 0)
            {
                _momentum = _momentum.RemoveDotVector(_tr.up);
                _wallDetector.ResetCeiling();
            }
            
            // Prevent from sticking to the wall
            if (_wallDetector.HitWall() && Vector3.Dot(_momentum, -_wallDetector.GetWallNormal()) > 0)
            {
                _momentum = _momentum.RemoveDotVector(-_wallDetector.GetWallNormal());
                _wallDetector.ResetWall();
            }

            if (useLocalMomentum) _momentum = _tr.worldToLocalMatrix * _momentum;
        }

        private Vector3 ApplyFriction(Vector3 horizontalMomentum)
        {
            float friction = _stateMachine.CurrentState switch
            {
                GroundedState => groundFriction,
                _ => airFriction
            };
            foreach (var movementMechanic in _movementMechanics)
            {
                movementMechanic.HandleFriction(ref friction, _stateMachine.CurrentState);
            }
                
            horizontalMomentum = Vector3.MoveTowards(horizontalMomentum, Vector3.zero, friction * Time.deltaTime);
            return horizontalMomentum;
        }
        
        private void AdjustHorizontalMomentum(ref Vector3 horizontalMomentum, Vector3 movementVelocity) {
            float airControlRate = this.airControlRate;
            foreach (var movementMechanic in _movementMechanics)
            {
                movementMechanic.HandleAirControlRate(ref airControlRate, _stateMachine.CurrentState);
            }

            if (horizontalMomentum.magnitude > movementSpeed) {
                if (Vector3.Dot(movementVelocity, horizontalMomentum.normalized) > 0f) {
                    movementVelocity = movementVelocity.RemoveDotVector(horizontalMomentum.normalized);
                }
                horizontalMomentum += movementVelocity * (Time.deltaTime * airControlRate * 0.25f);
            }
            else {
                horizontalMomentum += movementVelocity * (Time.deltaTime * airControlRate);
                horizontalMomentum = Vector3.ClampMagnitude(horizontalMomentum, movementSpeed);
            }
        }

        private void HandleForcedSliding(ref Vector3 horizontalMomentum) {
            Vector3 pointDownVector = Vector3.ProjectOnPlane(_mover.GetGroundNormal(), _tr.up).normalized;

            Vector3 movementVelocity = CalculateMovementVelocity();
            movementVelocity = movementVelocity.RemoveDotVector(pointDownVector);

            horizontalMomentum += movementVelocity * Time.fixedDeltaTime;
        }

        public void OnGroundContactLost() {
            if (useLocalMomentum) _momentum = _tr.localToWorldMatrix * _momentum;

            Vector3 velocity = CalculateMovementVelocity();
            if (velocity.sqrMagnitude >= 0f && _momentum.sqrMagnitude > 0f) {
                Vector3 projectedMomentum = Vector3.Project(_momentum, velocity.normalized);
                float dot = Vector3.Dot(projectedMomentum.normalized, velocity.normalized);
                
                if (projectedMomentum.sqrMagnitude >= velocity.sqrMagnitude && dot > 0f) velocity = Vector3.zero;
                else if (dot > 0f) velocity -= projectedMomentum;
            }
            
            _momentum += velocity;
            
            if (useLocalMomentum) _momentum = _tr.worldToLocalMatrix * _momentum;
        }
        
        public void OnGroundContactLostFalling() {
            if (useLocalMomentum) _momentum = _tr.localToWorldMatrix * _momentum;

            Vector3 velocity = CalculateMovementVelocity();
            float speed = Mathf.Lerp(0, movementSpeed, Mathf.Clamp01((Time.time - _lastTimeMoved) / (timeToReachSpeed*20f)));
            velocity = velocity.normalized * speed;
            
            if (velocity.sqrMagnitude >= 0f && _momentum.sqrMagnitude > 0f) {
                Vector3 projectedMomentum = Vector3.Project(_momentum, velocity.normalized);
                float dot = Vector3.Dot(projectedMomentum.normalized, velocity.normalized);
                
                if (projectedMomentum.sqrMagnitude >= velocity.sqrMagnitude && dot > 0f) velocity = Vector3.zero;
                else if (dot > 0f) velocity -= projectedMomentum;
            }
            
            _momentum += velocity;
            
            if (useLocalMomentum) _momentum = _tr.worldToLocalMatrix * _momentum;
        }

        public void OnGroundContactRegained() {
            Vector3 collisionVelocity = useLocalMomentum ? _tr.localToWorldMatrix * _momentum : _momentum;
            OnLand.Invoke(collisionVelocity);
        }
    }
} 