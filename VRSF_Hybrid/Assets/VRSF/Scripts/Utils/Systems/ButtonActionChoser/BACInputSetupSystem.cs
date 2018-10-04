using System;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRSF.Controllers;
using VRSF.Gaze;
using VRSF.Inputs;
using VRSF.Utils.Components;
using VRSF.Utils.Components.ButtonActionChoser;

namespace VRSF.Utils.Systems.ButtonActionChoser
{
    /// <summary>
    /// Setup the Action button Parameter that the user has chosen and check the parameters linked to it (Like the thumb position for a Thumbstick button)
    /// </summary>
    public class BACInputSetupSystem : ComponentSystem
    {
        struct Filter
        {
            public BACGeneralVariablesComponents ButtonComponents;
        }

        #region PRIVATE_VARIBALES
        private Filter _currentEntitiy;

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

            SceneManager.sceneUnloaded += OnSceneUnloaded;

            foreach (var entity in GetEntities<Filter>())
            {
                if (entity.ButtonComponents.GetComponent<SDKChoserComponent>() == null)
                {
                    _currentEntitiy = entity;

                    // We check on which hand is set the Action Button selected
                    CheckButtonHand();

                    // We check that all the parameters are set correctly
                    if (entity.ButtonComponents.ParametersAreInvalid || !CheckParameters())
                    {
                        Debug.LogError("The Button Action Choser parameters for the ButtonActionChoserComponents on the " + entity.ButtonComponents.transform.name + " object are invalid.\n" +
                            "Please specify valid values as displayed in the Help Boxes under your script. Setting CanBeUsed of ButtonActionChoserComponents to false.");
                        entity.ButtonComponents.CanBeUsed = false;
                    }
                }
            }
        }


        protected override void OnUpdate()
        {
            bool systemStillRunning = false;

            foreach (var entity in GetEntities<Filter>())
            {
                var sdkChoser = entity.ButtonComponents.GetComponent<SDKChoserComponent>();

                if (sdkChoser != null)
                {
                    if (!sdkChoser.IsSetup)
                    {
                        systemStillRunning = true;
                    }
                    else if (sdkChoser.IsSetup && entity.ButtonComponents.CorrectSDK)
                    {
                        _currentEntitiy = entity;

                        // We check on which hand is set the Action Button selected
                        CheckButtonHand();

                        // We check that all the parameters are set correctly
                        if (entity.ButtonComponents.ParametersAreInvalid || !CheckParameters())
                        {
                            Debug.LogError("The Button Action Choser parameters for the ButtonActionChoserComponents on the " + entity.ButtonComponents.transform.name + " object are invalid.\n" +
                                "Please specify valid values as displayed in the Help Boxes under your script. Setting CanBeUsed of ButtonActionChoserComponents to false.");
                            entity.ButtonComponents.CanBeUsed = false;
                        }
                    }
                    else
                    {
                        entity.ButtonComponents.ActionButtonIsReady = true;
                    }
                }
            }

            this.Enabled = systemStillRunning;
        }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }
        #endregion


        #region PRIVATES_METHODS
        /// <summary>
        /// We check which hand correspond to the Action Button that was choosen
        /// </summary>
        private void CheckButtonHand()
        {
            EControllersButton gazeClick = GetGazeClick();

            // If we use the Gaze Button but the Controllers are inactive
            if (_currentEntitiy.ButtonComponents.UseGazeButton && !_controllersParameters.UseControllers)
            {
                _currentEntitiy.ButtonComponents.CanBeUsed = false;
                throw new Exception("The Button Action Choser parameters for the " + this.GetType().Name + " script are invalid.\n" +
                    "If you want to use the Gaze Click, please activate the Controllers by setting the UseControllers bool in the Window VRSF/Controllers Parameters to true.\n" +
                    "Disabling the script.");
            }
            // If we use the Gaze Button but the chosen gaze button is None
            else if (_currentEntitiy.ButtonComponents.UseGazeButton && gazeClick == EControllersButton.NONE)
            {
                _currentEntitiy.ButtonComponents.CanBeUsed = false;
                throw new Exception("The Button Action Choser parameters for the " + this.GetType().Name + " script are invalid.\n" +
                    "Please specify a GazeButton in the Gaze Parameters Window to use the Gaze Click feature. Disabling the script.");
            }

            // if the Action Button is set to the Wheel Button (SIMULATOR SPECIFIC)
            else if (_currentEntitiy.ButtonComponents.ActionButton == EControllersButton.WHEEL_BUTTON)
            {
                _currentEntitiy.ButtonComponents.IsUsingWheelButton = true;
            }

            // if the Action Button is set to the A, B or Right Thumbrest option (OCULUS SPECIFIC)
            else if (_currentEntitiy.ButtonComponents.ActionButton == EControllersButton.A_BUTTON ||
                     _currentEntitiy.ButtonComponents.ActionButton == EControllersButton.B_BUTTON ||
                     (_currentEntitiy.ButtonComponents.ActionButton == EControllersButton.THUMBREST && 
                      _currentEntitiy.ButtonComponents.ButtonHand == EHand.RIGHT))
            {
                _currentEntitiy.ButtonComponents.IsUsingOculusButton = true;
                _currentEntitiy.ButtonComponents.ButtonHand = EHand.RIGHT;
            }
            // if the Action Button is set to the X, Y or Left Thumbrest option (OCULUS SPECIFIC)
            else if (_currentEntitiy.ButtonComponents.ActionButton == EControllersButton.X_BUTTON ||
                     _currentEntitiy.ButtonComponents.ActionButton == EControllersButton.Y_BUTTON ||
                     (_currentEntitiy.ButtonComponents.ActionButton == EControllersButton.THUMBREST &&
                      _currentEntitiy.ButtonComponents.ButtonHand == EHand.LEFT))
            {
                _currentEntitiy.ButtonComponents.IsUsingOculusButton = true;
                _currentEntitiy.ButtonComponents.ButtonHand = EHand.LEFT;
            }

            // if the Action Button is set to the Right Menu option (VIVE AND SIMULATOR SPECIFIC)
            else if (_currentEntitiy.ButtonComponents.ActionButton == EControllersButton.MENU &&
                     _currentEntitiy.ButtonComponents.ButtonHand == EHand.RIGHT)
            {
                _currentEntitiy.ButtonComponents.IsUsingViveButton = true;
                _currentEntitiy.ButtonComponents.ButtonHand = EHand.RIGHT;
            }

            // if the Action Button is set to the Right Menu option (VIVE AND SIMULATOR SPECIFIC)
            else if (_currentEntitiy.ButtonComponents.ActionButton == EControllersButton.BACK_BUTTON)
            {
                _currentEntitiy.ButtonComponents.IsUsingPortableOVRButton = true;
                _currentEntitiy.ButtonComponents.ButtonHand = EHand.RIGHT;
            }

            // If non of the previous solution was chosen, we just check if the button is on the right or left controller
            else if (_currentEntitiy.ButtonComponents.ActionButton.ToString().Contains("RIGHT"))
            {
                _currentEntitiy.ButtonComponents.ButtonHand = EHand.RIGHT;
            }
            else if (_currentEntitiy.ButtonComponents.ActionButton.ToString().Contains("LEFT"))
            {
                _currentEntitiy.ButtonComponents.ButtonHand = EHand.LEFT;
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
                case EDevice.OPENVR:
                    return _gazeParameters.GazeButtonOpenVR;
                case EDevice.OCULUS_RIFT:
                    return _gazeParameters.GazeButtonRift;
                case EDevice.PORTABLE_OVR:
                    return _gazeParameters.GazeButtonPortableOVR;
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

            if (_currentEntitiy.ButtonComponents.UseGazeButton)
            {
                return (_currentEntitiy.ButtonComponents.InteractionType != EControllerInteractionType.NONE);
            }
            else
            {
                return (_currentEntitiy.ButtonComponents.InteractionType != EControllerInteractionType.NONE && _currentEntitiy.ButtonComponents.ActionButton != EControllersButton.NONE);
            }
        }


        /// <summary>
        /// Called if the User is using his Thumb for this feature. Check if the Position to use on the thumbstick are set correctly in the Inspector.
        /// </summary>
        /// <returns>true if everything is set correctly</returns>
        private bool CheckGivenThumbParameter()
        {
            if (_currentEntitiy.ButtonComponents.ActionButton == EControllersButton.THUMBSTICK &&
                _currentEntitiy.ButtonComponents.ButtonHand == EHand.LEFT)
            {
                if (_currentEntitiy.ButtonComponents.LeftClickThumbPosition == EThumbPosition.NONE &&
                    _currentEntitiy.ButtonComponents.LeftTouchThumbPosition == EThumbPosition.NONE)
                {
                    Debug.LogError("VRSF : You need to assign a Thumb Position for the Left Thumbstick in this script : " + _currentEntitiy.ButtonComponents.name);
                    return false;
                }

                _currentEntitiy.ButtonComponents.ThumbPos = _inputsContainer.LeftThumbPosition;
            }
            else if (_currentEntitiy.ButtonComponents.ActionButton == EControllersButton.THUMBSTICK &&
                _currentEntitiy.ButtonComponents.ButtonHand == EHand.RIGHT)
            {
                if (_currentEntitiy.ButtonComponents.RightClickThumbPosition == EThumbPosition.NONE &&
                    _currentEntitiy.ButtonComponents.RightTouchThumbPosition == EThumbPosition.NONE)
                {
                    Debug.LogError("VRSF : You need to assign a Thumb Position for the Right Thumbstick in this script : " + _currentEntitiy.ButtonComponents.name);
                    return false;
                }

                _currentEntitiy.ButtonComponents.ThumbPos = _inputsContainer.RightThumbPosition;
            }

            _currentEntitiy.ButtonComponents.ActionButtonIsReady = true;

            return true;
        }


        /// <summary>
        /// Check that the ActionButton chosed by the user is corresponding to the SDK that was loaded.
        /// </summary>
        /// <returns>true if the ActionButton is correctly set</returns>
        private bool CheckActionButton()
        {
            // If we are using an Oculus Touch Specific Button but the device loaded is not the Oculus
            if (_currentEntitiy.ButtonComponents.IsUsingOculusButton && VRSF_Components.DeviceLoaded == EDevice.OPENVR)
            {
                Debug.LogError("The Button Action Choser parameters for the " + this.GetType().Name + " script are invalid.\n" +
                    "Please specify a button that is available for the current device (" + VRSF_Components.DeviceLoaded + ") and not only for the Oculus. Disabling the script.");
                return false;
            }
            // If we are using an OpenVR Specific Button but the device loaded is not the OpenVR
            else if (_currentEntitiy.ButtonComponents.IsUsingViveButton && VRSF_Components.DeviceLoaded == EDevice.OCULUS_RIFT)
            {
                Debug.LogError("The Button Action Choser parameters for the " + this.GetType().Name + " script are invalid.\n" +
                    "Please specify a button that is available for the current device (" + VRSF_Components.DeviceLoaded + ") and not only for the Vive. Disabling the script.");
                return false;
            }
            // If we are using a Simulator Specific Button but the device loaded is not the Simulator
            else if (_currentEntitiy.ButtonComponents.IsUsingWheelButton && VRSF_Components.DeviceLoaded != EDevice.SIMULATOR)
            {
                Debug.LogError("The Button Action Choser parameters for the " + this.GetType().Name + " script are invalid.\n" +
                    "Please specify a button that is available for the current device (" + VRSF_Components.DeviceLoaded + ") and not only for the Simulator. Disabling the script.");
                return false;
            }
            // If we are using a Simulator Specific Button but the device loaded is not the Simulator
            else if (_currentEntitiy.ButtonComponents.IsUsingPortableOVRButton && VRSF_Components.DeviceLoaded != EDevice.PORTABLE_OVR)
            {
                Debug.LogError("The Button Action Choser parameters for the " + this.GetType().Name + " script are invalid.\n" +
                    "Please specify a button that is available for the current device (" + VRSF_Components.DeviceLoaded + ") and not only for the GearVR or Oculus Go. Disabling the script.");
                return false;
            }
            else
            {
                return true;
            }
        }


        /// <summary>
        /// Reactivate the System when switching to another Scene.
        /// </summary>
        /// <param name="oldScene">The previous scene before switching</param>
        private void OnSceneUnloaded(Scene oldScene)
        {
            this.Enabled = true;
        }
        #endregion PRIVATES_METHODS
    }
}