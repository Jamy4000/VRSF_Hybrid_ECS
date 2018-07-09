using ScriptableFramework.Events;
using ScriptableFramework.Variables;
using Unity.Entities;
using UnityEngine;
using VRSF.Controllers;
using VRSF.Gaze;
using VRSF.Interactions.Components;
using VRSF.Utils;

namespace VRSF.Interactions.Systems
{
    public class OnColliderOverSystems : ComponentSystem
    {
        struct Filter
        {
            public OnColliderOverComponents OnOverComponents;
        }


        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnStartRunning()
        {
            foreach (var entity in GetEntities<Filter>())
            {
                entity.OnOverComponents.ControllersParameters = ControllersParametersVariable.Instance;
                entity.OnOverComponents.GazeParameters = GazeParametersVariable.Instance;
                entity.OnOverComponents.InteractionsContainer = InteractionVariableContainer.Instance;

                // Set to true to avoid error on the first frame.
                entity.OnOverComponents.InteractionsContainer.RightHit.isNull = true;
                entity.OnOverComponents.InteractionsContainer.LeftHit.isNull = true;
                entity.OnOverComponents.InteractionsContainer.GazeHit.isNull = true;

                // if we don't use the controllers and the gaze
                entity.OnOverComponents.CheckRaycast = entity.OnOverComponents.ControllersParameters.UseControllers || entity.OnOverComponents.GazeParameters.UseGaze;
            }
        }

        protected override void OnUpdate()
        {
            foreach (var entity in GetEntities<Filter>())
            {
                if (entity.OnOverComponents.CheckRaycast)
                {
                    CheckIsOver(entity.OnOverComponents);
                }
            }
        }
        #endregion


        //EMPTY
        #region PUBLIC_METHODS
        #endregion PUBLIC_METHODS


        #region PRIVATE_METHODS
        /// <summary>
        /// Check if each raycast (Controllers and Gaze) are over something
        /// </summary>
        void CheckIsOver(OnColliderOverComponents onOverComp)
        {
            // assigning pointerRaycast variable for a better readability
            var interactions = onOverComp.InteractionsContainer;
            var controllersParam = onOverComp.ControllersParameters;
            var gazeParam = onOverComp.GazeParameters;

            if (controllersParam.UseControllers)
            {
                HandleOver(interactions.IsOverSomethingRight, interactions.RightHit, interactions.RightOverObject);
                HandleOver(interactions.IsOverSomethingLeft, interactions.LeftHit, interactions.LeftOverObject);
            }

            if (gazeParam.UseGaze)
            {
                HandleOver(interactions.IsOverSomethingGaze, interactions.GazeHit, interactions.GazeOverObject);
            }
        }

        /// <summary>
        /// Handle the raycastHits to check if one of them touch something
        /// </summary>
        /// <param name="isOver">the BoolVariable to set if something got hit</param>
        /// <param name="hit">The Hit Point where the raycast collide</param>
        /// <param name="objectOver">The GameEvent to raise with the transform of the hit</param>
        private void HandleOver(BoolVariable isOver, RaycastHitVariable hit, GameEventTransform objectOver)
        {
            //If nothing is hit, we set the isOver value to false
            if (hit.isNull)
            {
                isOver.SetValue(false);
            }
            else
            {
                if (hit.Value.collider != null)
                {
                    var hitTransform = hit.Value.collider.transform;
                    objectOver.Raise(hitTransform);

                    isOver.SetValue(true);
                }
            }
        }
        #endregion PRIVATE_METHODS
    }
}