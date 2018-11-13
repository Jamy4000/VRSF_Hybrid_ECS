using System.Collections.Generic;
using UnityEngine.Events;
using VRSF.Controllers;
using VRSF.Inputs;

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

        public ThumstickChecker(BACUpdateSystem.Filter entity, EControllerInteractionType interactionType, UnityEvent eventToRaise = null)
        {
            if (interactionType == EControllerInteractionType.CLICK)
            {
                if (eventToRaise != null)
                    EventToRaise = eventToRaise;
                else
                    EventToRaise = entity.BACGeneralComp.OnButtonIsClicking;

                ThumbPositions = new Dictionary<EHand, EThumbPosition>
                {
                    { EHand.LEFT, entity.BACGeneralComp.LeftClickThumbPosition },
                    { EHand.RIGHT, entity.BACGeneralComp.RightClickThumbPosition }
                };
                InteractionThreshold = entity.BACGeneralComp.ClickThreshold;
                EventWasRaised = entity.BACCalculationsComp.UnclickEventWasRaised;
            }
            else if (interactionType == EControllerInteractionType.TOUCH)
            {
                if (eventToRaise != null)
                    EventToRaise = eventToRaise;
                else
                    EventToRaise = entity.BACGeneralComp.OnButtonIsTouching;

                ThumbPositions = new Dictionary<EHand, EThumbPosition>
                {
                    { EHand.LEFT, entity.BACGeneralComp.LeftTouchThumbPosition },
                    { EHand.RIGHT, entity.BACGeneralComp.RightTouchThumbPosition }
                };
                InteractionThreshold = entity.BACGeneralComp.TouchThreshold;
                EventWasRaised = entity.BACCalculationsComp.UntouchedEventWasRaised;
            }

            ButtonHand = entity.BACGeneralComp.ButtonHand;
            ThumbPosValue = entity.BACCalculationsComp.ThumbPos.Value;
        }
    }
}