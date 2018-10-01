using EventCallbacks;
using VRSF.Controllers;

namespace VRSF.Inputs.Events
{
    public class ButtonInteractingEvent : Event<ButtonInteractingEvent>
    {
        public readonly EControllerInteractionType InteractionType;
        public readonly EControllersInput ButtonInteracting;
        public readonly EFingerMovement FingerMovement;
        public readonly EHand HandInteracting;

        public ButtonInteractingEvent(EControllerInteractionType interactionType, EHand handInteracting,
            EControllersInput buttonInteracting, EFingerMovement fingerMovement) 
            : base("The base event to raise when the user make an input on the controllers.") 
        {
            InteractionType = interactionType;
            ButtonInteracting = buttonInteracting;
            FingerMovement = fingerMovement;
            HandInteracting = handInteracting;

            FireEvent(this);
        }
    }
}