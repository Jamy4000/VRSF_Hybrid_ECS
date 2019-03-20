using ScriptableFramework.Utils;
using ScriptableFramework.Variables;
using UnityEngine;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Contain all the references to Variables, RuntimeSets and GameEvents for the Inputs in VR
    /// </summary>
    public class InputVariableContainer : ScriptableSingleton<InputVariableContainer>
    {
        [Multiline(10)]
        public string DeveloperDescription = "";

        [Header("Runtime Dictionnary to get the BoolVariable (Touch and Click) for each button")]
        public VRInputsBoolean RightClickBoolean;
        public VRInputsBoolean LeftClickBoolean;
        public VRInputsBoolean RightTouchBoolean;
        public VRInputsBoolean LeftTouchBoolean;

        [Header("BoolVariable (Touch and Click) for The Gaze Button if used")]
        public BoolVariable GazeIsCliking;
        public BoolVariable GazeIsTouching;

        [Header("Click BoolVariable for The Wheel Button")]
        public BoolVariable WheelIsClicking;

        [Header("Vector2Variable for the Thumb position")]
        public Vector2Variable RightThumbPosition;
        public Vector2Variable LeftThumbPosition;
    }
}