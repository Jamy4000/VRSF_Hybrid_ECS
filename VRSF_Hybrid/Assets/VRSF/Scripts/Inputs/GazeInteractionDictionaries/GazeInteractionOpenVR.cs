using System.Collections.Generic;

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

            { EControllersInput.TRIGGER, SteamVR_Controller.ButtonMask.Trigger },

            { EControllersInput.GRIP, SteamVR_Controller.ButtonMask.Grip },

            { EControllersInput.THUMBSTICK, SteamVR_Controller.ButtonMask.Touchpad },

            { EControllersInput.MENU, SteamVR_Controller.ButtonMask.ApplicationMenu }
        };
    }
}