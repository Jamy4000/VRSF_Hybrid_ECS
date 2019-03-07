using UnityEngine;
using VRSF.Core.Controllers;

namespace VRSF.Core.Events
{
    /// <summary>
    /// Event raised when an object is hovered by the laser
    /// </summary>
    public class ObjectWasHoveredEvent : EventCallbacks.Event<ObjectWasHoveredEvent>
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