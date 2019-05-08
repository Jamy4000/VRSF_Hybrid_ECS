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
            OnCrossplatformComponentIsSetup.Listeners += InitRiftInputComp;
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
                        CheckRightControllerInput(e.RiftControllersInput);

                        // We check the Input for the Left controller
                        CheckLeftControllerInput(e.RiftControllersInput);
                    }
                }
            }
        }

        protected override void OnDestroyManager()
        {
            OnSetupVRReady.Listeners -= CheckDevice;
            OnCrossplatformComponentIsSetup.Listeners -= InitRiftInputComp;
            base.OnDestroyManager();
        }

        #region PRIVATE_METHODS
        /// <summary>
        /// Handle the Right Controller input and put them in the Events
        /// </summary>
        private void CheckRightControllerInput(RiftControllersInputCaptureComponent inputCapture)
        {
            #region A
            // Check Click Events
            if (Input.GetButtonDown("RiftAButtonClick"))
            {
                inputCapture.AButtonClick.SetValue(true);
                inputCapture.AButtonTouch.SetValue(false);
                new ButtonClickEvent(EHand.RIGHT, EControllersButton.A_BUTTON);
            }
            else if (Input.GetButtonUp("RiftAButtonClick"))
            {
                inputCapture.AButtonClick.SetValue(false);
                new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.A_BUTTON);
            }
            // Check Touch Events if user is not clicking
            else if (!inputCapture.AButtonClick.Value && Input.GetButtonDown("RiftAButtonTouch"))
            {
                inputCapture.AButtonTouch.SetValue(true);
                new ButtonTouchEvent(EHand.RIGHT, EControllersButton.A_BUTTON);
            }
            else if (Input.GetButtonUp("RiftAButtonTouch"))
            {
                inputCapture.AButtonTouch.SetValue(false);
                new ButtonUntouchEvent(EHand.RIGHT, EControllersButton.A_BUTTON);
            }
            #endregion A

            #region B
            // Check Click Events
            if (Input.GetButtonDown("RiftBButtonClick"))
            {
                inputCapture.BButtonClick.SetValue(true);
                inputCapture.BButtonTouch.SetValue(false);
                new ButtonClickEvent(EHand.RIGHT, EControllersButton.B_BUTTON);
            }
            else if (Input.GetButtonUp("RiftBButtonClick"))
            {
                inputCapture.BButtonClick.SetValue(false);
                new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.B_BUTTON);
            }
            // Check Touch Events if user is not clicking
            else if (!inputCapture.BButtonClick.Value && Input.GetButtonDown("RiftBButtonTouch"))
            {
                inputCapture.BButtonTouch.SetValue(true);
                new ButtonTouchEvent(EHand.RIGHT, EControllersButton.B_BUTTON);
            }
            else if (Input.GetButtonUp("RiftBButtonTouch"))
            {
                inputCapture.BButtonTouch.SetValue(false);
                new ButtonUntouchEvent(EHand.RIGHT, EControllersButton.B_BUTTON);
            }
            #endregion B

            #region THUMBREST
            // Check Touch Events
            if (!inputCapture.RightThumbrestTouch.Value && Input.GetButton("RiftRightThumbrestTouch"))
            {
                inputCapture.RightThumbrestTouch.SetValue(true);
                new ButtonTouchEvent(EHand.RIGHT, EControllersButton.THUMBREST);
            }
            else if (Input.GetButtonUp("RiftRightThumbrestTouch"))
            {
                inputCapture.RightThumbrestTouch.SetValue(false);
                new ButtonUntouchEvent(EHand.RIGHT, EControllersButton.THUMBREST);
            }
            #endregion THUMBREST
        }

        /// <summary>
        /// Handle the Left Controller input and put them in the Events
        /// </summary>
        private void CheckLeftControllerInput(RiftControllersInputCaptureComponent inputCapture)
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

        private void InitRiftInputComp(OnCrossplatformComponentIsSetup info)
        {
            foreach (var e in GetEntities<Filter>())
            {
                e.RiftControllersInput.LeftMenuClick = e.InputCapture.LeftParameters.ClickBools.Get("MenuIsDown");

                e.RiftControllersInput.AButtonClick = e.InputCapture.RightParameters.ClickBools.Get("AButtonIsDown");
                e.RiftControllersInput.AButtonTouch = e.InputCapture.RightParameters.TouchBools.Get("AButtonIsTouching");

                e.RiftControllersInput.BButtonClick = e.InputCapture.RightParameters.ClickBools.Get("BButtonIsDown");
                e.RiftControllersInput.BButtonTouch = e.InputCapture.RightParameters.TouchBools.Get("BButtonIsTouching");

                e.RiftControllersInput.XButtonClick = e.InputCapture.LeftParameters.ClickBools.Get("XButtonIsDown");
                e.RiftControllersInput.XButtonTouch = e.InputCapture.LeftParameters.TouchBools.Get("XButtonIsTouching");

                e.RiftControllersInput.YButtonClick = e.InputCapture.LeftParameters.ClickBools.Get("YButtonIsDown");
                e.RiftControllersInput.YButtonTouch = e.InputCapture.LeftParameters.TouchBools.Get("YButtonIsTouching");

                e.RiftControllersInput.RightThumbrestTouch = e.InputCapture.RightParameters.TouchBools.Get("ThumbrestIsTouching");
                e.RiftControllersInput.LeftThumbrestTouch = e.InputCapture.LeftParameters.TouchBools.Get("ThumbrestIsTouching");
            }
        }
        #endregion PRIVATE_METHODS
    }
}
