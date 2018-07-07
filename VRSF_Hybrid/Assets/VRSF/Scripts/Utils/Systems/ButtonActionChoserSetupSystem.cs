using ScriptableFramework.Events;
using System;
using System.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Events;
using VRSF.Controllers;
using VRSF.Gaze;
using VRSF.Inputs;
using VRSF.Interactions;
using VRSF.Utils.Components;
using static VRSF.Utils.ButtonActionChoser;

namespace VRSF.Utils.Systems.ButtonActionChoser
{
    public class ButtonActionChoserSetupSystem : ComponentSystem
    {
        struct Filter
        {
            public ButtonActionChoserComponents ButtonComponents;
        }

        #region PRIVATE_VARIBALES
        private ButtonActionChoserComponents _currentComp;
        #endregion PRIVATE_VARIABLES


        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnStartRunning()
        {
            foreach (var entity in GetEntities<Filter>())
            {
                _currentComp = entity.ButtonComponents;

                // We check if the current loaded sdk is used for this feature
                if (!CheckUseSDKToggles())
                {
                    _currentComp.CanBeUsed = false;
                    return;
                }

                // We init the Scriptable Objects References
                InitSOs();

                // We check which hit to use for this feature with the RayOrigin
                CheckRayAndHit();

                // We check on which hand is set the Action Button selected
                CheckButtonHand();

                // We check that all the parameters are set correctly
                if (_currentComp.ParametersAreInvalid || !CheckParameters())
                {
                    Debug.LogError("The Button Action Choser parameters for the " + this.GetType().Name + " script are invalid.\n" +
                        "Please specify valid values as displayed in the Help Boxes under your script. Setting CanBeUsed of ButtonActionChoserComponents to false.");
                    _currentComp.CanBeUsed = false;
                    return;
                }

                // We init the Scriptable Object references and how they work
                if (!InitSOsReferences())
                {
                    Debug.LogError("VRSF : An error has occured while initializing the Scriptable Objects reference in the " + this.GetType().Name + " script.\n" +
                        "If the error persist after reloading the Editor, please open an issue on Github. Setting CanBeUsed of ButtonActionChoserComponents to false.");
                    _currentComp.CanBeUsed = false;
                    return;
                }
            }
            
        }

        protected override void OnUpdate()
        {
            throw new System.NotImplementedException();
        }
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Check if at least one of the three toggles for the SDK to Use is set at true, and if the current loaded Device is listed in those bool
        /// </summary>
        /// <returns>true if the current loaded SDK is selected in the inspector</returns>
        private bool CheckUseSDKToggles()
        {
            if (!_currentComp.UseOpenVR && !_currentComp.UseOVR && !_currentComp.UseSimulator)
            {
                Debug.LogError("VRSF : You need to chose at least one SDK to use the " + GetType().Name + " script. Setting CanBeUsed of ButtonActionChoserComponents to false.");
                _currentComp.CanBeUsed = false;
                return false;
            }

            switch (VRSF_Components.DeviceLoaded)
            {
                case EDevice.OPENVR:
                    return _currentComp.UseOpenVR;

                case EDevice.OVR:
                    return _currentComp.UseOVR;

                case EDevice.SIMULATOR:
                    return _currentComp.UseSimulator;
            }

            return true;
        }


        /// <summary>
        /// Check which RaycastHitVariable is used depending on the RayOrigin specified
        /// </summary>
        private void CheckRayAndHit()
        {
            switch (_currentComp.RayOrigin)
            {
                case (EHand.LEFT):
                    _currentComp.HitVar = _currentComp.InteractionsContainer.LeftHit;
                    _currentComp.RayVar = _currentComp.InteractionsContainer.LeftRay;
                    break;

                case (EHand.RIGHT):
                    _currentComp.HitVar = _currentComp.InteractionsContainer.RightHit;
                    _currentComp.RayVar = _currentComp.InteractionsContainer.RightRay;
                    break;

                case (EHand.GAZE):
                    _currentComp.HitVar = _currentComp.InteractionsContainer.GazeHit;
                    _currentComp.RayVar = _currentComp.InteractionsContainer.GazeRay;
                    break;

                default:
                    Debug.LogError("VRSF : You need to specify the RayOrigin for the " + GetType().Name + " script. Setting CanBeUsed of ButtonActionChoserComponents to false.");
                    _currentComp.CanBeUsed = false;
                    break;
            }
        }


        /// <summary>
        /// Instantiate and set the GameEventListeners and BoolVariable
        /// </summary>
        private bool InitSOsReferences()
        {
            // We set the GameEvents and BoolVariables depending on the _currentComp.InteractionType and the Hand of the ActionButton
            if (!SetupSOReferences())
            {
                return false;
            }

            //We create the container for the GameEventListeners
            _currentComp.GameEventsContainer = CreateGEContainer();

            // We add the GameEventListeners to the GameEventContainer, set the events and response for the listeners, 
            // and register the listeners in the GameEvent
            // Placed in coroutine as sometimes we need to wait for another ButtonActionChoser script to create the GELContainer 
            _currentComp.StartCoroutine(CreateGELInContainer());

            return true;
        }


        /// <summary>
        /// Set the GameEvents and GameEventListeners to null
        /// </summary>
        private void InitSOs()
        {
            _currentComp.ControllersParameters = ControllersParametersVariable.Instance;
            _currentComp.GazeParameters = GazeParametersVariable.Instance;
            _currentComp.InputsContainer = InputVariableContainer.Instance;
            _currentComp.InteractionsContainer = InteractionVariableContainer.Instance;

            _currentComp.GeDown = null;
            _currentComp.GeUp = null;
            _currentComp.GeTouched = null;
            _currentComp.GeUntouched = null;

            _currentComp.GelDown = null;
            _currentComp.GelUp = null;
            _currentComp.GelTouched = null;
            _currentComp.GelUntouched = null;
        }


        /// <summary>
        /// We check which hand correspond to the Action Button that was choosen
        /// </summary>
        private void CheckButtonHand()
        {
            EControllersInput gazeClick = GetGazeClick();

            // If we use the Gaze Button but the Controllers are inactive
            if (_currentComp.UseGazeButton && !_currentComp.ControllersParameters.UseControllers)
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
                    return _currentComp.GazeParameters.GazeButtonOpenVR;
                case EDevice.OVR:
                    return _currentComp.GazeParameters.GazeButtonOVR;
                default:
                    return _currentComp.GazeParameters.GazeButtonSimulator;
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

                _currentComp.ThumbPos = _currentComp.InputsContainer.LeftThumbPosition;
            }
            else if (_currentComp.ActionButton == EControllersInput.RIGHT_THUMBSTICK)
            {
                if (_currentComp.RightClickThumbPosition == EThumbPosition.NONE &&
                    _currentComp.RightTouchThumbPosition == EThumbPosition.NONE)
                {
                    Debug.LogError("VRSF : You need to assign a Thumb Position for the Right Thumbstick in this script : " + _currentComp.name);
                    return false;
                }

                _currentComp.ThumbPos = _currentComp.InputsContainer.RightThumbPosition;
            }

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


        /// <summary>
        /// Depending on the Button used for the feature and the Interaction Type, setup the BoolVariable and GameEvents accordingly
        /// </summary>
        /// <returns>true if everything was setup correctly</returns>
        private bool SetupSOReferences()
        {
            // If we use the Gaze Button specified in the Gaze Parameters Window
            if (_currentComp.UseGazeButton)
            {
                return SetupGazeInteraction();
            }
            // If we use the Mouse Wheel Button
            else if (_currentComp.IsUsingWheelButton)
            {
                if (_currentComp.InteractionType == EControllerInteractionType.NONE || _currentComp.InteractionType == EControllerInteractionType.TOUCH)
                {
                    return false;
                }
                _currentComp.GeDown = _currentComp.InputsContainer.WheelClickDown;
                _currentComp.GeUp = _currentComp.InputsContainer.WheelClickUp;
                _currentComp.IsClicking = _currentComp.InputsContainer.WheelIsClicking;
                return true;
            }
            else
            {
                return SetupNormalButton();
            }
        }


        /// <summary>
        /// Check the Interaction Type specified and set it to corresponds to the Gaze BoolVariable
        /// </summary>
        /// <returns>true if everything was setup correctly</returns>
        private bool SetupGazeInteraction()
        {
            if (_currentComp.InteractionType == EControllerInteractionType.NONE)
            {
                return false;
            }
            if ((_currentComp.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                _currentComp.GeDown = _currentComp.InputsContainer.GazeClickDown;
                _currentComp.GeUp = _currentComp.InputsContainer.GazeClickUp;
                _currentComp.IsClicking = _currentComp.InputsContainer.GazeIsCliking;
            }
            if ((_currentComp.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                _currentComp.GeTouched = _currentComp.InputsContainer.GazeStartTouching;
                _currentComp.GeUntouched = _currentComp.InputsContainer.GazeStopTouching;
                _currentComp.IsTouching = _currentComp.InputsContainer.GazeIsTouching;
            }
            return true;
        }


        /// <summary>
        /// Setup the _currentComp._isClicking and _isTouching BoolVariable depending on the _currentComp.InteractionType and the _currentComp._buttonHand variable.
        /// </summary>
        /// <returns>true if everything was setup correctly</returns>
        private bool SetupNormalButton()
        {
            // If the Interaction Type is set at NONE
            if (_currentComp.InteractionType == EControllerInteractionType.NONE)
            {
                Debug.LogError("Please chose a valid Interaction type in the Inspector. Disabling " + _currentComp.name + " script.");
                _currentComp.CanBeUsed = false;
                return false;
            }

            // If the Interaction Type contains at least CLICK
            if ((_currentComp.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                switch (_currentComp.ButtonHand)
                {
                    case EHand.RIGHT:
                        _currentComp.GeDown = _currentComp.InputsContainer.RightClickEvents.Items[ControllerInputToSO.GetDownGameEventFor(_currentComp.ActionButton)] as GameEvent;
                        _currentComp.GeUp = _currentComp.InputsContainer.RightClickEvents.Items[ControllerInputToSO.GetUpGameEventFor(_currentComp.ActionButton)] as GameEvent;
                        _currentComp.IsClicking = _currentComp.InputsContainer.RightClickBoolean.Items[ControllerInputToSO.GetClickVariableFor(_currentComp.ActionButton)];
                        break;
                    case EHand.LEFT:
                        _currentComp.GeDown = _currentComp.InputsContainer.LeftClickEvents.Items[ControllerInputToSO.GetDownGameEventFor(_currentComp.ActionButton)] as GameEvent;
                        _currentComp.GeUp = _currentComp.InputsContainer.LeftClickEvents.Items[ControllerInputToSO.GetUpGameEventFor(_currentComp.ActionButton)] as GameEvent;
                        _currentComp.IsClicking = _currentComp.InputsContainer.LeftClickBoolean.Items[ControllerInputToSO.GetClickVariableFor(_currentComp.ActionButton)];
                        break;
                    default:
                        return false;
                }
            }

            // If the Interaction Type contains at least TOUCH
            if ((_currentComp.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                // Handle Touch events
                switch (_currentComp.ButtonHand)
                {
                    case EHand.RIGHT:
                        _currentComp.GeTouched = _currentComp.InputsContainer.RightTouchEvents.Items[ControllerInputToSO.GetTouchGameEventFor(_currentComp.ActionButton)] as GameEvent;
                        _currentComp.GeUntouched = _currentComp.InputsContainer.RightTouchEvents.Items[ControllerInputToSO.GetReleasedGameEventFor(_currentComp.ActionButton)] as GameEvent;
                        _currentComp.IsTouching = _currentComp.InputsContainer.RightTouchBoolean.Items[ControllerInputToSO.GetTouchVariableFor(_currentComp.ActionButton)];
                        break;
                    case EHand.LEFT:
                        _currentComp.GeTouched = _currentComp.InputsContainer.LeftTouchEvents.Items[ControllerInputToSO.GetTouchGameEventFor(_currentComp.ActionButton)] as GameEvent;
                        _currentComp.GeUntouched = _currentComp.InputsContainer.LeftTouchEvents.Items[ControllerInputToSO.GetReleasedGameEventFor(_currentComp.ActionButton)] as GameEvent;
                        _currentComp.IsTouching = _currentComp.InputsContainer.LeftTouchBoolean.Items[ControllerInputToSO.GetTouchVariableFor(_currentComp.ActionButton)];
                        break;
                    default:
                        return false;
                }
            }

            return true;
        }


        /// <summary>
        /// Create and Setup the GameEventListeners for the Click and the Touch Events
        /// </summary>
        private IEnumerator CreateGELInContainer()
        {
            // We wait until the GEL were created, if necessary
            yield return new WaitForEndOfFrame();

            // CLICK
            if (_currentComp.GeDown != null)
            {
                _currentComp.GelDown = _currentComp.GameEventsContainer.AddComponent<GameEventListener>();
                SetupGEListener(_currentComp.GelDown, _currentComp.GeDown, StartActionDown);

                _currentComp.GelUp = _currentComp.GameEventsContainer.AddComponent<GameEventListener>();
                SetupGEListener(_currentComp.GelUp, _currentComp.GeUp, StartActionUp);
            }

            // TOUCH
            if (_currentComp.GeTouched != null)
            {
                _currentComp.GelTouched = _currentComp.GameEventsContainer.AddComponent<GameEventListener>();
                SetupGEListener(_currentComp.GelTouched, _currentComp.GeTouched, StartActionTouched);

                _currentComp.GelUntouched = _currentComp.GameEventsContainer.AddComponent<GameEventListener>();
                SetupGEListener(_currentComp.GelUntouched, _currentComp.GeUntouched, StartActionUntouched);
            }
        }


        /// <summary>
        /// Setup the GameEventListeners for the corresponding GameEvent
        /// </summary>
        /// <param name="gel">The gameEventListener to set</param>
        /// <param name="ge">The GameEvent corresponding</param>
        /// <param name="actionDelegate">The down delegate if relevant. If not, write null</param>
        /// <param name="upDelegate">The up delegate, optionnal</param>
        private void SetupGEListener(GameEventListener gel, GameEvent ge, OnButtonDelegate actionDelegate)
        {
            gel.Event = ge;
            gel.Response = new UnityEvent();
            ge.RegisterListener(gel);
            gel.Response.AddListener(delegate { actionDelegate(); });
        }


        /// <summary>
        /// Create the GameEventListener Container as a child of this transform
        /// </summary>
        /// <returns>The GameObject created</returns>
        private GameObject CreateGEContainer()
        {
            GameObject toReturn = null;
            var bac = _currentComp.GetComponents<ButtonActionChoserComponents>();

            // If there's at least one another ButtonActionChoser on this gameObject and that his script is not the first of the least
            if (bac.Length > 1 && Array.IndexOf(bac, this) != 0)
            {
                _currentComp.StartCoroutine(GetGEL());
            }
            else
            {
                toReturn = new GameObject();
                toReturn.transform.SetParent(_currentComp.transform);
                toReturn.transform.name = _currentComp.name + "_GameEvent_Listeners";
            }
            return toReturn;
        }


        private IEnumerator GetGEL()
        {
            yield return new WaitForEndOfFrame();

            foreach (Transform t in _currentComp.GetComponentsInChildren<Transform>())
            {
                if (t.name.Contains("_GameEvent_Listeners"))
                {
                    _currentComp.GameEventsContainer = t.gameObject;
                    break;
                }
            }
            if (_currentComp.GameEventsContainer == null)
            {
                throw new Exception("VRSF : The gameEventsContainer couldn't be found");
            }
        }


        /// <summary>
        /// Method called when user click the specified button
        /// </summary>
        private void StartActionDown()
        {
            if (_currentComp.CanBeUsed)
            {
                // if we use the Thumb, we need to check its position on the Thumbstick/Touchpad
                if (_currentComp.ThumbPos != null && _currentComp.ClickThreshold > 0.0f)
                {
                    _currentComp.UnclickEventWasRaised = false;

                    switch (_currentComp.ButtonHand)
                    {
                        case EHand.RIGHT:
                            HandleThumbPosition.CheckThumbPosition(_currentComp.RightClickThumbPosition, _currentComp.OnButtonStartClicking, _currentComp.ClickThreshold, _currentComp.ThumbPos.Value);
                            break;
                        case EHand.LEFT:
                            HandleThumbPosition.CheckThumbPosition(_currentComp.LeftClickThumbPosition, _currentComp.OnButtonStartClicking, _currentComp.ClickThreshold, _currentComp.ThumbPos.Value);
                            break;
                    }
                }
                else
                {
                    _currentComp.OnButtonStartClicking.Invoke();
                }
            }
        }


        /// <summary>
        /// Method called when user release the specified button
        /// </summary>
        private void StartActionUp()
        {
            if (_currentComp.CanBeUsed)
            {
                // If we don't use the Thumb
                if (_currentComp.ThumbPos == null)
                    _currentComp.OnButtonStopClicking.Invoke();

                // If we use the Thumb and the click action is beyond the threshold
                else if (_currentComp.ThumbPos != null && _currentComp.ClickActionBeyondThreshold)
                    _currentComp.OnButtonStopClicking.Invoke();

                // If we use the Thumb and the ClickThreshold is equal to 0
                else if (_currentComp.ThumbPos != null && _currentComp.ClickThreshold == 0.0f)
                    _currentComp.OnButtonStopClicking.Invoke();
            }
        }


        /// <summary>
        /// Method called when user start touching the specified button
        /// </summary>
        private void StartActionTouched()
        {
            if (_currentComp.CanBeUsed)
            {
                // if we use the Thumb, we need to check its position on the Thumbstick/Touchpad
                if (_currentComp.ThumbPos != null && _currentComp.TouchThreshold > 0.0f)
                {
                    _currentComp.UntouchedEventWasRaised = false;

                    switch (_currentComp.ButtonHand)
                    {
                        case EHand.RIGHT:
                            HandleThumbPosition.CheckThumbPosition(_currentComp.RightTouchThumbPosition, _currentComp.OnButtonStartTouching, _currentComp.TouchThreshold, _currentComp.ThumbPos.Value);
                            break;
                        case EHand.LEFT:
                            HandleThumbPosition.CheckThumbPosition(_currentComp.LeftTouchThumbPosition, _currentComp.OnButtonStartTouching, _currentComp.TouchThreshold, _currentComp.ThumbPos.Value);
                            break;
                    }
                }
                else
                {
                    _currentComp.OnButtonStartTouching.Invoke();
                }
            }
        }


        /// <summary>
        /// Method called when user stop touching the specified button
        /// </summary>
        private void StartActionUntouched()
        {
            if (_currentComp.CanBeUsed)
            {
                // If we don't use the Thumb
                if (_currentComp.ThumbPos == null)
                    _currentComp.OnButtonStopTouching.Invoke();

                // If we use the Thumb and the click action is beyond the threshold
                else if (_currentComp.ThumbPos != null && _currentComp.TouchActionBeyondThreshold)
                    _currentComp.OnButtonStopTouching.Invoke();

                // If we use the Thumb and the ClickThreshold is equal to 0
                else if (_currentComp.ThumbPos != null && _currentComp.TouchThreshold == 0.0f)
                    _currentComp.OnButtonStopTouching.Invoke();
            }
        }
        #endregion
    }

}