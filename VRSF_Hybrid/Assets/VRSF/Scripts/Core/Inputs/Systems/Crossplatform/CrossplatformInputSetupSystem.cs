using Unity.Entities;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// System common for the Simulator, Vive and Rift, capture the basic inputs.
    /// </summary>
    public class CrossplatformInputSetupSystem : ComponentSystem
    {
        private struct Filter
        {
            public CrossplatformInputCapture InputCapture;
        }

        protected override void OnStartRunning()
        {
            OnSetupVRReady.Listeners += SetupControllersParameters;
            base.OnStartRunning();
        }

        protected override void OnUpdate() { }

        protected override void OnDestroyManager()
        {
            OnSetupVRReady.Listeners -= SetupControllersParameters;
            base.OnDestroyManager();
        }

        #region PRIVATE_METHODS
        /// <summary>
        /// Setup the two controllers parameters to use in the CheckControllersInput method.
        /// </summary>
        /// <param name="viveInputCapture">The ViveInputCaptureComponent on the CameraRig Entity</param>
        public void SetupControllersParameters(OnSetupVRReady setupVRReady)
        {
            foreach (var e in GetEntities<Filter>())
            {
                // We give the references to the Scriptable variable containers in the Left Parameters variable
                e.InputCapture.LeftParameters = new InputParameters
                {
                    ClickBools = InputVariableContainer.Instance.LeftClickBoolean,
                    TouchBools = InputVariableContainer.Instance.LeftTouchBoolean,
                    ThumbPosition = InputVariableContainer.Instance.LeftThumbPosition
                };

                // We give the references to the Scriptable variable containers in the Right Parameters variable
                e.InputCapture.RightParameters = new InputParameters
                {
                    ClickBools = InputVariableContainer.Instance.RightClickBoolean,
                    TouchBools = InputVariableContainer.Instance.RightTouchBoolean,
                    ThumbPosition = InputVariableContainer.Instance.RightThumbPosition
                };

                e.InputCapture.ControllersParametersSetup = true;
            }
        }
        #endregion PRIVATE_METHODS
    }
}
