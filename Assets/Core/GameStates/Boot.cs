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
        var C = Coroutine.Runner; //Create the Coroutine Runner.
        var res = IGlobalPrefab.Instantiate<Overlay>();
        Overlay.ActiveOverlays.Add(Overlay.OverlayLayer.OverMenus, res.GetComponent<Overlay>());
        DontDestroyOnLoad(res);
    }
}
