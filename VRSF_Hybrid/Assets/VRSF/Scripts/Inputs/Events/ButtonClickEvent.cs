using EventCallbacks;
using VRSF.Controllers;

namespace VRSF.Inputs.Events
{
    public class ButtonClickEvent : Event<ButtonClickEvent>
    {
        public readonly EControllersButton ButtonInteracting;
        public readonly EHand HandInteracting;

        public ButtonClickEvent(EHand handInteracting, EControllersButton buttonInteracting) : base("The base event to raise when the user start clicking on a controllers button.")
        {
            ButtonInteracting = buttonInteracting;
            HandInteracting = handInteracting;
            FireEvent(this);
        }
    }
}