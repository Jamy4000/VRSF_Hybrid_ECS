using VRSF.Controllers;

namespace VRSF.Inputs.Events
{
    public class ButtonClickEvent : ButtonInteractingEvent
    {
        public ButtonClickEvent(EHand handInteracting, EControllersButton buttonInteracting) : base(handInteracting, buttonInteracting)
        {
        }
    }
}