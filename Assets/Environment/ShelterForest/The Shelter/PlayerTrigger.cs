using UnityEngine;
using PlayerCore;

[RequireComponent(typeof(BoxCollider))]
public class PlayerTrigger : MonoBehaviour
{
    public UltEvents.UltEvent Event;

    private void OnTriggerEnter(Collider other)
    {
        if (!Player.IsPlayer(other)) return;
        Event?.Invoke();
    }
}