using UnityEngine;

public class PauseMenu : Menu, ISingleton<PauseMenu>
{
    public static bool Active = false;
    public static bool canPause = true;

    private static PauseMenu instance;
    public static PauseMenu Get => ISingleton<PauseMenu>.Get(ref instance);
    public static PauseMenu Getter() => ISingleton<PauseMenu>.Get(ref instance);
    public static bool TryGet(out PauseMenu instance) => ISingleton<PauseMenu>.TryGet(Getter, out instance);
    public static bool Loaded => instance != null;


    protected override void Awake()
    {
        base.Awake();
        ISingleton<PauseMenu>.Register(ref instance, this);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        ISingleton<PauseMenu>.Unregister(ref instance, this);
    }
}
