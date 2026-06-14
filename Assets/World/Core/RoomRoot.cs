using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using AYellowpaper;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Utilities.Xtensions;
using Core;

public class RoomRoot : MonoBehaviour
{
    [field: SerializeField] public RoomAsset asset { get; private set; }
    [field: SerializeField] public List<GameObject> RootGameObjects { get; private set; } = new();
    [field: SerializeField] public IComponentList<IRoomActor> roomActors { get; private set; } = new();
    [field: SerializeField] public List<Entrance> entrances { get; private set; } = new();

    private void Awake()
    {
        if (!Gameplay.Active)
        {
            Gameplay.BeginRoom(this);
            return;
        }
        asset.Connect(this);
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
    public class Editor : UnityEditor.Editor
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

            Foldout MakeDisplayOnlyList(string propName, string displayName)
            {
                SerializedProperty prop = serializedObject.FindProperty(propName);
                Foldout foldout = new()
                {
                    text = $"{displayName} : {prop.arraySize}",
                    value = false
                };
                for (int i = 0; i < prop.arraySize; i++)
                {
                    var iProp = prop.GetArrayElementAtIndex(i);
                    PropertyField iPropField = new(iProp, "");
                    foldout.Add(iPropField);
                    iPropField.SetEnabled(false);
                }
                return foldout;
            }

            root.Add(MakeDisplayOnlyList(nameof(RootGameObjects).BackingField(), "Root GameObjects"));
            root.Add(MakeDisplayOnlyList($"{nameof(roomActors).BackingField()}.list", "All RoomActors"));
            root.Add(MakeDisplayOnlyList(nameof(entrances).BackingField(), "Entrances"));

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

        public static void AttachAssetToRoot(RoomRoot root, RoomAsset asset)
        {
            root.asset = asset;
            UnityEditor.EditorUtility.SetDirty(root);
        }
    }
#endif
}

public interface IRoomActor
{
    RoomRoot Root { get; }

    public void Register();
    public void Deregister();

    public static bool RegisterWithRoot(Component actor, out RoomRoot rootResult)
    {
        rootResult = RoomRoot.Find(actor);
        if (rootResult != null) rootResult.roomActors.AddU(actor);
        return rootResult != null;
    }
    public static void DeregisterWithRoot(Component actor, ref RoomRoot rootResult)
    {
        RoomRoot root = RoomRoot.Find(actor);
        root.roomActors.Remove(actor);
    }
}

public static class Xtensions_RoomActors
{

}