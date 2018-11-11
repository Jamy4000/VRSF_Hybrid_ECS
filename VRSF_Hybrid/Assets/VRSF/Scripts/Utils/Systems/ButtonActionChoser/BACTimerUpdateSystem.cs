using System.Collections;
using UnityEngine;
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
    public class BACTimerUpdateSystem : BACUpdateSystem
    {
        private new struct Filter
        {
            public BACTimerComponent BACTimer;
            public BACGeneralComponent BAC_Comp;
            public BACCalculationsComponent BAC_Calcul;
        }

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            foreach (var e in GetEntities<Filter>())
            {
                e.BAC_Comp.BACTimer = e.BACTimer;
                SetupListenersResponses(e);
            }
        }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
            foreach (var e in GetEntities<Filter>())
            {
                RemoveListenersOnEndApp(e);
            }
        }

        public override void SetupListenersResponses(object entity)
        {
            var e = (Filter)entity;
            if ((e.BAC_Comp.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                e.BAC_Comp.OnButtonStartClicking.AddListener(delegate { OnStartInteractingCallback(e); });
                e.BAC_Comp.OnButtonIsClicking.AddListener(delegate { IsInteractingCallback(e); });
                e.BAC_Comp.OnButtonStopClicking.AddListener(delegate { e.BAC_Comp.StartCoroutine(OnStopInteractingCallback(e)); });
            }

            if ((e.BAC_Comp.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                e.BAC_Comp.OnButtonStartTouching.AddListener(delegate { OnStartInteractingCallback(e); });
                e.BAC_Comp.OnButtonIsTouching.AddListener(delegate { IsInteractingCallback(e); });
                e.BAC_Comp.OnButtonStopTouching.AddListener(delegate { e.BAC_Comp.StartCoroutine(OnStopInteractingCallback(e)); });
            }
        }

        public override void RemoveListenersOnEndApp(object entity)
        {
            var e = (Filter)entity;
            if ((e.BAC_Comp.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                e.BAC_Comp.OnButtonStartClicking.RemoveAllListeners();
                e.BAC_Comp.OnButtonIsClicking.RemoveAllListeners();
                e.BAC_Comp.OnButtonStopClicking.RemoveAllListeners();
            }

            if ((e.BAC_Comp.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                e.BAC_Comp.OnButtonStartTouching.RemoveAllListeners();
                e.BAC_Comp.OnButtonIsTouching.RemoveAllListeners();
                e.BAC_Comp.OnButtonStopTouching.RemoveAllListeners();
            }
        }


        /// <summary>
        /// Reset the timer to zero on Start Interacting
        /// </summary>
        /// <param name="e"></param>
        private void OnStartInteractingCallback(Filter e)
        {
            // We reset the timers stuffs
            e.BACTimer._Timer = 0.0f;
        }

        /// <summary>
        /// Waiting for one frame on stop interacting so all the systems using on stop interacting 
        /// can finish what they're doing with the final value of the timer.
        /// </summary>
        /// <param name="e">The entity in which the timer is</param>
        /// <returns></returns>
        private IEnumerator OnStopInteractingCallback(Filter e)
        {
            yield return new WaitForEndOfFrame();
            // We reset the timers stuffs
            e.BACTimer._Timer = 0.0f;
        }

        /// <summary>
        /// Update timer based on a fixed unscaled delta time when user interact with the button
        /// </summary>
        /// <param name="e"></param>
        private void IsInteractingCallback(Filter e)
        {
            e.BACTimer._Timer += Time.deltaTime;
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