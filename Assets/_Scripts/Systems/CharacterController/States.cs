using UnityEngine;

namespace AdvancedController {
    public class GroundedState : BaseState {
        readonly PlayerController controller;

        public GroundedState(PlayerController controller) {
            this.controller = controller;
        }

        public override void Enter()
        {
            base.Enter();
            controller.OnGroundContactRegained();
        }
    }

    public class FallingState : BaseState {
        readonly PlayerController controller;

        public FallingState(PlayerController controller) {
            this.controller = controller;
        }
        
        public override void Enter() {
            controller.OnGroundContactLostFalling();
        }
    }

    public class ForcedSlidingState : BaseState {
        readonly PlayerController controller;

        public ForcedSlidingState(PlayerController controller) {
            this.controller = controller;
        }

        public override void Enter(){
            controller.OnGroundContactLost();
        }
    }

    public class RisingState : BaseState {
        readonly PlayerController controller;
        
        public RisingState(PlayerController controller) {
            this.controller = controller;
        }

        public override void Enter() {
            controller.OnGroundContactLost();
        }
    }

    public class JumpingState : BaseState {
        readonly PlayerController controller;
        readonly JumpMechanic jumpMechanic;

        public JumpingState(PlayerController controller, JumpMechanic jumpMechanic) {
            this.controller = controller;
            this.jumpMechanic = jumpMechanic;
        }

        public override void Enter() {
            controller.OnGroundContactLost();
            jumpMechanic.OnJumpStart();
        }
    }
}