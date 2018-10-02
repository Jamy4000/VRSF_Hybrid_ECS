using VRSF.Controllers;

namespace VRSF.Inputs.Events
{
    public class ButtonTouchEvent : ButtonInteractingEvent
    {
        public ButtonTouchEvent(EHand handInteracting, EControllersButton buttonInteracting) : base(handInteracting, buttonInteracting)
        {
        }
    }
}