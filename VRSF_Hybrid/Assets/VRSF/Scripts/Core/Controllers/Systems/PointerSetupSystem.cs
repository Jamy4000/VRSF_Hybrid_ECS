using Unity.Entities;
using UnityEngine.SceneManagement;
using VRSF.Controllers.Components;
using VRSF.Utils.Components;

namespace VRSF.Controllers.Systems
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
            SceneManager.sceneLoaded += OnSceneLoaded;
        }


        // Update is called once per frame
        protected override void OnUpdate()
        {
            if (!Utils.VRSF_Components.SetupVRIsReady)
                return;

            bool systemStillRunning = false;
            var controllersParameters = ControllersParametersVariable.Instance;

            foreach (var e in GetEntities<Filter>())
            {
                if (!e.ControllerPointerComp.IsSetup)
                {
                    systemStillRunning = true;

                    bool isUsingController = controllersParameters.UseControllers &&
                        (e.RaycastComp.RayOrigin == EHand.LEFT && controllersParameters.UsePointerLeft) ||
                        (e.RaycastComp.RayOrigin == EHand.RIGHT && controllersParameters.UsePointerRight);

                    e.ControllerPointerComp._PointerState = isUsingController ? EPointerState.ON : EPointerState.OFF;
                    e.ControllerPointerComp.gameObject.SetActive(isUsingController);
                    e.ControllerPointerComp.IsSetup = true;
                }
            }

            this.Enabled = systemStillRunning;
        }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        #endregion ComponentSystem_Methods


        /// <summary>
        /// Reactivate the System when switching to another Scene.
        /// </summary>
        /// <param name="oldScene">The previous scene before switching</param>
        private void OnSceneLoaded(Scene oldScene, LoadSceneMode loadMode)
        {
            if (loadMode == LoadSceneMode.Single)
            {
                this.Enabled = true;
                foreach (var e in GetEntities<Filter>())
                {
                    e.ControllerPointerComp.IsSetup = false;
                }
            }
        }
    }
}