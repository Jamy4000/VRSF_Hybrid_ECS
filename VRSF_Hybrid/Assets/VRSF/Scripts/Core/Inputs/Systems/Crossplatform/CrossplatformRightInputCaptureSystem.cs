using ScriptableFramework.Variables;
using Unity.Entities;
using UnityEngine;
using VRSF.Core.Controllers;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// System common for the Simulator, Vive and Rift, capture the basic inputs for the right controllers
    /// </summary>
    public class CrossplatformRightInputCaptureSystem : ComponentSystem
    {
        private struct Filter
        {
            public CrossplatformInputCapture InputCapture;
        }

        protected override void OnCreateManager()
        {
            base.OnCreateManager();
            SetupControllersParameters();
        }

        protected override void OnUpdate()
        {
            // If we doesn't use the controllers, we don't check for the inputs.
            if (ControllersParametersVariable.Instance.UseControllers)
            {
                foreach (var entity in GetEntities<Filter>())
                {
                    // If the Setup for the controllers is not setup, we wait
                    if (!entity.InputCapture.ControllersParametersSetup)
                        return;

                    // We check the Input for the Right controller
                    CheckRightControllerInput(entity.InputCapture);
                }
            }
        }


        #region PRIVATE_METHODS
        /// <summary>
        /// Handle the Left Controller input and put them in the Events
        /// </summary>
        private void CheckRightControllerInput(CrossplatformInputCapture inputCapture)
        {
            #region TRIGGER
            BoolVariable tempClick = inputCapture.RightParameters.ClickBools.Get("TriggerIsDown");
            BoolVariable tempTouch = inputCapture.RightParameters.TouchBools.Get("TriggerIsTouching");

            // Check Click Events
            if (!tempClick.Value && Input.GetAxis("RightTriggerClick") > 0)
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                new ButtonClickEvent(EHand.RIGHT, EControllersButton.TRIGGER);
            }
            else if (tempClick.Value && Input.GetAxis("RightTriggerClick") == 0)
            {
                tempClick.SetValue(false);
                new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.TRIGGER);
            }
            // Check Touch Events if user is not clicking
            else if (!tempClick.Value && !tempTouch.Value && Input.GetButton("LeftTriggerTouch"))
            {
                tempTouch.SetValue(true);
                new ButtonTouchEvent(EHand.RIGHT, EControllersButton.TRIGGER);
            }
            else if (tempTouch.Value && !Input.GetButton("LeftTriggerTouch"))
            {
                tempTouch.SetValue(false);
                new ButtonUntouchEvent(EHand.RIGHT, EControllersButton.TRIGGER);
            }
            #endregion TRIGGER

            #region TOUCHPAD
            inputCapture.RightParameters.ThumbPosition.SetValue(new Vector2(Input.GetAxis("HorizontalRight"), Input.GetAxis("VerticalRight")));

            tempClick = inputCapture.RightParameters.ClickBools.Get("ThumbIsDown");
            tempTouch = inputCapture.RightParameters.TouchBools.Get("ThumbIsTouching");

            // Check Click Events
            if (!tempClick.Value && Input.GetButton("RightThumbClick"))
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                new ButtonClickEvent(EHand.RIGHT, EControllersButton.THUMBSTICK);
            }
            else if (tempClick.Value && !Input.GetButton("RightThumbClick"))
            {
                tempClick.SetValue(false);
                new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.THUMBSTICK);
            }
            // Check Touch Events if user is not clicking
            else if (!tempClick.Value && !tempTouch.Value && Input.GetButton("RightThumbTouch"))
            {
                tempTouch.SetValue(true);
                new ButtonTouchEvent(EHand.RIGHT, EControllersButton.THUMBSTICK);
            }
            else if (tempTouch.Value && !Input.GetButton("RightThumbTouch"))
            {
                tempTouch.SetValue(false);
                new ButtonUntouchEvent(EHand.RIGHT, EControllersButton.THUMBSTICK);
            }
            #endregion TOUCHPAD

            #region GRIP
            tempClick = inputCapture.RightParameters.ClickBools.Get("GripIsDown");

            // Check Click Events
            if (!tempClick.Value && Input.GetButton("RightGripClick"))
            {
                tempClick.SetValue(true);
                new ButtonClickEvent(EHand.RIGHT, EControllersButton.GRIP);
            }
            else if (tempClick.Value && !Input.GetButton("RightGripClick"))
            {
                tempClick.SetValue(false);
                new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.GRIP);
            }
            #endregion GRIP
        }


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

