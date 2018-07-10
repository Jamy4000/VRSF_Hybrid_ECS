using ScriptableFramework.Events;
using ScriptableFramework.Variables;
using Unity.Entities;
using UnityEngine;
using VRSF.Controllers;
using VRSF.Inputs.Components.Vive;

namespace VRSF.Inputs.Systems.Vive
{
    /// <summary>
    /// Script attached to the ViveSDK Prefab.
    /// Set the GameEvent depending on the Vive Inputs.
    /// </summary>
    public class ViveControllersInputCaptureSystem : ComponentSystem
    {
        struct ViveFilter
        {
            public ViveControllersInputCaptureComponent ViveInputCapture;
        }

        #region PRIVATE_VARIABLES
        // VRSF Parameters references
        private InputVariableContainer _inputContainer;
        #endregion PRIVATE_VARIABLES


        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            
            _inputContainer = InputVariableContainer.Instance;

            foreach (var entity in GetEntities<ViveFilter>())
            {
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
                }
            }
        }
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Handle the Left Controller input and put them in the Events
        /// </summary>
        private void CheckControllerInput(ViveInputParameters controllerParameters, ViveControllersInputCaptureComponent viveInputComponent)
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
        /// Setup the two controllers parameters to use in the CheckControllersInput method.
        /// </summary>
        /// <param name="viveInputCapture">The ViveInputCaptureComponent on the CameraRig Entity</param>
        private void SetupControllersParameters(ViveControllersInputCaptureComponent viveInputCapture)
        {
            viveInputCapture.LeftParameters = new ViveInputParameters
            {
                ClickEvents = _inputContainer.LeftClickEvents,
                TouchEvents = _inputContainer.LeftTouchEvents,
                ClickBools = _inputContainer.LeftClickBoolean,
                TouchBools = _inputContainer.LeftTouchBoolean,
                Controller = viveInputCapture.LeftController,
                ThumbPosition = _inputContainer.LeftThumbPosition
            };

            viveInputCapture.RightParameters = new ViveInputParameters
            {
                ClickEvents = _inputContainer.RightClickEvents,
                TouchEvents = _inputContainer.RightTouchEvents,
                ClickBools = _inputContainer.RightClickBoolean,
                TouchBools = _inputContainer.RightTouchBoolean,
                Controller = viveInputCapture.RightController,
                ThumbPosition = _inputContainer.RightThumbPosition
            };

            viveInputCapture.ControllersParametersSetup = true;
        }
        #endregion PRIVATE_METHODS
    }
}
