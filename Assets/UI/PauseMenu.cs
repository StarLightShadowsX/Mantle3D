using UnityEngine;
using Utilities.Singletons;

public class PauseMenu : Menu
{
    public static bool Active = false;
    public static bool canPause = true;

    private static Singleton<PauseMenu> Singleton;
    public static PauseMenu Get => Singleton.Get;
    public static bool TryGet(out PauseMenu instance) => Singleton.TryGet(out instance);
    public static bool Loaded => Singleton.Active;


    protected override void Awake()
    {
        base.Awake();
        Singleton.Register(this);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        Singleton.Deregister(this);
    }
}
