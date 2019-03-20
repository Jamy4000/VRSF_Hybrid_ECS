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
            base.OnDestroyManager();
        }

        #region PRIVATE_METHODS
        /// <summary>
        /// Handle the Right Controller input and put them in the Events
        /// </summary>
        private void CheckRightControllerInput(CrossplatformInputCapture inputCapture)
        {
            #region A
            BoolVariable tempClick = inputCapture.RightParameters.ClickBools.Get("AButtonIsDown");
            BoolVariable tempTouch = inputCapture.RightParameters.TouchBools.Get("AButtonIsTouching");

            // Check Click Events
            if (Input.GetButtonDown("Button0Click"))
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                new ButtonClickEvent(EHand.RIGHT, EControllersButton.A_BUTTON);
            }
            else if (Input.GetButtonUp("Button0Click"))
            {
                tempClick.SetValue(false);
                new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.A_BUTTON);
            }
            // Check Touch Events if user is not clicking
            else if (!tempClick.Value && Input.GetButtonDown("AButtonTouch"))
            {
                tempTouch.SetValue(true);
                new ButtonTouchEvent(EHand.RIGHT, EControllersButton.A_BUTTON);
            }
            else if (Input.GetButtonUp("AButtonTouch"))
            {
                tempTouch.SetValue(false);
                new ButtonUntouchEvent(EHand.RIGHT, EControllersButton.A_BUTTON);
            }
            #endregion A

            #region B
            tempClick = inputCapture.RightParameters.ClickBools.Get("BButtonIsDown");
            tempTouch = inputCapture.RightParameters.TouchBools.Get("BButtonIsTouching");

            // Check Click Events
            if (Input.GetButtonDown("BButtonClick"))
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                new ButtonClickEvent(EHand.RIGHT, EControllersButton.B_BUTTON);
            }
            else if (Input.GetButtonUp("BButtonClick"))
            {
                tempClick.SetValue(false);
                new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.B_BUTTON);
            }
            // Check Touch Events if user is not clicking
            else if (!tempClick.Value && Input.GetButtonDown("BButtonTouch"))
            {
                tempTouch.SetValue(true);
                new ButtonTouchEvent(EHand.RIGHT, EControllersButton.B_BUTTON);
            }
            else if (Input.GetButtonUp("BButtonTouch"))
            {
                tempTouch.SetValue(false);
                new ButtonUntouchEvent(EHand.RIGHT, EControllersButton.B_BUTTON);
            }
            #endregion B

            #region THUMBREST
            tempTouch = inputCapture.RightParameters.TouchBools.Get("ThumbrestIsTouching");

            // Check Touch Events
            if (!tempTouch.Value && Input.GetButton("RightThumbrestTouch"))
            {
                tempTouch.SetValue(true);
                new ButtonTouchEvent(EHand.RIGHT, EControllersButton.THUMBREST);
            }
            else if (Input.GetButtonUp("RightThumbrestTouch"))
            {
                tempTouch.SetValue(false);
                new ButtonUntouchEvent(EHand.RIGHT, EControllersButton.THUMBREST);
            }
            #endregion THUMBREST
        }

        /// <summary>
        /// Handle the Left Controller input and put them in the Events
        /// </summary>
        private void CheckLeftControllerInput(CrossplatformInputCapture inputCapture)
        {
            #region X
            BoolVariable tempClick = inputCapture.LeftParameters.ClickBools.Get("XButtonIsDown");
            BoolVariable tempTouch = inputCapture.LeftParameters.TouchBools.Get("XButtonIsTouching");

            // Check Click Events
            if (Input.GetButtonDown("Button2Click"))
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                new ButtonClickEvent(EHand.LEFT, EControllersButton.X_BUTTON);
            }
            else if (Input.GetButtonUp("Button2Click"))
            {
                tempClick.SetValue(false);
                new ButtonUnclickEvent(EHand.LEFT, EControllersButton.X_BUTTON);
            }
            // Check Touch Events if user is not clicking
            else if (!tempClick.Value && Input.GetButtonDown("XButtonTouch"))
            {
                tempTouch.SetValue(true);
                new ButtonTouchEvent(EHand.LEFT, EControllersButton.X_BUTTON);
            }
            else if (Input.GetButtonUp("XButtonTouch"))
            {
                tempTouch.SetValue(false);
                new ButtonUntouchEvent(EHand.LEFT, EControllersButton.X_BUTTON);
            }
            #endregion X

            #region Y
            tempClick = inputCapture.LeftParameters.ClickBools.Get("YButtonIsDown");
            tempTouch = inputCapture.LeftParameters.TouchBools.Get("YButtonIsTouching");

            // Check Click Events
            if (Input.GetButtonDown("YButtonClick"))
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                new ButtonClickEvent(EHand.LEFT, EControllersButton.Y_BUTTON);
            }
            else if (Input.GetButtonUp("YButtonClick"))
            {
                tempClick.SetValue(false);
                new ButtonUnclickEvent(EHand.LEFT, EControllersButton.Y_BUTTON);
            }
            // Check Touch Events if user is not clicking
            else if (!tempClick.Value && Input.GetButtonDown("YButtonTouch"))
            {
                tempTouch.SetValue(true);
                new ButtonTouchEvent(EHand.LEFT, EControllersButton.Y_BUTTON);
            }
            else if (Input.GetButtonUp("YButtonTouch"))
            {
                tempTouch.SetValue(false);
                new ButtonUntouchEvent(EHand.LEFT, EControllersButton.Y_BUTTON);
            }
            #endregion Y

            #region MENU
            tempClick = inputCapture.LeftParameters.ClickBools.Get("MenuIsDown");
            
            // Check Click Events
            if (Input.GetButtonDown("LeftMenuRift"))
            {
                tempClick.SetValue(true);
                new ButtonClickEvent(EHand.LEFT, EControllersButton.MENU);
            }
            else if (Input.GetButtonUp("LeftMenuRift"))
            {
                tempClick.SetValue(false);
                new ButtonUnclickEvent(EHand.LEFT, EControllersButton.MENU);
            }
            #endregion MENU

            #region THUMBREST
            tempTouch = inputCapture.LeftParameters.TouchBools.Get("ThumbrestIsTouching");

            // Check Touch Events
            if (!tempTouch.Value && Input.GetButton("LeftThumbrestTouch"))
            {
                tempTouch.SetValue(true);
                new ButtonTouchEvent(EHand.LEFT, EControllersButton.THUMBREST);
            }
            else if (Input.GetButtonUp("LeftThumbrestTouch"))
            {
                tempTouch.SetValue(false);
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
