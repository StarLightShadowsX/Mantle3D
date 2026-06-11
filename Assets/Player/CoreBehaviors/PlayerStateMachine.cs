using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SLS.StateMachineH;
using System;
using Unity.Cinemachine;
using System.Linq;
using SLS.ListUtilities;

namespace PlayerCore
{

    [DefaultExecutionOrder(ExecutionOrders.PlayerSystems)]
    public class PlayerStateMachine : StateMachine
    {
        #region Config

        #endregion

        #region Data
        public State pauseState;
        public State ragDollState;

        public DictionaryS<string, State> states = new();

        #endregion




        public void HaveDestroyed() { }

        protected override void PreSetup()
        {

        }

        protected override void OnAwake()
        {
            whenInitializedEvent?.Invoke(this);
        }

        private void OnDestroy()
        {
        }


        public static Action<PlayerStateMachine> whenInitializedEvent;

        public bool IsStableForOriginShift() => states["Grounded"].enabled || CurrentState == states["Fall"] || states["Glide"];

        public void ResetState()
        {
            Children[0].Enter();
            //signalReady = true;
            //Player.RagdollHandler.State = RagdollHandler.States.Off;
            Player.Animator.enabled = true;
            Player.Animator.Play("GroundBasic");
        }

        public State this[string stateName] => states[stateName];
    }

}