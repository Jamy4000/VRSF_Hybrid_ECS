using Unity.Entities;
using VRSF.Core.Controllers;
using VRSF.Core.Events;
using VRSF.Interactions;

namespace VRSF.Core.Interactions
{
    public class OnColliderLeftClickSystem : ComponentSystem
    {
        struct Filter
        {
            public OnColliderClickComponent OnClickComp;
            public Raycast.ScriptableRaycastComponent PointerRaycast;
        }

        private ControllersParametersVariable _controllersParam;
        private InteractionVariableContainer _interactionContainer;

        #region ComponentSystem_Methods
        protected override void OnCreateManager()
        {
            base.OnCreateManager();
            _controllersParam = ControllersParametersVariable.Instance;
            _interactionContainer = InteractionVariableContainer.Instance;
        }

        protected override void OnUpdate()
        {
            if (_controllersParam.UseControllers && _controllersParam.UsePointerLeft)
            {
                foreach (var entity in GetEntities<Filter>())
                {
                    if (entity.PointerRaycast.CheckRaycast)
                    {
                        CheckResetClick(entity.OnClickComp);

                        if (OnColliderClickComponent.LeftTriggerCanClick && entity.OnClickComp.LeftClickBool.Value && !_interactionContainer.HasClickSomethingLeft.Value)
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
        /// Reset the HasClickSomethingLeft bool if the user is not clicking anymore
        /// </summary>
        void CheckResetClick(OnColliderClickComponent onClickComp)
        {
            if (!onClickComp.LeftClickBool.Value && _interactionContainer.HasClickSomethingLeft.Value)
                _interactionContainer.HasClickSomethingLeft.SetValue(false);
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
            if (_interactionContainer.LeftHit.IsNull)
            {
                _interactionContainer.HasClickSomethingLeft.SetValue(false);
            }
            else
            {
                _interactionContainer.HasClickSomethingLeft.SetValue(true);

                var objectClicked = _interactionContainer.LeftHit.Value.collider.transform;
                new ObjectWasClickedEvent(Controllers.EHand.LEFT, objectClicked);
            }
        }
        #endregion PRIVATE_METHODS
    }
}