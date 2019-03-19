using UnityEngine;
using VRSF.Core.Inputs;
using VRSF.Core.SetupVR;
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
        protected override void OnCreateManager()
        {
            base.OnCreateManager();
            OnSetupVRReady.Listeners += OnSetupVRIsReady;
        }
        
        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();

            OnSetupVRReady.Listeners -= OnSetupVRIsReady;

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
                e.ButtonComponents.OnButtonStartClicking.AddListener(delegate { StartRotating(e.RotationComp); });
                e.ButtonComponents.OnButtonStopClicking.AddListener(delegate { StopRotating(e.RotationComp); });
            }

            if ((e.ButtonComponents.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                e.ButtonComponents.OnButtonStartTouching.AddListener(delegate { StartRotating(e.RotationComp); });
                e.ButtonComponents.OnButtonStopTouching.AddListener(delegate { StopRotating(e.RotationComp); });
            }
        }

        public override void RemoveListeners(object entity)
        {
            var e = (Filter)entity;

            if ((e.ButtonComponents.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                e.ButtonComponents.OnButtonStartClicking.RemoveAllListeners();
                e.ButtonComponents.OnButtonStopClicking.RemoveAllListeners();
            }

            if ((e.ButtonComponents.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                e.ButtonComponents.OnButtonStartTouching.RemoveAllListeners();
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
        }


        /// <summary>
        /// Callback for when SetupVR is setup. Setup the lsiteners.
        /// </summary>
        /// <param name="onSetupVR"></param>
        private void OnSetupVRIsReady(OnSetupVRReady onSetupVR)
        {
            foreach (var e in GetEntities<Filter>())
            {
                if (e.ButtonComponents.ActionButton != EControllersButton.THUMBSTICK)
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