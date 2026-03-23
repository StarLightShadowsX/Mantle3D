using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using AYellowpaper;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class RoomRoot : MonoBehaviour
{
    [field: SerializeField] public RoomAsset asset { get; private set; }
    [field: SerializeField, HideInInspector] public List<GameObject> RootGameObjects { get; private set; } = new();
    [field: SerializeField, HideInInspector] public IComponentList<IRoomActor> roomActors { get; private set; } = new();
    [field: SerializeField, HideInInspector] public List<Entrance> entrances { get; private set; } = new();

    private void Awake()
    {
        asset.Connect(this);

        if (GameSession.GameState == GameSession.GameStates.Null) GameSession.BeginRoom(this);
    }

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

#if UNITY_EDITOR
    [CustomEditor(typeof(RoomRoot))]
    private class Editor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new();

            SerializedProperty scriptProp = serializedObject.FindProperty("m_Script");
            PropertyField scriptField = new(scriptProp);
            scriptField.SetEnabled(false);
            root.Add(scriptField);


            RoomRoot This = target as RoomRoot;

            PropertyField assetField = new(serializedObject.FindProperty(nameof(asset).BackingField()));
            root.Add(assetField);

            SerializedProperty rootObjects = serializedObject.FindProperty(nameof(RoomRoot.RootGameObjects).BackingField());
            Foldout rootObjectsDisplay = new()
            {
                text = $"Root GameObjects : {This.RootGameObjects.Count}",
                value = false
            };
            root.Add(rootObjectsDisplay);
            for (int i = 0; i < This.RootGameObjects.Count; i++)
            {
                var iProp = rootObjects.GetArrayElementAtIndex(i);
                PropertyField iPropField = new(iProp, "");
                rootObjectsDisplay.Add(iPropField);
                iPropField.SetEnabled(false);
            }

            SerializedProperty roomActors = serializedObject.FindProperty($"{nameof(RoomRoot.roomActors).BackingField()}.list");
            Foldout roomActorsDisplay = new()
            {
                text = $"Room Actors : {This.roomActors.Count}",
                value = false
            };
            root.Add(roomActorsDisplay);
            for (int i = 0; i < This.roomActors.Count; i++)
            {
                var iProp = roomActors.GetArrayElementAtIndex(i);
                PropertyField iPropField = new(iProp, "");
                roomActorsDisplay.Add(iPropField);
                iPropField.SetEnabled(false);
            }

            SerializedProperty entrances = serializedObject.FindProperty(nameof(RoomRoot.entrances).BackingField());
            Foldout entrancesDisplay = new()
            {
                text = $"Entrances : {entrances.arraySize}",
                value = false
            };
            root.Add(entrancesDisplay);
            for (int i = 0; i < This.entrances.Count; i++)
            {
                var iProp = entrances.GetArrayElementAtIndex(i);
                PropertyField iPropField = new(iProp, "");
                entrancesDisplay.Add(iPropField);
                iPropField.SetEnabled(false);
            }

            return root;
        }

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
        RoomRoot root = RoomRoot.Find(actor);
        if (!root.roomActors.Contains(actor)) root.roomActors.Add(actor);
    }
    public static void DeregisterFromRoot<T>(this T actor) where T : Component, IRoomActor
    {
        RoomRoot root = RoomRoot.Find(actor);
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