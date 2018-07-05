using ScriptableFramework.Events;
using ScriptableFramework.Variables;
using Unity.Entities;
using UnityEngine;
using VRSF.Controllers;
using VRSF.Gaze;
using VRSF.Inputs.Components.Vive;

namespace VRSF.Inputs.Systems.Vive
{
    /// <summary>
    /// Script attached to the ViveSDK Prefab.
    /// Set the GameEvent depending on the Vive Inputs.
    /// </summary>
    public class ViveInputCaptureSystem : ComponentSystem
    {
        struct ViveFilter
        {
            public ViveInputCaptureComponent ViveInputCapture;
        }

        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private void Awake()
        {
            foreach (var entity in GetEntities<ViveFilter>())
            {
                entity.ViveInputCapture.ControllersParameters = ControllersParametersVariable.Instance;
                entity.ViveInputCapture.GazeParameters = GazeParametersVariable.Instance;
                entity.ViveInputCapture.InputContainer = InputVariableContainer.Instance;

                CheckReferencesVive(entity.ViveInputCapture);
                SetupControllersParameters(entity.ViveInputCapture);
            }
        }

        protected override void OnUpdate()
        {
            // If we doesn't use the controllers, we don't check for the inputs.
            if (ControllersParametersVariable.Instance.UseControllers)
            {
                foreach (var entity in GetEntities<ViveFilter>())
                {
                    // If the Setup with the Vive isn't finised, we look for the Vive Controllers
                    if (!entity.ViveInputCapture.ViveReferencesSetup)
                    {
                        CheckReferencesVive(entity.ViveInputCapture);
                        return;
                    }

                    // If the Setup for the controllers is not setup, we look for the Vive Controllers
                    if (!entity.ViveInputCapture.ControllersParametersSetup)
                    {
                        SetupControllersParameters(entity.ViveInputCapture);
                        return;
                    }

                    // We check the Input for the Left controller
                    CheckControllerInput(entity.ViveInputCapture.LeftParameters, entity.ViveInputCapture);

                    // We check the Input for the Right controller
                    CheckControllerInput(entity.ViveInputCapture.RightParameters, entity.ViveInputCapture);

                    // If we need to, we check the Gaze Interactions
                    if (entity.ViveInputCapture.CheckGazeInteractions)
                        CheckGazeInputs(entity.ViveInputCapture);
                }
            }
        }
        #endregion


        //EMPTY
        #region PUBLIC_METHODS
        #endregion PUBLIC_METHODS


        #region PRIVATE_METHODS
        /// <summary>
        /// Handle the Left Controller input and put them in the Events
        /// </summary>
        private void CheckControllerInput(ViveInputParameters controllerParameters, ViveInputCaptureComponent viveInputComponent)
        {
            BoolVariable tempClick;
            BoolVariable tempTouch;

            #region TRIGGER
            tempClick = controllerParameters.ClickBools.Get("TriggerIsDown");
            tempTouch = controllerParameters.TouchBools.Get("TriggerIsTouching");

            // Check Click Events
            if (!tempClick.Value && controllerParameters.Controller.GetHairTriggerDown())
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                viveInputComponent.TempEvent = (GameEvent)controllerParameters.ClickEvents.Get("TriggerDown");
                viveInputComponent.TempEvent.Raise();
            }
            else if (tempClick.Value && controllerParameters.Controller.GetHairTriggerUp())
            {
                tempClick.SetValue(false);
                viveInputComponent.TempEvent = (GameEvent)controllerParameters.ClickEvents.Get("TriggerUp");
                viveInputComponent.TempEvent.Raise();
            }
            // Check Touch Events if user is not clicking
            else if (!tempClick.Value && !tempTouch.Value && controllerParameters.Controller.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger))
            {
                tempTouch.SetValue(true);
                viveInputComponent.TempEvent = (GameEvent)controllerParameters.TouchEvents.Get("TriggerStartTouching");
                viveInputComponent.TempEvent.Raise();
            }
            else if (tempTouch.Value && controllerParameters.Controller.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger))
            {
                tempTouch.SetValue(false);
                viveInputComponent.TempEvent = (GameEvent)controllerParameters.TouchEvents.Get("TriggerStopTouching");
                viveInputComponent.TempEvent.Raise();
            }
            #endregion TRIGGER

            #region TOUCHPAD
            controllerParameters.ThumbPosition.SetValue(controllerParameters.Controller.GetAxis());

            tempClick = controllerParameters.ClickBools.Get("ThumbIsDown");
            tempTouch = controllerParameters.TouchBools.Get("ThumbIsTouching");

            // Check Click Events
            if (!tempClick.Value && controllerParameters.Controller.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                viveInputComponent.TempEvent = (GameEvent)controllerParameters.ClickEvents.Get("ThumbDown");
                viveInputComponent.TempEvent.Raise();
            }
            else if (tempClick.Value && controllerParameters.Controller.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
            {
                tempClick.SetValue(false);
                viveInputComponent.TempEvent = (GameEvent)controllerParameters.ClickEvents.Get("ThumbUp");
                viveInputComponent.TempEvent.Raise();
            }
            // Check Touch Events if user is not clicking
            else if (!tempClick.Value && !tempTouch.Value && controllerParameters.Controller.GetTouchDown(SteamVR_Controller.ButtonMask.Touchpad))
            {
                tempTouch.SetValue(true);
                viveInputComponent.TempEvent = (GameEvent)controllerParameters.TouchEvents.Get("ThumbStartTouching");
                viveInputComponent.TempEvent.Raise();
            }
            else if (tempTouch.Value && controllerParameters.Controller.GetTouchUp(SteamVR_Controller.ButtonMask.Touchpad))
            {
                tempTouch.SetValue(false);
                viveInputComponent.TempEvent = (GameEvent)controllerParameters.TouchEvents.Get("ThumbStopTouching");
                viveInputComponent.TempEvent.Raise();
            }
            #endregion TOUCHPAD

            #region GRIP
            tempClick = controllerParameters.ClickBools.Get("GripIsDown");

            // Check Click Events
            if (!tempClick.Value && controllerParameters.Controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
            {
                tempClick.SetValue(true);
                viveInputComponent.TempEvent = (GameEvent)controllerParameters.ClickEvents.Get("GripDown");
                viveInputComponent.TempEvent.Raise();
            }
            else if (tempClick.Value && controllerParameters.Controller.GetPressUp(SteamVR_Controller.ButtonMask.Grip))
            {
                tempClick.SetValue(false);
                viveInputComponent.TempEvent = (GameEvent)controllerParameters.ClickEvents.Get("GripUp");
                viveInputComponent.TempEvent.Raise();
            }
            #endregion GRIP

            #region MENU
            tempClick = controllerParameters.ClickBools.Get("MenuIsDown");

            // Check Click Events
            if (!tempClick.Value && controllerParameters.Controller.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
            {
                tempClick.SetValue(true);
                viveInputComponent.TempEvent = (GameEvent)controllerParameters.ClickEvents.Get("MenuDown");
                viveInputComponent.TempEvent.Raise();
            }
            else if (tempClick.Value && controllerParameters.Controller.GetPressUp(SteamVR_Controller.ButtonMask.ApplicationMenu))
            {
                tempClick.SetValue(false);
                viveInputComponent.TempEvent = (GameEvent)controllerParameters.ClickEvents.Get("MenuUp");
                viveInputComponent.TempEvent.Raise();
            }
            #endregion MENU
        }

        /// <summary>
        /// Handle the Gaze click button, in the simulator case, the mouse wheel
        /// </summary>
        private void CheckGazeInputs(ViveInputCaptureComponent viveInputCapture)
        {
            // Checking Click event
            if (!viveInputCapture.InputContainer.GazeIsCliking.Value && viveInputCapture.GazeController.GetPressDown(Gaze.GazeInteractionOpenVR.Dictionarry[viveInputCapture.GazeParameters.GazeButtonOpenVR]))
            {
                viveInputCapture.InputContainer.GazeIsCliking.SetValue(true);
                viveInputCapture.InputContainer.GazeClickDown.Raise();
            }
            else if (viveInputCapture.InputContainer.GazeIsCliking.Value && viveInputCapture.GazeController.GetPressUp(Gaze.GazeInteractionOpenVR.Dictionarry[viveInputCapture.GazeParameters.GazeButtonOpenVR]))
            {
                viveInputCapture.InputContainer.GazeIsCliking.SetValue(false);
                viveInputCapture.InputContainer.GazeClickUp.Raise();
            }

            // Checking Touch event
            if (!viveInputCapture.InputContainer.GazeIsTouching.Value && viveInputCapture.GazeController.GetTouchDown(Gaze.GazeInteractionOpenVR.Dictionarry[viveInputCapture.GazeParameters.GazeButtonOpenVR]))
            {
                viveInputCapture.InputContainer.GazeIsTouching.SetValue(true);
                viveInputCapture.InputContainer.GazeStartTouching.Raise();
            }
            else if (viveInputCapture.InputContainer.GazeIsTouching.Value && viveInputCapture.GazeController.GetTouchUp(Gaze.GazeInteractionOpenVR.Dictionarry[viveInputCapture.GazeParameters.GazeButtonOpenVR]))
            {
                viveInputCapture.InputContainer.GazeIsTouching.SetValue(false);
                viveInputCapture.InputContainer.GazeStopTouching.Raise();
            }
        }

        private void CheckReferencesVive(ViveInputCaptureComponent viveInputCapture)
        {
            try
            {
                if (viveInputCapture.GazeParameters.GazeButtonOpenVR == EControllersInput.NONE)
                {
                    viveInputCapture.CheckGazeInteractions = false;
                }
                else if (viveInputCapture.GazeParameters.GazeButtonOpenVR == EControllersInput.WHEEL_BUTTON)
                {
                    viveInputCapture.CheckGazeInteractions = false;
                    Debug.LogError("VRSF : Cannot check the Gaze Click with the Wheel Button of the mouse for the Vive.");
                }
                else if (viveInputCapture.GazeParameters.GazeButtonOpenVR == EControllersInput.A_BUTTON || viveInputCapture.GazeParameters.GazeButtonOpenVR == EControllersInput.B_BUTTON ||
                         viveInputCapture.GazeParameters.GazeButtonOpenVR == EControllersInput.X_BUTTON || viveInputCapture.GazeParameters.GazeButtonOpenVR == EControllersInput.Y_BUTTON ||
                         viveInputCapture.GazeParameters.GazeButtonOpenVR == EControllersInput.RIGHT_THUMBREST || viveInputCapture.GazeParameters.GazeButtonOpenVR == EControllersInput.LEFT_THUMBREST)
                {
                    viveInputCapture.CheckGazeInteractions = false;
                    Debug.LogError("VRSF : Cannot check the Gaze Click with the " + viveInputCapture.GazeParameters.GazeButtonOpenVR + " button for the Vive.");
                }
                else if (viveInputCapture.GazeParameters.GazeButtonOpenVR.ToString().Contains("RIGHT"))
                {
                    viveInputCapture.GazeController = viveInputCapture.RightController;
                }
                else if (viveInputCapture.GazeParameters.GazeButtonOpenVR.ToString().Contains("LEFT"))
                {
                    viveInputCapture.GazeController = viveInputCapture.LeftController;
                }

                viveInputCapture.ViveReferencesSetup = true;
            }
            catch (System.Exception e)
            {
                Debug.Log("VRSF : VR Components not setup yet, waiting for next frame.\n" + e);
            }
        }


        private void SetupControllersParameters(ViveInputCaptureComponent viveInputCapture)
        {
            viveInputCapture.LeftParameters = new ViveInputParameters
            {
                ClickEvents = viveInputCapture.InputContainer.LeftClickEvents,
                TouchEvents = viveInputCapture.InputContainer.LeftTouchEvents,
                ClickBools = viveInputCapture.InputContainer.LeftClickBoolean,
                TouchBools = viveInputCapture.InputContainer.LeftTouchBoolean,
                Controller = viveInputCapture.LeftController,
                ThumbPosition = viveInputCapture.InputContainer.LeftThumbPosition
            };

            viveInputCapture.RightParameters = new ViveInputParameters
            {
                ClickEvents = viveInputCapture.InputContainer.RightClickEvents,
                TouchEvents = viveInputCapture.InputContainer.RightTouchEvents,
                ClickBools = viveInputCapture.InputContainer.RightClickBoolean,
                TouchBools = viveInputCapture.InputContainer.RightTouchBoolean,
                Controller = viveInputCapture.RightController,
                ThumbPosition = viveInputCapture.InputContainer.RightThumbPosition
            };

            viveInputCapture.ControllersParametersSetup = true;
        }
        #endregion PRIVATE_METHODS
    }
}
