using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SLS.GameStateMachine;
using SLS.Singletons;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities.ObjectPooling;
using static UnityEngine.RuleTile.TilingRuleOutput;
using SceneState = SLS.AssetUtilties.SceneAsset.SceneState;

namespace Core
{
    public class Gameplay : GameStateSingle<Gameplay>
    {
        public static GameObject[] rootObjects;

        protected override void TransitionLogic(Action SetCurrent, Action PostAction)
        {
            Enum().Begin();
            IEnumerator Enum()
            {
                SetCurrent();
                Scene.Load();
                yield return new WaitUntil(() => Scene.CurrentState is SceneState.Loaded);
                rootObjects = Scene.LoadedStruct.GetRootGameObjects();

                //FUN.RollSession();
                //GlobalPool.poolParent = transform.Find("PooledObjects");
                //IGlobalPrefab.RegisterPrefab(pauseMenu.gameObject);
                //Overlay.OverMenus.BasicBlackout = 1;
                //Overlay.OverGameplay.Reset();
                //Overlay.OverHUD.Reset();

                for (int i = 0; i < rootObjects.Length; i++)
                {
                    DontDestroyOnLoad(rootObjects[i]);
                    if (rootObjects[i].TryGetComponent(out Player player)) player.Awake();
                    if (rootObjects[i].TryGetComponent(out Cameras cameras)) cameras.Awake();
                    //if (rootObjects[i].TryGetComponent(out HUD hud)) hud.Awake();
                    //if (rootObjects[i].TryGetComponent(out PauseMenu pauseMenu)) pauseMenu.Awake();
                }

                PostAction();
            }
        }

    }
}
