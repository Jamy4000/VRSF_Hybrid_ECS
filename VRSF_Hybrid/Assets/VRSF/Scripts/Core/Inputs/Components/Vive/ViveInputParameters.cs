using ScriptableFramework.Variables;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Used to handle the Right and Left controllers Inputs
    /// </summary>
    public struct InputParameters
    {
        public VRInputsBoolean ClickBools;
        public VRInputsBoolean TouchBools;
        public Vector2Variable ThumbPosition;
    }
}