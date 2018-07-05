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


        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private void SceneStarted()
        {
            foreach (var entity in GetEntities<Filter>())
            {
                entity.VRInputCapture.ControllersParameters = ControllersParametersVariable.Instance;
                entity.VRInputCapture.GazeParameters = GazeParametersVariable.Instance;
                entity.VRInputCapture.InputContainer = InputVariableContainer.Instance;

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
            tempClick = vrInputCapture.InputContainer.LeftClickBoolean.Get("TriggerIsDown");
            tempTouch = vrInputCapture.InputContainer.LeftTouchBoolean.Get("TriggerIsTouching");

            // Checking Click event 
            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.LeftClickEvents.Get("TriggerDown");
                vrInputCapture.TempEvent.Raise();
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
            {
                tempClick.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.LeftClickEvents.Get("TriggerUp");
                vrInputCapture.TempEvent.Raise();
            }
            // Checking Touch event if user is not clicking
            else if (!tempClick.Value && !tempTouch.Value && OVRInput.Get(OVRInput.Touch.PrimaryIndexTrigger))
            {
                tempTouch.SetValue(true);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.LeftTouchEvents.Get("TriggerStartTouching");
                vrInputCapture.TempEvent.Raise();
            }
            else if (!tempClick.Value && tempTouch.Value && !OVRInput.Get(OVRInput.Touch.PrimaryIndexTrigger))
            {
                tempTouch.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.LeftTouchEvents.Get("TriggerStopTouching");
                vrInputCapture.TempEvent.Raise();
            }
            #endregion TRIGGER

            #region THUMBSTICK
            vrInputCapture.InputContainer.LeftThumbPosition.SetValue(OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick));
            tempClick = vrInputCapture.InputContainer.LeftClickBoolean.Get("ThumbIsDown");
            tempTouch = vrInputCapture.InputContainer.LeftTouchBoolean.Get("ThumbIsTouching");

            // Checking Click event 
            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.PrimaryThumbstick))
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.LeftClickEvents.Get("ThumbDown");
                vrInputCapture.TempEvent.Raise();
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.PrimaryThumbstick))
            {
                tempClick.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.LeftClickEvents.Get("ThumbUp");
                vrInputCapture.TempEvent.Raise();
            }
            // Checking Touch event if user is not clicking
            else if (!tempClick.Value && !tempTouch.Value && (OVRInput.Get(OVRInput.Touch.PrimaryThumbstick) || vrInputCapture.InputContainer.LeftThumbPosition.Value != Vector2.zero))
            {
                tempTouch.SetValue(true);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.LeftTouchEvents.Get("ThumbStartTouching");
                vrInputCapture.TempEvent.Raise();
            }
            else if (tempTouch.Value && (!OVRInput.Get(OVRInput.Touch.PrimaryThumbstick) && vrInputCapture.InputContainer.LeftThumbPosition.Value == Vector2.zero))
            {
                tempTouch.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.LeftTouchEvents.Get("ThumbStopTouching");
                vrInputCapture.TempEvent.Raise();
            }
            #endregion THUMBSTICK

            #region GRIP
            tempClick = vrInputCapture.InputContainer.LeftClickBoolean.Get("GripIsDown");

            // Checking Click event 
            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.PrimaryHandTrigger))
            {
                tempClick.SetValue(true);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.LeftClickEvents.Get("GripDown");
                vrInputCapture.TempEvent.Raise();
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.PrimaryHandTrigger))
            {
                tempClick.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.LeftClickEvents.Get("GripUp");
                vrInputCapture.TempEvent.Raise();
            }
            // Touch Event not existing on Grip
            #endregion GRIP

            #region MENU
            tempClick = vrInputCapture.InputContainer.LeftClickBoolean.Get("MenuIsDown");

            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.Start))
            {
                tempClick.SetValue(true);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.LeftClickEvents.Get("MenuDown");
                vrInputCapture.TempEvent.Raise();
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.Start))
            {
                tempClick.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.LeftClickEvents.Get("MenuUp");
                vrInputCapture.TempEvent.Raise();
            }
            // Touch Event not existing on Start 
            #endregion MENU

            #region Button X
            tempClick = vrInputCapture.InputContainer.LeftClickBoolean.Get("XButtonIsDown");
            tempTouch = vrInputCapture.InputContainer.LeftTouchBoolean.Get("XButtonIsTouching");

            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.Three))
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.LeftClickEvents.Get("XButtonDown");
                vrInputCapture.TempEvent.Raise();
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.Three))
            {
                tempClick.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.LeftClickEvents.Get("XButtonUp");
                vrInputCapture.TempEvent.Raise();
            }
            // Checking Touch event if user is not clicking
            else if (!tempClick.Value && !tempTouch.Value && OVRInput.Get(OVRInput.Touch.Three))
            {
                tempTouch.SetValue(true);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.LeftTouchEvents.Get("XButtonStartTouching");
                vrInputCapture.TempEvent.Raise();
            }
            else if (!tempClick.Value && tempTouch.Value && !OVRInput.Get(OVRInput.Touch.Three))
            {
                tempTouch.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.LeftTouchEvents.Get("XButtonStopTouching");
                vrInputCapture.TempEvent.Raise();
            }
            #endregion Button X

            #region Button Y
            tempClick = vrInputCapture.InputContainer.LeftClickBoolean.Get("YButtonIsDown");
            tempTouch = vrInputCapture.InputContainer.LeftTouchBoolean.Get("YButtonIsTouching");

            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.Four))
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.LeftClickEvents.Get("YButtonDown");
                vrInputCapture.TempEvent.Raise();
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.Four))
            {
                tempClick.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.LeftClickEvents.Get("YButtonUp");
                vrInputCapture.TempEvent.Raise();
            }
            // Checking Touch event if user is not clicking
            else if (!tempClick.Value && !tempTouch.Value && OVRInput.Get(OVRInput.Touch.Four))
            {
                tempTouch.SetValue(true);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.LeftTouchEvents.Get("YButtonStartTouching");
                vrInputCapture.TempEvent.Raise();
            }
            else if (!tempClick.Value && tempTouch.Value && !OVRInput.Get(OVRInput.Touch.Four))
            {
                tempTouch.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.LeftTouchEvents.Get("YButtonStopTouching");
                vrInputCapture.TempEvent.Raise();
            }
            #endregion Button Y

            #region THUMBREST_TOUCH_ONLY
            tempTouch = vrInputCapture.InputContainer.LeftTouchBoolean.Get("ThumbrestIsTouching");

            // Checking Touch event
            if (!tempTouch.Value && OVRInput.Get(OVRInput.Touch.PrimaryThumbRest))
            {
                tempTouch.SetValue(true);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.LeftTouchEvents.Get("ThumbrestStartTouching");
                vrInputCapture.TempEvent.Raise();
            }
            else if (tempTouch.Value && !OVRInput.Get(OVRInput.Touch.PrimaryThumbRest))
            {
                tempTouch.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.LeftTouchEvents.Get("ThumbrestStopTouching");
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
            tempClick = vrInputCapture.InputContainer.RightClickBoolean.Get("TriggerIsDown");
            tempTouch = vrInputCapture.InputContainer.RightTouchBoolean.Get("TriggerIsTouching");

            // Checking Click event
            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.RightClickEvents.Get("TriggerDown");
                vrInputCapture.TempEvent.Raise();
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
            {
                tempClick.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.RightClickEvents.Get("TriggerUp");
                vrInputCapture.TempEvent.Raise();
            }
            // Checking Touch event if user is not clicking
            else if (!tempClick.Value && !tempTouch.Value && OVRInput.Get(OVRInput.Touch.SecondaryIndexTrigger))
            {
                tempTouch.SetValue(true);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.RightTouchEvents.Get("TriggerStartTouching");
                vrInputCapture.TempEvent.Raise();
            }
            else if (!tempClick.Value && tempTouch.Value && !OVRInput.Get(OVRInput.Touch.SecondaryIndexTrigger))
            {
                tempTouch.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.RightTouchEvents.Get("TriggerStopTouching");
                vrInputCapture.TempEvent.Raise();
            }
            #endregion TRIGGER

            #region THUMBSTICK
            vrInputCapture.InputContainer.RightThumbPosition.SetValue(OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick));

            tempClick = vrInputCapture.InputContainer.RightClickBoolean.Get("ThumbIsDown");
            tempTouch = vrInputCapture.InputContainer.RightTouchBoolean.Get("ThumbIsTouching");

            // Checking Click event
            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.SecondaryThumbstick))
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.RightClickEvents.Get("ThumbDown");
                vrInputCapture.TempEvent.Raise();
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.SecondaryThumbstick))
            {
                tempClick.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.RightClickEvents.Get("ThumbUp");
                vrInputCapture.TempEvent.Raise();
            }
            // Checking Touch event if user is not clicking
            else if (!tempClick.Value && !tempTouch.Value && (OVRInput.Get(OVRInput.Touch.SecondaryThumbstick) || vrInputCapture.InputContainer.RightThumbPosition.Value != Vector2.zero))
            {
                tempTouch.SetValue(true);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.RightTouchEvents.Get("ThumbStartTouching");
                vrInputCapture.TempEvent.Raise();
            }
            else if (tempTouch.Value && (!OVRInput.Get(OVRInput.Touch.SecondaryThumbstick) && vrInputCapture.InputContainer.RightThumbPosition.Value == Vector2.zero))
            {
                tempTouch.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.RightTouchEvents.Get("ThumbStopTouching");
                vrInputCapture.TempEvent.Raise();
            }
            #endregion THUMBSTICK

            #region GRIP
            tempClick = vrInputCapture.InputContainer.RightClickBoolean.Get("GripIsDown");

            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
            {
                tempClick.SetValue(true);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.RightClickEvents.Get("GripDown");
                vrInputCapture.TempEvent.Raise();
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
            {
                tempClick.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.RightClickEvents.Get("GripUp");
                vrInputCapture.TempEvent.Raise();
            }
            // Touch Event not existing on Grip 
            #endregion GRIP

            //No Right menu button on the oculus

            #region Button A
            tempClick = vrInputCapture.InputContainer.RightClickBoolean.Get("AButtonIsDown");
            tempTouch = vrInputCapture.InputContainer.RightTouchBoolean.Get("AButtonIsTouching");

            // Checking Click event
            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.One))
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.RightClickEvents.Get("AButtonDown");
                vrInputCapture.TempEvent.Raise();
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.One))
            {
                tempClick.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.RightClickEvents.Get("AButtonUp");
                vrInputCapture.TempEvent.Raise();
            }
            // Checking Touch event if user is not clicking
            else if (!tempClick.Value && !tempTouch.Value && OVRInput.Get(OVRInput.Touch.One))
            {
                tempTouch.SetValue(true);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.RightTouchEvents.Get("AButtonStartTouching");
                vrInputCapture.TempEvent.Raise();
            }
            else if (tempTouch.Value && !OVRInput.Get(OVRInput.Touch.One))
            {
                tempTouch.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.RightTouchEvents.Get("AButtonStopTouching");
                vrInputCapture.TempEvent.Raise();
            }
            #endregion

            #region Button B
            tempClick = vrInputCapture.InputContainer.RightClickBoolean.Get("BButtonIsDown");
            tempTouch = vrInputCapture.InputContainer.RightTouchBoolean.Get("BButtonIsTouching");

            // Checking Click event
            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.Two))
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.RightClickEvents.Get("BButtonDown");
                vrInputCapture.TempEvent.Raise();
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.Two))
            {
                tempClick.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.RightClickEvents.Get("BButtonUp");
                vrInputCapture.TempEvent.Raise();
            }
            // Checking Touch event if user is not clicking
            else if (!tempClick.Value && !tempTouch.Value && OVRInput.Get(OVRInput.Touch.Two))
            {
                tempTouch.SetValue(true);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.RightTouchEvents.Get("BButtonStartTouching");
                vrInputCapture.TempEvent.Raise();
            }
            else if (!tempClick.Value && tempTouch.Value && !OVRInput.Get(OVRInput.Touch.Two))
            {
                tempTouch.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.RightTouchEvents.Get("BButtonStopTouching");
                vrInputCapture.TempEvent.Raise();
            }
            #endregion

            #region THUMBREST_TOUCH_ONLY
            tempTouch = vrInputCapture.InputContainer.RightTouchBoolean.Get("ThumbrestIsTouching");

            // Checking Touch event
            if (!tempTouch.Value && OVRInput.Get(OVRInput.Touch.SecondaryThumbRest))
            {
                tempTouch.SetValue(true);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.RightTouchEvents.Get("ThumbrestStartTouching");
                vrInputCapture.TempEvent.Raise();
            }
            else if (tempTouch.Value && !OVRInput.Get(OVRInput.Touch.SecondaryThumbRest))
            {
                tempTouch.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.RightTouchEvents.Get("ThumbrestStopTouching");
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
            if (!vrInputCapture.InputContainer.GazeIsCliking.Value && OVRInput.Get(GazeInteractionOVR.ClickDictionnary[vrInputCapture.GazeParameters.GazeButtonOVR]))
            {
                vrInputCapture.InputContainer.GazeIsCliking.SetValue(true);
                vrInputCapture.InputContainer.GazeClickDown.Raise();
            }
            else if (vrInputCapture.InputContainer.GazeIsCliking.Value && !OVRInput.Get(GazeInteractionOVR.ClickDictionnary[vrInputCapture.GazeParameters.GazeButtonOVR]))
            {
                vrInputCapture.InputContainer.GazeIsCliking.SetValue(false);
                vrInputCapture.InputContainer.GazeClickUp.Raise();
            }

            // Checking Touch event
            if (!vrInputCapture.InputContainer.GazeIsTouching.Value && OVRInput.Get(GazeInteractionOVR.TouchDictionnary[vrInputCapture.GazeParameters.GazeButtonOVR]))
            {
                vrInputCapture.InputContainer.GazeIsTouching.SetValue(true);
                vrInputCapture.InputContainer.GazeStartTouching.Raise();
            }
            else if (vrInputCapture.InputContainer.GazeIsTouching.Value && !OVRInput.Get(GazeInteractionOVR.TouchDictionnary[vrInputCapture.GazeParameters.GazeButtonOVR]))
            {
                vrInputCapture.InputContainer.GazeIsTouching.SetValue(false);
                vrInputCapture.InputContainer.GazeStopTouching.Raise();
            }
        }


        /// <summary>
        /// Check if the specified button in the Gaze Parameters Window was set correctly 
        /// </summary>
        private void CheckGazeClickButton(VRInputCaptureComponent vrInputCapture)
        {
            if ((vrInputCapture.GazeParameters.GazeButtonOVR == EControllersInput.NONE) || !vrInputCapture.GazeParameters.UseGaze)
            {
                vrInputCapture.CheckGazeInteractions = false;
            }
            else if (vrInputCapture.GazeParameters.GazeButtonOVR == EControllersInput.WHEEL_BUTTON)
            {
                vrInputCapture.CheckGazeInteractions = false;
                Debug.LogError("VRSF : Cannot check the Gaze Click with the Wheel Button of the mouse for the Oculus.");
            }
            else if (vrInputCapture.GazeParameters.GazeButtonOVR == EControllersInput.RIGHT_MENU)
            {
                vrInputCapture.CheckGazeInteractions = false;
                Debug.LogError("VRSF : Cannot check the Gaze Click with the Right Menu button for the Oculus.");
            }
        }
        #endregion
    }
}