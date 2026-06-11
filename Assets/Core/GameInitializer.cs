using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using SaveSystem;
using Utilities.ObjectPooling;
using SLS.Singletons;
using SLS.GameStateMachine;






/// <summary>
/// A Global System managing the core gameplay systems and lifecycle. A singleton that persists as long as gameplay is running. <br/>
/// Provides static access to important gameplay-related properties and methods. <br/>
/// To begin gameplay, use methods such as <see cref="BeginFromSaveFile(int)"/> or <see cref="BeginRoom()"/>.
/// </summary>
[DefaultExecutionOrder(ExecutionOrders.GameplayRoot)]
public class GameInitializer : MonoBehaviour
{
    #region Instance Fields

    [SerializeField] Transform cameraTransform;
    [SerializeField] HUD hud;
    [SerializeField] GameObject overlayPrefab;
    [SerializeField] PlayerCore.Player inputPlayer;
    [SerializeField] PauseMenu pauseMenu;

    #endregion Instance Fields


    private void Awake()
    {

        
    }
}
