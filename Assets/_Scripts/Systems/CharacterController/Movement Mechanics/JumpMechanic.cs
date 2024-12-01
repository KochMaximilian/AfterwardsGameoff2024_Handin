using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityUtils;

namespace AdvancedController
{
    public class JumpMechanic : MovementMechanic
    {
        private bool _jumpKeyIsPressed;    // Tracks whether the jump key is currently being held down by the player
        private bool _jumpKeyWasPressed;   // Indicates if the jump key was pressed since the last reset, used to detect jump initiation
        private bool _jumpKeyWasLetGo;     // Indicates if the jump key was released since it was last pressed, used to detect when to stop jumping
        private bool _jumpInputIsLocked;   // Prevents jump initiation when true, used to ensure only one jump action per press
        private float _jumpTimer;
        
        [Header("Jump Settings")]
        public float jumpSpeed = 10f;
        public float jumpDuration = 0.2f;
        
        public event Action<Vector3> OnJump = delegate { };
        
        public override void HandleInput()
        {
            base.HandleInput();
            ResetJumpKeys();
            HandleJumpKeyInput(InputManager.Instance.SpaceBarInput.IsPressed());
        }

        public override void HandleMomentum(ref Vector3 momentum, IState state)
        {
            if (state is JumpingState)
            {
                HandleJumping();
            }
        }

        protected override IState GetState()
        {
            return new JumpingState(controller, this);
        }

        public override void InitializeStateTransitions(StateMachine stateMachine, GroundedState grounded,
            FallingState falling,
            ForcedSlidingState forcedSliding, RisingState rising, MovementMechanic[] movementMechanics)
        {
            State = new JumpingState(controller, this);

            //Exit transitions
            stateMachine.At(State, falling, new FuncPredicate(() => _jumpKeyWasLetGo));

            //Enter transitions
            stateMachine.At(grounded, State,
                new FuncPredicate(() => (_jumpKeyIsPressed || _jumpKeyWasPressed) && !_jumpInputIsLocked));
            stateMachine.At(forcedSliding, State,
                new FuncPredicate(() => (_jumpKeyIsPressed || _jumpKeyWasPressed) && !_jumpInputIsLocked));
        }

        private void HandleJumpKeyInput(bool isButtonPressed) {
            if (!_jumpKeyIsPressed && isButtonPressed) {
                _jumpKeyWasPressed = true;
            }

            if (_jumpKeyIsPressed && !isButtonPressed) {
                _jumpKeyWasLetGo = true;
                _jumpInputIsLocked = false;
            }
            
            if(_jumpKeyIsPressed && isButtonPressed && _jumpTimer >= jumpDuration)
            {
                _jumpKeyWasLetGo = true;
                _jumpInputIsLocked = false;
                _jumpTimer = 0;
            }
            
            _jumpKeyIsPressed = isButtonPressed;
        }

        private void HandleJumping() {
            _jumpTimer += Time.fixedDeltaTime;
            controller.Momentum = controller.Momentum.RemoveDotVector(tr.up);
            controller.Momentum += tr.up * jumpSpeed;
        }

        private void ResetJumpKeys() {
            _jumpKeyWasLetGo = false;
            _jumpKeyWasPressed = false;
        }

        public void OnJumpStart() {
            
            controller.Momentum += tr.up * jumpSpeed;
            _jumpInputIsLocked = true;
            OnJump.Invoke(controller.Momentum);
        }
    }
}