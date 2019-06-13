using Unity.Entities;
using VRSF.Core.Controllers;
using VRSF.Interactions;

namespace VRSF.Gaze.Utils
{
    /// <summary>
    /// System to handle the visibility of the Reticle based on whether it's hitting something
    /// </summary>
    public class ReticleVisibilitySystem : ComponentSystem
    {
        struct Filter
        {
            public ReticleVisibilityComponent ReticleVisibility;
            public GazeInputsComponent GazeCalculations;
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
                SetReticleVisibility(e);
            }
        }
        #endregion ComponentSystem_Methods


        #region PRIVATE_METHODS
        /// <summary>
        /// Set the alpha of the reticle depending on its state
        /// </summary>
        private void SetReticleVisibility(Filter e)
        {
            // If the Gaze is supposed to be off
            switch (e.ReticleVisibility.ReticleState)
            {
                case EPointerState.ON:
                    if (e.ReticleImage.color.a != 1.0f)
                        SetColorWithAlpha(1.0f);
                    break;

                case EPointerState.DISAPPEARING:
                    float newAlpha = e.ReticleImage.color.a - (UnityEngine.Time.deltaTime * e.ReticleVisibility.DisappearanceSpeed);
                    SetColorWithAlpha(newAlpha);
                    break;

                case EPointerState.OFF:

                    break;
            }

            void SetColorWithAlpha(float newAlpha)
            {
                var color = e.ReticleImage.color;
                color.a = newAlpha;
                e.ReticleImage.color = color;
            }
        }
        #endregion PRIVATE_METHODS
    }
}