using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class RoomManager
{
    public static RoomAsset CurrentRoom;

    public static IEnumerator Transition(RoomAsset nextRoom, int targetEntranceID = 0)
    {
        PlayerCore.Player.ActivityState = PlayerCore.Player.ActivityStates.Paused;

        if (Overlay.BetweenUI.Alpha < 1) yield return Overlay.BetweenUI.FadeAlpha(1, .4f);

        yield return CurrentRoom.UnloadRoutine();
        yield return nextRoom.LoadRoutine(); 
        CurrentRoom = nextRoom;
        yield return new WaitUntil(() => Cameras.Brain.IsValid);
        yield return nextRoom.root.entrances[targetEntranceID].Routine();

        yield return Overlay.BetweenUI.FadeAlpha(0, .4f);
        Cameras.LockPrimary(false);
        PlayerCore.Player.ActivityState = PlayerCore.Player.ActivityStates.Active;
    }
}
