using Unity.Entities;
using VRSF.Interactions.Components;
using VRSF.Utils.Components;

namespace VRSF.Interactions.Systems
{
    public class OnColliderRightClickSystem : ComponentSystem
    {
        struct Filter
        {
            public OnColliderClickComponent OnClickComp;
            public PointerRaycastComponent PointerRaycast;
            public ScriptableSingletonsComponent ScriptableSingletons;
        }


        #region ComponentSystem_Methods
        protected override void OnUpdate()
        {
            foreach (var entity in GetEntities<Filter>())
            {
                if (entity.ScriptableSingletons.IsSetup && entity.ScriptableSingletons.ControllersParameters.UseControllers &&
                    entity.PointerRaycast.CheckRaycast)
                {
                    CheckResetClick(entity);

                    if (entity.OnClickComp.RightClickBool.Value && !entity.ScriptableSingletons.InteractionsContainer.HasClickSomethingRight.Value)
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
            if (!entity.OnClickComp.RightClickBool.Value && entity.ScriptableSingletons.InteractionsContainer.HasClickSomethingRight.Value)
                entity.ScriptableSingletons.InteractionsContainer.HasClickSomethingRight.SetValue(false);
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
            if (entity.ScriptableSingletons.InteractionsContainer.RightHit.isNull)
            {
                entity.ScriptableSingletons.InteractionsContainer.HasClickSomethingRight.SetValue(false);
            }
            else
            {
                if (entity.ScriptableSingletons.InteractionsContainer.RightHit.Value.collider != null)
                {
                    entity.ScriptableSingletons.InteractionsContainer.HasClickSomethingRight.SetValue(true);

                    var objectClicked = entity.ScriptableSingletons.InteractionsContainer.RightHit.Value.collider.transform;
                    entity.ScriptableSingletons.InteractionsContainer.RightObjectWasClicked.Raise(objectClicked);
                }
            }
        }
        #endregion PRIVATE_METHODS
    }
} 