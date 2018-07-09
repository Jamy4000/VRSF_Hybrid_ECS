using ScriptableFramework.Events;
using ScriptableFramework.Variables;
using Unity.Entities;
using UnityEngine;
using VRSF.Controllers;
using VRSF.Gaze;
using VRSF.Interactions.Components;

namespace VRSF.Interactions.Systems
{
    public class OnColliderOverSystems : ComponentSystem
    {
        struct Filter
        {
            public OnColliderOverComponents OnOverComponents;
        }


        #region PRIVATE_VARIABLE
        private GazeParametersVariable _gazeParameters;
        private ControllersParametersVariable _controllersParameters;
        private InteractionVariableContainer _interactionsContainer;
        #endregion PRIVATE_VARIABLE


        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            _controllersParameters = ControllersParametersVariable.Instance;
            _gazeParameters = GazeParametersVariable.Instance;
            _interactionsContainer = InteractionVariableContainer.Instance;

            // Set to true to avoid error on the first frame.
            _interactionsContainer.RightHit.isNull = true;
            _interactionsContainer.LeftHit.isNull = true;
            _interactionsContainer.GazeHit.isNull = true;
            
            foreach (var entity in GetEntities<Filter>())
            {
                // if we don't use the controllers and the gaze
                entity.OnOverComponents.CheckRaycast = _controllersParameters.UseControllers || _gazeParameters.UseGaze;
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
        

        #region PRIVATE_METHODS
        /// <summary>
        /// Check if each raycast (Controllers and Gaze) are over something
        /// </summary>
        void CheckIsOver(OnColliderOverComponents onOverComp)
        {
            if (_controllersParameters.UseControllers)
            {
                HandleOver(_interactionsContainer.IsOverSomethingRight, _interactionsContainer.RightHit, _interactionsContainer.RightOverObject);
                HandleOver(_interactionsContainer.IsOverSomethingLeft, _interactionsContainer.LeftHit, _interactionsContainer.LeftOverObject);
            }

            if (_gazeParameters.UseGaze)
            {
                HandleOver(_interactionsContainer.IsOverSomethingGaze, _interactionsContainer.GazeHit, _interactionsContainer.GazeOverObject);
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