using EventCallbacks;
using VRSF.Controllers;

namespace VRSF.Inputs.Events
{
    public class ButtonUntouchEvent : Event<ButtonUntouchEvent>
    {
        public readonly EControllersButton ButtonInteracting;
        public readonly EHand HandInteracting;

        public ButtonUntouchEvent(EHand handInteracting, EControllersButton buttonInteracting) : base("The base event to raise when the user stop touching a controllers button.")
        {
            ButtonInteracting = buttonInteracting;
            HandInteracting = handInteracting;

            FireEvent(this);
        }
    }
}