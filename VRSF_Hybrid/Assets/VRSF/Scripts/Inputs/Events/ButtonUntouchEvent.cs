using VRSF.Controllers;

namespace VRSF.Inputs.Events
{
    public class ButtonUntouchEvent : ButtonInteractingEvent
    {
        public ButtonUntouchEvent(EHand handInteracting, EControllersButton buttonInteracting) : base(handInteracting, buttonInteracting)
        {
        }
    }
}