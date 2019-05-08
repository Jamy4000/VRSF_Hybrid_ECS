using Unity.Entities;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// 
    /// </summary>
    public class ViveControllersInputCaptureSystem : ComponentSystem
    {
        private struct Filter
        {
            public ViveControllersInputCaptureComponent ViveControllersInput;
            public CrossplatformInputCapture InputCapture;
        }

        protected override void OnCreateManager()
        {
            OnSetupVRReady.Listeners += CheckDevice;
            base.OnCreateManager();
        }

        protected override void OnUpdate()
        {
            // If we doesn't use the controllers, we don't check for the inputs.
            if (ControllersParametersVariable.Instance.UseControllers)
            {
                foreach (var e in GetEntities<Filter>())
                {
                    if (e.InputCapture.IsSetup)
                    {
                        // We check the Input for the Right controller
                        CheckRightControllerInput(e.ViveControllersInput);

                        // We check the Input for the Left controller
                        CheckLeftControllerInput(e.ViveControllersInput);
                    }
                }
            }
        }

        protected override void OnDestroyManager()
        {
            OnSetupVRReady.Listeners -= CheckDevice;
            base.OnDestroyManager();
        }

        #region PRIVATE_METHODS
        /// <summary>
        /// Handle the Right Controller input and put them in the Events
        /// </summary>
        private void CheckRightControllerInput(ViveControllersInputCaptureComponent inputCapture)
        {
            #region MENU
            // Check Click Events
            if (Input.GetButtonDown("ViveRightMenuClick"))
            {
                inputCapture.RightMenuClick.SetValue(true);
                new ButtonClickEvent(EHand.RIGHT, EControllersButton.MENU);
            }
            else if (Input.GetButtonUp("ViveRightMenuClick"))
            {
                inputCapture.RightMenuClick.SetValue(false);
                new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.MENU);
            }
            #endregion MENU
        }

        /// <summary>
        /// Handle the Left Controller input and put them in the Events
        /// </summary>
        private void CheckLeftControllerInput(ViveControllersInputCaptureComponent inputCapture)
        {
            #region MENU
            // Check Click Events
            if (Input.GetButtonDown("ViveLeftMenuClick"))
            {
                inputCapture.LeftMenuClick.SetValue(true);
                new ButtonClickEvent(EHand.LEFT, EControllersButton.MENU);
            }
            else if (Input.GetButtonUp("ViveLeftMenuClick"))
            {
                inputCapture.LeftMenuClick.SetValue(false);
                new ButtonUnclickEvent(EHand.LEFT, EControllersButton.MENU);
            }
            #endregion MENU
        }
        
        private void CheckDevice(OnSetupVRReady info)
        {
            this.Enabled = VRSF_Components.DeviceLoaded == EDevice.HTC_VIVE;
        }
        #endregion PRIVATE_METHODS
    }
}
