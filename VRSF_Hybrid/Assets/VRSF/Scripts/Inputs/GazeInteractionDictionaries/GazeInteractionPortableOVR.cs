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
        public static Dictionary<STuples<EControllersInput, EHand>, OVRInput.Button> ClickDictionnary = new Dictionary<STuples<EControllersInput, EHand>, OVRInput.Button>()
        {
            { new STuples<EControllersInput, EHand>(EControllersInput.NONE, EHand.NONE), OVRInput.Button.None },

            { new STuples<EControllersInput, EHand>(EControllersInput.TRIGGER, EHand.LEFT), OVRInput.Button.PrimaryIndexTrigger },
            { new STuples<EControllersInput, EHand>(EControllersInput.TRIGGER, EHand.RIGHT), OVRInput.Button.SecondaryIndexTrigger },

            { new STuples<EControllersInput, EHand>(EControllersInput.THUMBSTICK, EHand.LEFT), OVRInput.Button.PrimaryThumbstick },
            { new STuples<EControllersInput, EHand>(EControllersInput.THUMBSTICK, EHand.RIGHT), OVRInput.Button.SecondaryThumbstick },

            { new STuples<EControllersInput, EHand>(EControllersInput.BACK_BUTTON, EHand.LEFT), OVRInput.Button.Back },
            { new STuples<EControllersInput, EHand>(EControllersInput.BACK_BUTTON, EHand.RIGHT), OVRInput.Button.Back },
        };


        // The dictionary with references to the OVR Touch
        public static Dictionary<STuples<EControllersInput, EHand>, OVRInput.Touch> TouchDictionnary = new Dictionary<STuples<EControllersInput, EHand>, OVRInput.Touch>()
        {
            { new STuples<EControllersInput, EHand>(EControllersInput.NONE, EHand.NONE), OVRInput.Touch.None },

            { new STuples<EControllersInput, EHand>(EControllersInput.TRIGGER, EHand.LEFT), OVRInput.Touch.PrimaryIndexTrigger },
            { new STuples<EControllersInput, EHand>(EControllersInput.TRIGGER, EHand.RIGHT), OVRInput.Touch.SecondaryIndexTrigger },

            { new STuples<EControllersInput, EHand>(EControllersInput.THUMBSTICK, EHand.LEFT), OVRInput.Touch.PrimaryThumbstick },
            { new STuples<EControllersInput, EHand>(EControllersInput.THUMBSTICK, EHand.RIGHT), OVRInput.Touch.SecondaryThumbstick },

            // No touch for the back button
        };
    }
}