using ScriptableFramework.Variables;
using Unity.Entities;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Script attached to the ViveSDK Prefab.
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
            OnCrossplatformComponentIsSetup.Listeners += InitViveInputComp;
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
                        CheckRightControllerInput(e.InputCapture);

                        // We check the Input for the Left controller
                        CheckLeftControllerInput(e.InputCapture);
                    }
                }
            }
        }

        protected override void OnDestroyManager()
        {
            OnSetupVRReady.Listeners -= CheckDevice;
            OnCrossplatformComponentIsSetup.Listeners -= InitViveInputComp;
            base.OnDestroyManager();
        }

        #region PRIVATE_METHODS
        /// <summary>
        /// Handle the Right Controller input and put them in the Events
        /// </summary>
        private void CheckRightControllerInput(CrossplatformInputCapture inputCapture)
        {
            #region MENU
            BoolVariable menuClick = inputCapture.RightParameters.ClickBools.Get("MenuIsDown");

            // Check Click Events
            if (Input.GetButtonDown("ViveRightMenuClick"))
            {
                menuClick.SetValue(true);
                new ButtonClickEvent(EHand.RIGHT, EControllersButton.MENU);
            }
            else if (Input.GetButtonUp("ViveRightMenuClick"))
            {
                menuClick.SetValue(false);
                new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.MENU);
            }
            #endregion MENU
        }

        /// <summary>
        /// Handle the Left Controller input and put them in the Events
        /// </summary>
        private void CheckLeftControllerInput(CrossplatformInputCapture inputCapture)
        {
            #region MENU
            BoolVariable menuClick = inputCapture.LeftParameters.ClickBools.Get("MenuIsDown");

            // Check Click Events
            if (Input.GetButtonDown("ViveLeftMenuClick"))
            {
                menuClick.SetValue(true);
                new ButtonClickEvent(EHand.LEFT, EControllersButton.MENU);
            }
            else if (Input.GetButtonUp("ViveLeftMenuClick"))
            {
                menuClick.SetValue(false);
                new ButtonUnclickEvent(EHand.LEFT, EControllersButton.MENU);
            }
            #endregion MENU
        }
        
        private void CheckDevice(OnSetupVRReady info)
        {
            this.Enabled = VRSF_Components.DeviceLoaded == EDevice.HTC_VIVE;
        }
        
        private void InitViveInputComp(OnCrossplatformComponentIsSetup info)
        {
            foreach (var e in GetEntities<Filter>())
            {
                e.ViveControllersInput.RightMenuClick = e.InputCapture.RightParameters.ClickBools.Get("MenuIsDown");
                e.ViveControllersInput.LeftMenuClick = e.InputCapture.LeftParameters.ClickBools.Get("MenuIsDown");
            }
        }
        #endregion PRIVATE_METHODS
    }
}
