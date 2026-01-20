using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISingleton<T> where T : class, ISingleton<T>, new()
{
    public enum SingletonOperationMessage
    {
        Success,
        AlreadyRegistered,
        NullInstance,
        NotRegisteredInstance,
    }

    public static SingletonOperationMessage Register(ref T slot, T newInstance)
    {
        if(slot != null && slot != newInstance)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"Singleton of type {typeof(T)} is already registered. Ignoring new instance.");
#endif
            return SingletonOperationMessage.AlreadyRegistered;
        }
        if(newInstance == null)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"Cannot register null instance for singleton of type {typeof(T)}.");
#endif
            return SingletonOperationMessage.NullInstance;
        }

        slot = newInstance;
        return SingletonOperationMessage.Success;
    }

    public static SingletonOperationMessage Unregister(ref T slot, T instance)
    {
        if (slot == null)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"No singleton of type {typeof(T)} is registered to unregister.");
#endif
            return SingletonOperationMessage.NullInstance;
        }
        if (slot != instance)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"The provided instance does not match the registered singleton of type {typeof(T)}.");
#endif
            return SingletonOperationMessage.NotRegisteredInstance;
        }
        slot = null;
        return SingletonOperationMessage.Success;
    }

    public delegate T GetObjectDelegate();

    public static T Get(ref T slot, params GetObjectDelegate[] createAttempts)
    {
        if (slot == null)
        {
            for (int i = 0; i < createAttempts.Length; i++)
            {
                slot = createAttempts[i]() as T;
                if (slot != null) break;
            }
        }
        return slot;
    }

    public static bool TryGet(GetObjectDelegate getInstance, out T instance)
    {
        instance = getInstance();
        return instance != null;
    }
    public static bool TryGet(T getterPlug, out T instance)
    {
        instance = getterPlug;
        return instance != null;
    }
}


public abstract class GlobalAsset<T> : GlobalAssetGeneric, ISingleton<T> where T : GlobalAsset<T>, new()
{
    private static T _instance;
    public static T Get => ISingleton<T>.Get(ref _instance);
    public static bool TryGet(out T instance) => ISingleton<T>.TryGet(Get, out instance);

    public override void OnEnable() => ISingleton<T>.Register(ref _instance, this as T);
    private void OnDisable() => ISingleton<T>.Unregister(ref _instance, this as T);
}
public abstract class GlobalAssetGeneric : ScriptableObject
{
    public virtual void OnEnable() { }
}


public interface IGlobalPrefab
{
    public static Dictionary<Type, Prefab> prefabs { get; } = new();
    public static void RegisterPrefab(GameObject prefab)
    {
        Type type = prefab.GetComponent<IGlobalPrefab>().GetType();
        if (!prefabs.ContainsKey(type)) prefabs[type] = new(prefab);
    }
    public static GameObject Instantiate<T>(Transform parent = null)
    {
        if (prefabs.TryGetValue(typeof(T), out Prefab prefab)) return prefab.Instantiate(parent);
        Debug.LogWarning($"No prefab registered for type {typeof(T)}");
        return null;
    }
}