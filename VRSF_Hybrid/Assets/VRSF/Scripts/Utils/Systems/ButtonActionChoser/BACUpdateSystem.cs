using Unity.Entities;
using VRSF.Utils.Components.ButtonActionChoser;

namespace VRSF.Utils.Systems.ButtonActionChoser
{
    public abstract class BACUpdateSystem : ComponentSystem
    {
        public struct Filter
        {
            public BACGeneralComponent BACGeneralComp;
            public BACCalculationsComponent BACCalculationsComp;
        }

        // EMPTY
        #region PRIVATE_VARIBALES
        #endregion PRIVATE_VARIABLES


        #region ComponentSystem_Methods
        protected override void OnUpdate()
        {
            foreach (var e in GetEntities<Filter>())
            {
                if (e.BACCalculationsComp.ActionButtonIsReady && e.BACCalculationsComp.CanBeUsed)
                {
                    // If we use the touch event and the user is touching on the button
                    if (e.BACCalculationsComp.IsTouching != null && e.BACCalculationsComp.IsTouching.Value)
                    {
                        StartActionIsTouching(e);
                    }
                    // If we use the click event and the user is clicking on the button
                    if (e.BACCalculationsComp.IsClicking != null && e.BACCalculationsComp.IsClicking.Value)
                    {
                        StartActionIsClicking(e);
                    }
                }
            }
        }
        #endregion


        #region PUBLIC_METHODS
        public abstract void SetupListenersResponses(object entity);
        public abstract void RemoveListenersOnEndApp(object entity);
        #endregion PUBLIC_METHODS


        #region PRIVATE_VARIABLES
        /// <summary>
        /// Method called when user stop touching the specified button
        /// It's virtual as the timer update system doen't need to check if the timer is ready.
        /// </summary>
        public virtual void StartActionIsClicking(Filter entity)
        {
            if (entity.BACGeneralComp.BACTimer != null && !BACTimerUpdateSystem.TimerIsReady(entity.BACGeneralComp.BACTimer))
                return;
            
            // if we use the Thumb, we need to check its position on the Thumbstick/Touchpad
            if (entity.BACCalculationsComp.ThumbPos != null)
            {
                entity.BACCalculationsComp.UnclickEventWasRaised = CheckThumbstick(new ThumstickChecker(entity, Inputs.EControllerInteractionType.CLICK), ref entity.BACCalculationsComp.ClickActionBeyondThreshold);
            }
            else
            {
                entity.BACGeneralComp.OnButtonIsClicking.Invoke();
            }
        }


        /// <summary>
        /// Method called when the user stop touching the specified button.
        /// It's virtual as the timer update system doen't need to check if the timer is ready.
        /// </summary>
        public virtual void StartActionIsTouching(Filter entity)
        {
            if (entity.BACGeneralComp.BACTimer != null && !BACTimerUpdateSystem.TimerIsReady(entity.BACGeneralComp.BACTimer))
                return;

            // if we use the Thumb, we need to check its position on the Thumbstick/Touchpad
            if (entity.BACCalculationsComp.ThumbPos != null)
            {
                entity.BACCalculationsComp.UntouchedEventWasRaised = CheckThumbstick(new ThumstickChecker(entity, Inputs.EControllerInteractionType.TOUCH), ref entity.BACCalculationsComp.TouchActionBeyondThreshold);
            }
            else
            {
                entity.BACGeneralComp.OnButtonIsTouching.Invoke();
            }
        }

        /// <summary>
        /// Check if we use a thumbstick to call this feature. If so, we check the position of the thumbstick, and If not, we raise the event.
        /// </summary>
        /// <param name="entity"></param>
        public bool CheckThumbstick(ThumstickChecker thumbstickChecker, ref bool actionAboveThreshold)
        {
            bool oldState = actionAboveThreshold;

            actionAboveThreshold = HandleThumbPosition.CheckThumbPosition(thumbstickChecker.ThumbPositions[thumbstickChecker.ButtonHand], thumbstickChecker.EventToRaise, thumbstickChecker.InteractionThreshold, thumbstickChecker.ThumbPosValue);
            
            if (oldState && !actionAboveThreshold && !thumbstickChecker.EventWasRaised)
            {
                thumbstickChecker.EventToRaise.Invoke();
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}