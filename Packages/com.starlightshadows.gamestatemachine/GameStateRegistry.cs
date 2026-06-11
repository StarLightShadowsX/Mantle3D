using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SLS.Singletons;
using UnityEngine;

namespace SLS.GameStateMachine
{
    [DefaultExecutionOrder(-160)]
    public class GameStateRegistry : GlobalAsset<GameStateRegistry>
    {
        public List<GameStateBase> AllStates = new();
        public static Dictionary<string, GameStateBase> Dict;

        public static Action Setup;

        public override void OnInit()
        {
            Dict = AllStates.ToDictionary(s => s.name);
            for (int i = 0; i < AllStates.Count; i++) AllStates[i].OnEnable();

            Setup?.Invoke();

            if(Application.isPlaying) GameStateBase.Transition(AllStates[0]);
        }

#if UNITY_EDITOR

        public class PostProcessor : UnityEditor.AssetPostprocessor
        {
            private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
            {
                if (!TryGet(out GameStateRegistry registry))
                    registry = GetOrCreate(typeof(GameStateRegistry)) as GameStateRegistry;

                registry.OnEnable();

                Type GlobalAssetType = typeof(GameStateSingle<>);
                var globalAssetTypes =
                    AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(assembly => assembly.GetTypes())
                    .Where(i => ImplementsOrDerives(i, GlobalAssetType) && !i.IsAbstract && i != GlobalAssetType)
                    .ToArray();

                foreach (Type type in globalAssetTypes)
                {
                    // If no existing in-memory instance is found, ensure an asset exists on disk
                    if (_GameStateSingleBase.TryGetAlreadyActive(type, out _GameStateSingleBase currentInstance))
                    {
                        if (!registry.AllStates.Contains(currentInstance)) registry.AllStates.Add(currentInstance);
                        currentInstance.OnEnable();
                    }
                    else
                    {
                        _GameStateSingleBase created = _GameStateSingleBase.GetOrCreate(type);
                        if (!registry.AllStates.Contains(created)) registry.AllStates.Add(created);
                        created.OnEnable();
                    }
                }

                //last minute run through of registry's assets to get rid of Null values.
                for (int i = registry.AllStates.Count - 1; i >= 0; i--)
                    if (registry.AllStates[i] == null) registry.AllStates.RemoveAt(i);

                UnityEditor.AssetDatabase.SaveAssets();
                UnityEditor.AssetDatabase.Refresh();
            }

            static bool ImplementsOrDerives(Type @this, Type from)
            {
                if (from is null)
                    return false;

                if (!from.IsGenericType || !from.IsGenericTypeDefinition)
                    return from.IsAssignableFrom(@this);

                if (from.IsInterface)
                    foreach (Type @interface in @this.GetInterfaces())
                        if (@interface.IsGenericType && @interface.GetGenericTypeDefinition() == from)
                            return true;

                if (@this.IsGenericType && @this.GetGenericTypeDefinition() == from)
                    return true;

                return @this.BaseType != null && ImplementsOrDerives(@this.BaseType, from);
            }

        }
#endif
    }
}
