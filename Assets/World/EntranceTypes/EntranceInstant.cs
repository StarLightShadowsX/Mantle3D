using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntranceInstant : Entrance
{
    public override Type type => Type.Instant;

    public override void PlacePlayer() => Player.InstantMove(transform.position);
}
