using System.Collections.Generic;
using VRSF.Controllers;
using VRSF.Utils;

namespace VRSF.Inputs.Gaze
{
    /// <summary>
    /// Contain the two dictionnary for all Inputs corresponding to the possible gaze click with Rift
    /// </summary>
    public static class GazeInteractionRift
    {
        // The dictionary with references to the Rift Buttons
        public static Dictionary<STuples<EControllersInput, EHand>, OVRInput.Button> ClickDictionnary = new Dictionary<STuples<EControllersInput, EHand>, OVRInput.Button>()
        {
            { new STuples<EControllersInput, EHand>(EControllersInput.NONE, EHand.NONE), OVRInput.Button.None },

            { new STuples<EControllersInput, EHand>(EControllersInput.TRIGGER, EHand.LEFT), OVRInput.Button.PrimaryIndexTrigger },
            { new STuples<EControllersInput, EHand>(EControllersInput.TRIGGER, EHand.RIGHT), OVRInput.Button.SecondaryIndexTrigger },

            { new STuples<EControllersInput, EHand>(EControllersInput.GRIP, EHand.LEFT), OVRInput.Button.PrimaryHandTrigger },
            { new STuples<EControllersInput, EHand>(EControllersInput.GRIP, EHand.RIGHT), OVRInput.Button.SecondaryHandTrigger },

            { new STuples<EControllersInput, EHand>(EControllersInput.THUMBSTICK, EHand.LEFT), OVRInput.Button.PrimaryThumbstick },
            { new STuples<EControllersInput, EHand>(EControllersInput.THUMBSTICK, EHand.RIGHT), OVRInput.Button.SecondaryThumbstick },

            { new STuples<EControllersInput, EHand>(EControllersInput.MENU, EHand.LEFT), OVRInput.Button.Start },

            { new STuples<EControllersInput, EHand>(EControllersInput.A_BUTTON, EHand.RIGHT), OVRInput.Button.One },
            { new STuples<EControllersInput, EHand>(EControllersInput.B_BUTTON, EHand.RIGHT), OVRInput.Button.Two },

            { new STuples<EControllersInput, EHand>(EControllersInput.X_BUTTON, EHand.LEFT), OVRInput.Button.Three },
            { new STuples<EControllersInput, EHand>(EControllersInput.Y_BUTTON, EHand.LEFT), OVRInput.Button.Four }
        };


        // The dictionary with references to the OVR Touch
        public static Dictionary<STuples<EControllersInput, EHand>, OVRInput.Touch> TouchDictionnary = new Dictionary<STuples<EControllersInput, EHand>, OVRInput.Touch>()
        {
            { new STuples<EControllersInput, EHand>(EControllersInput.NONE, EHand.NONE), OVRInput.Touch.None },

            { new STuples<EControllersInput, EHand>(EControllersInput.TRIGGER, EHand.LEFT), OVRInput.Touch.PrimaryIndexTrigger },
            { new STuples<EControllersInput, EHand>(EControllersInput.TRIGGER, EHand.RIGHT), OVRInput.Touch.SecondaryIndexTrigger },
            
            // The Grip (HandTrigger) is not checking for touch with Oculus
            
            { new STuples<EControllersInput, EHand>(EControllersInput.THUMBSTICK, EHand.LEFT), OVRInput.Touch.PrimaryThumbstick },
            { new STuples<EControllersInput, EHand>(EControllersInput.THUMBSTICK, EHand.RIGHT), OVRInput.Touch.SecondaryThumbstick },

            // Start is not checking for touch with Oculus
            
            { new STuples<EControllersInput, EHand>(EControllersInput.A_BUTTON, EHand.RIGHT), OVRInput.Touch.One },
            { new STuples<EControllersInput, EHand>(EControllersInput.B_BUTTON, EHand.RIGHT), OVRInput.Touch.Two },

            { new STuples<EControllersInput, EHand>(EControllersInput.X_BUTTON, EHand.LEFT), OVRInput.Touch.Three },
            { new STuples<EControllersInput, EHand>(EControllersInput.Y_BUTTON, EHand.LEFT), OVRInput.Touch.Four },
            
            // The Oculus has a ThumbRest touch feature
            { new STuples<EControllersInput, EHand>(EControllersInput.THUMBREST, EHand.LEFT), OVRInput.Touch.PrimaryThumbRest },
            { new STuples<EControllersInput, EHand>(EControllersInput.THUMBREST, EHand.RIGHT), OVRInput.Touch.SecondaryThumbRest },
        };
    }
}