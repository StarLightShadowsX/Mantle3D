using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntranceInstant : Entrance, IRoomActor
{
    public override Type type => Type.Instant;

    public override IEnumerator Routine()
    {
        PlacePlayer();
        return null;
    }

    public override void PlacePlayer() => PlayerCore.Player.Place(transform.position);
}
