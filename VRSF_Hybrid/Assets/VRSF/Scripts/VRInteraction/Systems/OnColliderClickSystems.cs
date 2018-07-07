using ScriptableFramework.Events;
using ScriptableFramework.Variables;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using VRSF.Controllers;
using VRSF.Gaze;
using VRSF.Inputs;
using VRSF.Interactions.Components;

namespace VRSF.Interactions.Systems
{
    public class OnColliderClickSystems : ComponentSystem
    {
        struct Filter
        {
            public OnColliderClickComponents OnClickComp;
        }

        #region PRIVATE_VARIABLES
        [Tooltip("The Gaze Parameters as ScriptableSingletons")]
        private GazeParametersVariable _gazeParameters;

        [Tooltip("The Interactions and Input Container, as ScriptableSingletons, for the VRSF Scriptable Objects")]
        private InputVariableContainer _inputsContainer;
        private InteractionVariableContainer _interactionsContainer;
        #endregion


        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            
            _gazeParameters = GazeParametersVariable.Instance;
            _inputsContainer = InputVariableContainer.Instance;
            _interactionsContainer = InteractionVariableContainer.Instance;
            
            foreach (var entity in GetEntities<Filter>())
            {
                 entity.OnClickComp.LeftClickBool = _inputsContainer.LeftClickBoolean.Get("TriggerIsDown");
                 entity.OnClickComp.RightClickBool = _inputsContainer.RightClickBoolean.Get("TriggerIsDown");

                // Set to true to avoid error on the first frame.
                _interactionsContainer.RightHit.isNull = true;
                _interactionsContainer.LeftHit.isNull = true;
                _interactionsContainer.GazeHit.isNull = true;

                // As we cannot click without controllers, we disable this script if we don't use them
                entity.OnClickComp.CheckRaycast = ControllersParametersVariable.Instance.UseControllers;
            }
        }

        protected override void OnUpdate()
        {
            // If we doesn't use the controllers, we don't check for the inputs.
            if (ControllersParametersVariable.Instance.UseControllers)
            {
                foreach (var entity in GetEntities<Filter>())
                {
                    if (entity.OnClickComp.CheckRaycast)
                    {
                        CheckResetClick(entity.OnClickComp);
                        CheckClick(entity.OnClickComp);
                    }
                }
            }
        }
        #endregion


        //EMPTY
        #region PUBLIC_METHODS
        #endregion PUBLIC_METHODS


        #region PRIVATE_METHODS
        /// <summary>
        /// Check if there's 
        /// </summary>
        void CheckResetClick(OnColliderClickComponents onClickComp)
        {
            if (!onClickComp.RightClickBool.Value && _interactionsContainer.HasClickSomethingRight.Value)
                _interactionsContainer.HasClickSomethingRight.SetValue(false);

            if (!onClickComp.LeftClickBool.Value && _interactionsContainer.HasClickSomethingLeft.Value)
                _interactionsContainer.HasClickSomethingLeft.SetValue(false);

            if (_gazeParameters.UseGaze && !_inputsContainer.GazeIsCliking.Value && _interactionsContainer.HasClickSomethingGaze.Value)
                _interactionsContainer.HasClickSomethingGaze.SetValue(false);
        }

        /// <summary>
        /// If the click button was pressed for the right or left controller, or the gaze, set the Scriptable Object that match
        /// </summary>
        void CheckClick(OnColliderClickComponents onClickComp)
        {
            if (onClickComp.RightClickBool.Value && !_interactionsContainer.HasClickSomethingRight.Value)
                HandleClick(_interactionsContainer.RightHit, _interactionsContainer.HasClickSomethingRight, _interactionsContainer.RightObjectWasClicked);

            if (onClickComp.LeftClickBool.Value && !_interactionsContainer.HasClickSomethingLeft.Value)
                HandleClick(_interactionsContainer.LeftHit, _interactionsContainer.HasClickSomethingLeft, _interactionsContainer.LeftObjectWasClicked);

            if (_gazeParameters.UseGaze && _inputsContainer.GazeIsCliking.Value && !_interactionsContainer.HasClickSomethingGaze.Value)
                HandleClick(_interactionsContainer.GazeHit, _interactionsContainer.HasClickSomethingGaze, _interactionsContainer.GazeObjectWasClicked);
        }

        /// <summary>
        /// Handle the raycastHits to check if one object was clicked
        /// </summary>
        /// <param name="hits">The list of RaycastHits to check</param>
        /// <param name="hasClicked">the BoolVariable to set if something got clicked</param>
        /// <param name="objectClicked">The GameEvent to raise with the transform of the hit</param>
        private void HandleClick(RaycastHitVariable hit, BoolVariable hasClicked, GameEventTransform objectClickedEvent)
        {
            //If nothing is hit, we set the isOver value to false
            if (hit.isNull)
            {
                hasClicked.SetValue(false);
            }
            else
            {
                if (hit.Value.collider != null)
                {
                    hasClicked.SetValue(true);
                    objectClickedEvent.Raise(hit.Value.collider.transform);
                }
            }
        }
        #endregion PRIVATE_METHODS
    }
}