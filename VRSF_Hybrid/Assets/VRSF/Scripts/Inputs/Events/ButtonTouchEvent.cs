using VRSF.Controllers;

namespace VRSF.Inputs.Events
{
    public class ButtonTouchEvent : ButtonInteractingEvent
    {
        public ButtonTouchEvent(EHand handInteracting, EControllersInput buttonInteracting) : base(handInteracting, buttonInteracting)
        {
        }
    }
}