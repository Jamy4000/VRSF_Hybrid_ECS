using ScriptableFramework.Events;
using System;
using System.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Events;
using VRSF.Controllers;
using VRSF.Inputs;
using VRSF.Utils.Components;
using static VRSF.Utils.ButtonActionChoser;

namespace VRSF.Utils.Systems.ButtonActionChoser
{
    /// <summary>
    /// Setup the Scriptable Objects and Everything they need (Listeners Container, Listeners, Responses, ...) for a button action choser
    /// </summary>
    public class BAC_SOsSetupSystem : ComponentSystem
    {
        struct Filter
        {
            public ButtonActionChoserComponents ButtonComponents;
        }

        #region PRIVATE_VARIBALES
        private ButtonActionChoserComponents _currentComp;

        private InputVariableContainer _inputsContainer;
        #endregion PRIVATE_VARIABLES


        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            _inputsContainer = InputVariableContainer.Instance;

            foreach (var entity in GetEntities<Filter>())
            {
                _currentComp = entity.ButtonComponents;

                // We init the Scriptable Objects References
                InitGelAndGe();

                if (_currentComp.ActionButtonIsReady)
                {
                    CheckInitSOs();
                }
                else
                {
                    _currentComp.StartCoroutine(WaitForActionButton());
                }
            }
            
        }

        protected override void OnUpdate() {}
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Check that the Initialization of the ScriptableObjects are done properly.
        /// </summary>
        private void CheckInitSOs()
        {
            // We init the Scriptable Object references and how they work
            if (!InitSOsReferences())
            {
                Debug.LogError("VRSF : An error has occured while initializing the Scriptable Objects reference in the " + this.GetType().Name + " script.\n" +
                    "If the error persist after reloading the Editor, please open an issue on Github. Setting CanBeUsed of ButtonActionChoserComponents to false.");
                _currentComp.CanBeUsed = false;
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
        private void InitGelAndGe()
        {
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
                _currentComp.GeDown = _inputsContainer.WheelClickDown;
                _currentComp.GeUp = _inputsContainer.WheelClickUp;
                _currentComp.IsClicking = _inputsContainer.WheelIsClicking;
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
                _currentComp.GeDown = _inputsContainer.GazeClickDown;
                _currentComp.GeUp = _inputsContainer.GazeClickUp;
                _currentComp.IsClicking = _inputsContainer.GazeIsCliking;
            }
            if ((_currentComp.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                _currentComp.GeTouched = _inputsContainer.GazeStartTouching;
                _currentComp.GeUntouched = _inputsContainer.GazeStopTouching;
                _currentComp.IsTouching = _inputsContainer.GazeIsTouching;
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
                        _currentComp.GeDown = _inputsContainer.RightClickEvents.Items[ControllerInputToSO.GetDownGameEventFor(_currentComp.ActionButton)] as GameEvent;
                        _currentComp.GeUp = _inputsContainer.RightClickEvents.Items[ControllerInputToSO.GetUpGameEventFor(_currentComp.ActionButton)] as GameEvent;
                        _currentComp.IsClicking = _inputsContainer.RightClickBoolean.Items[ControllerInputToSO.GetClickVariableFor(_currentComp.ActionButton)];
                        break;
                    case EHand.LEFT:
                        _currentComp.GeDown = _inputsContainer.LeftClickEvents.Items[ControllerInputToSO.GetDownGameEventFor(_currentComp.ActionButton)] as GameEvent;
                        _currentComp.GeUp = _inputsContainer.LeftClickEvents.Items[ControllerInputToSO.GetUpGameEventFor(_currentComp.ActionButton)] as GameEvent;
                        _currentComp.IsClicking = _inputsContainer.LeftClickBoolean.Items[ControllerInputToSO.GetClickVariableFor(_currentComp.ActionButton)];
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
                        _currentComp.GeTouched = _inputsContainer.RightTouchEvents.Items[ControllerInputToSO.GetTouchGameEventFor(_currentComp.ActionButton)] as GameEvent;
                        _currentComp.GeUntouched = _inputsContainer.RightTouchEvents.Items[ControllerInputToSO.GetReleasedGameEventFor(_currentComp.ActionButton)] as GameEvent;
                        _currentComp.IsTouching = _inputsContainer.RightTouchBoolean.Items[ControllerInputToSO.GetTouchVariableFor(_currentComp.ActionButton)];
                        break;
                    case EHand.LEFT:
                        _currentComp.GeTouched = _inputsContainer.LeftTouchEvents.Items[ControllerInputToSO.GetTouchGameEventFor(_currentComp.ActionButton)] as GameEvent;
                        _currentComp.GeUntouched = _inputsContainer.LeftTouchEvents.Items[ControllerInputToSO.GetReleasedGameEventFor(_currentComp.ActionButton)] as GameEvent;
                        _currentComp.IsTouching = _inputsContainer.LeftTouchBoolean.Items[ControllerInputToSO.GetTouchVariableFor(_currentComp.ActionButton)];
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

        /// <summary>
        /// As some values are initialized in other Systems, we just want to be sure that everything is setup before checking the Scriptable Objects.
        /// </summary>
        /// <returns></returns>
        private IEnumerator WaitForActionButton()
        {
            while (!_currentComp.ActionButtonIsReady)
            {
                yield return new WaitForEndOfFrame();
            }
            CheckInitSOs();
        }
        #endregion
    }

}