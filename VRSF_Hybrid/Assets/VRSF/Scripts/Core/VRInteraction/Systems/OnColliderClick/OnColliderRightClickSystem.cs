using Unity.Entities;
using VRSF.Core.Events;
using VRSF.Core.Controllers;
using VRSF.Interactions;

namespace VRSF.Core.Interactions
{
    public class OnColliderRightClickSystem : ComponentSystem
    {
        struct Filter
        {
            public OnColliderClickComponent OnClickComp;
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
            if (_controllersParam.UseControllers && _controllersParam.UsePointerRight)
            {
                foreach (var entity in GetEntities<Filter>())
                {
                    if (entity.PointerRaycast.CheckRaycast)
                    {
                        CheckResetClick(entity.OnClickComp);

                        if (OnColliderClickComponent.RightTriggerCanClick && entity.OnClickComp.RightClickBool.Value && !_interactionsVariables.HasClickSomethingRight.Value)
                        {
                            HandleClick(entity);
                        }
                    }
                }
            }
        }
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Reset the HasClickSomethingRight bool if the user is not clicking anymore
        /// </summary>
        void CheckResetClick(OnColliderClickComponent onClickComp)
        {
            if (!onClickComp.RightClickBool.Value && _interactionsVariables.HasClickSomethingRight.Value)
                _interactionsVariables.HasClickSomethingRight.SetValue(false);
        }

        /// <summary>
        /// Handle the raycastHits to check if one object was clicked
        /// </summary>
        /// <param name="hits">The list of RaycastHits to check</param>
        /// <param name="hasClicked">the BoolVariable to set if something got clicked</param>
        /// <param name="objectClicked">The GameEvent to raise with the transform of the hit</param>
        private void HandleClick(Filter entity)
        {
            //If nothing is hit, we set the isOver value to false
            if (_interactionsVariables.RightHit.IsNull)
            {
                _interactionsVariables.HasClickSomethingRight.SetValue(false);
            }
            else
            {
                _interactionsVariables.HasClickSomethingRight.SetValue(true);

                var objectClicked = _interactionsVariables.RightHit.Value.collider.transform;
                new ObjectWasClickedEvent(EHand.RIGHT, objectClicked);
            }

        }
        #endregion PRIVATE_METHODS
    }
}