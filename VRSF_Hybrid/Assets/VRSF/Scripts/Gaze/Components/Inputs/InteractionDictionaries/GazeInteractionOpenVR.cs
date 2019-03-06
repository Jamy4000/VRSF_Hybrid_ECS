using System.Collections.Generic;
using VRSF.Core.Inputs;

namespace VRSF.Gaze.Inputs
{
    /// <summary>
    /// Dictionnary for all Inputs corresponding to the possible gaze Interaction with OpenVR
    /// </summary>
    public static class GazeInteractionOpenVR
    {
        //The dictionary with references to the button masks
        public static Dictionary<EControllersButton, ulong> Dictionarry = new Dictionary<EControllersButton, ulong>()
        {
            //{ EControllersButton.NONE, 0 },

            //{ EControllersButton.TRIGGER, SteamVR_Controller.ButtonMask.Trigger },

            //{ EControllersButton.GRIP, SteamVR_Controller.ButtonMask.Grip },

            //{ EControllersButton.THUMBSTICK, SteamVR_Controller.ButtonMask.Touchpad },

            //{ EControllersButton.MENU, SteamVR_Controller.ButtonMask.ApplicationMenu }
        };
        // TODO
    }
}