using System.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using VRSF.Controllers;
using VRSF.Inputs;
using VRSF.Inputs.Events;
using VRSF.Utils.Components;
using VRSF.Utils.Components.ButtonActionChoser;

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
        private InputVariableContainer _inputsContainer;
        private ButtonActionChoserComponents _currentBACSetup;
        private delegate void OnButtonDelegate();
        #endregion PRIVATE_VARIABLES


        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            _inputsContainer = InputVariableContainer.Instance;
            
            SceneManager.sceneUnloaded += OnSceneUnloaded;

            foreach (var entity in GetEntities<Filter>())
            {
                if (entity.ButtonComponents.ActionButtonIsReady)
                {
                    _currentBACSetup = entity.ButtonComponents;
                    CheckInitSOs();
                }
                else
                {
                    entity.ButtonComponents.StartCoroutine(WaitForActionButton(entity.ButtonComponents));
                }
            }
        }

        protected override void OnUpdate()
        {
            bool StillSettingUp = false;
            foreach (var e in GetEntities<Filter>())
            {
                if (!e.ButtonComponents.IsSetup)
                {
                    StillSettingUp = true;
                }
            }
            this.Enabled = StillSettingUp;
        }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();

            ButtonClickEvent.UnregisterListener(StartActionDown);
            ButtonUnclickEvent.UnregisterListener(StartActionUp);

            ButtonTouchEvent.UnregisterListener(StartActionTouched);
            ButtonUntouchEvent.UnregisterListener(StartActionUntouched);

            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Check that the Initialization of the ScriptableObjects are done properly.
        /// </summary>
        private void CheckInitSOs()
        {
            // We check that the interaction type is correct
            if (_currentBACSetup.InteractionType == EControllerInteractionType.NONE)
            {
                Debug.LogError("VRSF : Please specify a correct InteractionType for the " + this.GetType().Name + " script.\n" +
                    "Setting CanBeUsed of ButtonActionChoserComponents to false.");
                _currentBACSetup.CanBeUsed = false;
            }

            // We init the Scriptable Object references and how they work
            if (!InitSOsReferences())
            {
                Debug.LogError("VRSF : An error has occured while initializing the Scriptable Objects reference in the " + this.GetType().Name + " script.\n" +
                    "If the error persist after reloading the Editor, please open an issue on Github. Setting CanBeUsed of ButtonActionChoserComponents to false.");
                _currentBACSetup.CanBeUsed = false;
            }

            _currentBACSetup.IsSetup = true;
        }


        /// <summary>
        /// Instantiate and set the GameEventListeners and BoolVariable
        /// </summary>
        private bool InitSOsReferences()
        {
            // We set the GameEvents and BoolVariables depending on the comp.InteractionType and the Hand of the ActionButton
            if (!SetupScriptableVariablesReferences())
            {
                return false;
            }

            // We add the GameEventListeners to the GameEventContainer, set the events and response for the listeners, 
            // and register the listeners in the GameEvent
            // Placed in coroutine as sometimes we need to wait for another ButtonActionChoser script to create the GELContainer 
            SetupListeners();

            return true;
        }


        /// <summary>
        /// Depending on the Button used for the feature and the Interaction Type, setup the BoolVariable and GameEvents accordingly
        /// </summary>
        /// <returns>true if everything was setup correctly</returns>
        private bool SetupScriptableVariablesReferences()
        {
            // If we use the Gaze Button specified in the Gaze Parameters Window
            if (_currentBACSetup.UseGazeButton)
            {
                return SetupGazeInteraction();
            }
            // If we use the Mouse Wheel Button
            else if (_currentBACSetup.IsUsingWheelButton)
            {
                _currentBACSetup.IsClicking = _inputsContainer.WheelIsClicking;
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
            if ((_currentBACSetup.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                _currentBACSetup.IsClicking = _inputsContainer.GazeIsCliking;
            }
            if ((_currentBACSetup.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                _currentBACSetup.IsTouching = _inputsContainer.GazeIsTouching;
            }
            return true;
        }


        /// <summary>
        /// Setup the comp._isClicking and _isTouching BoolVariable depending on the comp.InteractionType and the comp._buttonHand variable.
        /// </summary>
        /// <returns>true if everything was setup correctly</returns>
        private bool SetupNormalButton()
        {
            // If the Interaction Type contains at least CLICK
            if ((_currentBACSetup.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                switch (_currentBACSetup.ButtonHand)
                {
                    case EHand.RIGHT:
                        _currentBACSetup.IsClicking = _inputsContainer.RightClickBoolean.Items[ControllerInputToSO.GetClickVariableFor(_currentBACSetup.ActionButton)];
                        break;
                    case EHand.LEFT:
                        _currentBACSetup.IsClicking = _inputsContainer.LeftClickBoolean.Items[ControllerInputToSO.GetClickVariableFor(_currentBACSetup.ActionButton)];
                        break;
                    default:
                        return false;
                }
            }

            // If the Interaction Type contains at least TOUCH
            if ((_currentBACSetup.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                // Handle Touch events
                switch (_currentBACSetup.ButtonHand)
                {
                    case EHand.RIGHT:
                        _currentBACSetup.IsTouching = _inputsContainer.RightTouchBoolean.Items[ControllerInputToSO.GetTouchVariableFor(_currentBACSetup.ActionButton)];
                        break;
                    case EHand.LEFT:
                        _currentBACSetup.IsTouching = _inputsContainer.LeftTouchBoolean.Items[ControllerInputToSO.GetTouchVariableFor(_currentBACSetup.ActionButton)];
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
        private void SetupListeners()
        {
            if ((_currentBACSetup.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                ButtonClickEvent.RegisterListener(StartActionDown);
                ButtonUnclickEvent.RegisterListener(StartActionUp);
            }

            if ((_currentBACSetup.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                ButtonTouchEvent.RegisterListener(StartActionTouched);
                ButtonUntouchEvent.RegisterListener(StartActionUntouched);
            }
        }

        /// <summary>
        /// As some values are initialized in other Systems, we just want to be sure that everything is setup before checking the Scriptable Objects.
        /// </summary>
        /// <returns></returns>
        private IEnumerator WaitForActionButton(ButtonActionChoserComponents comp)
        {
            while (!comp.ActionButtonIsReady)
            {
                yield return new WaitForEndOfFrame();
            }

            var sdkChoser = comp.GetComponent<SDKChoserComponent>();

            if (sdkChoser == null || (sdkChoser != null && comp.CorrectSDK))
            {
                CheckInitSOs();
            }
            else
            {
                comp.IsSetup = true;
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


        #region Delegates_OnButtonAction
        /// <summary>
        /// Method called when user click the specified button
        /// </summary>
        private void StartActionDown(ButtonClickEvent eventButton)
        {
            foreach (var entity in GetEntities<Filter>())
            {
                // We check if the button clicked is the one set in the ButtonActionChoser comp and that the BAC can be used
                if (entity.ButtonComponents.ButtonHand == eventButton.HandInteracting && entity.ButtonComponents.ActionButton == eventButton.ButtonInteracting && entity.ButtonComponents.CanBeUsed)
                {
                    // if we use the Thumb, we need to check its position on the Thumbstick/Touchpad
                    if (entity.ButtonComponents.ThumbPos != null && entity.ButtonComponents.ClickThreshold > 0.0f)
                    {
                        entity.ButtonComponents.UnclickEventWasRaised = false;

                        switch (entity.ButtonComponents.ButtonHand)
                        {
                            case EHand.RIGHT:
                                HandleThumbPosition.CheckThumbPosition(entity.ButtonComponents.RightClickThumbPosition, entity.ButtonComponents.OnButtonStartClicking, entity.ButtonComponents.ClickThreshold, entity.ButtonComponents.ThumbPos.Value);
                                break;
                            case EHand.LEFT:
                                HandleThumbPosition.CheckThumbPosition(entity.ButtonComponents.LeftClickThumbPosition, entity.ButtonComponents.OnButtonStartClicking, entity.ButtonComponents.ClickThreshold, entity.ButtonComponents.ThumbPos.Value);
                                break;
                        }
                    }
                    else
                    {
                        entity.ButtonComponents.OnButtonStartClicking.Invoke();
                    }
                }
            }
        }


        /// <summary>
        /// Method called when user release the specified button
        /// </summary>
        private void StartActionUp(ButtonUnclickEvent eventButton)
        {
            foreach (var entity in GetEntities<Filter>())
            {
                // We check if the button clicked is the one set in the ButtonActionChoser comp and that the BAC can be used
                if (entity.ButtonComponents.ButtonHand == eventButton.HandInteracting && entity.ButtonComponents.ActionButton == eventButton.ButtonInteracting && entity.ButtonComponents.CanBeUsed)
                {
                    // If we don't use the Thumb
                    if (entity.ButtonComponents.ThumbPos == null)
                        entity.ButtonComponents.OnButtonStopClicking.Invoke();

                    // If we use the Thumb and the click action is beyond the threshold
                    else if (entity.ButtonComponents.ThumbPos != null && entity.ButtonComponents.ClickActionBeyondThreshold)
                        entity.ButtonComponents.OnButtonStopClicking.Invoke();

                    // If we use the Thumb and the ClickThreshold is equal to 0
                    else if (entity.ButtonComponents.ThumbPos != null && entity.ButtonComponents.ClickThreshold == 0.0f)
                        entity.ButtonComponents.OnButtonStopClicking.Invoke();
                }
            }
        }


        /// <summary>
        /// Method called when user start touching the specified button
        /// </summary>
        private void StartActionTouched(ButtonTouchEvent eventButton)
        {
            foreach (var entity in GetEntities<Filter>())
            {
                // We check if the button clicked is the one set in the ButtonActionChoser comp and that the BAC can be used
                if (entity.ButtonComponents.ButtonHand == eventButton.HandInteracting && entity.ButtonComponents.ActionButton == eventButton.ButtonInteracting && entity.ButtonComponents.CanBeUsed)
                {
                    // if we use the Thumb, we need to check its position on the Thumbstick/Touchpad
                    if (entity.ButtonComponents.ThumbPos != null && entity.ButtonComponents.TouchThreshold > 0.0f)
                    {
                        entity.ButtonComponents.UntouchedEventWasRaised = false;

                        switch (entity.ButtonComponents.ButtonHand)
                        {
                            case EHand.RIGHT:
                                HandleThumbPosition.CheckThumbPosition(entity.ButtonComponents.RightTouchThumbPosition, entity.ButtonComponents.OnButtonStartTouching, entity.ButtonComponents.TouchThreshold, entity.ButtonComponents.ThumbPos.Value);
                                break;
                            case EHand.LEFT:
                                HandleThumbPosition.CheckThumbPosition(entity.ButtonComponents.LeftTouchThumbPosition, entity.ButtonComponents.OnButtonStartTouching, entity.ButtonComponents.TouchThreshold, entity.ButtonComponents.ThumbPos.Value);
                                break;
                        }
                    }
                    else
                    {
                        entity.ButtonComponents.OnButtonStartTouching.Invoke();
                    }
                }
            }
        }


        /// <summary>
        /// Method called when user stop touching the specified button
        /// </summary>
        private void StartActionUntouched(ButtonUntouchEvent eventButton)
        {
            foreach (var entity in GetEntities<Filter>())
            {
                // We check if the button clicked is the one set in the ButtonActionChoser comp and that the BAC can be used
                if (entity.ButtonComponents.ButtonHand == eventButton.HandInteracting && entity.ButtonComponents.ActionButton == eventButton.ButtonInteracting && entity.ButtonComponents.CanBeUsed)
                {
                    // If we don't use the Thumb
                    if (entity.ButtonComponents.ThumbPos == null)
                        entity.ButtonComponents.OnButtonStopTouching.Invoke();

                    // If we use the Thumb and the click action is beyond the threshold
                    else if (entity.ButtonComponents.ThumbPos != null && entity.ButtonComponents.TouchActionBeyondThreshold)
                        entity.ButtonComponents.OnButtonStopTouching.Invoke();

                    // If we use the Thumb and the ClickThreshold is equal to 0
                    else if (entity.ButtonComponents.ThumbPos != null && entity.ButtonComponents.TouchThreshold == 0.0f)
                        entity.ButtonComponents.OnButtonStopTouching.Invoke();
                }
            }
        }
        #endregion Delegates_OnButtonAction

        #endregion
    }

}