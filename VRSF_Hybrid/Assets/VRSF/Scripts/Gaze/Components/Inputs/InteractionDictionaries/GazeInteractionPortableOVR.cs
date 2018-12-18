using System.Collections.Generic;
using VRSF.Controllers;
using VRSF.Utils;

namespace VRSF.Inputs.Gaze
{
    /// <summary>
    /// Contain the two dictionnary for all Inputs corresponding to the possible gaze click with Rift
    /// </summary>
    public static class GazeInteractionPortableOVR
    {
        // The dictionary with references to the Rift Buttons
        public static Dictionary<STuples<EControllersButton, EHand>, OVRInput.Button> ClickDictionnary = new Dictionary<STuples<EControllersButton, EHand>, OVRInput.Button>()
        {
            { new STuples<EControllersButton, EHand>(EControllersButton.NONE, EHand.NONE), OVRInput.Button.None },

            { new STuples<EControllersButton, EHand>(EControllersButton.TRIGGER, EHand.LEFT), OVRInput.Button.PrimaryIndexTrigger },
            { new STuples<EControllersButton, EHand>(EControllersButton.TRIGGER, EHand.RIGHT), OVRInput.Button.SecondaryIndexTrigger },

            { new STuples<EControllersButton, EHand>(EControllersButton.THUMBSTICK, EHand.LEFT), OVRInput.Button.PrimaryThumbstick },
            { new STuples<EControllersButton, EHand>(EControllersButton.THUMBSTICK, EHand.RIGHT), OVRInput.Button.SecondaryThumbstick },

            { new STuples<EControllersButton, EHand>(EControllersButton.BACK_BUTTON, EHand.LEFT), OVRInput.Button.Back },
            { new STuples<EControllersButton, EHand>(EControllersButton.BACK_BUTTON, EHand.RIGHT), OVRInput.Button.Back },
        };


        // The dictionary with references to the OVR Touch
        public static Dictionary<STuples<EControllersButton, EHand>, OVRInput.Touch> TouchDictionnary = new Dictionary<STuples<EControllersButton, EHand>, OVRInput.Touch>()
        {
            { new STuples<EControllersButton, EHand>(EControllersButton.NONE, EHand.NONE), OVRInput.Touch.None },

            { new STuples<EControllersButton, EHand>(EControllersButton.TRIGGER, EHand.LEFT), OVRInput.Touch.PrimaryIndexTrigger },
            { new STuples<EControllersButton, EHand>(EControllersButton.TRIGGER, EHand.RIGHT), OVRInput.Touch.SecondaryIndexTrigger },

            { new STuples<EControllersButton, EHand>(EControllersButton.THUMBSTICK, EHand.LEFT), OVRInput.Touch.PrimaryThumbstick },
            { new STuples<EControllersButton, EHand>(EControllersButton.THUMBSTICK, EHand.RIGHT), OVRInput.Touch.SecondaryThumbstick },

            // No touch for the back button
        };
    }
}