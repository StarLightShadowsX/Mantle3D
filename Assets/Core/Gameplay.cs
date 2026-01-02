using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using SaveSystem;
using ObjectPooling;







#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// A Global System managing the core gameplay systems and lifecycle. A singleton that persists as long as gameplay is running. <br/>
/// Provides static access to important gameplay-related properties and methods. <br/>
/// To begin gameplay, use methods such as <see cref="BeginSaveFile(int)"/> or <see cref="BeginEditor()"/>.
/// </summary>
[DefaultExecutionOrder(ExecutionOrders.Gameplay)]
public class Gameplay : MonoBehaviour
{
    public enum GameStates
    {
        Null = -1,
        Active = 0,
        Paused = 1,
        Processing = 2,
    }
    private static GameStates _gameState = GameStates.Null;
    public static GameStates GameState
    {
        get => _gameState;
        set
        {
            if (_gameState == value
                || _gameState is GameStates.Null
                || value is GameStates.Null
                ) return;

            _gameState = value;

            Time.timeScale = value is GameStates.Paused ? 0 : 1;

        }
    }

    /// <summary>
    /// Whether Gameplay is currently active.
    /// <br/> Reads <see cref="GameState"/>, true if not <see cref="GameStates.Null"/>.
    /// </summary>
    public static bool Active => GameState is not GameStates.Null;




    /// <summary>
    /// The Script instance of the Gameplay system. Not truly relevant to much. Null if not active.
    /// <br/> Can be used as the source script for a Coroutine to ensure it runs.
    /// </summary>
    public static Gameplay Instance { get; private set; }
    /// <summary>
    /// The <see cref="UnityEngine.GameObject"/> that this script is attached to. Null if not active."/>
    /// </summary>
    public static GameObject GameObject { get; private set; }

    /// <summary>
    /// A reference to the Scene for this system.
    /// </summary>
    public static SceneReference GAMEPLAY_SCENE = new("GameplayScene");

    /// <summary>
    /// Callback event for when a Save is about to be reloaded.
    /// </summary>
    public static System.Action PreReloadSave;
    /// <summary>
    /// A Callback event for when the Gameplay system updates, invoked in <see cref="Update"/>.
    /// </summary>
    public static System.Action onUpdate;
    /// <summary>
    /// A Callbck event for when the Gameplay system is Unloaded.
    /// </summary>
    public static System.Action onDestroy;

    /// <summary>
    /// The last written time (in seconds) since the game been started that the player interacted with a save point. <br/>
    /// See <see cref="UpdateGameTime"/>
    /// </summary>
    public static double lastSaveInteractionTime;

    #region Instance Fields

    [SerializeField] Transform cameraTransform;
    [SerializeField] HUD hud;
    [SerializeField] DontDestroyMeOnLoad overlayPrefab;
    [SerializeField] Player inputPlayer;
    [SerializeField] PauseMenu pauseMenu;

    #endregion Instance Fields

    private void Awake()
    {
        if(Active)
        {
            if(Instance != this) Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        Instance = this;
        _gameState = GameStates.Active;
        GameObject = gameObject;
        if (Overlay.ActiveOverlays.Count == 0) Instantiate(overlayPrefab);
        DontDestroyOnLoad(gameObject);
        inputPlayer.Awake();
        //inputUI.Awake();
        GetComponent<Cameras>().Awake();
        ObjectPools.poolParent = transform.Find("PooledObjects");
        IGlobalPrefab.RegisterPrefab(pauseMenu.gameObject);
        Overlay.OverMenus.BasicBlackout = 1;
        Overlay.OverGameplay.Reset();
        Overlay.OverHUD.Reset();
        FUN.RollSession();

        Enum().Begin(this);
        static IEnumerator Enum() 
        {
            yield return null;
            yield return WaitFor.Until(Initialized);

            static bool Initialized() => Active
                && Player.Active
                //&& RoomManager.Active
                ;

            //RoomManager.ResetTransitionData(false);

            //RoomManager.TransitionStyle = new()
            //{
            //    forceFullTransition = true,
            //    FadeOutRoutine = null,
            //    FadeInRoutine = Overlay.OverMenus.BasicFadeInWait(0.5f),
            //    PreFadeInAction = () =>
            //    {
            //        UpdateGameTime();
            //        Input.Pause.performed += c => { Menu.Manager.Escape(); };
            //    },
            //};
            //yield return RoomManager.Transition();
        }
    }

    private void Update()
    {
        onUpdate?.Invoke();
    }


    /// <summary>
    /// Begins The Gameplay Phase using the specified Save File on Disk.
    /// </summary>
    /// <param name="fileNo"></param>
    public static void BeginSaveFile(int fileNo)
    {
        if (Active) return;

        Enum().Begin(Overlay.OverMenus);
        IEnumerator Enum()
        {

            yield return Overlay.OverMenus.BasicFadeOutWait();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            InitializeSaves(fileNo);
            //RoomManager.destination = SaveData.Current.location;

            Menu.CloseAllMenus();
            var Load = SceneManager.LoadSceneAsync(GAMEPLAY_SCENE);

            yield return WaitFor.Until(() => Load.isDone && Active);
            yield return WaitFor.SecondsRealtime(0.2f);
        }
    }
    /// <summary>
    /// Begins the Gameplay Phase in Editor Mode, using the settings in <see cref="EditorState"/> to determine spawn location. <br/>
    /// </summary>
    public static void BeginEditor()
    {
        if (Active) return;

        InitializeSaves(0);

        SceneManager.LoadScene(GAMEPLAY_SCENE);
    }

    public static void InitializeSaves(int fileNo)
    {
        SaveData.IO = new(fileNo);
        SaveData.IO.Load();
        SaveData.RevertToSaveFile();
    }




    public static void Respawn()
    {
        //RoomManager.PostFadeOutAction = () => { Player.onRespawn?.Invoke(); };
        //RoomManager.StartTransition(Destination.Current);
    }

    public static void Death()
    {
        SaveData.RevertToDeathData();
        //RoomManager.StartTransition(Destination.Current);
    }

    public static void ReloadSave()
    {
        SaveData.RevertToSaveFile();
        //RoomManager.StartTransition(Destination.Current);
    }

    /// <summary>
    /// Updates the <see cref="lastSaveInteractionTime"/> to the current time, returning the time (in seconds) since the last update. <br/>
    /// </summary>
    /// <returns></returns>
    public static double UpdateGameTime()
    {
        var previousSaveInteractionTime = lastSaveInteractionTime;
        lastSaveInteractionTime = Time.timeAsDouble;
        return Time.timeAsDouble - previousSaveInteractionTime;
    }





    //protected override void OnDeInitialize() => EnemyCullingGroup.DeInitialize();





    public static void DESTROY(bool areYouSure = false)
    {
        if (!areYouSure)
        {
            #if UNITY_EDITOR
            Debug.Log("Someone is trying to Destroy the gameplay without realizing the gravity of that situation.");
            #endif
            return;
        }
        Destroy(GameObject);
        _gameState = GameStates.Null;

    }

    private void OnDestroy()
    {
        onDestroy?.Invoke();
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(Gameplay))]
    public class Editor : UnityEditor.Editor
    {
    }
#endif
}
