using UnityEngine;

public class RoomRoot : MonoBehaviour
{
    [SerializeField] RoomAsset asset;

    private void Awake() => asset.Connect(this);


}
