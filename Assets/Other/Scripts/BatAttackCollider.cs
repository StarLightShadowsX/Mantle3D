using UnityEngine;

public class BatAttackCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Destructible d)) 
            d.DestroyThis();
    }
}