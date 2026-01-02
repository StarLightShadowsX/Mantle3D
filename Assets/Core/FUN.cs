using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// "Fluctuating Universe Number" A random number generator with different values that change at differing times.
/// </summary>
public static class FUN
{
    public static int Playthrough { get; private set; } = 0;
    public static int Session { get; private set; } = 0;
    public static int Room { get; private set; } = 0;
    public static int Hour { get; private set; } = 0;

    public static int Roll() => UnityEngine.Random.Range(0, 101);
    public static void SetPlaythrough(int v) => Playthrough = v;
    public static void RollSession() => Session = Roll();
    public static void RollRoom() => Room = Roll();
}