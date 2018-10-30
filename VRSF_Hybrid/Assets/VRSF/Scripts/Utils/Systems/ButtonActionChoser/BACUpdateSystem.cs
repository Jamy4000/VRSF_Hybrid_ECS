using Unity.Entities;
using VRSF.Controllers;
using VRSF.Utils.Components.ButtonActionChoser;

namespace VRSF.Utils.Systems.ButtonActionChoser
{
    public abstract class BACUpdateSystem<T> : ComponentSystem
    {
        public struct Filter
        {
            public BACGeneralComponent BACGeneralComp;
            public BACCalculationsComponent BACCalculationsComp;
            public T InheritedFilter;
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
        /// </summary>
        private void StartActionIsClicking(Filter entity)
        {
            // if we use the Thumb, we need to check its position on the Thumbstick/Touchpad
            if (entity.BACCalculationsComp.ThumbPos != null)
            {
                bool oldState = entity.BACCalculationsComp.ClickActionBeyondThreshold;

                switch (entity.BACGeneralComp.ButtonHand)
                {
                    case EHand.RIGHT:
                        entity.BACCalculationsComp.ClickActionBeyondThreshold = HandleThumbPosition.CheckThumbPosition(entity.BACGeneralComp.RightClickThumbPosition, entity.BACGeneralComp.OnButtonIsClicking, entity.BACGeneralComp.ClickThreshold, entity.BACCalculationsComp.ThumbPos.Value);
                        break;
                    case EHand.LEFT:
                        entity.BACCalculationsComp.ClickActionBeyondThreshold = HandleThumbPosition.CheckThumbPosition(entity.BACGeneralComp.LeftClickThumbPosition, entity.BACGeneralComp.OnButtonIsClicking, entity.BACGeneralComp.ClickThreshold, entity.BACCalculationsComp.ThumbPos.Value);
                        break;
                }

                if (oldState && !entity.BACCalculationsComp.ClickActionBeyondThreshold && !entity.BACCalculationsComp.UnclickEventWasRaised)
                {
                    entity.BACGeneralComp.OnButtonStopClicking.Invoke();
                    entity.BACCalculationsComp.UnclickEventWasRaised = true;
                }
                else
                {
                    entity.BACCalculationsComp.UnclickEventWasRaised = false;
                }
            }
            else
            {
                entity.BACGeneralComp.OnButtonIsClicking.Invoke();
            }
        }


        /// <summary>
        /// Method called when the user stop touching the specified button
        /// </summary>
        private void StartActionIsTouching(Filter entity)
        {
            // if we use the Thumb, we need to check its position on the Thumbstick/Touchpad
            if (entity.BACCalculationsComp.ThumbPos != null)
            {
                bool oldState = entity.BACCalculationsComp.TouchActionBeyondThreshold;

                switch (entity.BACGeneralComp.ButtonHand)
                {
                    case EHand.RIGHT:
                        entity.BACCalculationsComp.TouchActionBeyondThreshold = HandleThumbPosition.CheckThumbPosition(entity.BACGeneralComp.RightTouchThumbPosition, entity.BACGeneralComp.OnButtonIsTouching, entity.BACGeneralComp.TouchThreshold, entity.BACCalculationsComp.ThumbPos.Value);
                        break;
                    case EHand.LEFT:
                        entity.BACCalculationsComp.TouchActionBeyondThreshold = HandleThumbPosition.CheckThumbPosition(entity.BACGeneralComp.LeftTouchThumbPosition, entity.BACGeneralComp.OnButtonIsTouching, entity.BACGeneralComp.TouchThreshold, entity.BACCalculationsComp.ThumbPos.Value);
                        break;
                }

                // If the user was above the threshold, but moved his finger, we invoke the StopTouching Event
                if (oldState && !entity.BACCalculationsComp.TouchActionBeyondThreshold && !entity.BACCalculationsComp.UntouchedEventWasRaised)
                {
                    entity.BACGeneralComp.OnButtonStopTouching.Invoke();
                    entity.BACCalculationsComp.UntouchedEventWasRaised = true;
                }
                else
                {
                    entity.BACCalculationsComp.UntouchedEventWasRaised = false;
                }
            }
            else
            {
                entity.BACGeneralComp.OnButtonIsTouching.Invoke();
            }
        }
        #endregion
    }
}