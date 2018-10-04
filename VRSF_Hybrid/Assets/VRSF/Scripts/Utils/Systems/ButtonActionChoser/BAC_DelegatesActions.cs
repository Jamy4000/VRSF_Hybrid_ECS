using VRSF.Controllers;
using VRSF.Inputs.Events;
using VRSF.Utils.Components.ButtonActionChoser;

namespace VRSF.Utils.Systems.ButtonActionChoser
{
    public class BAC_DelegatesActions
    {
        public readonly BACGeneralVariablesComponents ButtonActionChoser;

        public BAC_DelegatesActions(BACGeneralVariablesComponents bac)
        {
            ButtonActionChoser = bac;
        }

        
        /// <summary>
        /// Method called when user click the specified button
        /// </summary>
        public void StartActionDown(ButtonClickEvent eventButton)
        {
            // We check if the button clicked is the one set in the ButtonActionChoser comp and that the BAC can be used
            if (ButtonActionChoser.ButtonHand == eventButton.HandInteracting && ButtonActionChoser.ActionButton == eventButton.ButtonInteracting && ButtonActionChoser.CanBeUsed)
            {
                // if we use the Thumb, we need to check its position on the Thumbstick/Touchpad
                if (ButtonActionChoser.ThumbPos != null && ButtonActionChoser.ClickThreshold > 0.0f)
                {
                    ButtonActionChoser.UnclickEventWasRaised = false;

                    switch (ButtonActionChoser.ButtonHand)
                    {
                        case EHand.RIGHT:
                            HandleThumbPosition.CheckThumbPosition(ButtonActionChoser.RightClickThumbPosition, ButtonActionChoser.OnButtonStartClicking, ButtonActionChoser.ClickThreshold, ButtonActionChoser.ThumbPos.Value);
                            break;
                        case EHand.LEFT:
                            HandleThumbPosition.CheckThumbPosition(ButtonActionChoser.LeftClickThumbPosition, ButtonActionChoser.OnButtonStartClicking, ButtonActionChoser.ClickThreshold, ButtonActionChoser.ThumbPos.Value);
                            break;
                    }
                }
                else
                {
                    ButtonActionChoser.OnButtonStartClicking.Invoke();
                }
            }
        }


        /// <summary>
        /// Method called when user release the specified button
        /// </summary>
        public void StartActionUp(ButtonUnclickEvent eventButton)
        {
            // We check if the button clicked is the one set in the ButtonActionChoser comp and that the BAC can be used
            if (ButtonActionChoser.ButtonHand == eventButton.HandInteracting && ButtonActionChoser.ActionButton == eventButton.ButtonInteracting && ButtonActionChoser.CanBeUsed)
            {
                // If we don't use the Thumb
                if (ButtonActionChoser.ThumbPos == null)
                    ButtonActionChoser.OnButtonStopClicking.Invoke();

                // If we use the Thumb and the click action is beyond the threshold
                else if (ButtonActionChoser.ThumbPos != null && ButtonActionChoser.ClickActionBeyondThreshold)
                    ButtonActionChoser.OnButtonStopClicking.Invoke();

                // If we use the Thumb and the ClickThreshold is equal to 0
                else if (ButtonActionChoser.ThumbPos != null && ButtonActionChoser.ClickThreshold == 0.0f)
                    ButtonActionChoser.OnButtonStopClicking.Invoke();
            }
        }


        /// <summary>
        /// Method called when user start touching the specified button
        /// </summary>
        public void StartActionTouched(ButtonTouchEvent eventButton)
        {
            // We check if the button clicked is the one set in the ButtonActionChoser comp and that the BAC can be used
            if (ButtonActionChoser.ButtonHand == eventButton.HandInteracting && ButtonActionChoser.ActionButton == eventButton.ButtonInteracting && ButtonActionChoser.CanBeUsed)
            {
                // if we use the Thumb, we need to check its position on the Thumbstick/Touchpad
                if (ButtonActionChoser.ThumbPos != null && ButtonActionChoser.TouchThreshold > 0.0f)
                {
                    ButtonActionChoser.UntouchedEventWasRaised = false;

                    switch (ButtonActionChoser.ButtonHand)
                    {
                        case EHand.RIGHT:
                            HandleThumbPosition.CheckThumbPosition(ButtonActionChoser.RightTouchThumbPosition, ButtonActionChoser.OnButtonStartTouching, ButtonActionChoser.TouchThreshold, ButtonActionChoser.ThumbPos.Value);
                            break;
                        case EHand.LEFT:
                            HandleThumbPosition.CheckThumbPosition(ButtonActionChoser.LeftTouchThumbPosition, ButtonActionChoser.OnButtonStartTouching, ButtonActionChoser.TouchThreshold, ButtonActionChoser.ThumbPos.Value);
                            break;
                    }
                }
                else
                {
                    ButtonActionChoser.OnButtonStartTouching.Invoke();
                }
            }
        }


        /// <summary>
        /// Method called when user stop touching the specified button
        /// </summary>
        public void StartActionUntouched(ButtonUntouchEvent eventButton)
        {
            // We check if the button clicked is the one set in the ButtonActionChoser comp and that the BAC can be used
            if (ButtonActionChoser.ButtonHand == eventButton.HandInteracting && ButtonActionChoser.ActionButton == eventButton.ButtonInteracting && ButtonActionChoser.CanBeUsed)
            {
                // If we don't use the Thumb
                if (ButtonActionChoser.ThumbPos == null)
                    ButtonActionChoser.OnButtonStopTouching.Invoke();

                // If we use the Thumb and the click action is beyond the threshold
                else if (ButtonActionChoser.ThumbPos != null && ButtonActionChoser.TouchActionBeyondThreshold)
                    ButtonActionChoser.OnButtonStopTouching.Invoke();

                // If we use the Thumb and the ClickThreshold is equal to 0
                else if (ButtonActionChoser.ThumbPos != null && ButtonActionChoser.TouchThreshold == 0.0f)
                    ButtonActionChoser.OnButtonStopTouching.Invoke();
            }
        }

    }
}