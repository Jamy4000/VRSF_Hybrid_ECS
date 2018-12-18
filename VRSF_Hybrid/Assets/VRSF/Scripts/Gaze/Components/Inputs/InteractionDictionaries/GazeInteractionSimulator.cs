using System.Collections.Generic;
using UnityEngine;
using VRSF.Controllers;
using VRSF.Utils;

namespace VRSF.Inputs.Gaze
{
    /// <summary>
    /// Contain the dictionary for all keys corresponding to the possible gaze click with the Simulator
    /// </summary>
    public static class GazeInteractionSimulator
    {
        // The dictionary with references to the Input Buttons
        public static Dictionary<STuples<EControllersButton, EHand>, KeyCode> Dictionnary = new Dictionary<STuples<EControllersButton, EHand>, KeyCode>()
        {
            { new STuples<EControllersButton, EHand>(EControllersButton.NONE, EHand.NONE), KeyCode.None },

            { new STuples<EControllersButton, EHand>(EControllersButton.TRIGGER, EHand.LEFT), KeyCode.Mouse0 },
            { new STuples<EControllersButton, EHand>(EControllersButton.TRIGGER, EHand.RIGHT), KeyCode.Mouse1 },

            { new STuples<EControllersButton, EHand>(EControllersButton.GRIP, EHand.LEFT), KeyCode.LeftShift },
            { new STuples<EControllersButton, EHand>(EControllersButton.GRIP, EHand.RIGHT), KeyCode.RightShift },

            // Using Up arrow for thumb
            { new STuples<EControllersButton, EHand>(EControllersButton.THUMBSTICK, EHand.LEFT), KeyCode.UpArrow },
            { new STuples<EControllersButton, EHand>(EControllersButton.THUMBSTICK, EHand.RIGHT), KeyCode.W },

            { new STuples<EControllersButton, EHand>(EControllersButton.MENU, EHand.LEFT), KeyCode.LeftControl },

            // Vive Particularities
            { new STuples<EControllersButton, EHand>(EControllersButton.MENU, EHand.RIGHT), KeyCode.RightControl },

            // Oculus Particularities
            { new STuples<EControllersButton, EHand>(EControllersButton.A_BUTTON, EHand.RIGHT), KeyCode.L },
            { new STuples<EControllersButton, EHand>(EControllersButton.B_BUTTON, EHand.RIGHT), KeyCode.B },
            { new STuples<EControllersButton, EHand>(EControllersButton.X_BUTTON, EHand.LEFT), KeyCode.F },
            { new STuples<EControllersButton, EHand>(EControllersButton.Y_BUTTON, EHand.LEFT), KeyCode.R },

            // Simulator Particularities
            { new STuples<EControllersButton, EHand>(EControllersButton.WHEEL_BUTTON, EHand.NONE), KeyCode.Mouse2 },
        };
    }

}