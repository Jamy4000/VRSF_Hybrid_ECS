using Unity.Entities;
using VRSF.Core.Raycast;
using VRSF.Core.Events;
using VRSF.Interactions;

namespace VRSF.Gaze
{
    /// <summary>
    /// System running to check if the user is looking at something
    /// </summary>
    public class GazeHoveringSystem : ComponentSystem
    {
        struct Filter
        {
            public ScriptableRaycastComponent ScriptableRaycast;
            public GazeRaycastComponent GazeRaycast;
        }
        
        private InteractionVariableContainer _interactionsVariables;

        #region ComponentSystem_Methods
        protected override void OnCreateManager()
        {
            base.OnCreateManager();
            _interactionsVariables = InteractionVariableContainer.Instance;
        }

        protected override void OnUpdate()
        {
            foreach (var entity in GetEntities<Filter>())
            {
                HandleOver();
            }
        }
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Handle the raycastHits to check if one of them touch something
        /// </summary>
        private void HandleOver()
        {
            //If nothing is hit, we set the isOver value to false
            if (_interactionsVariables.IsOverSomethingGaze.Value && _interactionsVariables.GazeHit.IsNull)
            {
                _interactionsVariables.IsOverSomethingGaze.SetValue(false);
                _interactionsVariables.PreviousGazeHit = null;
                new ObjectWasHoveredEvent(ERayOrigin.CAMERA, null);
            }
            //If something is hit, we check that the collider is still "alive", and we check that the new transform hit is not the same as the previous one
            else if (!_interactionsVariables.GazeHit.IsNull && _interactionsVariables.GazeHit.Value.collider != null &&
                    _interactionsVariables.GazeHit.Value.collider.transform != _interactionsVariables.PreviousGazeHit)
            {
                var hitTransform = _interactionsVariables.GazeHit.Value.collider.transform;

                _interactionsVariables.PreviousGazeHit = hitTransform;
                _interactionsVariables.IsOverSomethingGaze.SetValue(true);
                new ObjectWasHoveredEvent(ERayOrigin.CAMERA, hitTransform);
            }
        }
        #endregion PRIVATE_METHODS
    }
}