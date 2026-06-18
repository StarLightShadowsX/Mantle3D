using UnityEngine;

public class Destructible : MonoBehaviour
{
    public GameObject poofFX;
    public Vector3 poofOffset;

    public void DestroyThis()
    {
        gameObject.SetActive(false);
        poofFX.SetActive(true);
    }
}