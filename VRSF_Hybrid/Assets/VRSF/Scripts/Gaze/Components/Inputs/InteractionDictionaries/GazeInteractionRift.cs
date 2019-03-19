using System.Collections.Generic;
using VRSF.Core.Controllers;
using VRSF.Core.Inputs;
using VRSF.Utils;

namespace VRSF.Gaze.Inputs
{
    /// <summary>
    /// Contain the two dictionnary for all Inputs corresponding to the possible gaze click with Rift
    /// </summary>
    public static class GazeInteractionRift
    {
        // The dictionary with references to the Rift Buttons
        public static Dictionary<STuples<EControllersButton, EHand>, string> ClickDictionnary = new Dictionary<STuples<EControllersButton, EHand>, string>()
        {
            { new STuples<EControllersButton, EHand>(EControllersButton.NONE, EHand.NONE), "NONE" },

            { new STuples<EControllersButton, EHand>(EControllersButton.TRIGGER, EHand.LEFT), "LeftTriggerClick" },
            { new STuples<EControllersButton, EHand>(EControllersButton.TRIGGER, EHand.RIGHT), "RightTriggerClick" },

            { new STuples<EControllersButton, EHand>(EControllersButton.GRIP, EHand.LEFT), "LeftGripClick" },
            { new STuples<EControllersButton, EHand>(EControllersButton.GRIP, EHand.RIGHT), "RightGripClick" },

            { new STuples<EControllersButton, EHand>(EControllersButton.THUMBSTICK, EHand.LEFT), "LeftThumbClick" },
            { new STuples<EControllersButton, EHand>(EControllersButton.THUMBSTICK, EHand.RIGHT), "RightThumbClick" },

            { new STuples<EControllersButton, EHand>(EControllersButton.MENU, EHand.LEFT), "LeftMenuRift" },

            { new STuples<EControllersButton, EHand>(EControllersButton.A_BUTTON, EHand.RIGHT), "Button0Click" },
            { new STuples<EControllersButton, EHand>(EControllersButton.B_BUTTON, EHand.RIGHT), "BButtonClick" },

            { new STuples<EControllersButton, EHand>(EControllersButton.X_BUTTON, EHand.LEFT), "Button2Click" },
            { new STuples<EControllersButton, EHand>(EControllersButton.Y_BUTTON, EHand.LEFT), "YButtonClick" }
        };


        // The dictionary with references to the OVR Touch
        public static Dictionary<STuples<EControllersButton, EHand>, string> TouchDictionnary = new Dictionary<STuples<EControllersButton, EHand>, string>()
        {
            { new STuples<EControllersButton, EHand>(EControllersButton.NONE, EHand.NONE), "NONE"},

            { new STuples<EControllersButton, EHand>(EControllersButton.TRIGGER, EHand.LEFT), "LeftTriggerTouch" },
            { new STuples<EControllersButton, EHand>(EControllersButton.TRIGGER, EHand.RIGHT), "RightTriggerTouch" },

            // The Grip (HandTrigger) is not checking for touch with Oculus

            { new STuples<EControllersButton, EHand>(EControllersButton.THUMBSTICK, EHand.LEFT), "LeftThumbTouch" },
            { new STuples<EControllersButton, EHand>(EControllersButton.THUMBSTICK, EHand.RIGHT), "RightThumbTouch" },

            // Start is not checking for touch with Oculus

            { new STuples<EControllersButton, EHand>(EControllersButton.A_BUTTON, EHand.RIGHT), "AButtonTouch" },
            { new STuples<EControllersButton, EHand>(EControllersButton.B_BUTTON, EHand.RIGHT), "BButtonTouch" },

            { new STuples<EControllersButton, EHand>(EControllersButton.X_BUTTON, EHand.LEFT), "XButtonTouch" },
            { new STuples<EControllersButton, EHand>(EControllersButton.Y_BUTTON, EHand.LEFT), "YButtonTouch" },

            // The Oculus has a ThumbRest touch feature
            { new STuples<EControllersButton, EHand>(EControllersButton.THUMBREST, EHand.LEFT), "LeftThumbrestTouch" },
            { new STuples<EControllersButton, EHand>(EControllersButton.THUMBREST, EHand.RIGHT), "RightThumbrestTouch" }
        };
    }
}