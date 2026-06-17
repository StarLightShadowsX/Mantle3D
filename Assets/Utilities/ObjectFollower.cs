using UnityEngine;

public class ObjectFollower : MonoBehaviour
{
    public enum Target
    {
        PlayerObject,
        PlayerVisual,
        RealCamera,
        NormalCamera,
        Custom
    }
    public Target targetType;
    public enum Rate
    {
        Update,
        FixedUpdate,
        LateUpdate,
        Attach,
    }
    public Rate rate;

    public Transform targetCustom;
    private Transform target;
    private void Start()
    {
        target = targetType switch
        {
            Target.PlayerObject => PlayerCore.Player.Transform,
            Target.PlayerVisual => PlayerCore.Player.Animator.transform,
            Target.RealCamera => Cameras.UnityCamera.transform,
            Target.NormalCamera => Cameras.NormalCamera.transform,
            Target.Custom => targetCustom,
            _ => null
        };
        if (target == null) Destroy(this);
        if (rate is Rate.Attach)
        {
            RoomAsset room = RoomAsset.Find(gameObject.scene);
            room.OnUnLoad += Unload;
            void Unload()
            {
                room.OnUnLoad -= Unload;
                Destroy(this.gameObject);
            }
            transform.parent = target;
        }
    }
    private void Update()
    {
        if (rate is Rate.Update) transform.position = target.position;
    }
    private void FixedUpdate()
    {
        if (rate is Rate.FixedUpdate) transform.position = target.position;
    }
    private void LateUpdate()
    {
        if (rate is Rate.LateUpdate) transform.position = target.position;
    }

}
