using Unity.Entities;
using VRSF.Core.Controllers;
using VRSF.Core.Events;
using VRSF.Interactions;

namespace VRSF.Core.Interactions
{
    /// <summary>
    /// System raising the events for when the left controller is over something
    /// </summary>
    public class OnColliderOverLeftSystem : ComponentSystem
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
            if (_interactionsVariables.IsOverSomethingLeft.Value && _interactionsVariables.LeftHit.IsNull)
            {
                _interactionsVariables.IsOverSomethingLeft.SetValue(false);
                _interactionsVariables.PreviousLeftHit = null;
                new ObjectWasHoveredEvent(EHand.LEFT, null);
            }
            //If something is hit, we check that the collider is still "alive", and we check that the new transform hit is not the same as the previous one
            else if (!_interactionsVariables.LeftHit.IsNull && _interactionsVariables.LeftHit.Value.collider != null &&
                    _interactionsVariables.LeftHit.Value.collider.transform != _interactionsVariables.PreviousLeftHit)
            {
                var hitTransform = _interactionsVariables.LeftHit.Value.collider.transform;

                _interactionsVariables.PreviousLeftHit = hitTransform;
                _interactionsVariables.IsOverSomethingLeft.SetValue(true);
                new ObjectWasHoveredEvent(EHand.LEFT, hitTransform);
            }
        }
        #endregion PRIVATE_METHODS
    }
}