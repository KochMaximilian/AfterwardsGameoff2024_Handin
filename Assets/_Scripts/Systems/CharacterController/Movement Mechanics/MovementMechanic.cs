using UnityEngine;

namespace AdvancedController
{
    public abstract class MovementMechanic : MonoBehaviour
    {
        protected PlayerController controller;
        protected Transform tr;
        protected PlayerMover mover;
        protected WallDetector WallDetector;
        
        public IState State { get; set; }
        public void Initialize(PlayerController controller, PlayerMover mover, WallDetector wallDetector)
        {
            this.controller = controller;
            tr = controller.transform;
            this.mover = mover;
            this.WallDetector = wallDetector;
            State = GetState();
        }
        
        public virtual void HandleInput() { }
        public virtual void HandleFixedUpdate() { }

        public virtual void HandleMomentum(ref Vector3 momentum, IState state) { }
        public virtual void HandleFriction(ref float friction, IState state) { }
        public virtual void HandleGravity(ref Vector3 verticalMomentum, IState state) { }
        public virtual void HandleAirControlRate(ref float airControlRate, IState state) { }
        public virtual void HandleVelocity(ref Vector3 velocity, IState state) { }
        
        public abstract void InitializeStateTransitions(StateMachine stateMachine, GroundedState grounded,
            FallingState falling, ForcedSlidingState forcedSliding, RisingState rising,
            MovementMechanic[] movementMechanics);
        
        protected abstract IState GetState();

    }
}