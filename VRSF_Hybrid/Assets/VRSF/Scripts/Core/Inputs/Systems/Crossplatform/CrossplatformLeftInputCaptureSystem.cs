using ScriptableFramework.Variables;
using Unity.Entities;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// System common for the Simulator, Vive and Rift, capture the basic inputs for the left controllers
    /// </summary>
    public class CrossplatformLeftInputCaptureSystem : ComponentSystem
    {
        private struct Filter
        {
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
                        // We check the Input for the Left controller
                        CheckLeftControllerInput(e.InputCapture);
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
        private void CheckLeftControllerInput(CrossplatformInputCapture inputCapture)
        {
            #region TRIGGER
            BoolVariable tempClick = inputCapture.LeftParameters.ClickBools.Get("TriggerIsDown");
            BoolVariable tempTouch = inputCapture.LeftParameters.TouchBools.Get("TriggerIsTouching");

            // Check Click Events
            if (!tempClick.Value && Input.GetAxis("LeftTriggerClick") > 0.95f)
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                new ButtonClickEvent(EHand.LEFT, EControllersButton.TRIGGER);
            }
            else if (tempClick.Value && Input.GetAxis("LeftTriggerClick") < 0.95f)
            {
                tempClick.SetValue(false);
                new ButtonUnclickEvent(EHand.LEFT, EControllersButton.TRIGGER);
            }
            // Check Touch Events if user is not clicking
            else if (Input.GetButtonDown("LeftTriggerTouch"))
            {
                tempTouch.SetValue(true);
                new ButtonTouchEvent(EHand.LEFT, EControllersButton.TRIGGER);
            }
            else if (Input.GetButtonUp("LeftTriggerTouch"))
            {
                tempTouch.SetValue(false);
                new ButtonUntouchEvent(EHand.LEFT, EControllersButton.TRIGGER);
            }
            #endregion TRIGGER

            #region TOUCHPAD
            inputCapture.LeftParameters.ThumbPosition.SetValue(new Vector2(Input.GetAxis("HorizontalLeft"), Input.GetAxis("VerticalLeft")));

            tempClick = inputCapture.LeftParameters.ClickBools.Get("ThumbIsDown");
            tempTouch = inputCapture.LeftParameters.TouchBools.Get("ThumbIsTouching");

            // Check Click Events
            if (Input.GetButtonDown("LeftThumbClick"))
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                new ButtonClickEvent(EHand.LEFT, EControllersButton.THUMBSTICK);
            }
            else if (Input.GetButtonUp("LeftThumbClick"))
            {
                tempClick.SetValue(false);
                new ButtonUnclickEvent(EHand.LEFT, EControllersButton.THUMBSTICK);
            }
            // Check Touch Events if user is not clicking
            else if (!tempClick.Value && Input.GetButtonDown("LeftThumbTouch"))
            {
                tempTouch.SetValue(true);
                new ButtonTouchEvent(EHand.LEFT, EControllersButton.THUMBSTICK);
            }
            else if (Input.GetButtonUp("LeftThumbTouch"))
            {
                tempTouch.SetValue(false);
                new ButtonUntouchEvent(EHand.LEFT, EControllersButton.THUMBSTICK);
            }
            #endregion TOUCHPAD

            #region GRIP
            tempClick = inputCapture.LeftParameters.ClickBools.Get("GripIsDown");

            // Check Click Events
            if (!tempClick.Value && Input.GetAxis("LeftGripClick") > 0.95f)
            {
                tempClick.SetValue(true);
                new ButtonClickEvent(EHand.LEFT, EControllersButton.GRIP);
            }
            else if (tempClick.Value && Input.GetAxis("LeftGripClick") == 0)
            {
                tempClick.SetValue(false);
                new ButtonUnclickEvent(EHand.LEFT, EControllersButton.GRIP);
            }
            #endregion GRIP
        }

        private void CheckDevice(OnSetupVRReady info)
        {
            this.Enabled = VRSF_Components.DeviceLoaded != EDevice.SIMULATOR;
        }
        #endregion PRIVATE_METHODS
    }
}
