using VRSF.Controllers;
using VRSF.Inputs.Events;
using VRSF.Utils.Components.ButtonActionChoser;

namespace VRSF.Utils.Systems.ButtonActionChoser
{
    public class BAC_DelegatesActions
    {
        public readonly BACGeneralComponent BACGeneral;
        public readonly BACCalculationsComponent BACCalculations;

        public BAC_DelegatesActions(BACGeneralComponent bacGeneral, BACCalculationsComponent bacCalcul)
        {
            BACGeneral = bacGeneral;
            BACCalculations = bacCalcul;
        }

        
        /// <summary>
        /// Method called when user click the specified button
        /// </summary>
        public void StartActionDown(ButtonClickEvent eventButton)
        {
            // We check if the button clicked is the one set in the ButtonActionChoser comp and that the BAC can be used
            if (BACGeneral.ButtonHand == eventButton.HandInteracting && BACGeneral.ActionButton == eventButton.ButtonInteracting && BACCalculations.CanBeUsed)
            {
                // if we use the Thumb, we need to check its position on the Thumbstick/Touchpad
                if (BACCalculations.ThumbPos != null && BACGeneral.ClickThreshold > 0.0f)
                {
                    BACCalculations.UnclickEventWasRaised = false;

                    switch (BACGeneral.ButtonHand)
                    {
                        case EHand.RIGHT:
                            HandleThumbPosition.CheckThumbPosition(BACGeneral.RightClickThumbPosition, BACGeneral.OnButtonStartClicking, BACGeneral.ClickThreshold, BACCalculations.ThumbPos.Value);
                            break;
                        case EHand.LEFT:
                            HandleThumbPosition.CheckThumbPosition(BACGeneral.LeftClickThumbPosition, BACGeneral.OnButtonStartClicking, BACGeneral.ClickThreshold, BACCalculations.ThumbPos.Value);
                            break;
                    }
                }
                else
                {
                    BACGeneral.OnButtonStartClicking.Invoke();
                }
            }
        }


        /// <summary>
        /// Method called when user release the specified button
        /// </summary>
        public void StartActionUp(ButtonUnclickEvent eventButton)
        {
            // We check if the button clicked is the one set in the ButtonActionChoser comp and that the BAC can be used
            if (BACGeneral.ButtonHand == eventButton.HandInteracting && BACGeneral.ActionButton == eventButton.ButtonInteracting && BACCalculations.CanBeUsed)
            {
                // If we don't use the Thumb
                if (BACCalculations.ThumbPos == null)
                    BACGeneral.OnButtonStopClicking.Invoke();

                // If we use the Thumb and the click action is beyond the threshold
                else if (BACCalculations.ThumbPos != null && BACCalculations.ClickActionBeyondThreshold)
                    BACGeneral.OnButtonStopClicking.Invoke();

                // If we use the Thumb and the ClickThreshold is equal to 0
                else if (BACCalculations.ThumbPos != null && BACGeneral.ClickThreshold == 0.0f)
                    BACGeneral.OnButtonStopClicking.Invoke();
            }
        }


        /// <summary>
        /// Method called when user start touching the specified button
        /// </summary>
        public void StartActionTouched(ButtonTouchEvent eventButton)
        {
            // We check if the button clicked is the one set in the ButtonActionChoser comp and that the BAC can be used
            if (BACGeneral.ButtonHand == eventButton.HandInteracting && BACGeneral.ActionButton == eventButton.ButtonInteracting && BACCalculations.CanBeUsed)
            {
                // if we use the Thumb, we need to check its position on the Thumbstick/Touchpad
                if (BACCalculations.ThumbPos != null && BACGeneral.TouchThreshold > 0.0f)
                {
                    BACCalculations.UntouchedEventWasRaised = false;

                    switch (BACGeneral.ButtonHand)
                    {
                        case EHand.RIGHT:
                            HandleThumbPosition.CheckThumbPosition(BACGeneral.RightTouchThumbPosition, BACGeneral.OnButtonStartTouching, BACGeneral.TouchThreshold, BACCalculations.ThumbPos.Value);
                            break;
                        case EHand.LEFT:
                            HandleThumbPosition.CheckThumbPosition(BACGeneral.LeftTouchThumbPosition, BACGeneral.OnButtonStartTouching, BACGeneral.TouchThreshold, BACCalculations.ThumbPos.Value);
                            break;
                    }
                }
                else
                {
                    BACGeneral.OnButtonStartTouching.Invoke();
                }
            }
        }


        /// <summary>
        /// Method called when user stop touching the specified button
        /// </summary>
        public void StartActionUntouched(ButtonUntouchEvent eventButton)
        {
            // We check if the button clicked is the one set in the ButtonActionChoser comp and that the BAC can be used
            if (BACGeneral.ButtonHand == eventButton.HandInteracting && BACGeneral.ActionButton == eventButton.ButtonInteracting && BACCalculations.CanBeUsed)
            {
                // If we don't use the Thumb
                if (BACCalculations.ThumbPos == null)
                    BACGeneral.OnButtonStopTouching.Invoke();

                // If we use the Thumb and the click action is beyond the threshold
                else if (BACCalculations.ThumbPos != null && BACCalculations.TouchActionBeyondThreshold)
                    BACGeneral.OnButtonStopTouching.Invoke();

                // If we use the Thumb and the ClickThreshold is equal to 0
                else if (BACCalculations.ThumbPos != null && BACGeneral.TouchThreshold == 0.0f)
                    BACGeneral.OnButtonStopTouching.Invoke();
            }
        }

    }
}