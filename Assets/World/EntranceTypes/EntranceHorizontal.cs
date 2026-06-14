using System.Collections;
using UnityEngine;

public class EntranceHorizontal : Entrance
{
    public Transform targetPosition;

    public override Type type => Type.HorizontalPassage;

    public override IEnumerator Routine()
    {
        PlacePlayer();
        return null;
    }
    public override void PlacePlayer() => PlayerCore.Player.Place(targetPosition.position, targetPosition.eulerAngles.y);

}