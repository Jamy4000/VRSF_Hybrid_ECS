namespace VRSF.Controllers.Haptic
{
    public class OnHapticRequestedEvent : EventCallbacks.Event<OnHapticRequestedEvent>
    {
        /// <summary>
        /// The hand that is gonna receive the Haptic feedback
        /// </summary>
        public readonly EHand Hand;

        /// <summary>
        /// Time in microseconds the haptic impulse should take place
        /// </summary>
        public readonly EHapticForce HapticForce;

        public OnHapticRequestedEvent(EHand hand, EHapticForce hapticForce = EHapticForce.LIGHT) : base("Event to call when you want to launch haptic in the controller of the user.")
        {
            if (hand.Equals(EHand.GAZE) || hand.Equals(EHand.NONE))
            {
                UnityEngine.Debug.LogError("The specified hand for the Haptic Request is not vqlid. Please give right or left hand as parameter of the event. Returning.");
                return;
            }

            Hand = hand;
            HapticForce = hapticForce;
            FireEvent(this);
        }
    }
}