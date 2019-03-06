﻿using EventCallbacks;
using VRSF.Core.Controllers;

namespace VRSF.Core.Inputs
{
    public class ButtonUnclickEvent : Event<ButtonUnclickEvent>
    {
        public readonly EControllersButton ButtonInteracting;
        public readonly EHand HandInteracting;

        public ButtonUnclickEvent(EHand handInteracting, EControllersButton buttonInteracting) : base("The base event to raise when the user stop clicking on a controllers button.")
        {
            ButtonInteracting = buttonInteracting;
            HandInteracting = handInteracting;

            FireEvent(this);
        }
    }
}