using Unity.Entities;
using VRSF.Interactions.Components;

namespace VRSF.Interactions.Systems
{
    public class OnColliderOverLeftSystem : ComponentSystem
    {
        struct Filter
        {
            public OnColliderOverComponents OnOverComponents;
        }


        #region ComponentSystem_Methods
        protected override void OnUpdate()
        {
            foreach (var entity in GetEntities<Filter>())
            {
                if (entity.OnOverComponents.IsSetup && entity.OnOverComponents.ControllersParameters.UseControllers && entity.OnOverComponents.CheckRaycast)
                {
                    HandleOver(entity.OnOverComponents);
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
        private void HandleOver(OnColliderOverComponents comp)
        {
            //If nothing is hit, we set the isOver value to false
            if (comp.InteractionsContainer.LeftHit.isNull)
            {
                comp.InteractionsContainer.IsOverSomethingLeft.SetValue(false);
            }
            else
            {
                if (comp.InteractionsContainer.LeftHit.Value.collider != null)
                {
                    var hitTransform = comp.InteractionsContainer.LeftHit.Value.collider.transform;
                    comp.InteractionsContainer.LeftOverObject.Raise(hitTransform);

                    comp.InteractionsContainer.IsOverSomethingLeft.SetValue(true);
                }
            }
        }
        #endregion PRIVATE_METHODS
    }
}