using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRSF.Inputs.Gaze
{
    /// <summary>
    /// Dictionnary for all Inputs corresponding to the possible gaze Interaction with OpenVR
    /// </summary>
    public static class GazeInteractionOpenVR
    {
        //The dictionary with references to the button masks
        public static Dictionary<EControllersInput, ulong> Dictionarry = new Dictionary<EControllersInput, ulong>()
        {
            { EControllersInput.NONE, 0 },

            { EControllersInput.LEFT_TRIGGER, SteamVR_Controller.ButtonMask.Trigger },
            { EControllersInput.RIGHT_TRIGGER, SteamVR_Controller.ButtonMask.Trigger },

            { EControllersInput.LEFT_GRIP, SteamVR_Controller.ButtonMask.Grip },
            { EControllersInput.RIGHT_GRIP, SteamVR_Controller.ButtonMask.Grip },

            { EControllersInput.LEFT_THUMBSTICK, SteamVR_Controller.ButtonMask.Touchpad },
            { EControllersInput.RIGHT_THUMBSTICK, SteamVR_Controller.ButtonMask.Touchpad },

            { EControllersInput.LEFT_MENU, SteamVR_Controller.ButtonMask.ApplicationMenu },
            { EControllersInput.RIGHT_MENU, SteamVR_Controller.ButtonMask.ApplicationMenu },
        };
    }
}