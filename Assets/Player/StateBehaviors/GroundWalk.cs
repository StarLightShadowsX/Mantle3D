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
            if (Input.Move.sqrMagnitude == 0) return;
            Player.MovementBody.Direction.Set(Input.Move.ToXZ(), 180f);
            Player.MovementBody.Velocity.f = Input.Move.magnitude * (Input.Run.IsPressed() ? runSpeed : walkSpeed);
        }
    }

}