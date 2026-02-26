using UnityEngine;

public class PauseMenu : Menu
{
    public static bool Active = false;
    public static bool canPause = true;

    private static PauseMenu instance;
    public static PauseMenu Get => Singleton.Get(ref instance);
    public static bool TryGet(out PauseMenu instance) => Get.Gotten(out instance);
    public static bool Loaded => instance != null;


    protected override void Awake()
    {
        base.Awake();
        Singleton.Register(ref instance, this);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        Singleton.Unregister(ref instance, this);
    }
}
