using ScriptableFramework.Events;
using ScriptableFramework.Variables;
using UnityEngine;
using VRSF.Inputs.Gaze;

namespace VRSF.Inputs
{
    /// <summary>
    /// Script attached to the OculusSDK Prefab.
    /// Set the GameEvent depending on the Oculus Inputs
    /// </summary>
    public class OculusInputCapture : VRInputCapture
    {
        // EMPTY
        #region PUBLIC_VARIABLES
        #endregion


        #region PRIVATE_VARIABLES
        GameEvent _leftClickEvent;
        GameEvent _leftTouchEvent;

        GameEvent _rightClickEvent;
        GameEvent _rightTouchEvent;
        #endregion


        #region MONOBEHAVIOURS
        public override void Start()
        {
            base.Start();
            CheckGazeClickButton();
        }

        // Update is called once per frame
        void Update()
        {
            CheckLeftControllerInput();
            CheckRightControllerInput();

            if (CheckGazeInteractions && GazeParameters.UseGaze)
                CheckGazeInputs();
        }
        #endregion


        //EMPTY
        #region PUBLIC_METHODS
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Handle the Left Controller input and put them in the Events
        /// </summary>
        void CheckLeftControllerInput()
        {
            BoolVariable tempClick;
            BoolVariable tempTouch;

            #region TRIGGER
            tempClick = InputContainer.LeftClickBoolean.Get("TriggerIsDown");
            tempTouch = InputContainer.LeftTouchBoolean.Get("TriggerIsTouching");

            // Checking Click event 
            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                _leftClickEvent = (GameEvent)InputContainer.LeftClickEvents.Get("TriggerDown");
                _leftClickEvent.Raise();
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
            {
                tempClick.SetValue(false);
                _leftClickEvent = (GameEvent)InputContainer.LeftClickEvents.Get("TriggerUp");
                _leftClickEvent.Raise();
            }
            // Checking Touch event if user is not clicking
            else if (!tempClick.Value && !tempTouch.Value && OVRInput.Get(OVRInput.Touch.PrimaryIndexTrigger))
            {
                tempTouch.SetValue(true);
                _leftTouchEvent = (GameEvent)InputContainer.LeftTouchEvents.Get("TriggerStartTouching");
                _leftTouchEvent.Raise();
            }
            else if (!tempClick.Value && tempTouch.Value && !OVRInput.Get(OVRInput.Touch.PrimaryIndexTrigger))
            {
                tempTouch.SetValue(false);
                _leftTouchEvent = (GameEvent)InputContainer.LeftTouchEvents.Get("TriggerStopTouching");
                _leftTouchEvent.Raise();
            }
            #endregion TRIGGER

            #region THUMBSTICK
            InputContainer.LeftThumbPosition.SetValue(OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick));
            tempClick = InputContainer.LeftClickBoolean.Get("ThumbIsDown");
            tempTouch = InputContainer.LeftTouchBoolean.Get("ThumbIsTouching");

            // Checking Click event 
            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.PrimaryThumbstick))
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                _leftClickEvent = (GameEvent)InputContainer.LeftClickEvents.Get("ThumbDown");
                _leftClickEvent.Raise();
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.PrimaryThumbstick))
            {
                tempClick.SetValue(false);
                _leftClickEvent = (GameEvent)InputContainer.LeftClickEvents.Get("ThumbUp");
                _leftClickEvent.Raise();
            }
            // Checking Touch event if user is not clicking
            else if (!tempClick.Value && !tempTouch.Value && (OVRInput.Get(OVRInput.Touch.PrimaryThumbstick) || InputContainer.LeftThumbPosition.Value != Vector2.zero))
            {
                tempTouch.SetValue(true);
                _leftTouchEvent = (GameEvent)InputContainer.LeftTouchEvents.Get("ThumbStartTouching");
                _leftTouchEvent.Raise();
            }
            else if (tempTouch.Value && (!OVRInput.Get(OVRInput.Touch.PrimaryThumbstick) && InputContainer.LeftThumbPosition.Value == Vector2.zero))
            {
                tempTouch.SetValue(false);
                _leftClickEvent = (GameEvent)InputContainer.LeftTouchEvents.Get("ThumbStopTouching");
                _leftClickEvent.Raise();
            }
            #endregion THUMBSTICK

            #region GRIP
            tempClick = InputContainer.LeftClickBoolean.Get("GripIsDown");

            // Checking Click event 
            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.PrimaryHandTrigger))
            {
                tempClick.SetValue(true);
                _leftClickEvent = (GameEvent)InputContainer.LeftClickEvents.Get("GripDown");
                _leftClickEvent.Raise();
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.PrimaryHandTrigger))
            {
                tempClick.SetValue(false);
                _leftClickEvent = (GameEvent)InputContainer.LeftClickEvents.Get("GripUp");
                _leftClickEvent.Raise();
            }
            // Touch Event not existing on Grip
            #endregion GRIP

            #region MENU
            tempClick = InputContainer.LeftClickBoolean.Get("MenuIsDown");

            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.Start))
            {
                tempClick.SetValue(true);
                _leftClickEvent = (GameEvent)InputContainer.LeftClickEvents.Get("MenuDown");
                _leftClickEvent.Raise();
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.Start))
            {
                tempClick.SetValue(false);
                _leftClickEvent = (GameEvent)InputContainer.LeftClickEvents.Get("MenuUp");
                _leftClickEvent.Raise();
            }
            // Touch Event not existing on Start 
            #endregion MENU

            #region Button X
            tempClick = InputContainer.LeftClickBoolean.Get("XButtonIsDown");
            tempTouch = InputContainer.LeftTouchBoolean.Get("XButtonIsTouching");

            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.Three))
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                _leftClickEvent = (GameEvent)InputContainer.LeftClickEvents.Get("XButtonDown");
                _leftClickEvent.Raise();
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.Three))
            {
                tempClick.SetValue(false);
                _leftClickEvent = (GameEvent)InputContainer.LeftClickEvents.Get("XButtonUp");
                _leftClickEvent.Raise();
            }
            // Checking Touch event if user is not clicking
            else if (!tempClick.Value && !tempTouch.Value && OVRInput.Get(OVRInput.Touch.Three))
            {
                tempTouch.SetValue(true);
                _leftTouchEvent = (GameEvent)InputContainer.LeftTouchEvents.Get("XButtonStartTouching");
                _leftTouchEvent.Raise();
            }
            else if (!tempClick.Value && tempTouch.Value && !OVRInput.Get(OVRInput.Touch.Three))
            {
                tempTouch.SetValue(false);
                _leftTouchEvent = (GameEvent)InputContainer.LeftTouchEvents.Get("XButtonStopTouching");
                _leftTouchEvent.Raise();
            }
            #endregion Button X

            #region Button Y
            tempClick = InputContainer.LeftClickBoolean.Get("YButtonIsDown");
            tempTouch = InputContainer.LeftTouchBoolean.Get("YButtonIsTouching");

            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.Four))
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                _leftClickEvent = (GameEvent)InputContainer.LeftClickEvents.Get("YButtonDown");
                _leftClickEvent.Raise();
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.Four))
            {
                tempClick.SetValue(false);
                _leftClickEvent = (GameEvent)InputContainer.LeftClickEvents.Get("YButtonUp");
                _leftClickEvent.Raise();
            }
            // Checking Touch event if user is not clicking
            else if (!tempClick.Value && !tempTouch.Value && OVRInput.Get(OVRInput.Touch.Four))
            {
                tempTouch.SetValue(true);
                _leftTouchEvent = (GameEvent)InputContainer.LeftTouchEvents.Get("YButtonStartTouching");
                _leftTouchEvent.Raise();
            }
            else if (!tempClick.Value && tempTouch.Value && !OVRInput.Get(OVRInput.Touch.Four))
            {
                tempTouch.SetValue(false);
                _leftTouchEvent = (GameEvent)InputContainer.LeftTouchEvents.Get("YButtonStopTouching");
                _leftTouchEvent.Raise();
            }
            #endregion Button Y

            #region THUMBREST_TOUCH_ONLY
            tempTouch = InputContainer.LeftTouchBoolean.Get("ThumbrestIsTouching");

            // Checking Touch event
            if (!tempTouch.Value && OVRInput.Get(OVRInput.Touch.PrimaryThumbRest))
            {
                tempTouch.SetValue(true);
                _leftTouchEvent = (GameEvent)InputContainer.LeftTouchEvents.Get("ThumbrestStartTouching");
                _leftTouchEvent.Raise();
            }
            else if (tempTouch.Value && !OVRInput.Get(OVRInput.Touch.PrimaryThumbRest))
            {
                tempTouch.SetValue(false);
                _leftTouchEvent = (GameEvent)InputContainer.LeftTouchEvents.Get("ThumbrestStopTouching");
                _leftTouchEvent.Raise();
            }
            #endregion
        }

        /// <summary>
        /// Handle the Right Controller input and put them in the Events
        /// </summary>
        void CheckRightControllerInput()
        {
            BoolVariable tempClick;
            BoolVariable tempTouch;

            #region TRIGGER
            tempClick = InputContainer.RightClickBoolean.Get("TriggerIsDown");
            tempTouch = InputContainer.RightTouchBoolean.Get("TriggerIsTouching");

            // Checking Click event
            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                _rightClickEvent = (GameEvent)InputContainer.RightClickEvents.Get("TriggerDown");
                _rightClickEvent.Raise();
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
            {
                tempClick.SetValue(false);
                _rightClickEvent = (GameEvent)InputContainer.RightClickEvents.Get("TriggerUp");
                _rightClickEvent.Raise();
            }
            // Checking Touch event if user is not clicking
            else if (!tempClick.Value && !tempTouch.Value && OVRInput.Get(OVRInput.Touch.SecondaryIndexTrigger))
            {
                tempTouch.SetValue(true);
                _rightTouchEvent = (GameEvent)InputContainer.RightTouchEvents.Get("TriggerStartTouching");
                _rightTouchEvent.Raise();
            }
            else if (!tempClick.Value && tempTouch.Value && !OVRInput.Get(OVRInput.Touch.SecondaryIndexTrigger))
            {
                tempTouch.SetValue(false);
                _rightTouchEvent = (GameEvent)InputContainer.RightTouchEvents.Get("TriggerStopTouching");
                _rightTouchEvent.Raise();
            }
            #endregion TRIGGER

            #region THUMBSTICK
            InputContainer.RightThumbPosition.SetValue(OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick));

            tempClick = InputContainer.RightClickBoolean.Get("ThumbIsDown");
            tempTouch = InputContainer.RightTouchBoolean.Get("ThumbIsTouching");

            // Checking Click event
            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.SecondaryThumbstick))
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                _rightClickEvent = (GameEvent)InputContainer.RightClickEvents.Get("ThumbDown");
                _rightClickEvent.Raise();
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.SecondaryThumbstick))
            {
                tempClick.SetValue(false);
                _rightClickEvent = (GameEvent)InputContainer.RightClickEvents.Get("ThumbUp");
                _rightClickEvent.Raise();
            }
            // Checking Touch event if user is not clicking
            else if (!tempClick.Value && !tempTouch.Value && (OVRInput.Get(OVRInput.Touch.SecondaryThumbstick) || InputContainer.RightThumbPosition.Value != Vector2.zero))
            {
                tempTouch.SetValue(true);
                _rightTouchEvent = (GameEvent)InputContainer.RightTouchEvents.Get("ThumbStartTouching");
                _rightTouchEvent.Raise();
            }
            else if (tempTouch.Value && (!OVRInput.Get(OVRInput.Touch.SecondaryThumbstick) && InputContainer.RightThumbPosition.Value == Vector2.zero))
            {
                tempTouch.SetValue(false);
                _rightTouchEvent = (GameEvent)InputContainer.RightTouchEvents.Get("ThumbStopTouching");
                _rightTouchEvent.Raise();
            }
            #endregion THUMBSTICK

            #region GRIP
            tempClick = InputContainer.RightClickBoolean.Get("GripIsDown");

            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
            {
                tempClick.SetValue(true);
                _rightClickEvent = (GameEvent)InputContainer.RightClickEvents.Get("GripDown");
                _rightClickEvent.Raise();
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
            {
                tempClick.SetValue(false);
                _rightClickEvent = (GameEvent)InputContainer.RightClickEvents.Get("GripUp");
                _rightClickEvent.Raise();
            }
            // Touch Event not existing on Grip 
            #endregion GRIP

            //No Right menu button on the oculus

            #region Button A
            tempClick = InputContainer.RightClickBoolean.Get("AButtonIsDown");
            tempTouch = InputContainer.RightTouchBoolean.Get("AButtonIsTouching");

            // Checking Click event
            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.One))
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                _rightClickEvent = (GameEvent)InputContainer.RightClickEvents.Get("AButtonDown");
                _rightClickEvent.Raise();
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.One))
            {
                tempClick.SetValue(false);
                _rightClickEvent = (GameEvent)InputContainer.RightClickEvents.Get("AButtonUp");
                _rightClickEvent.Raise();
            }
            // Checking Touch event if user is not clicking
            else if (!tempClick.Value && !tempTouch.Value && OVRInput.Get(OVRInput.Touch.One))
            {
                tempTouch.SetValue(true);
                _rightTouchEvent = (GameEvent)InputContainer.RightTouchEvents.Get("AButtonStartTouching");
                _rightTouchEvent.Raise();
            }
            else if (tempTouch.Value && !OVRInput.Get(OVRInput.Touch.One))
            {
                tempTouch.SetValue(false);
                _rightTouchEvent = (GameEvent)InputContainer.RightTouchEvents.Get("AButtonStopTouching");
                _rightTouchEvent.Raise();
            }
            #endregion

            #region Button B
            tempClick = InputContainer.RightClickBoolean.Get("BButtonIsDown");
            tempTouch = InputContainer.RightTouchBoolean.Get("BButtonIsTouching");

            // Checking Click event
            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.Two))
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                _rightClickEvent = (GameEvent)InputContainer.RightClickEvents.Get("BButtonDown");
                _rightClickEvent.Raise();
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.Two))
            {
                tempClick.SetValue(false);
                _rightClickEvent = (GameEvent)InputContainer.RightClickEvents.Get("BButtonUp");
                _rightClickEvent.Raise();
            }
            // Checking Touch event if user is not clicking
            else if (!tempClick.Value && !tempTouch.Value && OVRInput.Get(OVRInput.Touch.Two))
            {
                tempTouch.SetValue(true);
                _rightTouchEvent = (GameEvent)InputContainer.RightTouchEvents.Get("BButtonStartTouching");
                _rightTouchEvent.Raise();
            }
            else if (!tempClick.Value && tempTouch.Value && !OVRInput.Get(OVRInput.Touch.Two))
            {
                tempTouch.SetValue(false);
                _rightTouchEvent = (GameEvent)InputContainer.RightTouchEvents.Get("BButtonStopTouching");
                _rightTouchEvent.Raise();
            }
            #endregion

            #region THUMBREST_TOUCH_ONLY
            tempTouch = InputContainer.RightTouchBoolean.Get("ThumbrestIsTouching");

            // Checking Touch event
            if (!tempTouch.Value && OVRInput.Get(OVRInput.Touch.SecondaryThumbRest))
            {
                tempTouch.SetValue(true);
                _rightTouchEvent = (GameEvent)InputContainer.RightTouchEvents.Get("ThumbrestStartTouching");
                _rightTouchEvent.Raise();
            }
            else if (tempTouch.Value && !OVRInput.Get(OVRInput.Touch.SecondaryThumbRest))
            {
                tempTouch.SetValue(false);
                _rightTouchEvent = (GameEvent)InputContainer.RightTouchEvents.Get("ThumbrestStopTouching");
                _rightTouchEvent.Raise();
            }
            #endregion
        }


        /// <summary>
        /// Handle the Gaze click button, in the simulator case, the mouse wheel
        /// </summary>
        void CheckGazeInputs()
        {
            // Checking Click event
            if (!InputContainer.GazeIsCliking.Value && OVRInput.Get(GazeInteractionDictionaryOVR.GazeClickOVRInput[GazeParameters.GazeButtonOVR]))
            {
                InputContainer.GazeIsCliking.SetValue(true);
                InputContainer.GazeClickDown.Raise();
            }
            else if (InputContainer.GazeIsCliking.Value && !OVRInput.Get(GazeInteractionDictionaryOVR.GazeClickOVRInput[GazeParameters.GazeButtonOVR]))
            {
                InputContainer.GazeIsCliking.SetValue(false);
                InputContainer.GazeClickUp.Raise();
            }

            // Checking Touch event
            if (!InputContainer.GazeIsTouching.Value && OVRInput.Get(GazeInteractionDictionaryOVR.GazeTouchOVRInput[GazeParameters.GazeButtonOVR]))
            {
                InputContainer.GazeIsTouching.SetValue(true);
                InputContainer.GazeStartTouching.Raise();
            }
            else if (InputContainer.GazeIsTouching.Value && !OVRInput.Get(GazeInteractionDictionaryOVR.GazeTouchOVRInput[GazeParameters.GazeButtonOVR]))
            {
                InputContainer.GazeIsTouching.SetValue(false);
                InputContainer.GazeStopTouching.Raise();
            }
        }

        /// <summary>
        /// Check if the specified button in the Gaze Parameters Window was set correctly 
        /// </summary>
        private void CheckGazeClickButton()
        {
            if (GazeParameters.GazeButtonOVR == EControllersInput.NONE)
            {
                CheckGazeInteractions = false;
            }
            else if (GazeParameters.GazeButtonOVR == EControllersInput.WHEEL_BUTTON)
            {
                CheckGazeInteractions = false;
                Debug.LogError("VRSF : Cannot check the Gaze Click with the Wheel Button of the mouse for the Oculus.");
            }
            else if (GazeParameters.GazeButtonOVR == EControllersInput.RIGHT_MENU)
            {
                CheckGazeInteractions = false;
                Debug.LogError("VRSF : Cannot check the Gaze Click with the Right Menu button for the Oculus.");
            }
        }
        #endregion
    }
}