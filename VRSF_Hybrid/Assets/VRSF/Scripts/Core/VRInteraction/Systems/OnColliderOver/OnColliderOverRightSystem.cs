using Unity.Entities;
using VRSF.Core.Controllers;
using VRSF.Core.Events;
using VRSF.Interactions;

namespace VRSF.Core.Interactions
{
    public class OnColliderOverRightSystem : ComponentSystem
    {
        struct Filter
        {
            public Raycast.ScriptableRaycastComponent PointerRaycast;
        }

        private ControllersParametersVariable _controllersParam;
        private InteractionVariableContainer _interactionsVariables;

        #region ComponentSystem_Methods
        protected override void OnCreateManager()
        {
            base.OnCreateManager();
            _controllersParam = ControllersParametersVariable.Instance;
            _interactionsVariables = InteractionVariableContainer.Instance;
        }

        protected override void OnUpdate()
        {
            foreach (var entity in GetEntities<Filter>())
            {
                if (_controllersParam.UseControllers && entity.PointerRaycast.CheckRaycast)
                {
                    HandleOver();
                }
            }
        }
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Handle the raycastHits to check if one of them touch something
        /// </summary>
        private void HandleOver()
        {
            //If nothing is hit, we set the isOver value to false
            if (_interactionsVariables.IsOverSomethingRight.Value && _interactionsVariables.RightHit.IsNull)
            {
                _interactionsVariables.IsOverSomethingRight.SetValue(false);
                _interactionsVariables.PreviousRightHit = null;
                new ObjectWasHoveredEvent(EHand.RIGHT, null);
            }
            //If something is hit, we check that the collider is still "alive", and we check that the new transform hit is not the same as the previous one
            else if (!_interactionsVariables.RightHit.IsNull && _interactionsVariables.RightHit.Value.collider != null &&
                    _interactionsVariables.RightHit.Value.collider.transform != _interactionsVariables.PreviousRightHit)
            {
                var hitTransform = _interactionsVariables.RightHit.Value.collider.transform;

                _interactionsVariables.PreviousRightHit = hitTransform;
                _interactionsVariables.IsOverSomethingRight.SetValue(true);
                new ObjectWasHoveredEvent(EHand.RIGHT, hitTransform);
            }
        }
        #endregion PRIVATE_METHODS
    }
}