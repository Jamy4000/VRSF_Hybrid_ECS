using ScriptableFramework.Variables;
using Unity.Entities;
using UnityEngine;
using VRSF.Core.Controllers;

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

                    // We check the Input for the Left controller
                    CheckLeftControllerInput(entity.InputCapture);
                }
            }
        }


        #region PRIVATE_METHODS
        /// <summary>
        /// Handle the Left Controller input and put them in the Events
        /// </summary>
        private void CheckLeftControllerInput(CrossplatformInputCapture inputCapture)
        {
            BoolVariable tempClick;
            BoolVariable tempTouch;

            #region TRIGGER
            tempClick = inputCapture.LeftParameters.ClickBools.Get("TriggerIsDown");
            tempTouch = inputCapture.LeftParameters.TouchBools.Get("TriggerIsTouching");

            // Check Click Events
            if (!tempClick.Value && Input.GetAxis("LeftTriggerClick") > 0)
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                new ButtonClickEvent(EHand.LEFT, EControllersButton.TRIGGER);
            }
            else if (tempClick.Value && Input.GetAxis("LeftTriggerClick") == 0)
            {
                tempClick.SetValue(false);
                new ButtonUnclickEvent(EHand.LEFT, EControllersButton.TRIGGER);
            }
            // Check Touch Events if user is not clicking
            else if (!tempClick.Value && !tempTouch.Value && Input.GetButton("LeftTriggerTouch"))
            {
                tempTouch.SetValue(true);
                new ButtonTouchEvent(EHand.LEFT, EControllersButton.TRIGGER);
            }
            else if (tempTouch.Value && !Input.GetButton("LeftTriggerTouch"))
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
            if (!tempClick.Value && Input.GetButton("LeftThumbClick"))
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                new ButtonClickEvent(EHand.LEFT, EControllersButton.THUMBSTICK);
            }
            else if (tempClick.Value && !Input.GetButton("LeftThumbClick"))
            {
                tempClick.SetValue(false);
                new ButtonUnclickEvent(EHand.LEFT, EControllersButton.THUMBSTICK);
            }
            // Check Touch Events if user is not clicking
            else if (!tempClick.Value && !tempTouch.Value && Input.GetButton("LeftThumbTouch"))
            {
                tempTouch.SetValue(true);
                new ButtonTouchEvent(EHand.LEFT, EControllersButton.THUMBSTICK);
            }
            else if (tempTouch.Value && !Input.GetButton("LeftThumbTouch"))
            {
                tempTouch.SetValue(false);
                new ButtonUntouchEvent(EHand.LEFT, EControllersButton.THUMBSTICK);
            }
            #endregion TOUCHPAD

            #region GRIP
            tempClick = inputCapture.LeftParameters.ClickBools.Get("GripIsDown");

            // Check Click Events
            if (!tempClick.Value && Input.GetButton("LeftGripClick"))
            {
                tempClick.SetValue(true);
                new ButtonClickEvent(EHand.LEFT, EControllersButton.GRIP);
            }
            else if (tempClick.Value && !Input.GetButton("LeftGripClick"))
            {
                tempClick.SetValue(false);
                new ButtonUnclickEvent(EHand.LEFT, EControllersButton.GRIP);
            }
            #endregion GRIP
        }
        #endregion PRIVATE_METHODS
    }
}
