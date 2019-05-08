using ScriptableFramework.Variables;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Used to handle the Right and Left controllers Inputs
    /// </summary>
    public class InputParameters
    {
        public BoolVariable TriggerClick;
        public BoolVariable TriggerTouch;

        public BoolVariable TouchpadClick;
        public BoolVariable TouchpadTouch;
        public Vector2Variable ThumbPosition;

        public BoolVariable GripClick;
        public BoolVariable GripTouch;

        public VRInputsBoolean ClickBools;
        public VRInputsBoolean TouchBools;

        public InputParameters(VRInputsBoolean clickBools, VRInputsBoolean touchBools, Vector2Variable thumbPosition)
        {
            ClickBools = clickBools;
            TouchBools = touchBools;
            ThumbPosition = thumbPosition;

            TriggerClick = clickBools.Get("TriggerIsDown");
            TriggerTouch = touchBools.Get("TriggerIsTouching");

            TouchpadClick = clickBools.Get("ThumbIsDown");
            TouchpadTouch = touchBools.Get("ThumbIsTouching");

            GripClick = clickBools.Get("GripIsDown");
            GripTouch = touchBools.Get("GripIsTouching");
        }
    }
}