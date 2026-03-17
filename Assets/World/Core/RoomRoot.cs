using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomRoot : MonoBehaviour
{
    [field: SerializeField] public RoomAsset asset { get; private set; }
    [field: SerializeField] public List<GameObject> roomObjects { get; private set; }

    [field: SerializeField] public List<Entrance> entrances { get; private set; }

    private void Awake() => asset.Connect(this);



#if UNITY_EDITOR
    public class Editor
    {
        [InitializeOnLoad]
        public static class RoomRootSceneHook
        {
            static RoomRootSceneHook() => UnityEditor.SceneManagement.EditorSceneManager.sceneSaving += OnSceneSaving;

            private static void OnSceneSaving(Scene scene, string path)
            {
                var objectArray = scene.GetRootGameObjects();
                if (!objectArray[0].TryGetComponent(out RoomRoot roomRoot)) return;
                //roomRoot.roomObjects = objectArray;

                
            }
        }




    }
#endif
}

public interface IRoomActor
{
    RoomRoot Root { get; }
}