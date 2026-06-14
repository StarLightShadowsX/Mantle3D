using System;
using UnityEngine;

public class ExitRandomizer : MonoBehaviour
{
    public RandomizedExitData[] choices;
    public float max;
    [Serializable]
    public struct RandomizedExitData
    {
        public RoomAsset targetRoom;
        public int targetEntrance;
        public float chance;
    }

    private void OnEnable()
    {
        float r = UnityEngine.Random.Range(0, max);
        int i = 0;
        while (i < choices.Length)
        {
            if (r < choices[i].chance) break;
            r -= choices[i].chance;
            i++;
        }
    }
}