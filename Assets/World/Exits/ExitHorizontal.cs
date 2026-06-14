using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayerCore;
using UnityEngine;

public class ExitHorizontal : Exit
{
    private void OnTriggerEnter(Collider other)
    {
        if (Player.Collider != other) return;
        RoomManager.Transition(targetRoom, targetEntrance).Begin();
    }
}