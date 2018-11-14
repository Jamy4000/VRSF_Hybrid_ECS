using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using VRSF.Inputs;
using VRSF.Utils.Components.ButtonActionChoser;

namespace VRSF.Utils.Systems.ButtonActionChoser
{
    /// <summary>
    /// Handle the update for when you want to delay or stop a Button Action Update System feature before or after a timer.
    /// 
    /// If this timer is updated before the threshold, your BAC feature will launch the BAC events Callbacks
    /// if the user press and release the button before the end of the timer threshold.
    /// 
    /// If this timer is updated after the threshold, your BAC feature will launch the BAC events Callbacks
    /// only after the time specified if the user press the button until the end of the timer threshold.
    /// </summary>
    public class BACTimerUpdateSystem : BACListenersSetupSystem
    {
        struct Filter
        {
            public BACTimerComponent BACTimer;
            public BACGeneralComponent BAC_Comp;
            public BACCalculationsComponent BAC_Calc;
        }

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            foreach (var e in GetEntities<Filter>())
            {
                e.BAC_Comp.BACTimer = e.BACTimer;
                SetupListenersResponses(e);
                // if we use a thumbstick
                if (e.BAC_Comp.ActionButton == EControllersButton.THUMBSTICK)
                {
                    // We create a new event that will be use in the CheckThumbstick method
                    e.BACTimer.ThumbCheckEvent = new UnityEvent();
                    e.BACTimer.ThumbCheckEvent.AddListener(delegate { IsInteractingCallback(e.BACTimer); });
                }
            }
        }
        
        protected override void OnUpdate()
        {
            foreach (var e in GetEntities<Filter>())
            {
                if (e.BAC_Calc.ActionButtonIsReady && e.BAC_Calc.CanBeUsed)
                {
                    // If we use the touch event and the user is touching on the button
                    if (e.BAC_Calc.IsTouching != null && e.BAC_Calc.IsTouching.Value)
                    {
                        StartActionIsTouching(e);
                    }
                    // If we use the click event and the user is clicking on the button
                    if (e.BAC_Calc.IsClicking != null && e.BAC_Calc.IsClicking.Value)
                    {
                        StartActionIsClicking(e);
                    }
                }
            }
        }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
            foreach (var e in GetEntities<Filter>())
            {
                RemoveListenersOnEndApp(e);
                // Remove the listeners for the ThumbCheckEvent if it's not null
                e.BACTimer.ThumbCheckEvent?.RemoveAllListeners();
            }
        }

        public override void SetupListenersResponses(object entity)
        {
            var e = (Filter)entity;
            if ((e.BAC_Comp.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                //e.BAC_Comp.OnButtonStartClicking.AddListener(delegate { OnStartInteractingCallback(e.BACTimer); });
                e.BAC_Comp.OnButtonStopClicking.AddListener(delegate { e.BAC_Comp.StartCoroutine(OnStopInteractingCallback(e.BACTimer)); });
            }

            if ((e.BAC_Comp.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                //e.BAC_Comp.OnButtonStartTouching.AddListener(delegate { OnStartInteractingCallback(e.BACTimer); });
                e.BAC_Comp.OnButtonStopTouching.AddListener(delegate { e.BAC_Comp.StartCoroutine(OnStopInteractingCallback(e.BACTimer)); });
            }
        }

        public override void RemoveListenersOnEndApp(object entity)
        {
            var e = (Filter)entity;
            if ((e.BAC_Comp.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                //e.BAC_Comp.OnButtonStartClicking.RemoveAllListeners();
                e.BAC_Comp.OnButtonStopClicking.RemoveAllListeners();
            }

            if ((e.BAC_Comp.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                //e.BAC_Comp.OnButtonStartTouching.RemoveAllListeners();
                e.BAC_Comp.OnButtonStopTouching.RemoveAllListeners();
            }
        }


        /// <summary>
        /// Reset the timer to zero on Start Interacting
        /// </summary>
        /// <param name="timer"></param>
        //private void OnStartInteractingCallback(BACTimerComponent timer)
        //{
        //    // We reset the timers stuffs
        //    timer._Timer = 0.0f;
        //}

        /// <summary>
        /// Update timer based on a fixed unscaled delta time when user interact with the button
        /// </summary>
        /// <param name="e"></param>
        private void IsInteractingCallback(BACTimerComponent timer)
        {
            timer._Timer += Time.deltaTime;
        }

        /// <summary>
        /// Waiting for one frame on stop interacting so all the systems using on stop interacting 
        /// can finish what they're doing with the final value of the timer.
        /// </summary>
        /// <param name="e">The entity in which the timer is</param>
        /// <returns></returns>
        private IEnumerator OnStopInteractingCallback(BACTimerComponent timer)
        {
            yield return new WaitForEndOfFrame();
            // We reset the timers stuffs
            timer._Timer = 0.0f;
        }


        /// <summary>
        /// Override of StartActionIsClicking as the timer doesn't need to check the presence of a timer or if the timer is ready.
        /// </summary>
        /// <param name="e"></param>
        private void StartActionIsClicking(Filter e)
        {
            // if we use the Thumb, we need to check its position on the Thumbstick/Touchpad
            if (e.BAC_Calc.ThumbPos != null)
            {
                e.BAC_Calc.UnclickEventWasRaised = BACUpdateSystem.CheckThumbstick
                (
                    new ThumstickChecker(e.BAC_Comp, e.BAC_Calc, EControllerInteractionType.CLICK, e.BAC_Comp.BACTimer.ThumbCheckEvent), 
                    ref e.BAC_Calc.ClickActionBeyondThreshold
                );
            }
            else
            {
                IsInteractingCallback(e.BAC_Comp.BACTimer);
            }
        }

        /// <summary>
        /// Override of StartActionIsTouching as the timer doesn't need to check the presence of a timer or if the timer is ready.
        /// </summary>
        /// <param name="e"></param>
        private void StartActionIsTouching(Filter e)
        {
            // if we use the Thumb, we need to check its position on the Thumbstick/Touchpad
            if (e.BAC_Calc.ThumbPos != null)
            {
                e.BAC_Calc.UntouchedEventWasRaised = BACUpdateSystem.CheckThumbstick
                (
                    new ThumstickChecker(e.BAC_Comp, e.BAC_Calc, EControllerInteractionType.TOUCH, e.BAC_Comp.BACTimer.ThumbCheckEvent), 
                    ref e.BAC_Calc.TouchActionBeyondThreshold
                );
            }
            else
            {
                IsInteractingCallback(e.BAC_Comp.BACTimer);
            }
        }


        /// <summary>
        /// Check if the timer is under or above the timing required by the threshold, as specified with the IsUpdatedBeforeThreshold bool
        /// </summary>
        /// <param name="BACTimer">Reference to a BAC Timer component to access its data</param>
        /// <returns>true if the user clicking is under or above the time limit, as specified with the IsUpdatedBeforeThreshold bool</returns>
        public static bool TimerIsReady(BACTimerComponent BACTimer)
        {
            // If the system is updated before the threshold and the timer is inferior to the time limit OR
            // If the system is is updated before the threshold and the timer is superior to the time limit
            return (BACTimer.IsUpdatedBeforeThreshold && BACTimer._Timer < BACTimer.TimerThreshold) ||
                   (!BACTimer.IsUpdatedBeforeThreshold && BACTimer._Timer > BACTimer.TimerThreshold);
        }
    }
}