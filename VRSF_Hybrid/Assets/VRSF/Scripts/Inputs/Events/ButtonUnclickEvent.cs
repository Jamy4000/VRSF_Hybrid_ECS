using VRSF.Controllers;

namespace VRSF.Inputs.Events
{
    public class ButtonUnclickEvent : ButtonInteractingEvent
    {
        public ButtonUnclickEvent(EHand handInteracting, EControllersInput buttonInteracting) : base(handInteracting, buttonInteracting)
        {
        }
    }
}