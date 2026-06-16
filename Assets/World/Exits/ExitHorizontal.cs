using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayerCore;
using UnityEngine;

public class ExitHorizontal : Exit
{
    public bool freezePlayer = false;
    public bool freezeCamera = true;

    private void OnTriggerEnter(Collider other)
    {
        if (Player.Collider != other) return;
        if (freezePlayer) Player.ActivityState = Player.ActivityStates.Paused;
        if (freezeCamera) Cameras.LockPrimary();
        RoomManager.Transition(targetRoom, targetEntrance).Begin();
    }
}