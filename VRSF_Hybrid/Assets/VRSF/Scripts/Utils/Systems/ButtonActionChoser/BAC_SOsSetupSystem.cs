using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
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
            public BACGeneralVariablesComponents ButtonComponents;
        }

        #region PRIVATE_VARIBALES
        private InputVariableContainer _inputsContainer;
        private delegate void OnButtonDelegate();
        List<BAC_DelegatesActions> _bacDelegatesList = new List<BAC_DelegatesActions>();
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
                    CheckInitSOs(entity.ButtonComponents);
                }
                else
                {
                    entity.ButtonComponents.StartCoroutine(WaitForActionButton(entity.ButtonComponents));
                }
            }

            this.Enabled = false;
        }

        protected override void OnUpdate() { }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();

            foreach (var delegatesHandler in _bacDelegatesList)
            {
                ButtonClickEvent.UnregisterListener(delegatesHandler.StartActionDown);
                ButtonUnclickEvent.UnregisterListener(delegatesHandler.StartActionUp);

                ButtonTouchEvent.UnregisterListener(delegatesHandler.StartActionTouched);
                ButtonUntouchEvent.UnregisterListener(delegatesHandler.StartActionUntouched);
            }

            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Check that the Initialization of the ScriptableObjects are done properly.
        /// </summary>
        private void CheckInitSOs(BACGeneralVariablesComponents bacComp)
        {
            // We check that the interaction type is correct
            if (bacComp.InteractionType == EControllerInteractionType.NONE)
            {
                Debug.LogError("VRSF : Please specify a correct InteractionType for the " + this.GetType().Name + " script.\n" +
                    "Setting CanBeUsed of ButtonActionChoserComponents to false.");
                bacComp.CanBeUsed = false;
            }

            // We init the Scriptable Object references and how they work
            if (!InitSOsReferences(bacComp))
            {
                Debug.LogError("VRSF : An error has occured while initializing the Scriptable Objects reference in the " + this.GetType().Name + " script.\n" +
                    "If the error persist after reloading the Editor, please open an issue on Github. Setting CanBeUsed of ButtonActionChoserComponents to false.");
                bacComp.CanBeUsed = false;
            }

            // We setup the listeners only one time as they're gonna check each entities containing the bac Componenent
            SetupListeners(bacComp);

            bacComp.IsSetup = true;
        }


        /// <summary>
        /// Instantiate and set the GameEventListeners and BoolVariable
        /// </summary>
        private bool InitSOsReferences(BACGeneralVariablesComponents bacComp)
        {
            // We set the GameEvents and BoolVariables depending on the comp.InteractionType and the Hand of the ActionButton
            if (!SetupScriptableVariablesReferences(bacComp))
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// Depending on the Button used for the feature and the Interaction Type, setup the BoolVariable and GameEvents accordingly
        /// </summary>
        /// <returns>true if everything was setup correctly</returns>
        private bool SetupScriptableVariablesReferences(BACGeneralVariablesComponents bacComp)
        {
            // If we use the Gaze Button specified in the Gaze Parameters Window
            if (bacComp.UseGazeButton)
            {
                return SetupGazeInteraction(bacComp);
            }
            // If we use the Mouse Wheel Button
            else if (bacComp.IsUsingWheelButton)
            {
                bacComp.IsClicking = _inputsContainer.WheelIsClicking;
                return true;
            }
            else
            {
                return SetupNormalButton(bacComp);
            }
        }


        /// <summary>
        /// Check the Interaction Type specified and set it to corresponds to the Gaze BoolVariable
        /// </summary>
        /// <returns>true if everything was setup correctly</returns>
        private bool SetupGazeInteraction(BACGeneralVariablesComponents bacComp)
        {
            if ((bacComp.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                bacComp.IsClicking = _inputsContainer.GazeIsCliking;
            }
            if ((bacComp.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                bacComp.IsTouching = _inputsContainer.GazeIsTouching;
            }
            return true;
        }


        /// <summary>
        /// Setup the comp._isClicking and _isTouching BoolVariable depending on the comp.InteractionType and the comp._buttonHand variable.
        /// </summary>
        /// <returns>true if everything was setup correctly</returns>
        private bool SetupNormalButton(BACGeneralVariablesComponents bacComp)
        {
            // If the Interaction Type contains at least CLICK
            if ((bacComp.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                switch (bacComp.ButtonHand)
                {
                    case EHand.RIGHT:
                        bacComp.IsClicking = _inputsContainer.RightClickBoolean.Items[ControllerInputToSO.GetClickVariableFor(bacComp.ActionButton)];
                        break;
                    case EHand.LEFT:
                        bacComp.IsClicking = _inputsContainer.LeftClickBoolean.Items[ControllerInputToSO.GetClickVariableFor(bacComp.ActionButton)];
                        break;
                    default:
                        return false;
                }
            }

            // If the Interaction Type contains at least TOUCH
            if ((bacComp.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                // Handle Touch events
                switch (bacComp.ButtonHand)
                {
                    case EHand.RIGHT:
                        bacComp.IsTouching = _inputsContainer.RightTouchBoolean.Items[ControllerInputToSO.GetTouchVariableFor(bacComp.ActionButton)];
                        break;
                    case EHand.LEFT:
                        bacComp.IsTouching = _inputsContainer.LeftTouchBoolean.Items[ControllerInputToSO.GetTouchVariableFor(bacComp.ActionButton)];
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
        private void SetupListeners(BACGeneralVariablesComponents bacComp)
        {
            var delegatesHandler = new BAC_DelegatesActions(bacComp);

            if ((bacComp.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                ButtonClickEvent.RegisterListener(delegatesHandler.StartActionDown);
                ButtonUnclickEvent.RegisterListener(delegatesHandler.StartActionUp);
            }

            if ((bacComp.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                ButtonTouchEvent.RegisterListener(delegatesHandler.StartActionTouched);
                ButtonUntouchEvent.RegisterListener(delegatesHandler.StartActionUntouched);
            }

            _bacDelegatesList.Add(delegatesHandler);
        }

        /// <summary>
        /// As some values are initialized in other Systems, we just want to be sure that everything is setup before checking the Scriptable Objects.
        /// </summary>
        /// <returns></returns>
        private IEnumerator WaitForActionButton(BACGeneralVariablesComponents comp)
        {
            while (!comp.ActionButtonIsReady)
            {
                yield return new WaitForEndOfFrame();
            }

            var sdkChoser = comp.GetComponent<SDKChoserComponent>();

            if (sdkChoser == null || (sdkChoser != null && comp.CorrectSDK))
            {
                CheckInitSOs(comp);
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
        #endregion
    }

}