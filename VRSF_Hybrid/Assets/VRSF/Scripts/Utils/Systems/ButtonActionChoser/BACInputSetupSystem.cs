using System;
using Unity.Entities;
using UnityEngine;
using VRSF.Controllers;
using VRSF.Gaze;
using VRSF.Inputs;
using VRSF.Utils.Components;

namespace VRSF.Utils.Systems.ButtonActionChoser
{
    /// <summary>
    /// Setup the Action button Parameter that the user has chosen and check the parameters linked to it (Like the thumb position for a Thumbstick button)
    /// </summary>
    public class BACInputSetupSystem : ComponentSystem
    {
        struct Filter
        {
            public ButtonActionChoserComponents ButtonComponents;
        }

        #region PRIVATE_VARIBALES
        private ButtonActionChoserComponents _currentComp;

        private GazeParametersVariable _gazeParameters;
        private ControllersParametersVariable _controllersParameters;
        private InputVariableContainer _inputsContainer;
        #endregion PRIVATE_VARIABLES


        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            _gazeParameters = GazeParametersVariable.Instance;
            _controllersParameters = ControllersParametersVariable.Instance;
            _inputsContainer = InputVariableContainer.Instance;

            foreach (var entity in GetEntities<Filter>())
            {
                _currentComp = entity.ButtonComponents;
                
                // We check on which hand is set the Action Button selected
                CheckButtonHand();

                // We check that all the parameters are set correctly
                if (_currentComp.ParametersAreInvalid || !CheckParameters())
                {
                    Debug.LogError("The Button Action Choser parameters for the " + this.GetType().Name + " script are invalid.\n" +
                        "Please specify valid values as displayed in the Help Boxes under your script. Setting CanBeUsed of ButtonActionChoserComponents to false.");
                    _currentComp.CanBeUsed = false;
                }
            }
        }


        protected override void OnUpdate() { }
        #endregion


        #region PRIVATES_METHODS
        /// <summary>
        /// We check which hand correspond to the Action Button that was choosen
        /// </summary>
        private void CheckButtonHand()
        {
            EControllersInput gazeClick = GetGazeClick();

            // If we use the Gaze Button but the Controllers are inactive
            if (_currentComp.UseGazeButton && !_controllersParameters.UseControllers)
            {
                _currentComp.CanBeUsed = false;
                throw new Exception("The Button Action Choser parameters for the " + this.GetType().Name + " script are invalid.\n" +
                    "If you want to use the Gaze Click, please activate the Controllers by setting the UseControllers bool in the Window VRSF/Controllers Parameters to true.\n" +
                    "Disabling the script.");
            }
            // If we use the Gaze Button but the chosen gaze button is None
            else if (_currentComp.UseGazeButton && gazeClick == EControllersInput.NONE)
            {
                _currentComp.CanBeUsed = false;
                throw new Exception("The Button Action Choser parameters for the " + this.GetType().Name + " script are invalid.\n" +
                    "Please specify a GazeButton in the Gaze Parameters Window to use the Gaze Click feature. Disabling the script.");
            }

            // if the Action Button is set to the Wheel Button (SIMULATOR SPECIFIC)
            else if (_currentComp.ActionButton == EControllersInput.WHEEL_BUTTON)
            {
                _currentComp.IsUsingWheelButton = true;
            }

            // if the Action Button is set to the A, B or Right Thumbrest option (OCULUS SPECIFIC)
            else if (_currentComp.ActionButton == EControllersInput.A_BUTTON ||
                     _currentComp.ActionButton == EControllersInput.B_BUTTON ||
                     _currentComp.ActionButton == EControllersInput.RIGHT_THUMBREST)
            {
                _currentComp.IsUsingOculusButton = true;
                _currentComp.ButtonHand = EHand.RIGHT;
            }
            // if the Action Button is set to the X, Y or Left Thumbrest option (OCULUS SPECIFIC)
            else if (_currentComp.ActionButton == EControllersInput.X_BUTTON ||
                     _currentComp.ActionButton == EControllersInput.Y_BUTTON ||
                     _currentComp.ActionButton == EControllersInput.LEFT_THUMBREST)
            {
                _currentComp.IsUsingOculusButton = true;
                _currentComp.ButtonHand = EHand.LEFT;
            }

            // if the Action Button is set to the Right Menu option (VIVE AND SIMULATOR SPECIFIC)
            else if (_currentComp.ActionButton == EControllersInput.RIGHT_MENU)
            {
                _currentComp.IsUsingViveButton = true;
                _currentComp.ButtonHand = EHand.RIGHT;
            }

            // If non of the previous solution was chosen, we just check if the button is on the right or left controller
            else if (_currentComp.ActionButton.ToString().Contains("RIGHT"))
            {
                _currentComp.ButtonHand = EHand.RIGHT;
            }
            else if (_currentComp.ActionButton.ToString().Contains("LEFT"))
            {
                _currentComp.ButtonHand = EHand.LEFT;
            }
        }


        /// <summary>
        /// Check which button to use for the Gaze depending on the SDK Loaded
        /// </summary>
        /// <returns>The EControllersInput (button) to use for the Gaze Click</returns>
        private EControllersInput GetGazeClick()
        {
            switch (VRSF_Components.DeviceLoaded)
            {
                case EDevice.OPENVR:
                    return _gazeParameters.GazeButtonOpenVR;
                case EDevice.OVR:
                    return _gazeParameters.GazeButtonOVR;
                default:
                    return _gazeParameters.GazeButtonSimulator;
            }
        }


        /// <summary>
        /// Check that all the parameters are set correctly in the Inspector.
        /// </summary>
        /// <returns>false if the parameters are incorrect</returns>
        private bool CheckParameters()
        {
            //Check if the Thumbstick are used, and if they are set correctly in that case.
            if (!CheckGivenThumbParameter())
            {
                return false;
            }

            //Check if the Action Button specified is set correctly
            if (!CheckActionButton())
            {
                return false;
            }

            if (_currentComp.UseGazeButton)
            {
                return (_currentComp.RayOrigin != EHand.NONE && _currentComp.InteractionType != EControllerInteractionType.NONE);
            }
            else
            {
                return (_currentComp.RayOrigin != EHand.NONE && _currentComp.InteractionType != EControllerInteractionType.NONE && _currentComp.ActionButton != EControllersInput.NONE);
            }
        }


        /// <summary>
        /// Called if the User is using his Thumb for this feature. Check if the Position to use on the thumbstick are set correctly in the Inspector.
        /// </summary>
        /// <returns>true if everything is set correctly</returns>
        private bool CheckGivenThumbParameter()
        {
            if (_currentComp.ActionButton == EControllersInput.LEFT_THUMBSTICK)
            {
                if (_currentComp.LeftClickThumbPosition == EThumbPosition.NONE &&
                    _currentComp.LeftTouchThumbPosition == EThumbPosition.NONE)
                {
                    Debug.LogError("VRSF : You need to assign a Thumb Position for the Left Thumbstick in this script : " + _currentComp.name);
                    return false;
                }

                _currentComp.ThumbPos = _inputsContainer.LeftThumbPosition;
            }
            else if (_currentComp.ActionButton == EControllersInput.RIGHT_THUMBSTICK)
            {
                if (_currentComp.RightClickThumbPosition == EThumbPosition.NONE &&
                    _currentComp.RightTouchThumbPosition == EThumbPosition.NONE)
                {
                    Debug.LogError("VRSF : You need to assign a Thumb Position for the Right Thumbstick in this script : " + _currentComp.name);
                    return false;
                }

                _currentComp.ThumbPos = _inputsContainer.RightThumbPosition;
            }

            _currentComp.ActionButtonIsReady = true;

            return true;
        }


        /// <summary>
        /// Check that the ActionButton chosed by the user is corresponding to the SDK that was loaded.
        /// </summary>
        /// <returns>true if the ActionButton is correctly set</returns>
        private bool CheckActionButton()
        {
            // If we are using an Oculus Touch Specific Button but the device loaded is not the Oculus
            if (_currentComp.IsUsingOculusButton && (VRSF_Components.DeviceLoaded != EDevice.OVR && VRSF_Components.DeviceLoaded != EDevice.SIMULATOR))
            {
                Debug.LogError("The Button Action Choser parameters for the " + this.GetType().Name + " script are invalid.\n" +
                    "Please specify a button that is available for the current device (" + VRSF_Components.DeviceLoaded + ") and not only for the Oculus. Disabling the script.");
                return false;
            }
            // If we are using an OpenVR Specific Button but the device loaded is not the OpenVR
            else if (_currentComp.IsUsingViveButton && (VRSF_Components.DeviceLoaded != EDevice.OPENVR && VRSF_Components.DeviceLoaded != EDevice.SIMULATOR))
            {
                Debug.LogError("The Button Action Choser parameters for the " + this.GetType().Name + " script are invalid.\n" +
                    "Please specify a button that is available for the current device (" + VRSF_Components.DeviceLoaded + ") and not only for the Vive. Disabling the script.");
                return false;
            }
            // If we are using a Simulator Specific Button but the device loaded is not the Simulator
            else if (_currentComp.IsUsingWheelButton && VRSF_Components.DeviceLoaded != EDevice.SIMULATOR)
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
        #endregion PRIVATES_METHODS
    }
}