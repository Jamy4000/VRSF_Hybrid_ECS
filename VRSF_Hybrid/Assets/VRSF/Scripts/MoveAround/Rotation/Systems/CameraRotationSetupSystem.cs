using UnityEngine;
using VRSF.Core.Inputs;
using VRSF.Core.SetupVR;
using VRSF.Core.Utils;
using VRSF.Core.Utils.ButtonActionChoser;

namespace VRSF.MoveAround.Rotate
{
    /// <summary>
    /// Setup the camera rotation systems by adding the required listeners
    /// </summary>
    public class CameraRotationSetupSystem : BACListenersSetupSystem
    {
        struct Filter
        {
            public CameraRotationComponent RotationComp;
            public BACGeneralComponent ButtonComponents;
            public BACCalculationsComponent BACCalculations;
        }


        #region ComponentSystem_Methods
        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            Init();
        }

        protected override void OnStopRunning()
        {
            base.OnStopRunning();
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
            if (e.RotationComp.StartInteractingAction == null && e.RotationComp.StopInteractingAction == null)
            {
                e.RotationComp.StartInteractingAction = delegate { StartRotating(e.RotationComp); };
                e.RotationComp.StopInteractingAction = delegate { StopRotating(e.RotationComp); };

                if ((e.ButtonComponents.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
                {
                    e.ButtonComponents.OnButtonStartClicking.AddListenerExtend(e.RotationComp.StartInteractingAction);
                    e.ButtonComponents.OnButtonStopClicking.AddListenerExtend(e.RotationComp.StopInteractingAction);
                }

                if ((e.ButtonComponents.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
                {
                    e.ButtonComponents.OnButtonStartTouching.AddListenerExtend(e.RotationComp.StartInteractingAction);
                    e.ButtonComponents.OnButtonStopTouching.AddListenerExtend(e.RotationComp.StopInteractingAction);
                }
            }
        }

        public override void RemoveListeners(object entity)
        {
            var e = (Filter)entity;

            if ((e.ButtonComponents.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                e.ButtonComponents.OnButtonStartClicking.RemoveListenerExtend(e.RotationComp.StartInteractingAction);
                e.ButtonComponents.OnButtonStopClicking.RemoveListenerExtend(e.RotationComp.StopInteractingAction);
            }

            if ((e.ButtonComponents.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                e.ButtonComponents.OnButtonStartTouching.RemoveListenerExtend(e.RotationComp.StartInteractingAction);
                e.ButtonComponents.OnButtonStopTouching.RemoveListenerExtend(e.RotationComp.StopInteractingAction);
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
        }


        /// <summary>
        /// Setup the lsiteners.
        /// </summary>
        private void Init()
        {
            foreach (var e in GetEntities<Filter>())
            {
                if (e.ButtonComponents.ActionButton != EControllersButton.TOUCHPAD)
                {
                    Debug.LogError("<b>[VRSF] :</b> You need to assign Left Thumbstick or Right Thumbstick to use the Rotation script. Setting CanBeUsed at false.");
                    e.BACCalculations.CanBeUsed = false;
                    return;
                }

                SetupListenersResponses(e);
            }
        }
        #endregion
    }
}