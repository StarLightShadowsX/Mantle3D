using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovablePlatform
{
    public List<IMovableBody> bodies { get; }

    public void AddBody(IMovableBody body) => bodies.Add(body);
    public void RemoveBody(IMovableBody body) => bodies.Remove(body);

    protected static void DoAnchorMove(IMovablePlatform This, Vector3 offset)
    {
        for (int i = 0; i < This.bodies.Count; i++) 
            This.bodies[i].Position += offset;
    }
}

public interface IMovableBody
{
    public Vector3 Position { get; set; }
}