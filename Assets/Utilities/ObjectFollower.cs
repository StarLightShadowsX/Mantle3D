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
    }
    public Rate rate;

    public Transform targetCustom;
    private Transform target;
    private void Awake()
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
    }
    private void Update()
    {
        if(rate is Rate.Update) transform.position = target.position;
    }
    private void FixedUpdate()
    {
        if(rate is Rate.FixedUpdate) transform.position = target.position;
    }
    private void LateUpdate()
    {
        if(rate is Rate.LateUpdate) transform.position = target.position;
    }
}
