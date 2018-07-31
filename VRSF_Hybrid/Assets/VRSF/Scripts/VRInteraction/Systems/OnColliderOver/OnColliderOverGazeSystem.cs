using Unity.Entities;
using UnityEngine;
using VRSF.Utils.Components;

namespace VRSF.Interactions.Systems
{
    public class OnColliderOverGazeSystem : ComponentSystem
    {
        struct Filter
        {
            public PointerRaycastComponent PointerRaycast;
            public ScriptableSingletonsComponent ScriptableSingletons;
        }


        #region ComponentSystem_Methods
        protected override void OnUpdate()
        {
            foreach (var entity in GetEntities<Filter>())
            {
                if (entity.ScriptableSingletons.IsSetup && entity.ScriptableSingletons.GazeParameters.UseGaze && entity.PointerRaycast.CheckRaycast)
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
            if (comp.InteractionsContainer.IsOverSomethingGaze.Value && comp.InteractionsContainer.GazeHit.isNull)
            {
                comp.InteractionsContainer.IsOverSomethingGaze.SetValue(false);
                comp.InteractionsContainer.GazeOverObject.Raise(null);
                comp.InteractionsContainer.PreviousGazeHit = null;
            }
            //If something is hit, we check that the collider is still "alive", and we check that the new transform hit is not the same as the previous one
            else if (comp.InteractionsContainer.GazeHit.Value.collider != null &&
                    comp.InteractionsContainer.GazeHit.Value.transform != comp.InteractionsContainer.PreviousGazeHit)
            {
                var hitTransform = comp.InteractionsContainer.GazeHit.Value.collider.transform;
                comp.InteractionsContainer.GazeOverObject.Raise(hitTransform);

                comp.InteractionsContainer.PreviousGazeHit = hitTransform;

                comp.InteractionsContainer.IsOverSomethingGaze.SetValue(true);
            }
        }
        #endregion PRIVATE_METHODS
    }
}