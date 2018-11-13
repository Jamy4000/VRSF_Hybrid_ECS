using System;
using System.Collections;
using UnityEngine;
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

        IEnumerator WaitForTimer(Func<bool> toInvoke)
        {
            yield return new WaitForSeconds(BACGeneral.BACTimer.TimerThreshold);
            
            if (BACTimerUpdateSystem.TimerIsReady(BACGeneral.BACTimer))
                toInvoke.Invoke();
        }
        
        /// <summary>
        /// Method called when user click the specified button
        /// </summary>
        public void StartActionDown(ButtonClickEvent eventButton)
        {
            // We check if the button clicked is the one set in the ButtonActionChoser comp and that the BAC can be used
            if (BACGeneral.ButtonHand == eventButton.HandInteracting && BACGeneral.ActionButton == eventButton.ButtonInteracting && BACCalculations.CanBeUsed)
            {
                // Check if we use a timer and if the timer is ready
                if (BACGeneral.BACTimer == null)
                {
                    ActionDown();
                }
                else
                {
                    // We reset the timer
                    BACGeneral.BACTimer._Timer = 0.0f;

                    if (!BACTimerUpdateSystem.TimerIsReady(BACGeneral.BACTimer))
                    {
                        Func<bool> newFunc = new Func<bool>(() => ActionDown());
                        BACGeneral.StartCoroutine(WaitForTimer(newFunc));
                    }
                    else
                    {
                        ActionDown();
                    }
                }
            }


            bool ActionDown()
            {
                // if we use the Thumb, we need to check its position on the Thumbstick/Touchpad
                if (BACCalculations.ThumbPos != null && BACGeneral.ClickThreshold > 0.0f)
                {
                    BACCalculations.UnclickEventWasRaised = false;

                    switch (BACGeneral.ButtonHand)
                    {
                        case EHand.RIGHT:
                            return HandleThumbPosition.CheckThumbPosition(BACGeneral.RightClickThumbPosition, BACGeneral.OnButtonStartClicking, BACGeneral.ClickThreshold, BACCalculations.ThumbPos.Value);
                        case EHand.LEFT:
                            return HandleThumbPosition.CheckThumbPosition(BACGeneral.LeftClickThumbPosition, BACGeneral.OnButtonStartClicking, BACGeneral.ClickThreshold, BACCalculations.ThumbPos.Value);
                    }
                }
                else
                {
                    BACGeneral.OnButtonStartClicking.Invoke();
                    return true;
                }
                return false;
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
                // Check if we use a timer
                if (BACGeneral.BACTimer == null || BACTimerUpdateSystem.TimerIsReady(BACGeneral.BACTimer))
                {
                    ActionUp();
                }
            }

            /// <summary>
            /// Actual Method checking if everwting is ok
            /// </summary>
            bool ActionUp()
            {
                // If we don't use the Thumb
                if (BACCalculations.ThumbPos == null)
                {
                    BACGeneral.OnButtonStopClicking.Invoke();
                    return true;
                }

                // If we use the Thumb and the click action is beyond the threshold
                else if (BACCalculations.ThumbPos != null && BACCalculations.ClickActionBeyondThreshold)
                { 
                    BACGeneral.OnButtonStopClicking.Invoke();
                    return true;
                }

                // If we use the Thumb and the ClickThreshold is equal to 0
                else if (BACCalculations.ThumbPos != null && BACGeneral.ClickThreshold == 0.0f)
                { 
                    BACGeneral.OnButtonStopClicking.Invoke();
                    return true;
                }
                return false;
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
                // Check if we use a timer and if the timer is ready
                if (BACGeneral.BACTimer == null)
                {
                    ActionTouched();
                }
                else
                {
                    // We reset the timer
                    BACGeneral.BACTimer._Timer = 0.0f;

                    if (!BACTimerUpdateSystem.TimerIsReady(BACGeneral.BACTimer))
                    {
                        Func<bool> newFunc = new Func<bool>(() => ActionTouched());
                        BACGeneral.StartCoroutine(WaitForTimer(newFunc));
                    }
                    else
                    {
                        ActionTouched();
                    }
                }
            }

            /// <summary>
            /// Actual Method checking if everwting is ok
            /// </summary>
            bool ActionTouched()
            {
                // if we use the Thumb, we need to check its position on the Thumbstick/Touchpad
                if (BACCalculations.ThumbPos != null && BACGeneral.TouchThreshold > 0.0f)
                {
                    BACCalculations.UntouchedEventWasRaised = false;

                    switch (BACGeneral.ButtonHand)
                    {
                        case EHand.RIGHT:
                            return HandleThumbPosition.CheckThumbPosition(BACGeneral.RightTouchThumbPosition, BACGeneral.OnButtonStartTouching, BACGeneral.TouchThreshold, BACCalculations.ThumbPos.Value);
                        case EHand.LEFT:
                            return HandleThumbPosition.CheckThumbPosition(BACGeneral.LeftTouchThumbPosition, BACGeneral.OnButtonStartTouching, BACGeneral.TouchThreshold, BACCalculations.ThumbPos.Value);
                    }
                }
                else
                {
                    BACGeneral.OnButtonStartTouching.Invoke();
                    return true;
                }
                return false;
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
                // Check if we use a timer and, if so, if the timer is ready
                if (BACGeneral.BACTimer == null || BACTimerUpdateSystem.TimerIsReady(BACGeneral.BACTimer))
                {
                    ActionUntouched();
                }
            }

            /// <summary>
            /// Actual Method checking if everwting is ok
            /// </summary>
            bool ActionUntouched()
            {
                // If we don't use the Thumb
                if (BACCalculations.ThumbPos == null)
                {
                    BACGeneral.OnButtonStopTouching.Invoke();
                    return true;
                }

                // If we use the Thumb and the click action is beyond the threshold
                else if (BACCalculations.ThumbPos != null && BACCalculations.TouchActionBeyondThreshold)
                {
                    BACGeneral.OnButtonStopTouching.Invoke();
                    return true;
                }

                // If we use the Thumb and the ClickThreshold is equal to 0
                else if (BACCalculations.ThumbPos != null && BACGeneral.TouchThreshold == 0.0f)
                {
                    BACGeneral.OnButtonStopTouching.Invoke();
                    return true;
                }
                return false;
            }
        }

    }
}