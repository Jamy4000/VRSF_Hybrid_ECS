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
            base.OnCreateManager();
        }

        protected override void OnUpdate()
        {
            // If we doesn't use the controllers, we don't check for the inputs.
            if (ControllersParametersVariable.Instance.UseControllers)
            {
                foreach (var entity in GetEntities<Filter>())
                {
                    // We check the Input for the Right controller
                    CheckRightControllerInput(entity.InputCapture);

                    // We check the Input for the Left controller
                    CheckLeftControllerInput(entity.InputCapture);
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
        private void CheckRightControllerInput(CrossplatformInputCapture inputCapture)
        {
            #region MENU
            BoolVariable tempClick = inputCapture.RightParameters.ClickBools.Get("MenuIsDown");

            // Check Click Events
            if (Input.GetButtonDown("Button0Click"))
            {
                tempClick.SetValue(true);
                new ButtonClickEvent(EHand.RIGHT, EControllersButton.MENU);
            }
            else if (Input.GetButtonUp("Button0Click"))
            {
                tempClick.SetValue(false);
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
            BoolVariable tempClick = inputCapture.LeftParameters.ClickBools.Get("MenuIsDown");

            // Check Click Events
            if (Input.GetButtonDown("Button2Click"))
            {
                tempClick.SetValue(true);
                new ButtonClickEvent(EHand.LEFT, EControllersButton.MENU);
            }
            else if (Input.GetButtonUp("Button2Click"))
            {
                tempClick.SetValue(false);
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
