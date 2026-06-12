using System.Collections.Generic;
using SLS.Singletons;

public class RoomRegistry : GlobalAsset<RoomRegistry>
{
    [UnityEngine.SerializeField] internal List<RoomAsset> allRooms = new();

    public static IReadOnlyList<RoomAsset> AllRooms => Get.allRooms;

#if UNITY_EDITOR
    internal static void EnsureListed(RoomAsset room)
    {
        if (!Active) return;
        if(!Get.allRooms.Contains(room)) Get.allRooms.Add(room);
    }
#endif

}