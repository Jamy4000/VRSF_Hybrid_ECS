using Unity.Entities;
using VRSF.Controllers;
using VRSF.Utils.Components.ButtonActionChoser;

namespace VRSF.Utils.Systems.ButtonActionChoser
{
    public abstract class BACUpdateSystem<T> : ComponentSystem
    {
        public struct Filter
        {
            public BACGeneralVariablesComponents ButtonComponents;
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
                if (e.ButtonComponents.ActionButtonIsReady && e.ButtonComponents.CanBeUsed)
                {
                    // If we use the touch event and the user is touching on the button
                    if (e.ButtonComponents.IsTouching != null && e.ButtonComponents.IsTouching.Value)
                    {
                        StartActionIsTouching(e.ButtonComponents);
                    }
                    // If we use the click event and the user is clicking on the button
                    if (e.ButtonComponents.IsClicking != null && e.ButtonComponents.IsClicking.Value)
                    {
                        StartActionIsClicking(e.ButtonComponents);
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
        private void StartActionIsClicking(BACGeneralVariablesComponents bacComp)
        {
            // if we use the Thumb, we need to check its position on the Thumbstick/Touchpad
            if (bacComp.ThumbPos != null)
            {
                bool oldState = bacComp.ClickActionBeyondThreshold;

                switch (bacComp.ButtonHand)
                {
                    case EHand.RIGHT:
                        bacComp.ClickActionBeyondThreshold = HandleThumbPosition.CheckThumbPosition(bacComp.RightClickThumbPosition, bacComp.OnButtonIsClicking, bacComp.ClickThreshold, bacComp.ThumbPos.Value);
                        break;
                    case EHand.LEFT:
                        bacComp.ClickActionBeyondThreshold = HandleThumbPosition.CheckThumbPosition(bacComp.LeftClickThumbPosition, bacComp.OnButtonIsClicking, bacComp.ClickThreshold, bacComp.ThumbPos.Value);
                        break;
                }

                if (oldState && !bacComp.ClickActionBeyondThreshold && !bacComp.UnclickEventWasRaised)
                {
                    bacComp.OnButtonStopClicking.Invoke();
                    bacComp.UnclickEventWasRaised = true;
                }
                else
                {
                    bacComp.UnclickEventWasRaised = false;
                }
            }
            else
            {
                bacComp.OnButtonIsClicking.Invoke();
            }
        }


        /// <summary>
        /// Method called when the user stop touching the specified button
        /// </summary>
        private void StartActionIsTouching(BACGeneralVariablesComponents bacComp)
        {
            // if we use the Thumb, we need to check its position on the Thumbstick/Touchpad
            if (bacComp.ThumbPos != null)
            {
                bool oldState = bacComp.TouchActionBeyondThreshold;

                switch (bacComp.ButtonHand)
                {
                    case EHand.RIGHT:
                        bacComp.TouchActionBeyondThreshold = HandleThumbPosition.CheckThumbPosition(bacComp.RightTouchThumbPosition, bacComp.OnButtonIsTouching, bacComp.TouchThreshold, bacComp.ThumbPos.Value);
                        break;
                    case EHand.LEFT:
                        bacComp.TouchActionBeyondThreshold = HandleThumbPosition.CheckThumbPosition(bacComp.LeftTouchThumbPosition, bacComp.OnButtonIsTouching, bacComp.TouchThreshold, bacComp.ThumbPos.Value);
                        break;
                }

                // If the user was above the threshold, but moved his finger, we invoke the StopTouching Event
                if (oldState && !bacComp.TouchActionBeyondThreshold && !bacComp.UntouchedEventWasRaised)
                {
                    bacComp.OnButtonStopTouching.Invoke();
                    bacComp.UntouchedEventWasRaised = true;
                }
                else
                {
                    bacComp.UntouchedEventWasRaised = false;
                }
            }
            else
            {
                bacComp.OnButtonIsTouching.Invoke();
            }
        }
        #endregion
    }
}