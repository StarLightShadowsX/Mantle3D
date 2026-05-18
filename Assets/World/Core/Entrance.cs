using UnityEngine;
using UnityEngine.UIElements;
using Utilities.Xtensions;


#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class Entrance : MonoBehaviour, IRoomActor
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

    [System.Serializable]
    public struct Data
    {
        [field: SerializeField] public string name { get; private set; }
        [field: SerializeField] public Type type { get; private set; }

        public Data(Entrance input)
        {
            name = input.name;
            type = input.type;
        }

#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(Data))]
        private class Editor : PropertyDrawer
        {
            public override VisualElement CreatePropertyGUI(SerializedProperty property)
            {
                string nameString = property.FindPropertyRelative(nameof(name).BackingField()).stringValue;
                var enumProp = property.FindPropertyRelative(nameof(type).BackingField());
                string typeString = enumProp.enumDisplayNames[enumProp.enumValueIndex];
                return new Label($"{nameString} ({typeString})");
            }
        }
#endif
    }

    public abstract Type type { get; }
    [SerializeField] private RoomRoot root; public RoomRoot Root => root;

    public string Name => gameObject.name;
    public int ID => root.entrances.IndexOf(this);

    private void Reset()
    {
        if (Application.isEditor)
        {
            Register();
            return;
        }
    }
    [ExecuteInEditMode]
    private void Awake()
    {
        if (Application.isEditor)
        {
            Register();
            return;
        }
    }
    public virtual void Register()
    {
        if (IRoomActor.RegisterWithRoot(this, out root) || !Application.isEditor || Application.isPlaying || root.entrances.Contains(this)) return;
        root.entrances.Add(this);
    }

    [ExecuteInEditMode]
    private void OnDestroy()
    {
        if (Application.isEditor)
        {
            Deregister();
            return;
        }
    }
    public virtual void Deregister()
    {
        if (!Application.isEditor || Application.isPlaying || !root.entrances.Contains(this)) return;
        IRoomActor.DeregisterWithRoot(this, ref root);
        root.entrances.Remove(this);
    }

    public abstract void PlacePlayer();
}

