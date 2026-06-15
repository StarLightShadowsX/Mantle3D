using UnityEngine;

public class PlayerFollower : MonoBehaviour
{
    private Transform player;
    private void Awake() => player = PlayerCore.Player.Transform;
    private void FixedUpdate() => transform.position = player.position;
}
