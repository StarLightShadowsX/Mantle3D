using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlayerCore;
using SLS.GameStateMachine;
using SLS.Singletons;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities.ObjectPooling;

namespace Core
{
    public class Gameplay : GameStateSingle<Gameplay>
    {
        public override bool Additive => false;
        public static GameObject[] rootObjects;
        public static bool Active => Get.isActive;

        protected override void TransitionLogic(Action SetCurrent, Action PostAction)
        {
            E().Begin();
            IEnumerator E()
            {
                SetCurrent();
                SceneManager.LoadScene(Scene, LoadSceneMode.Single);
                var s = SceneManager.GetSceneByName(Scene);
                rootObjects = s.GetRootGameObjects();
                yield return null;
                Player.ActivityState = Player.ActivityStates.Paused;

                //FUN.RollSession();
                //GlobalPool.poolParent = transform.Find("PooledObjects");
                //IGlobalPrefab.RegisterPrefab(pauseMenu.gameObject);
                //Overlay.OverMenus.BasicBlackout = 1;
                //Overlay.OverGameplay.Reset();
                //Overlay.OverHUD.Reset();

                for (int i = 0; i < rootObjects.Length; i++)
                {
                    DontDestroyOnLoad(rootObjects[i]);
                    if (rootObjects[i].TryGetComponent(out PlayerCore.Player player)) player.Awake();
                    if (rootObjects[i].TryGetComponent(out Cameras cameras)) cameras.Awake();
                    //if (rootObjects[i].TryGetComponent(out HUD hud)) hud.Awake();
                    //if (rootObjects[i].TryGetComponent(out PauseMenu pauseMenu)) pauseMenu.Awake();
                }

                PostAction();

                loadTargetRoom ??= RoomRegistry.Get.allRooms[0];

                yield return loadTargetRoom.LoadRoutine();
                yield return null;
                RoomManager.CurrentRoom = loadTargetRoom;
                Player.ActivityState = Player.ActivityStates.Active;
                RoomManager.CurrentRoom.root.entrances[0].PlacePlayer();
                Player.MovementBody.Ground.Land();
                Overlay.UnderHUD.Alpha = 1f;
                yield return null;
                Overlay.UnderHUD.DoFadeAlpha(0);
            }
        }


        public static void BeginRoom(RoomRoot room, int entranceID = 0)
        {
            loadTargetRoom = room.asset;
            loadTargetEntranceID = entranceID;
            Get.Enter();
        }

        private static RoomAsset loadTargetRoom;
        private static int loadTargetEntranceID;
    }
}
