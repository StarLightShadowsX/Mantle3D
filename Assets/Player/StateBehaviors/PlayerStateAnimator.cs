
using PlayerCore;
using SLS.StateMachineH;
using UnityEngine;

public class PlayerStateAnimator : StateAnimator
{
    public override Animator Animator => Player.Animator.Animator;
}