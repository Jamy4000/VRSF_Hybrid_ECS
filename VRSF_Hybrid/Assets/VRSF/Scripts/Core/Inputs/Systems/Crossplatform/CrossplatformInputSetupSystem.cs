using Unity.Entities;
using UnityEngine;

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

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            SetupControllersParameters();
        }

        protected override void OnUpdate() { }

        #region PRIVATE_METHODS
        /// <summary>
        /// Setup the two controllers parameters to use in the CheckControllersInput method.
        /// </summary>
        /// <param name="viveInputCapture">The ViveInputCaptureComponent on the CameraRig Entity</param>
        public void SetupControllersParameters()
        {
            foreach (var e in GetEntities<Filter>())
            {
                // We give the references to the Scriptable variable containers in the Left Parameters variable
                e.InputCapture.LeftParameters = new InputParameters
                (
                    InputVariableContainer.Instance.LeftClickBoolean, 
                    InputVariableContainer.Instance.LeftTouchBoolean,
                    InputVariableContainer.Instance.LeftThumbPosition,
                    InputVariableContainer.Instance.LeftTriggerSqueezeValue,
                    InputVariableContainer.Instance.LeftGripSqueezeValue
                );

                // We give the references to the Scriptable variable containers in the Right Parameters variable
                e.InputCapture.RightParameters = new InputParameters
                (
                    InputVariableContainer.Instance.RightClickBoolean,
                    InputVariableContainer.Instance.RightTouchBoolean,
                    InputVariableContainer.Instance.RightThumbPosition,
                    InputVariableContainer.Instance.RightTriggerSqueezeValue,
                    InputVariableContainer.Instance.RightGripSqueezeValue
                );

                new OnCrossplatformComponentIsSetup();
                e.InputCapture.IsSetup = true;
            }
        }
        #endregion PRIVATE_METHODS
    }
}
