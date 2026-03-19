using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using AYellowpaper;

public class RoomRoot : MonoBehaviour
{
    [field: SerializeField] public RoomAsset asset { get; private set; }
    [field: SerializeField] public List<GameObject> RootGameObjects { get; private set; } = new();


    [field: SerializeField] public IComponentList<IRoomActor> roomActors { get; private set; } = new();
    [field: SerializeField] public List<Entrance> entrances { get; private set; } = new();

    private void Awake() => asset.Connect(this);

    public static RoomRoot Find(Scene scene)
    {
        if (scene == null) return null;

        if (scene.GetRootGameObjects()[0].TryGetComponent(out RoomRoot firstAttempt)) return firstAttempt;

        foreach (GameObject root in scene.GetRootGameObjects())
        {
            RoomRoot room = root.GetComponentInChildren<RoomRoot>(true);
            if (room != null)
                return room;
        }

        return null;
    }
    public static RoomRoot Find(GameObject G) => Find(G.scene);
    public static RoomRoot Find(Component C) => Find(C.gameObject.scene);

    public void AddEntrance(Entrance entrance)
    {
        if (!Application.isEditor || Application.isPlaying || entrances.Contains(entrance)) return;
        roomActors.Add(entrance);
        entrances.Add(entrance);
    }
    public void RemoveEntrance(Entrance entrance)
    {
        if (!Application.isEditor || Application.isPlaying || !entrances.Contains(entrance)) return;
        roomActors.Remove(entrance);
        entrances.Remove(entrance);
    }


#if UNITY_EDITOR
    public class Editor
    {
        [InitializeOnLoad]
        public static class RoomRootSceneHook
        {
            static RoomRootSceneHook() => UnityEditor.SceneManagement.EditorSceneManager.sceneSaving += OnSceneSaving;

            private static void OnSceneSaving(Scene scene, string path)
            {
                GameObject[] objectArray = scene.GetRootGameObjects();
                if (!objectArray[0].TryGetComponent(out RoomRoot roomRoot)) return;
                roomRoot.RootGameObjects = objectArray.ToList();

                foreach (GameObject item in objectArray)
                {
                    IRoomActor[] actors = item.GetComponentsInChildren<IRoomActor>(true);
                    foreach (IRoomActor actor in actors) actor.Register();
                }

                roomRoot.roomActors.ClearNull();
                roomRoot.entrances.ClearNull();

                roomRoot.asset.entrances.Clear();
                foreach (Entrance entrance in roomRoot.entrances) roomRoot.asset.entrances.Add(new(entrance));

                EditorUtility.SetDirty(roomRoot);
            }
        }




    }
#endif
}

public interface IRoomActor
{
    RoomRoot Root { get; }

    public void Register();
    public void Deregister();
}

public static class Xtensions_RoomActors
{
    public static void RegisterWithRoot<T>(this T actor) where T : Component, IRoomActor
    {
        var root = RoomRoot.Find(actor);
        if (!root.roomActors.Contains(actor)) root.roomActors.Add(actor);
    }
    public static void DeregisterFromRoot<T>(this T actor) where T : Component, IRoomActor
    {
        var root = RoomRoot.Find(actor);
        root.roomActors.Remove(actor);
    }
    public static RoomRoot FindRoot<T>(this T actor) where T : Component, IRoomActor
    {
        if (actor == null || actor.gameObject.scene == null) return null;

        actor.gameObject.scene.GetRootGameObjects()[0].TryGetComponent(out RoomRoot res);
        return res;
    }
    public static bool FindRoot<T>(this T actor, out RoomRoot result) where T : Component, IRoomActor
    {
        result = null;
        return actor != null && actor.gameObject.scene != null && actor.gameObject.scene.GetRootGameObjects()[0].TryGetComponent(out result);
    }

}