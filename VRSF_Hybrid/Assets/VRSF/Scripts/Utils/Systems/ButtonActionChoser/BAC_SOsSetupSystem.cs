using ScriptableFramework.Events;
using System;
using System.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using VRSF.Controllers;
using VRSF.Inputs;
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
        private delegate void OnButtonDelegate();
        private InputVariableContainer _inputsContainer;
        #endregion PRIVATE_VARIABLES


        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            _inputsContainer = InputVariableContainer.Instance;
            SceneManager.activeSceneChanged += OnSceneChanged;

            foreach (var entity in GetEntities<Filter>())
            {
                // We init the Scriptable Objects References
                InitGelAndGe(entity.ButtonComponents);

                if (entity.ButtonComponents.ActionButtonIsReady)
                {
                    CheckInitSOs(entity.ButtonComponents);
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
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Check that the Initialization of the ScriptableObjects are done properly.
        /// </summary>
        private void CheckInitSOs(ButtonActionChoserComponents comp)
        {
            // We check that the interaction type is correct
            if (comp.InteractionType == EControllerInteractionType.NONE)
            {
                Debug.LogError("VRSF : Please specify a correct InteractionType for the " + this.GetType().Name + " script.\n" +
                    "Setting CanBeUsed of ButtonActionChoserComponents to false.");
                comp.CanBeUsed = false;
            }

            // We init the Scriptable Object references and how they work
            if (!InitSOsReferences(comp))
            {
                Debug.LogError("VRSF : An error has occured while initializing the Scriptable Objects reference in the " + this.GetType().Name + " script.\n" +
                    "If the error persist after reloading the Editor, please open an issue on Github. Setting CanBeUsed of ButtonActionChoserComponents to false.");
                comp.CanBeUsed = false;
            }
            else
            {
                comp.IsSetup = true;
            }
        }


        /// <summary>
        /// Instantiate and set the GameEventListeners and BoolVariable
        /// </summary>
        private bool InitSOsReferences(ButtonActionChoserComponents comp)
        {
            // We set the GameEvents and BoolVariables depending on the comp.InteractionType and the Hand of the ActionButton
            if (!SetupSOReferences(comp))
            {
                return false;
            }
            
            //We create the container for the GameEventListeners
            comp.GameEventsContainer = CreateGEContainer(comp);

            // We add the GameEventListeners to the GameEventContainer, set the events and response for the listeners, 
            // and register the listeners in the GameEvent
            // Placed in coroutine as sometimes we need to wait for another ButtonActionChoser script to create the GELContainer 
            comp.StartCoroutine(CreateGELInContainer(comp));

            return true;
        }


        /// <summary>
        /// Set the GameEvents and GameEventListeners to null
        /// </summary>
        private void InitGelAndGe(ButtonActionChoserComponents comp)
        {
            comp.GeDown = null;
            comp.GeUp = null;
            comp.GeTouched = null;
            comp.GeUntouched = null;

            comp.GelDown = null;
            comp.GelUp = null;
            comp.GelTouched = null;
            comp.GelUntouched = null;
        }


        /// <summary>
        /// Depending on the Button used for the feature and the Interaction Type, setup the BoolVariable and GameEvents accordingly
        /// </summary>
        /// <returns>true if everything was setup correctly</returns>
        private bool SetupSOReferences(ButtonActionChoserComponents comp)
        {
            // If we use the Gaze Button specified in the Gaze Parameters Window
            if (comp.UseGazeButton)
            {
                return SetupGazeInteraction(comp);
            }
            // If we use the Mouse Wheel Button
            else if (comp.IsUsingWheelButton)
            {
                comp.GeDown = _inputsContainer.WheelClickDown;
                comp.GeUp = _inputsContainer.WheelClickUp;
                comp.IsClicking = _inputsContainer.WheelIsClicking;
                return true;
            }
            else
            {
                return SetupNormalButton(comp);
            }
        }


        /// <summary>
        /// Check the Interaction Type specified and set it to corresponds to the Gaze BoolVariable
        /// </summary>
        /// <returns>true if everything was setup correctly</returns>
        private bool SetupGazeInteraction(ButtonActionChoserComponents comp)
        {
            if ((comp.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                comp.GeDown = _inputsContainer.GazeClickDown;
                comp.GeUp = _inputsContainer.GazeClickUp;
                comp.IsClicking = _inputsContainer.GazeIsCliking;
            }
            if ((comp.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                comp.GeTouched = _inputsContainer.GazeStartTouching;
                comp.GeUntouched = _inputsContainer.GazeStopTouching;
                comp.IsTouching = _inputsContainer.GazeIsTouching;
            }
            return true;
        }


        /// <summary>
        /// Setup the comp._isClicking and _isTouching BoolVariable depending on the comp.InteractionType and the comp._buttonHand variable.
        /// </summary>
        /// <returns>true if everything was setup correctly</returns>
        private bool SetupNormalButton(ButtonActionChoserComponents comp)
        {
            // If the Interaction Type contains at least CLICK
            if ((comp.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                switch (comp.ButtonHand)
                {
                    case EHand.RIGHT:
                        comp.GeDown = _inputsContainer.RightClickEvents.Items[ControllerInputToSO.GetDownGameEventFor(comp.ActionButton)] as GameEvent;
                        comp.GeUp = _inputsContainer.RightClickEvents.Items[ControllerInputToSO.GetUpGameEventFor(comp.ActionButton)] as GameEvent;
                        comp.IsClicking = _inputsContainer.RightClickBoolean.Items[ControllerInputToSO.GetClickVariableFor(comp.ActionButton)];
                        break;
                    case EHand.LEFT:
                        comp.GeDown = _inputsContainer.LeftClickEvents.Items[ControllerInputToSO.GetDownGameEventFor(comp.ActionButton)] as GameEvent;
                        comp.GeUp = _inputsContainer.LeftClickEvents.Items[ControllerInputToSO.GetUpGameEventFor(comp.ActionButton)] as GameEvent;
                        comp.IsClicking = _inputsContainer.LeftClickBoolean.Items[ControllerInputToSO.GetClickVariableFor(comp.ActionButton)];
                        break;
                    default:
                        return false;
                }
            }

            // If the Interaction Type contains at least TOUCH
            if ((comp.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                // Handle Touch events
                switch (comp.ButtonHand)
                {
                    case EHand.RIGHT:
                        comp.GeTouched = _inputsContainer.RightTouchEvents.Items[ControllerInputToSO.GetTouchGameEventFor(comp.ActionButton)] as GameEvent;
                        comp.GeUntouched = _inputsContainer.RightTouchEvents.Items[ControllerInputToSO.GetReleasedGameEventFor(comp.ActionButton)] as GameEvent;
                        comp.IsTouching = _inputsContainer.RightTouchBoolean.Items[ControllerInputToSO.GetTouchVariableFor(comp.ActionButton)];
                        break;
                    case EHand.LEFT:
                        comp.GeTouched = _inputsContainer.LeftTouchEvents.Items[ControllerInputToSO.GetTouchGameEventFor(comp.ActionButton)] as GameEvent;
                        comp.GeUntouched = _inputsContainer.LeftTouchEvents.Items[ControllerInputToSO.GetReleasedGameEventFor(comp.ActionButton)] as GameEvent;
                        comp.IsTouching = _inputsContainer.LeftTouchBoolean.Items[ControllerInputToSO.GetTouchVariableFor(comp.ActionButton)];
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
        private IEnumerator CreateGELInContainer(ButtonActionChoserComponents comp)
        {
            // We wait until the GEL were created, if necessary
            yield return new WaitForEndOfFrame();

            OnButtonDelegate buttonActionDelegate;

            // CLICK
            if (comp.GeDown != null)
            {
                comp.GelDown = comp.GameEventsContainer.AddComponent<GameEventListener>();
                buttonActionDelegate = delegate { StartActionDown(comp); };
                SetupGEListener(comp.GelDown, comp.GeDown, buttonActionDelegate);

                comp.GelUp = comp.GameEventsContainer.AddComponent<GameEventListener>();
                buttonActionDelegate = delegate { StartActionUp(comp); };
                SetupGEListener(comp.GelUp, comp.GeUp, buttonActionDelegate);
            }

            // TOUCH
            if (comp.GeTouched != null)
            {
                comp.GelTouched = comp.GameEventsContainer.AddComponent<GameEventListener>();
                buttonActionDelegate = delegate { StartActionTouched(comp); };
                SetupGEListener(comp.GelTouched, comp.GeTouched, buttonActionDelegate);

                comp.GelUntouched = comp.GameEventsContainer.AddComponent<GameEventListener>();
                buttonActionDelegate = delegate { StartActionUntouched(comp); };
                SetupGEListener(comp.GelUntouched, comp.GeUntouched, buttonActionDelegate);
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
        private GameObject CreateGEContainer(ButtonActionChoserComponents comp)
        {
            GameObject toReturn = null;
            var bacs = comp.GetComponents<ButtonActionChoserComponents>();

            // If there's at least one another ButtonActionChoser on this gameObject and that his script is not the first of the least
            if (bacs.Length > 1 && Array.IndexOf(bacs, this) != 0)
            {
                comp.StartCoroutine(GetGEL(comp));
            }
            else
            {
                toReturn = new GameObject();
                toReturn.transform.SetParent(comp.transform);
                toReturn.transform.name = comp.name + "_GameEvent_Listeners";
            }
            return toReturn;
        }


        private IEnumerator GetGEL(ButtonActionChoserComponents comp)
        {
            yield return new WaitForEndOfFrame();

            foreach (Transform t in comp.GetComponentsInChildren<Transform>())
            {
                if (t.name.Contains("_GameEvent_Listeners"))
                {
                    comp.GameEventsContainer = t.gameObject;
                    break;
                }
            }
            if (comp.GameEventsContainer == null)
            {
                throw new Exception("VRSF : The gameEventsContainer couldn't be found");
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
            CheckInitSOs(comp);
        }


        /// <summary>
        /// Reactivate the System when switching to another Scene.
        /// </summary>
        /// <param name="oldScene">The previous scene before switching</param>
        /// <param name="newScene">The new scene after switching</param>
        private void OnSceneChanged(Scene oldScene, Scene newScene)
        {
            this.Enabled = true;

            foreach (var entity in GetEntities<Filter>())
            {
                // We init the Scriptable Objects References
                InitGelAndGe(entity.ButtonComponents);

                if (entity.ButtonComponents.ActionButtonIsReady)
                {
                    CheckInitSOs(entity.ButtonComponents);
                }
                else
                {
                    entity.ButtonComponents.StartCoroutine(WaitForActionButton(entity.ButtonComponents));
                }
            }
        }


        #region Delegates_OnButtonAction
        /// <summary>
        /// Method called when user click the specified button
        /// </summary>
        private void StartActionDown(ButtonActionChoserComponents comp)
        {
            if (comp.CanBeUsed)
            {
                // if we use the Thumb, we need to check its position on the Thumbstick/Touchpad
                if (comp.ThumbPos != null && comp.ClickThreshold > 0.0f)
                {
                    comp.UnclickEventWasRaised = false;

                    switch (comp.ButtonHand)
                    {
                        case EHand.RIGHT:
                            HandleThumbPosition.CheckThumbPosition(comp.RightClickThumbPosition, comp.OnButtonStartClicking, comp.ClickThreshold, comp.ThumbPos.Value);
                            break;
                        case EHand.LEFT:
                            HandleThumbPosition.CheckThumbPosition(comp.LeftClickThumbPosition, comp.OnButtonStartClicking, comp.ClickThreshold, comp.ThumbPos.Value);
                            break;
                    }
                }
                else
                {
                    comp.OnButtonStartClicking.Invoke();
                }
            }
        }


        /// <summary>
        /// Method called when user release the specified button
        /// </summary>
        private void StartActionUp(ButtonActionChoserComponents comp)
        {
            if (comp.CanBeUsed)
            {
                // If we don't use the Thumb
                if (comp.ThumbPos == null)
                    comp.OnButtonStopClicking.Invoke();

                // If we use the Thumb and the click action is beyond the threshold
                else if (comp.ThumbPos != null && comp.ClickActionBeyondThreshold)
                    comp.OnButtonStopClicking.Invoke();

                // If we use the Thumb and the ClickThreshold is equal to 0
                else if (comp.ThumbPos != null && comp.ClickThreshold == 0.0f)
                    comp.OnButtonStopClicking.Invoke();
            }
        }


        /// <summary>
        /// Method called when user start touching the specified button
        /// </summary>
        private void StartActionTouched(ButtonActionChoserComponents comp)
        {
            if (comp.CanBeUsed)
            {
                // if we use the Thumb, we need to check its position on the Thumbstick/Touchpad
                if (comp.ThumbPos != null && comp.TouchThreshold > 0.0f)
                {
                    comp.UntouchedEventWasRaised = false;

                    switch (comp.ButtonHand)
                    {
                        case EHand.RIGHT:
                            HandleThumbPosition.CheckThumbPosition(comp.RightTouchThumbPosition, comp.OnButtonStartTouching, comp.TouchThreshold, comp.ThumbPos.Value);
                            break;
                        case EHand.LEFT:
                            HandleThumbPosition.CheckThumbPosition(comp.LeftTouchThumbPosition, comp.OnButtonStartTouching, comp.TouchThreshold, comp.ThumbPos.Value);
                            break;
                    }
                }
                else
                {
                    comp.OnButtonStartTouching.Invoke();
                }
            }
        }


        /// <summary>
        /// Method called when user stop touching the specified button
        /// </summary>
        private void StartActionUntouched(ButtonActionChoserComponents comp)
        {
            if (comp.CanBeUsed)
            {
                // If we don't use the Thumb
                if (comp.ThumbPos == null)
                    comp.OnButtonStopTouching.Invoke();

                // If we use the Thumb and the click action is beyond the threshold
                else if (comp.ThumbPos != null && comp.TouchActionBeyondThreshold)
                    comp.OnButtonStopTouching.Invoke();

                // If we use the Thumb and the ClickThreshold is equal to 0
                else if (comp.ThumbPos != null && comp.TouchThreshold == 0.0f)
                    comp.OnButtonStopTouching.Invoke();
            }
        }
        #endregion Delegates_OnButtonAction

        #endregion
    }

}