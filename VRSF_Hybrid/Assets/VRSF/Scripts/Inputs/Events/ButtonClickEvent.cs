using VRSF.Controllers;

namespace VRSF.Inputs.Events
{
    public class ButtonClickEvent : ButtonInteractingEvent
    {
        public ButtonClickEvent(EHand handInteracting, EControllersInput buttonInteracting) : base(handInteracting, buttonInteracting)
        {
        }
    }
}