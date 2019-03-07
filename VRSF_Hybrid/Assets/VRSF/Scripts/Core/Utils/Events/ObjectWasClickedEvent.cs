using UnityEngine;
using VRSF.Core.Controllers;

namespace VRSF.Core.Events
{
    /// <summary>
    /// Event raised when an object is clicked with the Trigger
    /// </summary>
    public class ObjectWasClickedEvent : EventCallbacks.Event<ObjectWasClickedEvent>
    {
        public readonly Transform ObjectClicked;
        public readonly EHand HandClicking;

        public ObjectWasClickedEvent(EHand handClicking, Transform objectClicked) : base("Event raised when an object is clicked with the Trigger.")
        {
            HandClicking = handClicking;
            ObjectClicked = objectClicked;

            FireEvent(this);
        }
    }
}