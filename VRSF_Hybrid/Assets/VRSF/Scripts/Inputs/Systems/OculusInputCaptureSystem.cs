using ScriptableFramework.Events;
using ScriptableFramework.Variables;
using Unity.Entities;
using UnityEngine;
using VRSF.Controllers;
using VRSF.Gaze;
using VRSF.Inputs.Components;
using VRSF.Inputs.Gaze;

namespace VRSF.Inputs.Systems
{
    /// <summary>
    /// Script attached to the OculusSDK Prefab.
    /// Set the GameEvent depending on the Oculus Inputs
    /// </summary>
    public class OculusInputCaptureSystem : ComponentSystem
    {
        struct Filter
        {
            public VRInputCaptureComponent VRInputCapture;
        }

        #region PRIVATE_VARIABLES
        // VRSF Parameters references
        private GazeParametersVariable _gazeParameters;
        private InputVariableContainer _inputContainer;
        #endregion PRIVATE_VARIABLES


        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            _gazeParameters = GazeParametersVariable.Instance;
            _inputContainer = InputVariableContainer.Instance;
            
            foreach (var entity in GetEntities<Filter>())
            {
                CheckGazeClickButton(entity.VRInputCapture);
            }
        }

        protected override void OnUpdate()
        {
            // If we doesn't use the controllers, we don't check for the inputs.
            if (ControllersParametersVariable.Instance.UseControllers)
            {
                foreach (var entity in GetEntities<Filter>())
                {
                    CheckLeftControllerInput(entity.VRInputCapture);
                    CheckRightControllerInput(entity.VRInputCapture);

                    if (entity.VRInputCapture.CheckGazeInteractions)
                        CheckGazeInputs(entity.VRInputCapture);
                }
            }
        }
        #endregion


        //EMPTY
        #region PUBLIC_METHODS
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Handle the Left Controller input and put them in the Events
        /// </summary>
        void CheckLeftControllerInput(VRInputCaptureComponent vrInputCapture)
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

        /// <summary>
        /// Handle the Right Controller input and put them in the Events
        /// </summary>
        void CheckRightControllerInput(VRInputCaptureComponent vrInputCapture)
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


        /// <summary>
        /// Handle the Gaze click button, in the simulator case, the mouse wheel
        /// </summary>
        private void CheckGazeInputs(VRInputCaptureComponent vrInputCapture)
        {
            // Checking Click event
            if (!_inputContainer.GazeIsCliking.Value && OVRInput.Get(GazeInteractionOVR.ClickDictionnary[_gazeParameters.GazeButtonOVR]))
            {
                _inputContainer.GazeIsCliking.SetValue(true);
                _inputContainer.GazeClickDown.Raise();
            }
            else if (_inputContainer.GazeIsCliking.Value && !OVRInput.Get(GazeInteractionOVR.ClickDictionnary[_gazeParameters.GazeButtonOVR]))
            {
                _inputContainer.GazeIsCliking.SetValue(false);
                _inputContainer.GazeClickUp.Raise();
            }

            // Checking Touch event
            if (!_inputContainer.GazeIsTouching.Value && OVRInput.Get(GazeInteractionOVR.TouchDictionnary[_gazeParameters.GazeButtonOVR]))
            {
                _inputContainer.GazeIsTouching.SetValue(true);
                _inputContainer.GazeStartTouching.Raise();
            }
            else if (_inputContainer.GazeIsTouching.Value && !OVRInput.Get(GazeInteractionOVR.TouchDictionnary[_gazeParameters.GazeButtonOVR]))
            {
                _inputContainer.GazeIsTouching.SetValue(false);
                _inputContainer.GazeStopTouching.Raise();
            }
        }


        /// <summary>
        /// Check if the specified button in the Gaze Parameters Window was set correctly 
        /// </summary>
        private void CheckGazeClickButton(VRInputCaptureComponent vrInputCapture)
        {
            if ((_gazeParameters.GazeButtonOVR == EControllersInput.NONE) || !_gazeParameters.UseGaze)
            {
                vrInputCapture.CheckGazeInteractions = false;
            }
            else if (_gazeParameters.GazeButtonOVR == EControllersInput.WHEEL_BUTTON)
            {
                vrInputCapture.CheckGazeInteractions = false;
                Debug.LogError("VRSF : Cannot check the Gaze Click with the Wheel Button of the mouse for the Oculus.");
            }
            else if (_gazeParameters.GazeButtonOVR == EControllersInput.RIGHT_MENU)
            {
                vrInputCapture.CheckGazeInteractions = false;
                Debug.LogError("VRSF : Cannot check the Gaze Click with the Right Menu button for the Oculus.");
            }
        }
        #endregion
    }
}