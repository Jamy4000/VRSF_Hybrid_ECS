using VRSF.Core.Utils.ButtonActionChoser;

namespace VRSF.Core.Events
{
    /// <summary>
    /// Used in the BAC Setup, raised when the actions buttons are setup correctly
    /// </summary>
    public class OnActionButtonIsReady : EventCallbacks.Event<OnActionButtonIsReady>
    {
        public readonly BACCalculationsComponent BACCalculations;

        public OnActionButtonIsReady(BACCalculationsComponent bac) : base("Used in the BAC Setup, raised when the actions buttons are setup correctly")
        {
            BACCalculations = bac;
            FireEvent(this);
        }
    }
}