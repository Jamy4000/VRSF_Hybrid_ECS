using System.Collections.Generic;
using VRSF.Controllers;
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
        public static Dictionary<STuples<EControllersButton, EHand>, /* TODO OVRInput.Button*/bool> ClickDictionnary = new Dictionary<STuples<EControllersButton, EHand>, /* TODO OVRInput.Button*/bool>()
        {
            // TODO
            //{ new STuples<EControllersButton, EHand>(EControllersButton.NONE, EHand.NONE), OVRInput.Button.None },

            //{ new STuples<EControllersButton, EHand>(EControllersButton.TRIGGER, EHand.LEFT), OVRInput.Button.PrimaryIndexTrigger },
            //{ new STuples<EControllersButton, EHand>(EControllersButton.TRIGGER, EHand.RIGHT), OVRInput.Button.SecondaryIndexTrigger },

            //{ new STuples<EControllersButton, EHand>(EControllersButton.GRIP, EHand.LEFT), OVRInput.Button.PrimaryHandTrigger },
            //{ new STuples<EControllersButton, EHand>(EControllersButton.GRIP, EHand.RIGHT), OVRInput.Button.SecondaryHandTrigger },

            //{ new STuples<EControllersButton, EHand>(EControllersButton.THUMBSTICK, EHand.LEFT), OVRInput.Button.PrimaryThumbstick },
            //{ new STuples<EControllersButton, EHand>(EControllersButton.THUMBSTICK, EHand.RIGHT), OVRInput.Button.SecondaryThumbstick },

            //{ new STuples<EControllersButton, EHand>(EControllersButton.MENU, EHand.LEFT), OVRInput.Button.Start },

            //{ new STuples<EControllersButton, EHand>(EControllersButton.A_BUTTON, EHand.RIGHT), OVRInput.Button.One },
            //{ new STuples<EControllersButton, EHand>(EControllersButton.B_BUTTON, EHand.RIGHT), OVRInput.Button.Two },

            //{ new STuples<EControllersButton, EHand>(EControllersButton.X_BUTTON, EHand.LEFT), OVRInput.Button.Three },
            //{ new STuples<EControllersButton, EHand>(EControllersButton.Y_BUTTON, EHand.LEFT), OVRInput.Button.Four }
        };


        // The dictionary with references to the OVR Touch
        public static Dictionary<STuples<EControllersButton, EHand>, /* TODO OVRInput.Button*/bool> TouchDictionnary = new Dictionary<STuples<EControllersButton, EHand>, /* TODO OVRInput.Button*/bool>()
        {
            // TODO
            //{ new STuples<EControllersButton, EHand>(EControllersButton.NONE, EHand.NONE), OVRInput.Touch.None },

            //{ new STuples<EControllersButton, EHand>(EControllersButton.TRIGGER, EHand.LEFT), OVRInput.Touch.PrimaryIndexTrigger },
            //{ new STuples<EControllersButton, EHand>(EControllersButton.TRIGGER, EHand.RIGHT), OVRInput.Touch.SecondaryIndexTrigger },
            
            //// The Grip (HandTrigger) is not checking for touch with Oculus
            
            //{ new STuples<EControllersButton, EHand>(EControllersButton.THUMBSTICK, EHand.LEFT), OVRInput.Touch.PrimaryThumbstick },
            //{ new STuples<EControllersButton, EHand>(EControllersButton.THUMBSTICK, EHand.RIGHT), OVRInput.Touch.SecondaryThumbstick },

            //// Start is not checking for touch with Oculus
            
            //{ new STuples<EControllersButton, EHand>(EControllersButton.A_BUTTON, EHand.RIGHT), OVRInput.Touch.One },
            //{ new STuples<EControllersButton, EHand>(EControllersButton.B_BUTTON, EHand.RIGHT), OVRInput.Touch.Two },

            //{ new STuples<EControllersButton, EHand>(EControllersButton.X_BUTTON, EHand.LEFT), OVRInput.Touch.Three },
            //{ new STuples<EControllersButton, EHand>(EControllersButton.Y_BUTTON, EHand.LEFT), OVRInput.Touch.Four },
            
            //// The Oculus has a ThumbRest touch feature
            //{ new STuples<EControllersButton, EHand>(EControllersButton.THUMBREST, EHand.LEFT), OVRInput.Touch.PrimaryThumbRest },
            //{ new STuples<EControllersButton, EHand>(EControllersButton.THUMBREST, EHand.RIGHT), OVRInput.Touch.SecondaryThumbRest },
        };
    }
}