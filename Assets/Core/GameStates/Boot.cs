using SLS.GameStateMachine;
using SLS.Singletons;
using UnityEngine;

/// <summary>
/// A Game State deticated to Booting the game.
/// </summary>
public class Boot : GameStateSingle<Boot>
{
    protected override void OnEnterLogic()
    {
        DontDestroyOnLoad(Coroutine.Runner); //Create the Coroutine Runner.
        Overlay.Instantiate();
        Overlay.OverALL.Alpha = 1f;
    }
}
