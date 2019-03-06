using Unity.Entities;
using VRSF.Core.Raycast;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Controllers
{
    /// <summary>
    /// handle the references for the controller pointers
    /// </summary>
    public class PointerSetupSystem : ComponentSystem
    {
        struct Filter
        {
            public ScriptableRaycastComponent RaycastComp;
            public ControllerPointerComponents ControllerPointerComp;
        }


        #region ComponentSystem_Methods
        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            OnSetupVRReady.RegisterListener(Init);
        }
        
        protected override void OnUpdate() {}

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
            OnSetupVRReady.RegisterListener(Init);
        }
        #endregion ComponentSystem_Methods

        private void Init(OnSetupVRReady setupVRReady)
        {
            var controllersParameters = ControllersParametersVariable.Instance;

            foreach (var e in GetEntities<Filter>())
            {
                e.ControllerPointerComp.IsSetup = false;

                bool isUsingController = controllersParameters.UseControllers &&
                    (e.RaycastComp.RayOrigin == EHand.LEFT && controllersParameters.UsePointerLeft) ||
                    (e.RaycastComp.RayOrigin == EHand.RIGHT && controllersParameters.UsePointerRight);

                e.ControllerPointerComp._PointerState = isUsingController ? EPointerState.ON : EPointerState.OFF;
                e.ControllerPointerComp.gameObject.SetActive(isUsingController);
                e.ControllerPointerComp.IsSetup = true;
            }
        }
    }
}