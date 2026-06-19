using SLS.StateMachineH;
using UnityEngine;
using PlayerCore;

namespace PlayerBehaviors
{
    public class GroundWalk : StateBehavior
    {
        public float walkSpeed;
        public float runSpeed;

        protected override void OnFixedUpdate()
        {
            //Debug.Log($"Move is {Input.Move}, Direction is {Player.MovementBody.Direction.value}");
            Player.MovementBody.Direction.Set(Input.Move.ToXZ());
            Player.MovementBody.Velocity.f = Input.Move.magnitude * (PlayerController.Sprinting ? runSpeed : walkSpeed);
        }

        protected override void OnExit(State next) => Player.MovementBody.Velocity.f = 0;
    }

}