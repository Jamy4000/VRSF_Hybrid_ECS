using Unity.Entities;
using VRSF.Controllers;
using VRSF.Utils.Components;

namespace VRSF.Utils.Systems.ButtonActionChoser
{
    public class BACUpdateSystem : ComponentSystem
    {
        struct Filter
        {
            public ButtonActionChoserComponents ButtonComponents;
        }


        #region PRIVATE_VARIBALES
        private ButtonActionChoserComponents _currentComp;
        #endregion PRIVATE_VARIABLES


        #region ComponentSystem_Methods
        protected override void OnUpdate()
        {
            foreach (var e in GetEntities<Filter>())
            {
                _currentComp = e.ButtonComponents;

                // If we use the touch event and the user is touching on the button
                if (e.ButtonComponents.IsTouching != null && e.ButtonComponents.IsTouching.Value && !e.ButtonComponents.UntouchedEventWasRaised)
                {
                    StartActionIsTouching();
                }
                // If we use the click event and the user is clicking on the button
                if (e.ButtonComponents.IsClicking != null && e.ButtonComponents.IsClicking.Value && !e.ButtonComponents.UnclickEventWasRaised)
                {
                    StartActionIsClicking();
                }
            }
        }
        #endregion


        #region PRIVATE_VARIABLES
        /// <summary>
        /// Method called when user stop touching the specified button
        /// </summary>
        private void StartActionIsClicking()
        {
            _currentComp.UnclickEventWasRaised = false;

            // if we use the Thumb, we need to check its position on the Thumbstick/Touchpad
            if (_currentComp.ThumbPos != null)
            {
                bool oldState = _currentComp.ClickActionBeyondThreshold;

                switch (_currentComp.ButtonHand)
                {
                    case EHand.RIGHT:
                        _currentComp.ClickActionBeyondThreshold = HandleThumbPosition.CheckThumbPosition(_currentComp.RightClickThumbPosition, _currentComp.OnButtonIsClicking, _currentComp.ClickThreshold, _currentComp.ThumbPos.Value);
                        break;
                    case EHand.LEFT:
                        _currentComp.ClickActionBeyondThreshold = HandleThumbPosition.CheckThumbPosition(_currentComp.LeftClickThumbPosition, _currentComp.OnButtonIsClicking, _currentComp.ClickThreshold, _currentComp.ThumbPos.Value);
                        break;
                }

                if (oldState && !_currentComp.ClickActionBeyondThreshold)
                {
                    _currentComp.OnButtonStopClicking.Invoke();
                    _currentComp.UnclickEventWasRaised = true;
                }
            }
            else
            {
                _currentComp.OnButtonIsClicking.Invoke();
            }
        }


        /// <summary>
        /// Method called when the user stop touching the specified button
        /// </summary>
        private void StartActionIsTouching()
        {
            _currentComp.UntouchedEventWasRaised = false;

            // if we use the Thumb, we need to check its position on the Thumbstick/Touchpad
            if (_currentComp.ThumbPos != null)
            {
                bool oldState = _currentComp.TouchActionBeyondThreshold;

                switch (_currentComp.ButtonHand)
                {
                    case EHand.RIGHT:
                        _currentComp.TouchActionBeyondThreshold = HandleThumbPosition.CheckThumbPosition(_currentComp.RightTouchThumbPosition, _currentComp.OnButtonIsTouching, _currentComp.TouchThreshold, _currentComp.ThumbPos.Value);
                        break;
                    case EHand.LEFT:
                        _currentComp.TouchActionBeyondThreshold = HandleThumbPosition.CheckThumbPosition(_currentComp.LeftTouchThumbPosition, _currentComp.OnButtonIsTouching, _currentComp.TouchThreshold, _currentComp.ThumbPos.Value);
                        break;
                }

                // If the user was above the threshold, but moved his finger, we invoke the StopTouching Event
                if (oldState && !_currentComp.TouchActionBeyondThreshold)
                {
                    _currentComp.OnButtonStopTouching.Invoke();
                    _currentComp.UntouchedEventWasRaised = true;
                }
            }
            else
            {
                _currentComp.OnButtonIsTouching.Invoke();
            }
        }
        #endregion
    }
}