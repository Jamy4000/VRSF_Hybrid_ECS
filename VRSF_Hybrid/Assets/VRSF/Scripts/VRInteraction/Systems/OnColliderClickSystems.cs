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


        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            foreach (var entity in GetEntities<Filter>())
            {
                 entity.OnClickComp.ControllersParameters = ControllersParametersVariable.Instance;
                 entity.OnClickComp.GazeParameters = GazeParametersVariable.Instance;
                 entity.OnClickComp.InputsContainer = InputVariableContainer.Instance;
                 entity.OnClickComp.InteractionsContainer = InteractionVariableContainer.Instance;

                 entity.OnClickComp.LeftClickBool =  entity.OnClickComp.InputsContainer.LeftClickBoolean.Get("TriggerIsDown");
                 entity.OnClickComp.RightClickBool =  entity.OnClickComp.InputsContainer.RightClickBoolean.Get("TriggerIsDown");

                // Set to true to avoid error on the first frame.
                 entity.OnClickComp.InteractionsContainer.RightHit.isNull = true;
                 entity.OnClickComp.InteractionsContainer.LeftHit.isNull = true;
                 entity.OnClickComp.InteractionsContainer.GazeHit.isNull = true;

                // As we cannot click without controllers, we disable this script if we don't use them
                entity.OnClickComp.CheckRaycast = entity.OnClickComp.ControllersParameters.UseControllers;
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
            var interactions = onClickComp.InteractionsContainer;

            if (!onClickComp.RightClickBool.Value && interactions.HasClickSomethingRight.Value)
                interactions.HasClickSomethingRight.SetValue(false);

            if (!onClickComp.LeftClickBool.Value && interactions.HasClickSomethingLeft.Value)
                interactions.HasClickSomethingLeft.SetValue(false);

            if (onClickComp.GazeParameters.UseGaze && !onClickComp.InputsContainer.GazeIsCliking.Value && interactions.HasClickSomethingGaze.Value)
                interactions.HasClickSomethingGaze.SetValue(false);
        }

        /// <summary>
        /// If the click button was pressed for the right or left controller, or the gaze, set the Scriptable Object that match
        /// </summary>
        void CheckClick(OnColliderClickComponents onClickComp)
        {
            var interactions = onClickComp.InteractionsContainer;

            if (onClickComp.RightClickBool.Value && !interactions.HasClickSomethingRight.Value)
                HandleClick(interactions.RightHit, interactions.HasClickSomethingRight, interactions.RightObjectWasClicked);

            if (onClickComp.LeftClickBool.Value && !interactions.HasClickSomethingLeft.Value)
                HandleClick(interactions.LeftHit, interactions.HasClickSomethingLeft, interactions.LeftObjectWasClicked);

            if (onClickComp.GazeParameters.UseGaze && onClickComp.InputsContainer.GazeIsCliking.Value && !interactions.HasClickSomethingGaze.Value)
                HandleClick(interactions.GazeHit, interactions.HasClickSomethingGaze, interactions.GazeObjectWasClicked);
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