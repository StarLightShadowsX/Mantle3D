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
        if (Overlay.OverHUD.BasicBlackout < 1) yield return Overlay.OverHUD.BasicFadeOutWait();
    }
}
