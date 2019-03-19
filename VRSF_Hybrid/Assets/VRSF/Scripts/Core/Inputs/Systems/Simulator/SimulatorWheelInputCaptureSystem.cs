using Unity.Entities;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Script attached to the SimulatorSDK Prefab.
    /// Set the GameEvent depending on the Keyboard and Mouse Inputs.
    /// You can find a layout of the current mapping in the Wiki of the Repository.
    /// </summary>
    public class SimulatorWheelInputCaptureSystem : ComponentSystem
    {
        /// <summary>
        /// The filter for the entity component.
        /// </summary>
        struct Filter
        {
            public CrossplatformInputCapture CrossplatformInput;
            public SimulatorInputCaptureComponent VRInputCapture;
        }

        #region PRIVATE_VARIABLES
        // VRSF Parameters references
        private InputVariableContainer _inputContainer;
        #endregion PRIVATE_VARIABLES


        #region ComponentSystem_Methods
        /// <summary>
        /// Called after the scene was loaded, setup the entities variables
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnCreateManager()
        {
            OnSetupVRReady.Listeners += CheckDevice;
            _inputContainer = InputVariableContainer.Instance;
            base.OnCreateManager();
        }

        protected override void OnUpdate()
        {
            // If we doesn't use the controllers, we don't check for the inputs.
            if (ControllersParametersVariable.Instance.UseControllers)
            {
                CheckWheelClick();
            }
        }

        protected override void OnDestroyManager()
        {
            OnSetupVRReady.Listeners -= CheckDevice;
            base.OnDestroyManager();
        }
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Handle the click on the Wheel button of the Mouse
        /// </summary>
        private void CheckWheelClick()
        {
            // If the boolVariable for the wheel is clicking is at false but the user is pressing it
            if (Input.GetKeyDown(KeyCode.Mouse2))
            {
                _inputContainer.WheelIsClicking.SetValue(true);
                new ButtonClickEvent(EHand.LEFT, EControllersButton.WHEEL_BUTTON);
            }
            // If the boolVariable for the wheel is clicking is at true but the user is not pressing it
            else if (Input.GetKeyUp(KeyCode.Mouse2))
            {
                _inputContainer.WheelIsClicking.SetValue(false);
                new ButtonUnclickEvent(EHand.LEFT, EControllersButton.WHEEL_BUTTON);
            }
        }

        private void CheckDevice(OnSetupVRReady info)
        {
            this.Enabled = VRSF_Components.DeviceLoaded == EDevice.SIMULATOR;
        }
        #endregion
    }
}