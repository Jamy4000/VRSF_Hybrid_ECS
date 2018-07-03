using ScriptableFramework.Events;
using ScriptableFramework.RuntimeSet;
using ScriptableFramework.Variables;
using UnityEngine;

namespace VRSF.Inputs
{
    /// <summary>
    /// Script attached to the ViveSDK Prefab.
    /// Set the GameEvent depending on the Vive Inputs.
    /// </summary>
    public class ViveInputCapture : VRInputCapture
    {
        #region PUBLIC_VARIABLES
        [Header("SteamVR Tracked Object from the two Controllers")]
        public SteamVR_TrackedObject LeftTrackedObject;
        public SteamVR_TrackedObject RightTrackedObject;
        #endregion PUBLIC_VARIABLES


        #region PRIVATE_VARIABLES

        #region Left_Controller_Variables
        private SteamVR_Controller.Device LeftController
        {
            get { return SteamVR_Controller.Input((int)LeftTrackedObject.index); }
        }

        GameEvent _tempEvent;
        #endregion Left_Controller_Variables

        #region Right_Controller_Variables
        private SteamVR_Controller.Device RightController
        {
            get { return SteamVR_Controller.Input((int)RightTrackedObject.index); }
        }

        GameEvent _rightEvent;
        #endregion Right_Controller_Variables

        #region GazeClick
        Gaze.GazeInteractionDictionaryOpenVR _GazeClick;
        SteamVR_Controller.Device _GazeController;
        #endregion

        private bool _IsSetup = false;
        #endregion PRIVATE_VARIABLES


        #region MONOBEHAVIOURS
        public override void Start()
        {
            base.Start();
            CheckReferencesVive();
        }

        // Update is called once per frame
        void Update()
        {
            if (!_IsSetup)
            {
                CheckReferencesVive();
                return;
            }

            CheckControllerInput(InputContainer.LeftClickEvents, InputContainer.LeftTouchEvents, InputContainer.LeftClickBoolean, InputContainer.LeftTouchBoolean, LeftController, InputContainer.LeftThumbPosition);
            CheckControllerInput(InputContainer.RightClickEvents, InputContainer.RightTouchEvents, InputContainer.RightClickBoolean, InputContainer.RightTouchBoolean, RightController, InputContainer.RightThumbPosition);

            if (CheckGazeInteractions && GazeParameters.UseGaze)
                CheckGazeInputs();
        }
        #endregion MONOBEHAVIOURS

        //EMPTY
        #region PUBLIC_METHODS
        #endregion PUBLIC_METHODS


        #region PRIVATE_METHODS
        /// <summary>
        /// Handle the Left Controller input and put them in the Events
        /// </summary>
        public void CheckControllerInput(VRInputsEvents clickEvents, VRInputsEvents touchEvents, VRInputsBoolean clickBools, VRInputsBoolean touchBools, SteamVR_Controller.Device controller, Vector2Variable thumbOrientation)
        {
            BoolVariable tempClick;
            BoolVariable tempTouch;

            #region TRIGGER
            tempClick = clickBools.Get("TriggerIsDown");
            tempTouch = touchBools.Get("TriggerIsTouching");

            // Check Click Events
            if (!tempClick.Value && controller.GetHairTriggerDown())
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                _tempEvent = (GameEvent)clickEvents.Get("TriggerDown");
                _tempEvent.Raise();
            }
            else if (tempClick.Value && controller.GetHairTriggerUp())
            {
                tempClick.SetValue(false);
                _tempEvent = (GameEvent)clickEvents.Get("TriggerUp");
                _tempEvent.Raise();
            }
            // Check Touch Events if user is not clicking
            else if (!tempClick.Value && !tempTouch.Value && controller.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger))
            {
                tempTouch.SetValue(true);
                _tempEvent = (GameEvent)touchEvents.Get("TriggerStartTouching");
                _tempEvent.Raise();
            }
            else if (tempTouch.Value && controller.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger))
            {
                tempTouch.SetValue(false);
                _tempEvent = (GameEvent)touchEvents.Get("TriggerStopTouching");
                _tempEvent.Raise();
            }
            #endregion TRIGGER

            #region TOUCHPAD
            thumbOrientation.SetValue(controller.GetAxis());

            tempClick = clickBools.Get("ThumbIsDown");
            tempTouch = touchBools.Get("ThumbIsTouching");

            // Check Click Events
            if (!tempClick.Value && controller.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                _tempEvent = (GameEvent)clickEvents.Get("ThumbDown");
                _tempEvent.Raise();
            }
            else if (tempClick.Value && controller.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
            {
                tempClick.SetValue(false);
                _tempEvent = (GameEvent)clickEvents.Get("ThumbUp");
                _tempEvent.Raise();
            }
            // Check Touch Events if user is not clicking
            else if (!tempClick.Value && !tempTouch.Value && controller.GetTouchDown(SteamVR_Controller.ButtonMask.Touchpad))
            {
                tempTouch.SetValue(true);
                _tempEvent = (GameEvent)touchEvents.Get("ThumbStartTouching");
                _tempEvent.Raise();
            }
            else if (tempTouch.Value && controller.GetTouchUp(SteamVR_Controller.ButtonMask.Touchpad))
            {
                tempTouch.SetValue(false);
                _tempEvent = (GameEvent)touchEvents.Get("ThumbStopTouching");
                _tempEvent.Raise();
            }
            #endregion TOUCHPAD

            #region GRIP
            tempClick = clickBools.Get("GripIsDown");

            // Check Click Events
            if (!tempClick.Value && controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
            {
                tempClick.SetValue(true);
                _tempEvent = (GameEvent)clickEvents.Get("GripDown");
                _tempEvent.Raise();
            }
            else if (tempClick.Value && controller.GetPressUp(SteamVR_Controller.ButtonMask.Grip))
            {
                tempClick.SetValue(false);
                _tempEvent = (GameEvent)clickEvents.Get("GripUp");
                _tempEvent.Raise();
            }
            #endregion GRIP

            #region MENU
            tempClick = clickBools.Get("MenuIsDown");

            // Check Click Events
            if (!tempClick.Value && controller.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
            {
                tempClick.SetValue(true);
                _tempEvent = (GameEvent)clickEvents.Get("MenuDown");
                _tempEvent.Raise();
            }
            else if (tempClick.Value && controller.GetPressUp(SteamVR_Controller.ButtonMask.ApplicationMenu))
            {
                tempClick.SetValue(false);
                _tempEvent = (GameEvent)clickEvents.Get("MenuUp");
                _tempEvent.Raise();
            }
            #endregion MENU
        }

        /// <summary>
        /// Handle the Gaze click button, in the simulator case, the mouse wheel
        /// </summary>
        void CheckGazeInputs()
        {
            // Checking Click event
            if (!InputContainer.GazeIsCliking.Value && _GazeController.GetPressDown(_GazeClick.GazeButtonMask[GazeParameters.GazeButtonOpenVR]))
            {
                InputContainer.GazeIsCliking.SetValue(true);
                InputContainer.GazeClickDown.Raise();
            }
            else if (InputContainer.GazeIsCliking.Value && _GazeController.GetPressUp(_GazeClick.GazeButtonMask[GazeParameters.GazeButtonOpenVR]))
            {
                InputContainer.GazeIsCliking.SetValue(false);
                InputContainer.GazeClickUp.Raise();
            }

            // Checking Touch event
            if (!InputContainer.GazeIsTouching.Value && _GazeController.GetTouchDown(_GazeClick.GazeButtonMask[GazeParameters.GazeButtonOpenVR]))
            {
                InputContainer.GazeIsTouching.SetValue(true);
                InputContainer.GazeStartTouching.Raise();
            }
            else if (InputContainer.GazeIsTouching.Value && _GazeController.GetTouchUp(_GazeClick.GazeButtonMask[GazeParameters.GazeButtonOpenVR]))
            {
                InputContainer.GazeIsTouching.SetValue(false);
                InputContainer.GazeStopTouching.Raise();
            }
        }

        void CheckReferencesVive()
        {
            try
            {
                _GazeClick = new Gaze.GazeInteractionDictionaryOpenVR
                {
                    LeftTrackedObject = LeftTrackedObject,
                    RightTrackedObject = RightTrackedObject
                };

                if (GazeParameters.GazeButtonOpenVR == EControllersInput.NONE)
                {
                    CheckGazeInteractions = false;
                }
                else if (GazeParameters.GazeButtonOpenVR == EControllersInput.WHEEL_BUTTON)
                {
                    CheckGazeInteractions = false;
                    Debug.LogError("VRSF : Cannot check the Gaze Click with the Wheel Button of the mouse for the Vive.");
                }
                else if (GazeParameters.GazeButtonOpenVR == EControllersInput.A_BUTTON || GazeParameters.GazeButtonOpenVR == EControllersInput.B_BUTTON ||
                         GazeParameters.GazeButtonOpenVR == EControllersInput.X_BUTTON || GazeParameters.GazeButtonOpenVR == EControllersInput.Y_BUTTON ||
                         GazeParameters.GazeButtonOpenVR == EControllersInput.RIGHT_THUMBREST || GazeParameters.GazeButtonOpenVR == EControllersInput.LEFT_THUMBREST)
                {
                    CheckGazeInteractions = false;
                    Debug.LogError("VRSF : Cannot check the Gaze Click with the " + GazeParameters.GazeButtonOpenVR + " button for the Vive.");
                }
                else if (GazeParameters.GazeButtonOpenVR.ToString().Contains("RIGHT"))
                {
                    _GazeController = RightController;
                }
                else if (GazeParameters.GazeButtonOpenVR.ToString().Contains("LEFT"))
                {
                    _GazeController = LeftController;
                }

                _IsSetup = true;
            }
            catch (System.Exception e)
            {
                Debug.Log("VRSF : VR Components not setup yet, waiting for next frame.\n" + e);
            }
        }
        #endregion PRIVATE_METHODS
    }
}
