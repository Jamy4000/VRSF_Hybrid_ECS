using Unity.Entities;
using VRSF.Controllers;
using VRSF.Utils.Components;

namespace VRSF.Utils.Systems.ButtonActionChoser
{
    public class ButtonActionChoserSystem : ComponentSystem
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
                if (e.ButtonComponents.CanBeUsed)
                {
                    _currentComp = e.ButtonComponents;
                    // If we use the touch event and the user is touching on the button
                    if (e.ButtonComponents._isTouching != null && e.ButtonComponents._isTouching.Value && !e.ButtonComponents._untouchedEventWasRaised)
                    {
                        StartActionIsTouching();
                    }
                    // If we use the click event and the user is clicking on the button
                    if (e.ButtonComponents._isClicking != null && e.ButtonComponents._isClicking.Value && !e.ButtonComponents._unclickEventWasRaised)
                    {
                        StartActionIsClicking();
                    }
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
            _currentComp._unclickEventWasRaised = false;

            // if we use the Thumb, we need to check its position on the Thumbstick/Touchpad
            if (_currentComp._thumbPos != null)
            {
                bool oldState = _currentComp._clickActionBeyondThreshold;

                switch (_currentComp._buttonHand)
                {
                    case EHand.RIGHT:
                        _currentComp._clickActionBeyondThreshold = HandleThumbPosition.CheckThumbPosition(_currentComp.RightClickThumbPosition, _currentComp.OnButtonIsClicking, _currentComp.ClickThreshold, _currentComp._thumbPos.Value);
                        break;
                    case EHand.LEFT:
                        _currentComp._clickActionBeyondThreshold = HandleThumbPosition.CheckThumbPosition(_currentComp.LeftClickThumbPosition, _currentComp.OnButtonIsClicking, _currentComp.ClickThreshold, _currentComp._thumbPos.Value);
                        break;
                }

                if (oldState && !_currentComp._clickActionBeyondThreshold)
                {
                    _currentComp.OnButtonStopClicking.Invoke();
                    _currentComp._unclickEventWasRaised = true;
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
            _currentComp._untouchedEventWasRaised = false;

            // if we use the Thumb, we need to check its position on the Thumbstick/Touchpad
            if (_currentComp._thumbPos != null)
            {
                bool oldState = _currentComp._touchActionBeyondThreshold;

                switch (_currentComp._buttonHand)
                {
                    case EHand.RIGHT:
                        _currentComp._touchActionBeyondThreshold = HandleThumbPosition.CheckThumbPosition(_currentComp.RightTouchThumbPosition, _currentComp.OnButtonIsTouching, _currentComp.TouchThreshold, _currentComp._thumbPos.Value);
                        break;
                    case EHand.LEFT:
                        _currentComp._touchActionBeyondThreshold = HandleThumbPosition.CheckThumbPosition(_currentComp.LeftTouchThumbPosition, _currentComp.OnButtonIsTouching, _currentComp.TouchThreshold, _currentComp._thumbPos.Value);
                        break;
                }

                // If the user was above the threshold, but moved his finger, we invoke the StopTouching Event
                if (oldState && !_currentComp._touchActionBeyondThreshold)
                {
                    _currentComp.OnButtonStopTouching.Invoke();
                    _currentComp._untouchedEventWasRaised = true;
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