using UnityEngine;

[CreateAssetMenu(fileName = "RoomAsset", menuName = "Scriptable Objects/Room")]
public class RoomAsset : ScriptableObject
{
    // Serialized Data
    public string displayName;
    public SceneReference scene;

    //Active Data
    public RoomRoot root;

    /// <summary>
    /// Establishes a connection to the specified room root.
    /// </summary>
    /// <param name="root">The <see cref="RoomRoot"/> instance representing the room to connect to. Cannot be null.</param>
    public void Connect(RoomRoot root) => this.root = root;







}
