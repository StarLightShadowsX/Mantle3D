using System.Collections;
using UnityEngine;

public class EntranceHorizontal : Entrance
{
    public override Type type => Type.HorizontalPassage;

    public override IEnumerator Routine()
    {
        yield return null;
        PlacePlayer();
    }
    public override void PlacePlayer() => PlayerCore.Player.Place(transform.position, transform.forward);

}