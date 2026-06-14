using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class RoomManager
{
    public static RoomAsset CurrentRoom;

    public static IEnumerator Transition(RoomAsset nextRoom, int targetEntranceID = 0)
    {
        if (Overlay.BetweenUI.Alpha < 1) yield return Overlay.BetweenUI.FadeAlpha(1);

        PlayerCore.Player.ActivityState = PlayerCore.Player.ActivityStates.Paused;
        yield return CurrentRoom.UnloadRoutine();
        yield return nextRoom.LoadRoutine();
        CurrentRoom = nextRoom;
        yield return null;
        yield return nextRoom.root.entrances[targetEntranceID].Routine();
        PlayerCore.Player.ActivityState = PlayerCore.Player.ActivityStates.Active;

        yield return Overlay.BetweenUI.FadeAlpha(0);
    }
}
