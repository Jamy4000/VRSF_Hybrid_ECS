using Unity.Entities;
using VRSF.Core.SetupVR;

namespace VRSF.Gaze
{
    public class ReticleSetupSystem : ComponentSystem
    {
        struct Filter
        {
            public ReticleCalculationsComponent ReticleCalculations;
        }

        #region ComponentSystem_Methods
        protected override void OnCreateManager()
        {
            OnSetupVRReady.Listeners += Init;
            base.OnCreateManager();
            this.Enabled = false;
        }

        protected override void OnUpdate() {}

        protected override void OnDestroyManager()
        {
            OnSetupVRReady.Listeners -= Init;
            base.OnDestroyManager();
        }
        #endregion ComponentSystem_Methods


        #region PRIVATE_METHODS
        private void Init(OnSetupVRReady info)
        {
            foreach (var e in GetEntities<Filter>())
            {
                e.ReticleCalculations._ReticleImage = e.ReticleCalculations.GetComponent<UnityEngine.UI.Image>();
                e.ReticleCalculations._ReticleTransform = e.ReticleCalculations.GetComponent<UnityEngine.Transform>();

                // Store the original scale and rotation.
                e.ReticleCalculations._OriginalScale = e.ReticleCalculations._ReticleTransform.localScale;
                e.ReticleCalculations._OriginalRotation = e.ReticleCalculations._ReticleTransform.localRotation;
            }
        }
        #endregion PRIVATE_METHODS
    }
}