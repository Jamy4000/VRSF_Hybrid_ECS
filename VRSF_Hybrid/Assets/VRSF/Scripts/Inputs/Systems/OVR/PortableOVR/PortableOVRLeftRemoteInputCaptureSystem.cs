using ScriptableFramework.Events;
using ScriptableFramework.Variables;
using Unity.Entities;
using UnityEngine;
using VRSF.Controllers;
using VRSF.Inputs.Components;

namespace VRSF.Inputs.Systems
{
    /// <summary>
    /// Script attached to the Portable OVR Prefab.
    /// Set the GameEvent depending on the Oculus Inputs
    /// </summary>
    public class PortableOVRLeftRemoteInputCaptureSystem : ComponentSystem
    {
        struct Filter
        {
            public PortableOVRRemoteInputCaptureComponent VRInputCapture;
        }

        #region PRIVATE_VARIABLES
        // VRSF Parameters references
        private InputVariableContainer _inputContainer;
        private ControllersParametersVariable _controllersParameters;
        #endregion PRIVATE_VARIABLES


        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            if (OVRInput.IsControllerConnected(OVRInput.Controller.RTrackedRemote))
            {
                this.Enabled = false;
                return;
            }

            _inputContainer = InputVariableContainer.Instance;
            _controllersParameters = ControllersParametersVariable.Instance;
        }

        protected override void OnUpdate()
        {
            // If we doesn't use the controllers, we don't check for the inputs.
            if (_controllersParameters.UseControllers)
            {
                foreach (var entity in GetEntities<Filter>())
                {
                    CheckRemoteInput(entity.VRInputCapture);
                }
            }
        }
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Handle the Remote inputs when user is left handed and put them in the Events
        /// </summary>
        void CheckRemoteInput(PortableOVRRemoteInputCaptureComponent vrInputCapture)
        {
            BoolVariable tempClick;
            BoolVariable tempTouch;

            #region TRIGGER
            tempClick = _inputContainer.LeftClickBoolean.Get("TriggerIsDown");
            tempTouch = _inputContainer.LeftTouchBoolean.Get("TriggerIsTouching");

            // Checking Click event 
            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("TriggerDown");
                vrInputCapture.TempEvent.Raise();
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
            {
                tempClick.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("TriggerUp");
                vrInputCapture.TempEvent.Raise();
            }
            // Checking Touch event if user is not clicking
            else if (!tempClick.Value && !tempTouch.Value && OVRInput.Get(OVRInput.Touch.PrimaryIndexTrigger))
            {
                tempTouch.SetValue(true);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftTouchEvents.Get("TriggerStartTouching");
                vrInputCapture.TempEvent.Raise();
            }
            else if (!tempClick.Value && tempTouch.Value && !OVRInput.Get(OVRInput.Touch.PrimaryIndexTrigger))
            {
                tempTouch.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftTouchEvents.Get("TriggerStopTouching");
                vrInputCapture.TempEvent.Raise();
            }
            #endregion TRIGGER

            #region THUMBSTICK
            _inputContainer.LeftThumbPosition.SetValue(OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad));
            tempClick = _inputContainer.LeftClickBoolean.Get("ThumbIsDown");
            tempTouch = _inputContainer.LeftTouchBoolean.Get("ThumbIsTouching");

            // Checking Click event 
            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.PrimaryTouchpad))
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("ThumbDown");
                vrInputCapture.TempEvent.Raise();
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.PrimaryTouchpad))
            {
                tempClick.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("ThumbUp");
                vrInputCapture.TempEvent.Raise();
            }
            // Checking Touch event if user is not clicking
            else if (!tempClick.Value && !tempTouch.Value && (OVRInput.Get(OVRInput.Touch.PrimaryTouchpad) || _inputContainer.LeftThumbPosition.Value != Vector2.zero))
            {
                tempTouch.SetValue(true);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftTouchEvents.Get("ThumbStartTouching");
                vrInputCapture.TempEvent.Raise();
            }
            else if (tempTouch.Value && (!OVRInput.Get(OVRInput.Touch.PrimaryTouchpad) && _inputContainer.LeftThumbPosition.Value == Vector2.zero))
            {
                tempTouch.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftTouchEvents.Get("ThumbStopTouching");
                vrInputCapture.TempEvent.Raise();
            }
            #endregion THUMBSTICK

            #region BACK
            tempClick = _inputContainer.LeftClickBoolean.Get("BackButtonIsDown");

            // Checking Click event 
            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.Back))
            {
                tempClick.SetValue(true);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("BackButtonDown");
                vrInputCapture.TempEvent.Raise();
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.Back))
            {
                tempClick.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("BackButtonUp");
                vrInputCapture.TempEvent.Raise();
            }
            // Touch Event not existing on BACK
            #endregion BACK
        }
        #endregion
    }
}