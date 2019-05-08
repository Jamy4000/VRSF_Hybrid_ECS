using Unity.Entities;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// 
    /// </summary>
    public class RiftControllersInputCaptureSystem : ComponentSystem
    {
        private struct Filter
        {
            public RiftControllersInputCaptureComponent RiftControllersInput;
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
                        // We check the Input for the Left controller
                        CheckControllerInput(e.RiftControllersInput);
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
        /// Handle the Left Controller input and put them in the Events
        /// </summary>
        private void CheckControllerInput(RiftControllersInputCaptureComponent inputCapture)
        {
            #region X
            // Check Click Events
            if (Input.GetButtonDown("RiftXButtonClick"))
            {
                inputCapture.XButtonClick.SetValue(true);
                inputCapture.XButtonTouch.SetValue(false);
                new ButtonClickEvent(EHand.LEFT, EControllersButton.X_BUTTON);
            }
            else if (Input.GetButtonUp("RiftXButtonClick"))
            {
                inputCapture.XButtonClick.SetValue(false);
                new ButtonUnclickEvent(EHand.LEFT, EControllersButton.X_BUTTON);
            }
            // Check Touch Events if user is not clicking
            else if (!inputCapture.XButtonClick.Value && Input.GetButtonDown("RiftXButtonTouch"))
            {
                inputCapture.XButtonTouch.SetValue(true);
                new ButtonTouchEvent(EHand.LEFT, EControllersButton.X_BUTTON);
            }
            else if (Input.GetButtonUp("RiftXButtonTouch"))
            {
                inputCapture.XButtonTouch.SetValue(false);
                new ButtonUntouchEvent(EHand.LEFT, EControllersButton.X_BUTTON);
            }
            #endregion X

            #region Y
            // Check Click Events
            if (Input.GetButtonDown("RiftYButtonClick"))
            {
                inputCapture.YButtonClick.SetValue(true);
                inputCapture.YButtonTouch.SetValue(false);
                new ButtonClickEvent(EHand.LEFT, EControllersButton.Y_BUTTON);
            }
            else if (Input.GetButtonUp("RiftYButtonClick"))
            {
                inputCapture.YButtonClick.SetValue(false);
                new ButtonUnclickEvent(EHand.LEFT, EControllersButton.Y_BUTTON);
            }
            // Check Touch Events if user is not clicking
            else if (!inputCapture.YButtonClick.Value && Input.GetButtonDown("RiftYButtonTouch"))
            {
                inputCapture.YButtonTouch.SetValue(true);
                new ButtonTouchEvent(EHand.LEFT, EControllersButton.Y_BUTTON);
            }
            else if (Input.GetButtonUp("RiftYButtonTouch"))
            {
                inputCapture.YButtonTouch.SetValue(false);
                new ButtonUntouchEvent(EHand.LEFT, EControllersButton.Y_BUTTON);
            }
            #endregion Y

            #region MENU
            // Check Click Events
            if (Input.GetButtonDown("RiftMenuButtonClick"))
            {
                inputCapture.LeftMenuClick.SetValue(true);
                new ButtonClickEvent(EHand.LEFT, EControllersButton.MENU);
            }
            else if (Input.GetButtonUp("RiftMenuButtonClick"))
            {
                inputCapture.LeftMenuClick.SetValue(false);
                new ButtonUnclickEvent(EHand.LEFT, EControllersButton.MENU);
            }
            #endregion MENU

            #region THUMBREST
            // Check Touch Events
            if (!inputCapture.LeftThumbrestTouch.Value && Input.GetButton("RiftLeftThumbrestTouch"))
            {
                inputCapture.LeftThumbrestTouch.SetValue(true);
                new ButtonTouchEvent(EHand.LEFT, EControllersButton.THUMBREST);
            }
            else if (Input.GetButtonUp("RiftLeftThumbrestTouch"))
            {
                inputCapture.LeftThumbrestTouch.SetValue(false);
                new ButtonUntouchEvent(EHand.LEFT, EControllersButton.THUMBREST);
            }
            #endregion THUMBREST
        }

        private void CheckDevice(OnSetupVRReady info)
        {
            this.Enabled = VRSF_Components.DeviceLoaded == EDevice.OCULUS_RIFT;
        }
        #endregion PRIVATE_METHODS
    }
}
