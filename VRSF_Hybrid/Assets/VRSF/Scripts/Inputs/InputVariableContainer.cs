using ScriptableFramework.Events;
using ScriptableFramework.RuntimeSet;
using ScriptableFramework.Util;
using ScriptableFramework.Variables;
using UnityEngine;

namespace VRSF.Inputs
{
    /// <summary>
    /// Contain all the references to Variables, RuntimeSets and GameEvents for the Inputs in VR
    /// </summary>
    public class InputVariableContainer : ScriptableSingleton<InputVariableContainer>
    {
        // EMPTY
        #region PRIVATE_VARIABLE
        #endregion


        #region PUBLIC_VARIABLE
        [Multiline(10)]
        public string DeveloperDescription = "";

        [Header("Runtime Dictionnary to get the GameEvents (Touch and Click) for each button")]
        public VRInputsEvents RightClickEvents;
        public VRInputsEvents LeftClickEvents;
        public VRInputsEvents RightTouchEvents;
        public VRInputsEvents LeftTouchEvents;

        [Header("GameEvents (Touch and Click) for The Gaze Button if used")]
        public GameEvent GazeClickDown;
        public GameEvent GazeClickUp;
        public GameEvent GazeStartTouching;
        public GameEvent GazeStopTouching;

        [Header("Click GameEvents for The Wheel Button")]
        public GameEvent WheelClickDown;
        public GameEvent WheelClickUp;

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

        #endregion PUBLIC_VARIABLE


        // EMPTY
        #region PUBLIC_METHODS
        #endregion PUBLIC_METHODS
    }
}