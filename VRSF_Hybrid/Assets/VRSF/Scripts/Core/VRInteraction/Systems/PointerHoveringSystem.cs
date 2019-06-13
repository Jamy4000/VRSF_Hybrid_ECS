using ScriptableFramework.Variables;
using Unity.Entities;
using UnityEngine;
using VRSF.Core.Events;
using VRSF.Core.Raycast;
using VRSF.Interactions;

namespace VRSF.Core.Interactions
{
    public class PointerHoveringSystem : ComponentSystem
    {
        struct Filter
        {
            public PointerClickComponent PointerClick;
            public ScriptableRaycastComponent ScriptableRaycast;
        }
        
        private InteractionVariableContainer _interactionsVariables;

        #region ComponentSystem_Methods
        protected override void OnCreateManager()
        {
            base.OnCreateManager();
            _interactionsVariables = InteractionVariableContainer.Instance;
        }

        protected override void OnUpdate()
        {
            foreach (var entity in GetEntities<Filter>())
            {
                switch (entity.ScriptableRaycast.RayOrigin)
                {
                    case ERayOrigin.LEFT_HAND:
                        HandleOver(ref _interactionsVariables.PreviousLeftHit, _interactionsVariables.IsOverSomethingLeft, _interactionsVariables.LeftHit, ERayOrigin.LEFT_HAND);
                        break;
                    case ERayOrigin.RIGHT_HAND:
                        HandleOver(ref _interactionsVariables.PreviousRightHit, _interactionsVariables.IsOverSomethingRight, _interactionsVariables.RightHit, ERayOrigin.RIGHT_HAND);
                        break;
                    default:
                        Debug.LogError("<b>[VRSF] :</b> PointerClickComponent can only be placed on Right or Left hand. Please check your parameters.", entity.PointerClick.gameObject);
                        break;
                }
            }
        }
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Handle the raycastHits to check if one of them touch something
        /// </summary>
        private void HandleOver(ref Transform previousHit, BoolVariable isOverSomething, RaycastHitVariable hitVar, ERayOrigin origin)
        {
            //If nothing is hit, we set the isOver value to false
            if (isOverSomething.Value && hitVar.IsNull)
            {
                previousHit = null;
                SetVariables(null, true);
            }
            //If something is hit, we check that the collider is still "alive", and we check that the new transform hit is not the same as the previous one
            else if (!hitVar.IsNull && hitVar.Value.collider != null && hitVar.Value.collider.transform != previousHit)
            {
                var hitTransform = hitVar.Value.collider.transform;
                previousHit = hitTransform;
                SetVariables(hitTransform, true);
            }

            void SetVariables(Transform newHit, bool newOverValue)
            {
                isOverSomething.SetValue(newOverValue);
                new ObjectWasHoveredEvent(origin, newHit);
            }
        }
        #endregion PRIVATE_METHODS
    }
}