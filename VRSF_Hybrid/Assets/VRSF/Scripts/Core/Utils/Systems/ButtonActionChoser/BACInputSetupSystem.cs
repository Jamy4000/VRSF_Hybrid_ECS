using System;
using Unity.Entities;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.Inputs;
using VRSF.Core.SetupVR;
using VRSF.Core.Gaze;
using VRSF.Core.Events;

namespace VRSF.Core.Utils.ButtonActionChoser
{
    /// <summary>
    /// Setup the Action button Parameter that the user has chosen and check the parameters linked to it (Like the thumb position for a Thumbstick button)
    /// </summary>
    public class BACInputSetupSystem : ComponentSystem
    {
        struct Filter
        {
            public BACGeneralComponent BACGeneralComp;
            public BACCalculationsComponent BACCalculationsComp;
        }

        #region PRIVATE_VARIBALES
        private GazeParametersVariable _gazeParameters;
        private ControllersParametersVariable _controllersParameters;
        private InputVariableContainer _inputsContainer;
        #endregion PRIVATE_VARIABLES


        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnCreateManager()
        {
            base.OnCreateManager();

            _gazeParameters = GazeParametersVariable.Instance;
            _controllersParameters = ControllersParametersVariable.Instance;
            _inputsContainer = InputVariableContainer.Instance;
            
            SDKChoserIsSetup.Listeners += StartBACsSetup;
        }

        protected override void OnUpdate() {}

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
            SDKChoserIsSetup.Listeners -= StartBACsSetup;
        }
        #endregion


        #region PRIVATES_METHODS
        /// <summary>
        /// We check which hand correspond to the Action Button that was choosen
        /// </summary>
        private void CheckButtonHand(Filter entity)
        {
            EControllersButton gazeClick = GetGazeClick();

            // If we use the Gaze Button but the Controllers are inactive
            if (entity.BACGeneralComp.UseGazeButton && !_controllersParameters.UseControllers)
            {
                entity.BACCalculationsComp.CanBeUsed = false;
                throw new Exception("The Button Action Choser parameters for the " + this.GetType().Name + " script are invalid.\n" +
                    "If you want to use the Gaze Click, please activate the Controllers by setting the UseControllers bool in the Window VRSF/Controllers Parameters to true.\n" +
                    "Disabling the script.");
            }
            // If we use the Gaze Button but the chosen gaze button is None
            else if (entity.BACGeneralComp.UseGazeButton && gazeClick == EControllersButton.NONE)
            {
                entity.BACCalculationsComp.CanBeUsed = false;
                throw new Exception("The Button Action Choser parameters for the " + this.GetType().Name + " script are invalid.\n" +
                    "Please specify a GazeButton in the Gaze Parameters Window to use the Gaze Click feature. Disabling the script.");
            }

            // if the Action Button is set to the Wheel Button (SIMULATOR SPECIFIC)
            else if (entity.BACGeneralComp.ActionButton == EControllersButton.WHEEL_BUTTON)
            {
                entity.BACCalculationsComp.IsUsingWheelButton = true;
            }

            // if the Action Button is set to the A, B or Right Thumbrest option (OCULUS SPECIFIC)
            else if (entity.BACGeneralComp.ActionButton == EControllersButton.A_BUTTON ||
                     entity.BACGeneralComp.ActionButton == EControllersButton.B_BUTTON ||
                     (entity.BACGeneralComp.ActionButton == EControllersButton.THUMBREST && 
                      entity.BACGeneralComp.ButtonHand == EHand.RIGHT))
            {
                entity.BACCalculationsComp.IsUsingOculusButton = true;
                entity.BACGeneralComp.ButtonHand = EHand.RIGHT;
            }
            // if the Action Button is set to the X, Y or Left Thumbrest option (OCULUS SPECIFIC)
            else if (entity.BACGeneralComp.ActionButton == EControllersButton.X_BUTTON ||
                     entity.BACGeneralComp.ActionButton == EControllersButton.Y_BUTTON ||
                     (entity.BACGeneralComp.ActionButton == EControllersButton.THUMBREST &&
                      entity.BACGeneralComp.ButtonHand == EHand.LEFT))
            {
                entity.BACCalculationsComp.IsUsingOculusButton = true;
                entity.BACGeneralComp.ButtonHand = EHand.LEFT;
            }

            // if the Action Button is set to the Right Menu option (VIVE AND SIMULATOR SPECIFIC)
            else if (entity.BACGeneralComp.ActionButton == EControllersButton.MENU &&
                     entity.BACGeneralComp.ButtonHand == EHand.RIGHT)
            {
                entity.BACCalculationsComp.IsUsingViveButton = true;
                entity.BACGeneralComp.ButtonHand = EHand.RIGHT;
            }

            // If non of the previous solution was chosen, we just check if the button is on the right or left controller
            else if (entity.BACGeneralComp.ActionButton.ToString().Contains("RIGHT"))
            {
                entity.BACGeneralComp.ButtonHand = EHand.RIGHT;
            }
            else if (entity.BACGeneralComp.ActionButton.ToString().Contains("LEFT"))
            {
                entity.BACGeneralComp.ButtonHand = EHand.LEFT;
            }
        }


        /// <summary>
        /// Check which button to use for the Gaze depending on the SDK Loaded
        /// </summary>
        /// <returns>The EControllersInput (button) to use for the Gaze Click</returns>
        private EControllersButton GetGazeClick()
        {
            switch (VRSF_Components.DeviceLoaded)
            {
                case EDevice.HTC_VIVE:
                    return _gazeParameters.GazeButtonVive;
                case EDevice.OCULUS_RIFT:
                    return _gazeParameters.GazeButtonRift;
                default:
                    return _gazeParameters.GazeButtonSimulator;
            }
        }


        /// <summary>
        /// Check that all the parameters are set correctly in the Inspector.
        /// </summary>
        /// <returns>false if the parameters are incorrect</returns>
        private bool CheckParameters(Filter entity)
        {
            //Check if the Thumbstick are used, and if they are set correctly in that case.
            if (!CheckGivenThumbParameter(entity))
            {
                return false;
            }

            //Check if the Action Button specified is set correctly
            if (!CheckActionButton(entity))
            {
                return false;
            }

            if (entity.BACGeneralComp.UseGazeButton)
            {
                return (entity.BACGeneralComp.InteractionType != EControllerInteractionType.NONE);
            }
            else
            {
                return (entity.BACGeneralComp.InteractionType != EControllerInteractionType.NONE && entity.BACGeneralComp.ActionButton != EControllersButton.NONE);
            }
        }


        /// <summary>
        /// Called if the User is using his Thumb for this feature. Check if the Position to use on the thumbstick are set correctly in the Inspector.
        /// </summary>
        /// <returns>true if everything is set correctly</returns>
        private bool CheckGivenThumbParameter(Filter entity)
        {
            if (entity.BACGeneralComp.ActionButton == EControllersButton.THUMBSTICK &&
                entity.BACGeneralComp.ButtonHand == EHand.LEFT)
            {
                if (entity.BACGeneralComp.LeftClickThumbPosition == EThumbPosition.NONE &&
                    entity.BACGeneralComp.LeftTouchThumbPosition == EThumbPosition.NONE)
                {
                    Debug.LogError("<b>[VRSF] :</b> You need to assign a Thumb Position for the Left Thumbstick in this script : " + entity.BACGeneralComp.name);
                    return false;
                }

                entity.BACCalculationsComp.ThumbPos = _inputsContainer.LeftThumbPosition;
            }
            else if (entity.BACGeneralComp.ActionButton == EControllersButton.THUMBSTICK &&
                entity.BACGeneralComp.ButtonHand == EHand.RIGHT)
            {
                if (entity.BACGeneralComp.RightClickThumbPosition == EThumbPosition.NONE &&
                    entity.BACGeneralComp.RightTouchThumbPosition == EThumbPosition.NONE)
                {
                    Debug.LogError("<b>[VRSF] :</b> You need to assign a Thumb Position for the Right Thumbstick in this script : " + entity.BACGeneralComp.name);
                    return false;
                }

                entity.BACCalculationsComp.ThumbPos = _inputsContainer.RightThumbPosition;
            }
            
            return true;
        }


        /// <summary>
        /// Check that the ActionButton chosed by the user is corresponding to the SDK that was loaded.
        /// </summary>
        /// <returns>true if the ActionButton is correctly set</returns>
        private bool CheckActionButton(Filter entity)
        {
            // If we are using an Oculus Touch Specific Button but the device loaded is not the Oculus
            if (entity.BACCalculationsComp.IsUsingOculusButton && VRSF_Components.DeviceLoaded == EDevice.HTC_VIVE)
            {
                Debug.LogError("The Button Action Choser parameters for the " + this.GetType().Name + " script are invalid.\n" +
                    "Please specify a button that is available for the current device (" + VRSF_Components.DeviceLoaded + ") and not only for the Oculus. Disabling the script.");
                return false;
            }
            // If we are using an OpenVR Specific Button but the device loaded is not the OpenVR
            else if (entity.BACCalculationsComp.IsUsingViveButton && VRSF_Components.DeviceLoaded == EDevice.OCULUS_RIFT)
            {
                Debug.LogError("The Button Action Choser parameters for the " + this.GetType().Name + " script are invalid.\n" +
                    "Please specify a button that is available for the current device (" + VRSF_Components.DeviceLoaded + ") and not only for the Vive. Disabling the script.");
                return false;
            }
            // If we are using a Simulator Specific Button but the device loaded is not the Simulator
            else if (entity.BACCalculationsComp.IsUsingWheelButton && VRSF_Components.DeviceLoaded != EDevice.SIMULATOR)
            {
                Debug.LogError("The Button Action Choser parameters for the " + this.GetType().Name + " script are invalid.\n" +
                    "Please specify a button that is available for the current device (" + VRSF_Components.DeviceLoaded + ") and not only for the Simulator. Disabling the script.");
                return false;
            }
            else
            {
                return true;
            }
        }

        private void StartBACsSetup(SDKChoserIsSetup info)
        {
            foreach (var entity in GetEntities<Filter>())
            {
                // We check on which hand is set the Action Button selected
                CheckButtonHand(entity);

                // We check that all the parameters are set correctly
                if (entity.BACCalculationsComp.ParametersAreInvalid || !CheckParameters(entity))
                {
                    Debug.LogError("The Button Action Choser parameters for the ButtonActionChoserComponents on the " + entity.BACGeneralComp.transform.name + " object are invalid.\n" +
                        "Please specify valid values as displayed in the Help Boxes under your script. Setting CanBeUsed of ButtonActionChoserComponents to false.");
                    entity.BACCalculationsComp.CanBeUsed = false;
                }
                else
                {
                    entity.BACCalculationsComp.ActionButtonIsReady = true;
                }
            }
        }
        #endregion PRIVATES_METHODS
    }
}