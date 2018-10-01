
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
    public class RiftRightControllerInputCaptureSystem : ComponentSystem
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
                    CheckRightControllerInput(entity.VRInputCapture);
                }
            }
        }
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Handle the Right Controller input and put them in the Events
        /// </summary>
        void CheckRightControllerInput(RiftControllersInputCaptureComponent vrInputCapture)
        {
            BoolVariable tempClick;
            BoolVariable tempTouch;

            #region TRIGGER
            tempClick = _inputContainer.RightClickBoolean.Get("TriggerIsDown");
            tempTouch = _inputContainer.RightTouchBoolean.Get("TriggerIsTouching");

            // Checking Click event
            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.RightClickEvents.Get("TriggerDown");
                vrInputCapture.TempEvent.Raise();
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
            {
                tempClick.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.RightClickEvents.Get("TriggerUp");
                vrInputCapture.TempEvent.Raise();
            }
            // Checking Touch event if user is not clicking
            else if (!tempClick.Value && !tempTouch.Value && OVRInput.Get(OVRInput.Touch.SecondaryIndexTrigger))
            {
                tempTouch.SetValue(true);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.RightTouchEvents.Get("TriggerStartTouching");
                vrInputCapture.TempEvent.Raise();
            }
            else if (!tempClick.Value && tempTouch.Value && !OVRInput.Get(OVRInput.Touch.SecondaryIndexTrigger))
            {
                tempTouch.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.RightTouchEvents.Get("TriggerStopTouching");
                vrInputCapture.TempEvent.Raise();
            }
            #endregion TRIGGER

            #region THUMBSTICK
            _inputContainer.RightThumbPosition.SetValue(OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick));

            tempClick = _inputContainer.RightClickBoolean.Get("ThumbIsDown");
            tempTouch = _inputContainer.RightTouchBoolean.Get("ThumbIsTouching");

            // Checking Click event
            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.SecondaryThumbstick))
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.RightClickEvents.Get("ThumbDown");
                vrInputCapture.TempEvent.Raise();
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.SecondaryThumbstick))
            {
                tempClick.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.RightClickEvents.Get("ThumbUp");
                vrInputCapture.TempEvent.Raise();
            }
            // Checking Touch event if user is not clicking
            else if (!tempClick.Value && !tempTouch.Value && (OVRInput.Get(OVRInput.Touch.SecondaryThumbstick) || _inputContainer.RightThumbPosition.Value != Vector2.zero))
            {
                tempTouch.SetValue(true);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.RightTouchEvents.Get("ThumbStartTouching");
                vrInputCapture.TempEvent.Raise();
            }
            else if (tempTouch.Value && (!OVRInput.Get(OVRInput.Touch.SecondaryThumbstick) && _inputContainer.RightThumbPosition.Value == Vector2.zero))
            {
                tempTouch.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.RightTouchEvents.Get("ThumbStopTouching");
                vrInputCapture.TempEvent.Raise();
            }
            #endregion THUMBSTICK

            #region GRIP
            tempClick = _inputContainer.RightClickBoolean.Get("GripIsDown");

            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
            {
                tempClick.SetValue(true);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.RightClickEvents.Get("GripDown");
                vrInputCapture.TempEvent.Raise();
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
            {
                tempClick.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.RightClickEvents.Get("GripUp");
                vrInputCapture.TempEvent.Raise();
            }
            // Touch Event not existing on Grip 
            #endregion GRIP

            //No Right menu button on the oculus

            #region Button A
            tempClick = _inputContainer.RightClickBoolean.Get("AButtonIsDown");
            tempTouch = _inputContainer.RightTouchBoolean.Get("AButtonIsTouching");

            // Checking Click event
            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.One))
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.RightClickEvents.Get("AButtonDown");
                vrInputCapture.TempEvent.Raise();
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.One))
            {
                tempClick.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.RightClickEvents.Get("AButtonUp");
                vrInputCapture.TempEvent.Raise();
            }
            // Checking Touch event if user is not clicking
            else if (!tempClick.Value && !tempTouch.Value && OVRInput.Get(OVRInput.Touch.One))
            {
                tempTouch.SetValue(true);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.RightTouchEvents.Get("AButtonStartTouching");
                vrInputCapture.TempEvent.Raise();
            }
            else if (tempTouch.Value && !OVRInput.Get(OVRInput.Touch.One))
            {
                tempTouch.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.RightTouchEvents.Get("AButtonStopTouching");
                vrInputCapture.TempEvent.Raise();
            }
            #endregion

            #region Button B
            tempClick = _inputContainer.RightClickBoolean.Get("BButtonIsDown");
            tempTouch = _inputContainer.RightTouchBoolean.Get("BButtonIsTouching");

            // Checking Click event
            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.Two))
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.RightClickEvents.Get("BButtonDown");
                vrInputCapture.TempEvent.Raise();
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.Two))
            {
                tempClick.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.RightClickEvents.Get("BButtonUp");
                vrInputCapture.TempEvent.Raise();
            }
            // Checking Touch event if user is not clicking
            else if (!tempClick.Value && !tempTouch.Value && OVRInput.Get(OVRInput.Touch.Two))
            {
                tempTouch.SetValue(true);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.RightTouchEvents.Get("BButtonStartTouching");
                vrInputCapture.TempEvent.Raise();
            }
            else if (!tempClick.Value && tempTouch.Value && !OVRInput.Get(OVRInput.Touch.Two))
            {
                tempTouch.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.RightTouchEvents.Get("BButtonStopTouching");
                vrInputCapture.TempEvent.Raise();
            }
            #endregion

            #region THUMBREST_TOUCH_ONLY
            tempTouch = _inputContainer.RightTouchBoolean.Get("ThumbrestIsTouching");

            // Checking Touch event
            if (!tempTouch.Value && OVRInput.Get(OVRInput.Touch.SecondaryThumbRest))
            {
                tempTouch.SetValue(true);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.RightTouchEvents.Get("ThumbrestStartTouching");
                vrInputCapture.TempEvent.Raise();
            }
            else if (tempTouch.Value && !OVRInput.Get(OVRInput.Touch.SecondaryThumbRest))
            {
                tempTouch.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.RightTouchEvents.Get("ThumbrestStopTouching");
                vrInputCapture.TempEvent.Raise();
            }
            #endregion
        }
        #endregion
    }
}