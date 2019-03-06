using ScriptableFramework.RuntimeSet;
using ScriptableFramework.Variables;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Used to handle the Right and Left controllers Inputs in the Vive
    /// </summary>
    public struct ViveInputParameters
    {
        public VRInputsBoolean ClickBools;
        public VRInputsBoolean TouchBools;
        public Vector2Variable ThumbPosition;
    }
}