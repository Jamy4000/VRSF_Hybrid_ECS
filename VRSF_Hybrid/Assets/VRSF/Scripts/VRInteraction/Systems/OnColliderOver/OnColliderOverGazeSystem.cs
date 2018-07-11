using Unity.Entities;
using UnityEngine;
using VRSF.Interactions.Components;

namespace VRSF.Interactions.Systems
{
    public class OnColliderOverGazeSystem : ComponentSystem
    {
        struct Filter
        {
            public OnColliderOverComponents OnOverComponents;
        }


        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnStartRunning()
        {
            base.OnStartRunning();
        }

        protected override void OnUpdate()
        {
            foreach (var entity in GetEntities<Filter>())
            {
                if (entity.OnOverComponents.IsSetup && entity.OnOverComponents.GazeParameters.UseGaze && entity.OnOverComponents.CheckRaycast)
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
            if (comp.InteractionsContainer.GazeHit.isNull)
            {
                comp.InteractionsContainer.IsOverSomethingGaze.SetValue(false);
            }
            else
            {
                if (comp.InteractionsContainer.GazeHit.Value.collider != null)
                {
                    var hitTransform = comp.InteractionsContainer.GazeHit.Value.collider.transform;
                    comp.InteractionsContainer.GazeOverObject.Raise(hitTransform);

                    comp.InteractionsContainer.IsOverSomethingGaze.SetValue(true);
                }
            }
        }
        #endregion PRIVATE_METHODS
    }
}