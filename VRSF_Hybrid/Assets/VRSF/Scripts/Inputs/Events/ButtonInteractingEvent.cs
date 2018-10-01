using EventCallbacks;
using VRSF.Controllers;

namespace VRSF.Inputs.Events
{
    public class ButtonInteractingEvent : Event<ButtonInteractingEvent>
    {
        public readonly EControllersInput ButtonInteracting;
        public readonly EHand HandInteracting;

        public ButtonInteractingEvent(EHand handInteracting, EControllersInput buttonInteracting) 
            : base("The base event to raise when the user make an input on the controllers.") 
        {
            ButtonInteracting = buttonInteracting;
            HandInteracting = handInteracting;

            FireEvent(this);
        }
    }
}