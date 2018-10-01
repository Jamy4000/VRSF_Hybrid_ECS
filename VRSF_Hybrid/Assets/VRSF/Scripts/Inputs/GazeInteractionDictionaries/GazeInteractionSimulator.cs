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
        public static Dictionary<STuples<EControllersInput, EHand>, KeyCode> Dictionnary = new Dictionary<STuples<EControllersInput, EHand>, KeyCode>()
        {
            { new STuples<EControllersInput, EHand>(EControllersInput.NONE, EHand.NONE), KeyCode.None },

            { new STuples<EControllersInput, EHand>(EControllersInput.TRIGGER, EHand.LEFT), KeyCode.Mouse0 },
            { new STuples<EControllersInput, EHand>(EControllersInput.TRIGGER, EHand.RIGHT), KeyCode.Mouse1 },

            { new STuples<EControllersInput, EHand>(EControllersInput.GRIP, EHand.LEFT), KeyCode.LeftShift },
            { new STuples<EControllersInput, EHand>(EControllersInput.GRIP, EHand.RIGHT), KeyCode.RightShift },

            // Using Up arrow for thumb
            { new STuples<EControllersInput, EHand>(EControllersInput.THUMBSTICK, EHand.LEFT), KeyCode.UpArrow },
            { new STuples<EControllersInput, EHand>(EControllersInput.THUMBSTICK, EHand.RIGHT), KeyCode.W },

            { new STuples<EControllersInput, EHand>(EControllersInput.MENU, EHand.LEFT), KeyCode.LeftControl },

            // Vive Particularities
            { new STuples<EControllersInput, EHand>(EControllersInput.MENU, EHand.RIGHT), KeyCode.RightControl },

            // Oculus Particularities
            { new STuples<EControllersInput, EHand>(EControllersInput.A_BUTTON, EHand.RIGHT), KeyCode.L },
            { new STuples<EControllersInput, EHand>(EControllersInput.B_BUTTON, EHand.RIGHT), KeyCode.B },
            { new STuples<EControllersInput, EHand>(EControllersInput.X_BUTTON, EHand.LEFT), KeyCode.F },
            { new STuples<EControllersInput, EHand>(EControllersInput.Y_BUTTON, EHand.LEFT), KeyCode.R },

            // Simulator Particularities
            { new STuples<EControllersInput, EHand>(EControllersInput.WHEEL_BUTTON, EHand.NONE), KeyCode.Mouse2 },
        };
    }

}