using UnityEngine;
using VRSF.Core.Inputs;
using VRSF.Core.SetupVR;
using VRSF.Core.Utils.ButtonActionChoser;
using VRSF.Core.Raycast;

namespace VRSF.MoveAround.Fly
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
        protected override void OnStartRunning()
        {
            OnSetupVRReady.Listeners += OnSetupVRIsReady;
            base.OnStartRunning();
        }


        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();

            foreach (var e in GetEntities<Filter>())
            {
                RemoveListeners(e);
            }
            
            OnSetupVRReady.Listeners -= OnSetupVRIsReady;
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

        public override void RemoveListeners(object entity)
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
            entity.FlyComponent._IsInteracting = !entity.RaycastComp.RaycastHitVar.RaycastHitIsOnUI();
        }
        #endregion PUBLIC_METHODS


        #region PRIVATE_METHODS
        /// <summary>
        /// Callback for when SetupVR is setup. Setup the lsiteners.
        /// </summary>
        /// <param name="onSetupVR"></param>
        private void OnSetupVRIsReady(OnSetupVRReady onSetupVR)
        {
            foreach (var e in GetEntities<Filter>())
            {
                if (e.BACGeneral.ActionButton != EControllersButton.THUMBSTICK)
                {
                    Debug.LogError("<b>[VRSF] :</b> You need to assign Left Thumbstick or Right Thumbstick to use the Fly script. Setting CanBeUsed at false.");
                    e.BACCalculations.CanBeUsed = false;
                    return;
                }

                SetupListenersResponses(e);
            }
        }
        #endregion PRIVATE_METHODS
    }
}