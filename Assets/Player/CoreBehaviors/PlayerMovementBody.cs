using PlayerCore;
using SLS.Physics3D;
using UnityEngine;

public class PlayerMovementBody : PhysicsBody
{
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        Player.Animator.UpdatePosition();
        Player.Animator.SetSpeed(Velocity.f);
    }

    public void DoAwake() => Awake();
}
