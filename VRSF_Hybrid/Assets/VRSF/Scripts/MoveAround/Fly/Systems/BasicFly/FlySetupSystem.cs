using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRSF.Inputs;
using VRSF.MoveAround.Components;
using VRSF.Utils;
using VRSF.Utils.Components;
using VRSF.Utils.Components.ButtonActionChoser;
using VRSF.Utils.Systems.ButtonActionChoser;

namespace VRSF.MoveAround.Systems
{
    /// <summary>
    /// System Allowing the user to fly with the thumbstick / touchpad. 
    /// </summary>
    public class FlySetupSystem : BACUpdateSystem
    {
        public struct Filter
        {
            public FlyParametersComponent FlyComponent;
            public ButtonActionChoserComponents ButtonComponents;
            public ScriptableRaycastComponent RaycastComp;
        }


        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            SceneManager.sceneLoaded += OnSceneUnloaded;
            
            foreach (var e in GetEntities<Filter>())
            {
                if (e.ButtonComponents.ActionButton != EControllersButton.THUMBSTICK)
                {
                    Debug.LogError("VRSF : You need to assign Left Thumbstick or Right Thumbstick to use the Fly script. Setting CanBeUsed at false.");
                    e.ButtonComponents.CanBeUsed = false;
                }
                    
                e.FlyComponent.StartCoroutine(SetupListernersCoroutine(e));
            }
        }


        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();

            foreach (var e in GetEntities<Filter>())
            {
                RemoveListenersOnEndApp(e);
            }

            SceneManager.sceneLoaded -= OnSceneUnloaded;
        }
        #endregion ComponentSystem_Methods


        #region PUBLIC_METHODS
        public override void SetupListenersResponses(object entity)
        {
            var e = (Filter)entity;

            if ((e.ButtonComponents.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                e.ButtonComponents.OnButtonIsClicking.AddListener(delegate { ButtonIsInteracting(e); });
                e.ButtonComponents.OnButtonStopClicking.AddListener(delegate { ButtonStopInteracting(e); });
            }

            if ((e.ButtonComponents.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                e.ButtonComponents.OnButtonIsTouching.AddListener(delegate { ButtonIsInteracting(e); });
                e.ButtonComponents.OnButtonStopTouching.AddListener(delegate { ButtonStopInteracting(e); });
            }
        }

        public override void RemoveListenersOnEndApp(object entity)
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


        /// <summary>
        /// Called from OnButtonStopClicking or OnButtonStopTouching event
        /// </summary>
        public void ButtonStopInteracting(Filter entity)
        {
            entity.FlyComponent._SlowDownTimer = entity.FlyComponent._TimeSinceStartFlying;
            entity.FlyComponent._IsSlowingDown = true;
            entity.FlyComponent._WantToFly = false;
            entity.FlyComponent._IsInteracting = false;
        }

        /// <summary>
        /// Called from OnButtonIsTouching or OnButtonIsClickingevent
        /// </summary>
        public void ButtonIsInteracting(Filter entity)
        {
            // If the user is aiming to the UI, we don't activate the system
            if (!entity.RaycastComp.RaycastHitVar.isNull && entity.RaycastComp.RaycastHitVar.Value.collider.gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                return;
            }
            else
            {
                entity.FlyComponent._IsInteracting = true;
            }
        }
        #endregion PUBLIC_METHODS


        #region PRIVATE_METHODS
        private IEnumerator SetupListernersCoroutine(Filter entity)
        {
            while (!VRSF_Components.SetupVRIsReady && entity.ButtonComponents.ActionButtonIsReady && entity.ButtonComponents.IsSetup)
            {
                yield return new WaitForEndOfFrame();
            }

            SetupListenersResponses(entity);
        }

        /// <summary>
        /// Reactivate the System when switching to another Scene.
        /// </summary>
        /// <param name="oldScene">The previous scene before switching</param>
        private void OnSceneUnloaded(Scene newScene, LoadSceneMode sceneMode)
        {
            OnStartRunning();
        }
        #endregion PRIVATE_METHODS
    }
}