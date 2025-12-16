using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SLS.StateMachineH;
using System;
using Unity.Cinemachine;
using System.Linq;
using AYellowpaper.SerializedCollections;

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

        public SerializedDictionary<string, State> states = new SerializedDictionary<string, State>();

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

        private State prevState;
        public void CutsceneState()
        {
            prevState = CurrentState;
            pauseState.Enter();
            //body.velocity = Vector3.zero;
            //body.CurrentSpeed = 0;
            Player.Animator.CrossFade("GroundBasic", .2f);
        }
        public void UnCutsceneState()
        {
            prevState.Enter();
        }

        public State this[string stateName] => states[stateName];
    }

}