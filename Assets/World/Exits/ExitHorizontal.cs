using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayerCore;
using UnityEngine;

public class ExitHorizontal : MonoBehaviour
{
    public Destination destination;

    private void OnTriggerEnter(Collider other)
    {
        if (Player.Controller != other) return;
        RoomManager.Transition(destination.roomDestination, destination.targetEntrance);
    }
}