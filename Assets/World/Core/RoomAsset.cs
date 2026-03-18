using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif

[CreateAssetMenu(fileName = "RoomAsset", menuName = "Scriptable Objects/Room")]
public class RoomAsset : ScriptableObject
{
    // Serialized Data
    public string displayName;
    public SceneReference scene;
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
    [CustomEditor(typeof(RoomAsset))]
    private class Editor : UnityEditor.Editor
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

            sceneProp = serializedObject.FindProperty(nameof(scene));
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
    }
#endif 
}
