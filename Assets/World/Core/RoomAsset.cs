using System.Collections.Generic;
using UnityEngine;
using Scene = UnityEngine.SceneManagement.Scene;
using UnityEngine.UIElements;
using System.Collections;
using UnityEditor.SceneManagement;


#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif

[CreateAssetMenu(fileName = "RoomAsset", menuName = "Scriptable Objects/Room")]
public class RoomAsset : SceneAsset
{
    // Serialized Data
    public string displayName;
    public List<Entrance.Data> entrances = new();

    //Active Data
    public RoomRoot root { get; private set; }

    /// <summary>
    /// Establishes a connection to the specified room root.
    /// </summary>
    /// <param name="root">The <see cref="RoomRoot"/> instance representing the room to connect to. Cannot be null.</param>
    public void Connect(RoomRoot root) => this.root = root;

    public static RoomAsset Find(Scene scene)
    {
        if (scene == null) return null;
        RoomRoot FindRes = RoomRoot.Find(scene);
        return FindRes == null ? null : FindRes.asset;
    }
    public static RoomAsset Find(GameObject G) => Find(G.scene);
    public static RoomAsset Find(Component C) => Find(C.gameObject.scene);

#if UNITY_EDITOR
    private void OnEnable() => RoomRegistry.EnsureListed(this);
#endif
    public void EnterAtEntrance(int id)
    {
        En().Begin();
        IEnumerator En()
        {
            Load();
            yield return new WaitUntil(() => root != null);
            root.entrances[id].PlacePlayer();
        }
    }


    public static implicit operator RoomAsset(RoomRoot room) => room.asset;
    public static implicit operator RoomRoot(RoomAsset room) => room.root;

#if UNITY_EDITOR
    [UnityEditor.Callbacks.OnOpenAsset()]
    private static bool DoubleClick(int instanceID, int line)
    {
        Object target = EditorUtility.InstanceIDToObject(instanceID);
        if(target is RoomAsset room)
        {
            room.OpenScene();
            return true;
        }
        return false;
    }

    public void OpenScene() => UnityEditor.SceneManagement.EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(Scene.asset));

    [CustomEditor(typeof(RoomAsset))]
    public class Editor : UnityEditor.Editor
    {
        SerializedProperty displayNameProp;
        PropertyField displayNameField;
        SerializedProperty sceneProp;
        PropertyField sceneField;
        SerializedProperty entrancesProp;
        Foldout entrancesDisplay;

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new();

            displayNameProp = serializedObject.FindProperty(nameof(displayName));
            displayNameField = new(displayNameProp);

            sceneProp = serializedObject.FindBackingField(nameof(Scene));
            sceneField = new(sceneProp);

            entrancesProp = serializedObject.FindProperty(nameof(entrances));
            entrancesDisplay = new()
            {
                text = "Entrances",
            };
            for (int i = 0; i < entrancesProp.arraySize; i++)
            {
                SerializedProperty ElementProp = entrancesProp.GetArrayElementAtIndex(i);
                string EName = ElementProp.FindPropertyRelative(nameof(Entrance.Data.name).BackingField()).stringValue;
                SerializedProperty ETypeProp = ElementProp.FindPropertyRelative(nameof(Entrance.Data.type).BackingField());
                string EType = ETypeProp.enumDisplayNames[ETypeProp.enumValueIndex];
                Label L = new($"{i} - {EName} ({EType})");
                entrancesDisplay.Add(L);
            }

            root.Add(displayNameField);
            root.Add(sceneField);
            root.Add(entrancesDisplay);

            return root;
        }

        public static RoomAsset CREATE(string displayName)
        {
            string fileName = displayName.Replace(" ", "");

            string roomPath = $"Assets/Content/Rooms/{fileName}.asset";
            string scenePath = $"Assets/Content/Rooms/{fileName}.unity";

            RoomAsset room = RoomAsset.CreateInstance<RoomAsset>();
            AssetDatabase.CreateAsset(room, roomPath);

            if (!AssetDatabase.CopyAsset("Assets/World/RoomPrefab.unity", scenePath)) return null;

            room.displayName = displayName;
            room.Scene = new(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(scenePath));
            EditorUtility.SetDirty(room);
            RoomRegistry.EnsureListed(room);
            EditorUtility.SetDirty(RoomRegistry.Get);

            AssetDatabase.SaveAssets();

            // Open, attach, save, and close scene
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            RoomRoot.Editor.AttachAssetToRoot(scene.GetRootGameObjects()[0].GetComponent<RoomRoot>(), room);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            EditorSceneManager.CloseScene(scene, true);

            Debug.Log($"Successfully created new Room: {displayName}. Note that its Scene cannot be automatically registered in the build settings, YOU have to do that.");

            return room;
        }
    }
#endif 
}
