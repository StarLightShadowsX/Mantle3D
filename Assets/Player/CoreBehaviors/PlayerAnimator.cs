using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerCore
{
    /// <summary>
    /// A unique Player behavior that is actually a separate object from the player itself, and simply tries its best to visually match the current state of the actual Player.
    /// </summary>
    [DefaultExecutionOrder(ExecutionOrders.PlayerVisualizer)]
    public class PlayerAnimator : MonoBehaviour
    {

        [field: SerializeField] public Animator Animator { get; private set; }

        private readonly int dirXHash = Animator.StringToHash("DirX");
        private readonly int dirYHash = Animator.StringToHash("DirY");
        private readonly int speedHash = Animator.StringToHash("RunSpeed");

        //private void Update() => transform.position = Player.Position;
        //private void FixedUpdate() => transform.position = Player.Position;
        public void UpdatePosition()
        {
            transform.position = Player.Position;
            SetDirection(Player.Forward);
        }

        public void Play(string name) => Animator.Play(name);


        public void SetDirection() => SetDirection(Cameras.AdjustVector(Player.Forward));
        public void SetDirection(Vector3 direction)
        {
            Animator.SetFloat(dirXHash, direction.x);
            Animator.SetFloat(dirYHash, direction.z);
            //Change the current animation to the correct visual direction.
        }
        public void SetSpeed(float speed) => Animator.SetFloat(speedHash, speed);
    }

}