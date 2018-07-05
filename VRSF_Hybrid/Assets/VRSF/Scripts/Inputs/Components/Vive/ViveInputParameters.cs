using ScriptableFramework.RuntimeSet;
using ScriptableFramework.Variables;

namespace VRSF.Inputs.Components.Vive
{
    /// <summary>
    /// Used to handle the Right and Left controllers Inputs in the Vive, 
    /// </summary>
    public struct ViveInputParameters
    {
        public VRInputsEvents ClickEvents;
        public VRInputsEvents TouchEvents;
        public VRInputsBoolean ClickBools;
        public VRInputsBoolean TouchBools;
        public SteamVR_Controller.Device Controller;
        public Vector2Variable ThumbPosition;
    }
}