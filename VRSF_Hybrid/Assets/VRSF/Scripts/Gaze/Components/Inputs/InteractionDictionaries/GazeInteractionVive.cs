using System.Collections.Generic;
using VRSF.Core.Controllers;
using VRSF.Core.Inputs;
using VRSF.Utils;

namespace VRSF.Gaze.Inputs
{
    /// <summary>
    /// Contain the two dictionnary for all Inputs corresponding to the possible gaze click with Vive
    /// </summary>
    public static class GazeInteractionVive
    {
        // The dictionary with references to the Vive Buttons
        public static Dictionary<STuples<EControllersButton, EHand>, string> ClickDictionnary = new Dictionary<STuples<EControllersButton, EHand>, string>()
        {
            { new STuples<EControllersButton, EHand>(EControllersButton.NONE, EHand.NONE), "NONE" },

            { new STuples<EControllersButton, EHand>(EControllersButton.TRIGGER, EHand.LEFT), "LeftTriggerClick" },
            { new STuples<EControllersButton, EHand>(EControllersButton.TRIGGER, EHand.RIGHT), "RightTriggerClick" },

            { new STuples<EControllersButton, EHand>(EControllersButton.GRIP, EHand.LEFT), "LeftGripClick" },
            { new STuples<EControllersButton, EHand>(EControllersButton.GRIP, EHand.RIGHT), "RightGripClick" },

            { new STuples<EControllersButton, EHand>(EControllersButton.THUMBSTICK, EHand.LEFT), "LeftThumbClick" },
            { new STuples<EControllersButton, EHand>(EControllersButton.THUMBSTICK, EHand.RIGHT), "RightThumbClick" },

            { new STuples<EControllersButton, EHand>(EControllersButton.MENU, EHand.LEFT), "Button2Click" },
            { new STuples<EControllersButton, EHand>(EControllersButton.MENU, EHand.RIGHT), "Button0Click" }
        };


        // The dictionary with references to the Vive Touch
        public static Dictionary<STuples<EControllersButton, EHand>, string> TouchDictionnary = new Dictionary<STuples<EControllersButton, EHand>, string>()
        {
            { new STuples<EControllersButton, EHand>(EControllersButton.NONE, EHand.NONE), "NONE"},

            { new STuples<EControllersButton, EHand>(EControllersButton.TRIGGER, EHand.LEFT), "LeftTriggerTouch" },
            { new STuples<EControllersButton, EHand>(EControllersButton.TRIGGER, EHand.RIGHT), "RightTriggerTouch" },
            
            { new STuples<EControllersButton, EHand>(EControllersButton.THUMBSTICK, EHand.LEFT), "LeftThumbTouch" },
            { new STuples<EControllersButton, EHand>(EControllersButton.THUMBSTICK, EHand.RIGHT), "RightThumbTouch" }
        };
    }
}