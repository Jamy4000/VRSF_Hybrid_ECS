using Unity.Entities;
using VRSF.Core.Events;

namespace VRSF.Core.Interactions
{
    public class OnColliderLeftClickSystem : ComponentSystem
    {
        struct Filter
        {
            public OnColliderClickComponent OnClickComp;
            public Raycast.ScriptableRaycastComponent PointerRaycast;
            public SetupVR.ScriptableSingletonsComponent ScriptableSingletons;
        }


        #region ComponentSystem_Methods
        protected override void OnUpdate()
        {
            foreach (var entity in GetEntities<Filter>())
            {
                if (entity.ScriptableSingletons._IsSetup && entity.ScriptableSingletons.ControllersParameters.UseControllers && entity.PointerRaycast.CheckRaycast)
                {
                    CheckResetClick(entity);

                    if (OnColliderClickComponent.LeftTriggerCanClick && entity.OnClickComp.LeftClickBool.Value && !entity.ScriptableSingletons.InteractionsContainer.HasClickSomethingLeft.Value)
                    {
                        HandleClick(entity);
                    }
                }
            }
        }
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Check if there's 
        /// </summary>
        void CheckResetClick(Filter entity)
        {
            if (!entity.OnClickComp.LeftClickBool.Value && entity.ScriptableSingletons.InteractionsContainer.HasClickSomethingLeft.Value)
                entity.ScriptableSingletons.InteractionsContainer.HasClickSomethingLeft.SetValue(false);
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
            if (entity.ScriptableSingletons.InteractionsContainer.LeftHit.IsNull)
            {
                entity.ScriptableSingletons.InteractionsContainer.HasClickSomethingLeft.SetValue(false);
            }
            else
            {
                entity.ScriptableSingletons.InteractionsContainer.HasClickSomethingLeft.SetValue(true);

                var objectClicked = entity.ScriptableSingletons.InteractionsContainer.LeftHit.Value.collider.transform;
                new ObjectWasClickedEvent(Controllers.EHand.LEFT, objectClicked);
            }
        }
        #endregion PRIVATE_METHODS
    }
}