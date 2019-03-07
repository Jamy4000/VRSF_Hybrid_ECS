using Unity.Entities;
using VRSF.Core.Events;
using VRSF.Core.SetupVR;

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
            public ScriptableSingletonsComponent ScriptableSingletons;
        }


        #region ComponentSystem_Methods
        protected override void OnUpdate()
        {
            foreach (var entity in GetEntities<Filter>())
            {
                if (entity.ScriptableSingletons._IsSetup && entity.ScriptableSingletons.ControllersParameters.UseControllers && entity.PointerRaycast.CheckRaycast)
                {
                    HandleOver(entity.ScriptableSingletons);
                }
            }
        }
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Handle the raycastHits to check if one of them touch something
        /// </summary>
        /// <param name="isOver">the BoolVariable to set if something got hit</param>
        /// <param name="hit">The Hit Point where the raycast collide</param>
        /// <param name="objectOver">The GameEvent to raise with the transform of the hit</param>
        private void HandleOver(ScriptableSingletonsComponent comp)
        {
            //If nothing is hit, we set the isOver value to false
            if (comp.InteractionsContainer.IsOverSomethingLeft.Value && comp.InteractionsContainer.LeftHit.IsNull)
            {
                comp.InteractionsContainer.IsOverSomethingLeft.SetValue(false);
                comp.InteractionsContainer.PreviousLeftHit = null;
                new ObjectWasHoveredEvent(Controllers.EHand.LEFT, null);
            }
            //If something is hit, we check that the collider is still "alive", and we check that the new transform hit is not the same as the previous one
            else if (!comp.InteractionsContainer.LeftHit.IsNull && comp.InteractionsContainer.LeftHit.Value.collider != null &&
                    comp.InteractionsContainer.LeftHit.Value.collider.transform != comp.InteractionsContainer.PreviousLeftHit)
            {
                var hitTransform = comp.InteractionsContainer.LeftHit.Value.collider.transform;

                comp.InteractionsContainer.PreviousLeftHit = hitTransform;
                comp.InteractionsContainer.IsOverSomethingLeft.SetValue(true);
                new ObjectWasHoveredEvent(Controllers.EHand.LEFT, hitTransform);
            }
        }
        #endregion PRIVATE_METHODS
    }
}