using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRSF.Inputs.Gaze
{
    /// <summary>
    /// Contain the two dictionnary for all Inputs corresponding to the possible gaze Interaction with OpenVR
    /// </summary>
    public class GazeInteractionDictionaryOpenVR
    {
        #region PUBLIC_VARIABLES
        [Header("SteamVR Tracked Object from the two Controllers")]
        public SteamVR_TrackedObject LeftTrackedObject;
        public SteamVR_TrackedObject RightTrackedObject;
        
        [Header("The dictionary with references to the button masks")]
        public Dictionary<EControllersInput, ulong> GazeButtonMask = new Dictionary<EControllersInput, ulong>()
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
        #endregion PUBLIC_VARIABLES


        #region PRIVATE_VARIABLES
        private SteamVR_Controller.Device LeftController
        {
            get { return SteamVR_Controller.Input((int)LeftTrackedObject.index); }
        }

        private SteamVR_Controller.Device RightController
        {
            get { return SteamVR_Controller.Input((int)RightTrackedObject.index); }
        }
        #endregion PRIVATE_VARIABLES

        
        // EMPTY
        #region PRIVATE_METHODS
        #endregion
    }
}