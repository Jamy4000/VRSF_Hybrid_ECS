using EventCallbacks;
using VRSF.Controllers;

namespace VRSF.Inputs.Events
{
    public class ButtonTouchEvent : Event<ButtonTouchEvent>
    {
        public readonly EControllersButton ButtonInteracting;
        public readonly EHand HandInteracting;

        public ButtonTouchEvent(EHand handInteracting, EControllersButton buttonInteracting) : base("The base event to raise when the user start touching a controllers button.")
        {
            ButtonInteracting = buttonInteracting;
            HandInteracting = handInteracting;

            FireEvent(this);
        }
    }
}