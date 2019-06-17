using Unity.Entities;
using UnityEngine;
using VRSF.Core.Controllers;

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
            public ReticleImageComponent ReticleImage;
        }

        #region ComponentSystem_Methods
        protected override void OnUpdate()
        {
            foreach (var e in GetEntities<Filter>())
            {
                if (Core.SetupVR.VRSF_Components.SetupVRIsReady && e.ReticleImage.ReticleGraphic != null)
                {
                    SetReticleVisibility(e);
                }
                else if (e.ReticleImage.ReticleGraphic == null)
                {
                    Debug.Log("[b]VRSF :[/b] No graphic for Reticle detected, trying to fetch one.");
                    e.ReticleImage.ReticleGraphic = e.ReticleImage.GetComponent<UnityEngine.UI.Graphic>();
                    if (e.ReticleImage.ReticleGraphic == null)
                    {
                        Debug.LogError("[b]VRSF :[/b] Cannot set reticle visibility without a reference to the graphic target of this reticle. Deactivating system.");
                        this.Enabled = false;
                    }
                    else
                    {
                        Debug.Log("[b]VRSF :[/b] Successfully fetch graphic target for Reticle visibility with name : " + e.ReticleImage.ReticleGraphic.name);
                    }
                }
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
                    if (e.ReticleImage.ReticleGraphic.color.a != 1.0f)
                        SetColorWithAlpha(1.0f);
                    break;

                case EPointerState.DISAPPEARING:
                    float newAlpha = e.ReticleImage.ReticleGraphic.color.a - (UnityEngine.Time.deltaTime * e.ReticleVisibility.DisappearanceSpeed);
                    SetColorWithAlpha(newAlpha);

                    if (newAlpha <= 0.0f)
                        e.ReticleVisibility.PointerState = EPointerState.OFF;
                    break;
            }

            void SetColorWithAlpha(float newAlpha)
            {
                var color = e.ReticleImage.ReticleGraphic.color;
                color.a = newAlpha;
                e.ReticleImage.ReticleGraphic.color = color;
            }
        }
        #endregion PRIVATE_METHODS
    }
}