using UnityEngine;

public abstract class Entrance : MonoBehaviour
{
    public enum Type
    {
        Instant = -1,
        HorizontalPassage,
        Door,
        Elevator,
        Stairs,
        FallFromCeiling,
        JumpFromPit,
    }

    public class Data
    {
        public Type type { get; private set; }
        public string name { get; private set; }
        public int id { get; private set; }
    }

    public abstract Type type { get; }
    public string Name;
    public int ID;
    public RoomRoot RoomRoot;

    public abstract void PlacePlayer()
    {

    }
}

