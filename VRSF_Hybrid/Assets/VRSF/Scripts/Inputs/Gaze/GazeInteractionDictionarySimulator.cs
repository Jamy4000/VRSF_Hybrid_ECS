using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRSF.Inputs.Gaze
{
    /// <summary>
    /// Contain the dictionary for all keys corresponding to the possible gaze click with the Simulator
    /// </summary>
    public static class GazeInteractionDictionarySimulator
    {
        [Header("The dictionary with references to the Input Buttons")]
        public static Dictionary<EControllersInput, KeyCode> GazeToKeyCode = new Dictionary<EControllersInput, KeyCode>()
        {
            { EControllersInput.NONE, KeyCode.None },

            { EControllersInput.LEFT_TRIGGER, KeyCode.Mouse0 },
            { EControllersInput.RIGHT_TRIGGER, KeyCode.Mouse1 },

            { EControllersInput.LEFT_GRIP, KeyCode.LeftShift },
            { EControllersInput.RIGHT_GRIP, KeyCode.RightShift },

            // Using Up arrow for thumb
            { EControllersInput.LEFT_THUMBSTICK, KeyCode.UpArrow },
            { EControllersInput.RIGHT_THUMBSTICK, KeyCode.W },

            { EControllersInput.LEFT_MENU, KeyCode.LeftControl },

            // Vive Particularities
            { EControllersInput.RIGHT_MENU, KeyCode.RightControl },

            // Oculus Particularities
            { EControllersInput.A_BUTTON, KeyCode.L },
            { EControllersInput.B_BUTTON, KeyCode.B },
            { EControllersInput.X_BUTTON, KeyCode.F },
            { EControllersInput.Y_BUTTON, KeyCode.R },

            // Simulator Particularities
            { EControllersInput.WHEEL_BUTTON, KeyCode.Mouse2 },
        };
    }

}