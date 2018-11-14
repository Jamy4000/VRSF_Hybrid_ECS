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
    public class FlySetupSystem : BACListenersSetupSystem
    {
        public struct Filter
        {
            public FlyParametersComponent FlyComponent;
            public BACGeneralComponent BACGeneral;
            public BACCalculationsComponent BACCalculations;
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
                if (e.BACGeneral.ActionButton != EControllersButton.THUMBSTICK)
                {
                    Debug.LogError("VRSF : You need to assign Left Thumbstick or Right Thumbstick to use the Fly script. Setting CanBeUsed at false.");
                    e.BACCalculations.CanBeUsed = false;
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

            if ((e.BACGeneral.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                e.BACGeneral.OnButtonIsClicking.AddListener(delegate { ButtonIsInteracting(e); });
                e.BACGeneral.OnButtonStopClicking.AddListener(delegate { ButtonStopInteracting(e); });
            }

            if ((e.BACGeneral.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                e.BACGeneral.OnButtonIsTouching.AddListener(delegate { ButtonIsInteracting(e); });
                e.BACGeneral.OnButtonStopTouching.AddListener(delegate { ButtonStopInteracting(e); });
            }
        }

        public override void RemoveListenersOnEndApp(object entity)
        {
            var e = (Filter)entity;
            if ((e.BACGeneral.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                e.BACGeneral.OnButtonIsClicking.RemoveAllListeners();
                e.BACGeneral.OnButtonStopClicking.RemoveAllListeners();
            }

            if ((e.BACGeneral.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                e.BACGeneral.OnButtonIsTouching.RemoveAllListeners();
                e.BACGeneral.OnButtonStopTouching.RemoveAllListeners();
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
            if (!entity.RaycastComp.RaycastHitVar.IsNull && 
                entity.RaycastComp.RaycastHitVar.Value.collider.gameObject.layer == LayerMask.NameToLayer("UI"))
                return;

            entity.FlyComponent._IsInteracting = true;
        }
        #endregion PUBLIC_METHODS


        #region PRIVATE_METHODS
        private IEnumerator SetupListernersCoroutine(Filter entity)
        {
            while (!VRSF_Components.SetupVRIsReady && entity.BACCalculations.ActionButtonIsReady && entity.BACCalculations.IsSetup)
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
            foreach (var e in GetEntities<Filter>())
            {
                if (e.BACGeneral.ActionButton != EControllersButton.THUMBSTICK)
                {
                    Debug.LogError("VRSF : You need to assign Left Thumbstick or Right Thumbstick to use the Fly script. Setting CanBeUsed at false.");
                    e.BACCalculations.CanBeUsed = false;
                }

                e.FlyComponent.StartCoroutine(SetupListernersCoroutine(e));
            }
        }
        #endregion PRIVATE_METHODS
    }
}