using ScriptableFramework.Events;
using ScriptableFramework.Variables;
using Unity.Entities;
using UnityEngine;
using VRSF.Controllers;
using VRSF.Inputs.Components;

namespace VRSF.Inputs.Systems
{
    /// <summary>
    /// Script attached to the OculusSDK Prefab.
    /// Set the GameEvent depending on the Oculus Inputs
    /// </summary>
    public class RiftLeftControllerInputCaptureSystem : ComponentSystem
    {
        struct Filter
        {
            public RiftControllersInputCaptureComponent VRInputCapture;
        }

        #region PRIVATE_VARIABLES
        // VRSF Parameters references
        private InputVariableContainer _inputContainer;
        private ControllersParametersVariable _controllersParameters;
        #endregion PRIVATE_VARIABLES


        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnStartRunning()
        {
            base.OnStartRunning();
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
                    CheckLeftControllerInput(entity.VRInputCapture);
                }
            }
        }
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Handle the Left Controller input and put them in the Events
        /// </summary>
        void CheckLeftControllerInput(RiftControllersInputCaptureComponent vrInputCapture)
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
            _inputContainer.LeftThumbPosition.SetValue(OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick));
            tempClick = _inputContainer.LeftClickBoolean.Get("ThumbIsDown");
            tempTouch = _inputContainer.LeftTouchBoolean.Get("ThumbIsTouching");

            // Checking Click event 
            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.PrimaryThumbstick))
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("ThumbDown");
                vrInputCapture.TempEvent.Raise();
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.PrimaryThumbstick))
            {
                tempClick.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("ThumbUp");
                vrInputCapture.TempEvent.Raise();
            }
            // Checking Touch event if user is not clicking
            else if (!tempClick.Value && !tempTouch.Value && (OVRInput.Get(OVRInput.Touch.PrimaryThumbstick) || _inputContainer.LeftThumbPosition.Value != Vector2.zero))
            {
                tempTouch.SetValue(true);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftTouchEvents.Get("ThumbStartTouching");
                vrInputCapture.TempEvent.Raise();
            }
            else if (tempTouch.Value && (!OVRInput.Get(OVRInput.Touch.PrimaryThumbstick) && _inputContainer.LeftThumbPosition.Value == Vector2.zero))
            {
                tempTouch.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftTouchEvents.Get("ThumbStopTouching");
                vrInputCapture.TempEvent.Raise();
            }
            #endregion THUMBSTICK

            #region GRIP
            tempClick = _inputContainer.LeftClickBoolean.Get("GripIsDown");

            // Checking Click event 
            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.PrimaryHandTrigger))
            {
                tempClick.SetValue(true);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("GripDown");
                vrInputCapture.TempEvent.Raise();
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.PrimaryHandTrigger))
            {
                tempClick.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("GripUp");
                vrInputCapture.TempEvent.Raise();
            }
            // Touch Event not existing on Grip
            #endregion GRIP

            #region MENU
            tempClick = _inputContainer.LeftClickBoolean.Get("MenuIsDown");

            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.Start))
            {
                tempClick.SetValue(true);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("MenuDown");
                vrInputCapture.TempEvent.Raise();
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.Start))
            {
                tempClick.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("MenuUp");
                vrInputCapture.TempEvent.Raise();
            }
            // Touch Event not existing on Start 
            #endregion MENU

            #region Button X
            tempClick = _inputContainer.LeftClickBoolean.Get("XButtonIsDown");
            tempTouch = _inputContainer.LeftTouchBoolean.Get("XButtonIsTouching");

            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.Three))
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("XButtonDown");
                vrInputCapture.TempEvent.Raise();
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.Three))
            {
                tempClick.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("XButtonUp");
                vrInputCapture.TempEvent.Raise();
            }
            // Checking Touch event if user is not clicking
            else if (!tempClick.Value && !tempTouch.Value && OVRInput.Get(OVRInput.Touch.Three))
            {
                tempTouch.SetValue(true);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftTouchEvents.Get("XButtonStartTouching");
                vrInputCapture.TempEvent.Raise();
            }
            else if (!tempClick.Value && tempTouch.Value && !OVRInput.Get(OVRInput.Touch.Three))
            {
                tempTouch.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftTouchEvents.Get("XButtonStopTouching");
                vrInputCapture.TempEvent.Raise();
            }
            #endregion Button X

            #region Button Y
            tempClick = _inputContainer.LeftClickBoolean.Get("YButtonIsDown");
            tempTouch = _inputContainer.LeftTouchBoolean.Get("YButtonIsTouching");

            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.Four))
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("YButtonDown");
                vrInputCapture.TempEvent.Raise();
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.Four))
            {
                tempClick.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("YButtonUp");
                vrInputCapture.TempEvent.Raise();
            }
            // Checking Touch event if user is not clicking
            else if (!tempClick.Value && !tempTouch.Value && OVRInput.Get(OVRInput.Touch.Four))
            {
                tempTouch.SetValue(true);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftTouchEvents.Get("YButtonStartTouching");
                vrInputCapture.TempEvent.Raise();
            }
            else if (!tempClick.Value && tempTouch.Value && !OVRInput.Get(OVRInput.Touch.Four))
            {
                tempTouch.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftTouchEvents.Get("YButtonStopTouching");
                vrInputCapture.TempEvent.Raise();
            }
            #endregion Button Y

            #region THUMBREST_TOUCH_ONLY
            tempTouch = _inputContainer.LeftTouchBoolean.Get("ThumbrestIsTouching");

            // Checking Touch event
            if (!tempTouch.Value && OVRInput.Get(OVRInput.Touch.PrimaryThumbRest))
            {
                tempTouch.SetValue(true);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftTouchEvents.Get("ThumbrestStartTouching");
                vrInputCapture.TempEvent.Raise();
            }
            else if (tempTouch.Value && !OVRInput.Get(OVRInput.Touch.PrimaryThumbRest))
            {
                tempTouch.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftTouchEvents.Get("ThumbrestStopTouching");
                vrInputCapture.TempEvent.Raise();
            }
            #endregion
        }
        #endregion
    }
}