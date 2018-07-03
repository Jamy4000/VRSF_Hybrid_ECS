using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRSF.Inputs.Gaze
{
    /// <summary>
    /// Contain the two dictionnary for all Inputs corresponding to the possible gaze click with OVR
    /// </summary>
    public static class GazeInteractionDictionaryOVR
    {
        [Header("The dictionary with references to the OVR Buttons")]
        public static Dictionary<EControllersInput, OVRInput.Button> GazeClickOVRInput = new Dictionary<EControllersInput, OVRInput.Button>()
        {
            { EControllersInput.NONE, OVRInput.Button.None },

            { EControllersInput.LEFT_TRIGGER, OVRInput.Button.PrimaryIndexTrigger },
            { EControllersInput.RIGHT_TRIGGER, OVRInput.Button.SecondaryIndexTrigger },

            { EControllersInput.LEFT_GRIP, OVRInput.Button.PrimaryHandTrigger },
            { EControllersInput.RIGHT_GRIP, OVRInput.Button.SecondaryHandTrigger },

            { EControllersInput.LEFT_THUMBSTICK, OVRInput.Button.PrimaryThumbstick },
            { EControllersInput.RIGHT_THUMBSTICK, OVRInput.Button.SecondaryThumbstick },

            { EControllersInput.LEFT_MENU, OVRInput.Button.Start },

            { EControllersInput.A_BUTTON, OVRInput.Button.One },
            { EControllersInput.B_BUTTON, OVRInput.Button.Two },

            { EControllersInput.X_BUTTON, OVRInput.Button.Three },
            { EControllersInput.Y_BUTTON, OVRInput.Button.Four }
        };


        [Header("The dictionary with references to the OVR Touch ")]
        public static Dictionary<EControllersInput, OVRInput.Touch> GazeTouchOVRInput = new Dictionary<EControllersInput, OVRInput.Touch>()
        {
            { EControllersInput.NONE, OVRInput.Touch.None },

            { EControllersInput.LEFT_TRIGGER, OVRInput.Touch.PrimaryIndexTrigger },
            { EControllersInput.RIGHT_TRIGGER, OVRInput.Touch.SecondaryIndexTrigger },
            
            // The Grip (HandTrigger) is not checking for touch with Oculus

            { EControllersInput.LEFT_THUMBSTICK, OVRInput.Touch.PrimaryThumbstick },
            { EControllersInput.RIGHT_THUMBSTICK, OVRInput.Touch.SecondaryThumbstick },

            // Start is not checking for touch with Oculus

            { EControllersInput.A_BUTTON, OVRInput.Touch.One },
            { EControllersInput.B_BUTTON, OVRInput.Touch.Two },

            { EControllersInput.X_BUTTON, OVRInput.Touch.Three },
            { EControllersInput.Y_BUTTON, OVRInput.Touch.Four },
            
            // The Oculus has a ThumbRest touch feature
            { EControllersInput.LEFT_THUMBREST, OVRInput.Touch.PrimaryThumbRest },
            { EControllersInput.RIGHT_THUMBREST, OVRInput.Touch.SecondaryThumbRest }
        };
    }
}