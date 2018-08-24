using System.Collections.Generic;

namespace VRSF.Inputs.Gaze
{
    /// <summary>
    /// Contain the two dictionnary for all Inputs corresponding to the possible gaze click with Rift
    /// </summary>
    public static class GazeInteractionPortableOVR
    {
        // The dictionary with references to the Rift Buttons
        public static Dictionary<EControllersInput, OVRInput.Button> ClickDictionnary = new Dictionary<EControllersInput, OVRInput.Button>()
        {
            { EControllersInput.NONE, OVRInput.Button.None },

            { EControllersInput.LEFT_TRIGGER, OVRInput.Button.PrimaryIndexTrigger },
            { EControllersInput.RIGHT_TRIGGER, OVRInput.Button.PrimaryIndexTrigger },
            
            { EControllersInput.LEFT_THUMBSTICK, OVRInput.Button.PrimaryTouchpad },
            { EControllersInput.RIGHT_THUMBSTICK, OVRInput.Button.PrimaryTouchpad },

            { EControllersInput.BACK_BUTTON, OVRInput.Button.Back },
        };


        // The dictionary with references to the OVR Touch
        public static Dictionary<EControllersInput, OVRInput.Touch> TouchDictionnary = new Dictionary<EControllersInput, OVRInput.Touch>()
        {
            { EControllersInput.NONE, OVRInput.Touch.None },

            { EControllersInput.LEFT_TRIGGER, OVRInput.Touch.PrimaryIndexTrigger },
            { EControllersInput.RIGHT_TRIGGER, OVRInput.Touch.PrimaryIndexTrigger },

            { EControllersInput.LEFT_THUMBSTICK, OVRInput.Touch.PrimaryTouchpad },
            { EControllersInput.RIGHT_THUMBSTICK, OVRInput.Touch.PrimaryTouchpad },

            // No touch for the back button
        };
    }
}