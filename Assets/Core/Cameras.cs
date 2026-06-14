using System.Collections;
using System.Collections.Generic;
using PlayerCore;
using SLS.Singletons;
using Unity.Cinemachine;
using UnityEngine;

[DefaultExecutionOrder(ExecutionOrders.GameplaySystems)]
public class Cameras : MonoBehaviour
{
    private static Singleton<Cameras> Singleton;
    public static Cameras Get => Singleton.Get;

    #region Instance Fields

    [SerializeField] CinemachineBrain inputBrain;
    [SerializeField] CinemachineCamera inputNormalCamera;

    #endregion Instance Fields

    public void Awake()
    {
        Singleton.Register(this);

        Brain = inputBrain;
        UnityCamera = inputBrain.GetComponent<Camera>();
        CurrentTransform = inputBrain.transform;

        NormalCamera = inputNormalCamera;

        CurrentVirtualCamera = NormalCamera;
        SetTargetVirtualCamera(NormalCamera);
    }

    private void OnDestroy()
    {
        Singleton.Deregister(this);
    }


    public static Camera UnityCamera;
    public static Transform CurrentTransform;
    public static CinemachineBrain Brain;

    public static CinemachineCamera CurrentVirtualCamera;
    public static CinemachineCamera NormalCamera;


    public static void SetTargetVirtualCamera(CinemachineCamera newTarget)
    {
        if(CurrentVirtualCamera != null) CurrentVirtualCamera.Priority = 0;
        if(CurrentVirtualCamera != null) CurrentVirtualCamera.gameObject.SetActive(false);
        CurrentVirtualCamera = newTarget;
        CurrentVirtualCamera.Priority = 10;
        CurrentVirtualCamera.gameObject.SetActive(true);
    }


    public static Quaternion CurrentRotationQ => CurrentVirtualCamera.State.GetFinalOrientation();
    public static Vector3 CurrentRotation => CurrentVirtualCamera.State.GetFinalOrientation().eulerAngles;

    public static Vector3 AdjustVector(Vector3 vector, bool yToo = false) => !yToo 
        ? vector.Rotated(CurrentRotation.y, Vector3.up) 
        : CurrentVirtualCamera.transform.TransformDirection(vector);


    public static void LockPrimary(bool lockPosition = true, bool lockRotation = false)
    {
        NormalCamera.Follow = lockPosition ? null : Player.Transform;
        NormalCamera.LookAt = lockRotation ? null : Player.Transform;
    }
}
