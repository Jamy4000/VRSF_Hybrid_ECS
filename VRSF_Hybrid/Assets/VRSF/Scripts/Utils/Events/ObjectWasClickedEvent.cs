using EventCallbacks;
using UnityEngine;
using VRSF.Controllers;

namespace VRSF.Utils.Events
{
    public class ObjectWasClickedEvent : Event<ObjectWasClickedEvent>
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