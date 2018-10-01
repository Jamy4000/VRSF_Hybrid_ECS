using EventCallbacks;
using UnityEngine;
using VRSF.Controllers;

namespace VRSF.Utils.Events
{
    public class ObjectWasHoveredEvent : Event<ObjectWasHoveredEvent>
    {
        public readonly Transform ObjectHovered;
        public readonly EHand HandHovering;

        public ObjectWasHoveredEvent(EHand handHovering, Transform objectHovered) : base("Event raised when an object is hovered by the laser.")
        {
            HandHovering = handHovering;
            ObjectHovered = objectHovered;

            FireEvent(this);
        }
    }
}