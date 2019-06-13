using Unity.Entities;
using VRSF.Core.SetupVR;

namespace VRSF.Gaze
{
    public class ReticleSetupSystem : ComponentSystem
    {
        struct Filter
        {
            public ReticleCalculationsComponent ReticleCalculations;
            public UnityEngine.Transform ReticleTransform;
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
                // Store the original scale and rotation.
                e.ReticleCalculations._OriginalScale = e.ReticleTransform.localScale;
                e.ReticleCalculations._OriginalRotation = e.ReticleTransform.localRotation;
            }
        }
        #endregion PRIVATE_METHODS
    }
}