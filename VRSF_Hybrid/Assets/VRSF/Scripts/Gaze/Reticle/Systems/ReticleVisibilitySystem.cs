using Unity.Entities;
using VRSF.Core.Controllers;
using VRSF.Core.Raycast;

namespace VRSF.Gaze.Utils
{
    /// <summary>
    /// System to handle the visibility of the Reticle based on whether it's hitting something
    /// </summary>
    public class ReticleVisibilitySystem : ComponentSystem
    {
        struct Filter
        {
            public PointerVisibilityComponents ReticleVisibility;
            public ReticleCalculationsComponent ReticleCalculations;
            public ScriptableRaycastComponent ScriptableRaycast;
        }

        #region ComponentSystem_Methods
        protected override void OnUpdate()
        {
            foreach (var e in GetEntities<Filter>())
            {
                if (e.ScriptableRaycast.IsSetup)
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
            switch (e.ReticleVisibility.PointerState)
            {
                case EPointerState.ON:
                    UnityEngine.Debug.Log("ON Reticle ");
                    if (e.ReticleCalculations._ReticleImage.color.a != 1.0f)
                        SetColorWithAlpha(1.0f);
                    break;

                case EPointerState.DISAPPEARING:
                    UnityEngine.Debug.Log("DISAPPEARING Reticle ");
                    float newAlpha = e.ReticleCalculations._ReticleImage.color.a - (UnityEngine.Time.deltaTime * e.ReticleVisibility.DisappearanceSpeed);
                    SetColorWithAlpha(newAlpha);

                    if (newAlpha <= 0.0f)
                        e.ReticleVisibility.PointerState = EPointerState.OFF;
                    break;
            }

            void SetColorWithAlpha(float newAlpha)
            {
                var color = e.ReticleCalculations._ReticleImage.color;
                color.a = newAlpha;
                e.ReticleCalculations._ReticleImage.color = color;
                //UnityEngine.Debug.Log("New color alpha RETICLE : " + color.a);
            }
        }
        #endregion PRIVATE_METHODS
    }
}