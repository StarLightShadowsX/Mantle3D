using UnityEngine;
using PlayerCore;

[RequireComponent(typeof(Collider))]
public class PlayerTrigger : MonoBehaviour
{
    public UltEvents.UltEvent Event;
    public UltEvents.UltEvent EventLeave;

    private void OnTriggerEnter(Collider other)
    {
        if (!Player.IsPlayer(other) || !this.enabled) return;
        Event?.Invoke();
    }
    private void OnTriggerExit(Collider other)
    {
        if (!Player.IsPlayer(other) || !this.enabled) return;
        EventLeave?.Invoke();
    }
}