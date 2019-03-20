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
        protected override void OnCreateManager()
        {
            base.OnCreateManager();
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
            var activationDic = new System.Collections.Generic.Dictionary<UnityEngine.GameObject, bool>();

            foreach (var e in GetEntities<Filter>())
            {
                e.ControllerPointerComp.IsSetup = false;

                bool isUsingController = VRSF_Components.DeviceLoaded != EDevice.SIMULATOR && controllersParameters.UseControllers &&
                    ((e.RaycastComp.RayOrigin == EHand.LEFT && controllersParameters.UsePointerLeft) || (e.RaycastComp.RayOrigin == EHand.RIGHT && controllersParameters.UsePointerRight));

                e.ControllerPointerComp._PointerState = isUsingController ? EPointerState.ON : EPointerState.OFF;
                activationDic.Add(e.RaycastComp.gameObject, isUsingController);

                e.ControllerPointerComp.IsSetup = true;
            }

            foreach (var kvp in activationDic)
            {
                kvp.Key.SetActive(kvp.Value);
            }
        }
    }
}