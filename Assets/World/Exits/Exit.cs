using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using SLS.EditorUtilities.Editor;
#endif

public abstract class Exit : MonoBehaviour
{
    public RoomAsset targetRoom;
    public int targetEntrance;

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(Exit), true)]
    public class Editor : UnityEditor.Editor
    {
        SerializedProperty roomProp;
        SerializedProperty spawnProp;
        ObjectField roomField;
        DynamicEnumField spawnField;

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new();

            roomProp = serializedObject.FindProperty(nameof(targetRoom));
            spawnProp = serializedObject.FindProperty(nameof(targetEntrance));

            // Room selection field (ObjectField so we can detect value changes easily)
            roomField = new("Room")
            {
                objectType = typeof(RoomAsset),
                allowSceneObjects = false,
                value = roomProp.objectReferenceValue as RoomAsset
            };

            List<string> GetNames() => roomField.value != null
                ? (roomField.value as RoomAsset).entrances.Select(e => e.name).ToList()
                : new();
            spawnField = new(GetNames(), spawnProp.intValue, Changed)
            {
                label = "Spawn"
            };

            void Changed(int v)
            {
                spawnProp.intValue = v;
                serializedObject.ApplyModifiedProperties();
            }



            // When room selection changes, update the serialized property and rebuild the spawn list
            roomField.RegisterValueChangedCallback(evt =>
            {
                var so = serializedObject;
                so.Update();

                RoomAsset target = evt.newValue as RoomAsset;
                roomProp.objectReferenceValue = target;

                spawnProp.intValue = 0;
                if (target) spawnField.SetOptions(GetNames(), 0);
                else spawnField.SetOptions(null, -1);



                so.ApplyModifiedProperties();
            });

            // Add the room field and the spawn container to the inspector UI
            root.Add(roomField);
            root.Add(spawnField);

            var it = serializedObject.GetIterator();
            it.NextVisible(true); //Into the Breach.
            it.NextVisible(false); //Pass over first two items.
            it.NextVisible(false);

            while (it.NextVisible(false)) root.Add(new PropertyField(it));

            return root;
        }
    }
#endif

}
