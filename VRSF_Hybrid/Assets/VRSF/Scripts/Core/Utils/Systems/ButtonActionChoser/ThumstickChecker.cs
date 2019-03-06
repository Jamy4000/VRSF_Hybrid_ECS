using System.Collections.Generic;
using UnityEngine.Events;
using VRSF.Controllers;
using VRSF.Core.Inputs;
using VRSF.Utils.Components.ButtonActionChoser;

namespace VRSF.Utils.Systems.ButtonActionChoser
{
    public class ThumstickChecker
    {
        public readonly UnityEvent EventToRaise;
        public readonly Dictionary<EHand, EThumbPosition> ThumbPositions;
        public readonly float InteractionThreshold;
        public readonly bool EventWasRaised;
        public readonly EHand ButtonHand;
        public readonly UnityEngine.Vector2 ThumbPosValue;

        public ThumstickChecker(BACGeneralComponent bacGeneral, BACCalculationsComponent bacCalc, EControllerInteractionType interactionType, UnityEvent eventToRaise = null)
        {
            if (interactionType == EControllerInteractionType.CLICK)
            {
                if (eventToRaise != null)
                    EventToRaise = eventToRaise;
                else
                    EventToRaise = bacGeneral.OnButtonIsClicking;

                ThumbPositions = new Dictionary<EHand, EThumbPosition>
                {
                    { EHand.LEFT, bacGeneral.LeftClickThumbPosition },
                    { EHand.RIGHT, bacGeneral.RightClickThumbPosition }
                };
                InteractionThreshold = bacGeneral.ClickThreshold;
                EventWasRaised = bacCalc.UnclickEventWasRaised;
            }
            else if (interactionType == EControllerInteractionType.TOUCH)
            {
                if (eventToRaise != null)
                    EventToRaise = eventToRaise;
                else
                    EventToRaise = bacGeneral.OnButtonIsTouching;

                ThumbPositions = new Dictionary<EHand, EThumbPosition>
                {
                    { EHand.LEFT, bacGeneral.LeftTouchThumbPosition },
                    { EHand.RIGHT, bacGeneral.RightTouchThumbPosition }
                };
                InteractionThreshold = bacGeneral.TouchThreshold;
                EventWasRaised = bacCalc.UntouchedEventWasRaised;
            }

            ButtonHand = bacGeneral.ButtonHand;
            ThumbPosValue = bacCalc.ThumbPos.Value;
        }
    }
}