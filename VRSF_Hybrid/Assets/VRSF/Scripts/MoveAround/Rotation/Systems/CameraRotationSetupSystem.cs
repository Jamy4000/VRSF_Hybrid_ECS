﻿using UnityEngine;
using UnityEngine.SceneManagement;
using VRSF.Core.Inputs;
using VRSF.MoveAround.Components;
using VRSF.Utils.ButtonActionChoser;

namespace VRSF.MoveAround.Systems
{
    public class CameraRotationSetupSystem : BACListenersSetupSystem
    {
        struct Filter
        {
            public CameraRotationComponent RotationComp;
            public BACGeneralComponent ButtonComponents;
        }


        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            SceneManager.sceneLoaded += OnSceneLoaded;

            foreach (var e in GetEntities<Filter>())
            {
                SetupListenersResponses(e);
            }
        }
        
        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();

            SceneManager.sceneLoaded -= OnSceneLoaded;

            foreach (var e in GetEntities<Filter>())
            {
                RemoveListeners(e);
            }
        }
        #endregion


        #region PUBLIC_METHODS

        #region Setup_Listeners_Responses
        public override void SetupListenersResponses(object entity)
        {
            var e = (Filter)entity;

            if ((e.ButtonComponents.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                e.ButtonComponents.OnButtonIsClicking.AddListener(delegate { StartRotating(e.RotationComp); });
                e.ButtonComponents.OnButtonStopClicking.AddListener(delegate { StopRotating(e.RotationComp); });
            }

            if ((e.ButtonComponents.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                e.ButtonComponents.OnButtonIsTouching.AddListener(delegate { StartRotating(e.RotationComp); });
                e.ButtonComponents.OnButtonStopTouching.AddListener(delegate { StopRotating(e.RotationComp); });
            }
        }

        public override void RemoveListeners(object entity)
        {
            var e = (Filter)entity;

            if ((e.ButtonComponents.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                e.ButtonComponents.OnButtonIsClicking.RemoveAllListeners();
                e.ButtonComponents.OnButtonStopClicking.RemoveAllListeners();
            }

            if ((e.ButtonComponents.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                e.ButtonComponents.OnButtonIsTouching.RemoveAllListeners();
                e.ButtonComponents.OnButtonStopTouching.RemoveAllListeners();
            }
        }
        #endregion Setup_Listeners_Responses

        #endregion PUBLIC_METHODS


        #region PRIVATE_METHODS
        /// <summary>
        /// Set the _rotating bool to true. Need to be called from IsTouching, IsClicking, StartTouching or StartClicking.
        /// </summary>
        private void StartRotating(CameraRotationComponent comp)
        {
            comp.IsRotating = true;
        }

        /// <summary>
        /// Set the _rotating bool to false. Need to be called from StopTouching or StopClicking.
        /// </summary>
        private void StopRotating(CameraRotationComponent comp)
        {
            comp.IsRotating = false;
            comp.HasRotated = false;
        }


        /// <summary>
        /// Reactivate the System when switching to another Scene.
        /// </summary>
        /// <param name="scene">The previous scene before switching</param>
        private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
        {
            if (loadMode == LoadSceneMode.Single)
            {
                foreach (var e in GetEntities<Filter>())
                {
                    SetupListenersResponses(e);
                }
            }
            
        }
        #endregion
    }
}