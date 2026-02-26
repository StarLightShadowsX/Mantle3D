using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Singleton
{
    public enum OperationMessage
    {
        Success,
        AlreadyRegistered,
        NullInstance,
        NotRegisteredInstance,
    }

    public static OperationMessage Register<T>(ref T slot, T newInstance) where T : class
    {
        if(slot != null && slot != newInstance)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"Singleton of type {typeof(T)} is already registered. Ignoring new instance.");
#endif
            return OperationMessage.AlreadyRegistered;
        }
        if(newInstance == null)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"Cannot register null instance for singleton of type {typeof(T)}.");
#endif
            return OperationMessage.NullInstance;
        }

        slot = newInstance;
        return OperationMessage.Success;
    }

    public static OperationMessage Unregister<T>(ref T slot, T instance) where T : class
    {
        if (slot == null)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"No singleton of type {typeof(T)} is registered to unregister.");
#endif
            return OperationMessage.NullInstance;
        }
        if (slot != instance)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"The provided instance does not match the registered singleton of type {typeof(T)}.");
#endif
            return OperationMessage.NotRegisteredInstance;
        }
        slot = null;
        return OperationMessage.Success;
    }

    public static T Get<T>(ref T slot, params Func<T>[] createAttempts) where T : class
    {
        if (slot == null)
        {
            for (int i = 0; i < createAttempts.Length; i++)
            {
                slot = createAttempts[i]();
                if (slot != null) break;
            }
        }
        return slot;
    }

    public static bool TryGet<T>(Func<T> getInstance, out T instance)
    {
        instance = getInstance();
        return instance != null;
    }
    public static bool TryGet<T>(T getterPlug, out T instance)
    {
        instance = getterPlug;
        return instance != null;
    }
}


public abstract class GlobalAsset<T> : GlobalAssetGeneric where T : class
{
    private static T _instance;
    public static T Get => Singleton.Get(ref _instance);
    public static bool TryGet(out T instance) => Singleton.TryGet(Get, out instance);

    public override void OnEnable() => Singleton.Register(ref _instance, this as T);
    private void OnDisable() => Singleton.Unregister(ref _instance, this as T);
}
public abstract class GlobalAssetGeneric : ScriptableObject
{
    public virtual void OnEnable() { }
}


public interface IGlobalPrefab
{
    public static Dictionary<Type, Prefab> typedPrefabs { get; } = new();
    public static Dictionary<string, Prefab> namedPrefabs { get; } = new();
    public static void RegisterPrefab(GameObject prefab)
    {
        Type type = prefab.GetComponent<IGlobalPrefab>().GetType();
        if (!typedPrefabs.ContainsKey(type)) typedPrefabs[type] = new(prefab);
    }
    public static void RegisterPrefab(GameObject prefab, string name)
    {
        if (!namedPrefabs.ContainsKey(name)) namedPrefabs[name] = new(prefab);
    }
    public static GameObject Instantiate<T>(Transform parent = null)
    {
        if (typedPrefabs.TryGetValue(typeof(T), out Prefab prefab)) return prefab.Instantiate(parent);
        Debug.LogWarning($"No prefab registered for type {typeof(T)}");
        return null;
    }
    public static GameObject Instantiate(string name, Transform parent = null)
    {
        if (namedPrefabs.TryGetValue(name, out Prefab prefab)) return prefab.Instantiate(parent);
        Debug.LogWarning($"No prefab registered for name {name}");
        return null;
    }
}