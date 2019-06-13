using Unity.Entities;
using VRSF.Core.Controllers;
using VRSF.Core.Events;
using VRSF.Interactions;

namespace VRSF.Gaze.Utils
{
    /// <summary>
    /// System to handle the visibility of the Reticle based on whether it's hitting something
    /// </summary>
    public class ReticleStateSystem : ComponentSystem
    {
        struct Filter
        {
            public ReticleVisibilityComponent ReticleVisibility;
            public UnityEngine.UI.Image ReticleImage;
        }


        #region PRIVATE_VARIABLES
        private InteractionVariableContainer _interactionsContainer;
        #endregion PRIVATE_VARIABLES


        #region ComponentSystem_Methods
        protected override void OnCreateManager()
        {
            base.OnCreateManager();
            _interactionsContainer = InteractionVariableContainer.Instance;
        }

        protected override void OnUpdate()
        {
            foreach (var e in GetEntities<Filter>())
            {
                CheckReticleState(e);
            }
        }
        #endregion ComponentSystem_Methods


        #region PRIVATE_METHODS
        private void OnGazeHitSomething(ObjectWasHoveredEvent info)
        {
            if (info.RaycastOrigin == Core.Raycast.ERayOrigin.CAMERA && info.ObjectHovered != null)
            {
                foreach (var e in GetEntities<Filter>())
                {
                    e.ReticleVisibility.ReticleState = EPointerState.ON;
                }
            }
        }

        /// <summary>
        /// Set the state of the reticle based on the hits 
        /// </summary>
        private void CheckReticleState(Filter e)
        {
            if (!_interactionsContainer.IsOverSomethingGaze)
            {
                switch (e.ReticleVisibility.ReticleState)
                {
                    case EPointerState.ON:
                        e.ReticleVisibility.ReticleState = EPointerState.DISAPPEARING;
                        break;
                    case EPointerState.DISAPPEARING:
                        if (e.ReticleImage.color.a <= 0.0f)
                            e.ReticleVisibility.ReticleState = EPointerState.OFF;
                        break;
                }
            }
        }
        #endregion PRIVATE_METHODS
    }
}