using System;

namespace VRSF.Core.Controllers.Haptic
{
    /// <summary>
    /// Event to call when you want to launch haptic in the controller of the user
    /// </summary>
    public class OnHapticRequestedEvent : EventCallbacks.Event<OnHapticRequestedEvent>
    {
        /// <summary>
        /// The hand that is gonna receive the Haptic feedback
        /// </summary>
        public readonly EHand Hand;

        /// <summary>
        /// Time in microseconds the haptic impulse should take place
        /// </summary>
        public readonly float HapticDuration;
        
        /// <summary>
        /// The Frequency of the Haptic
        /// </summary>
        public readonly float HapticFrequency;

        /// <summary>
        /// The Amplitude of the Haptic impulse
        /// </summary>
        public readonly float HapticAmplitude;

        public OnHapticRequestedEvent(EHand hand, EHapticDuration hapticDuration, EHapticAmplitude hapticAmplitude = EHapticAmplitude.MEDIUM, EHapticFrequency hapticFrequency = EHapticFrequency.MEDIUM) : base("Event to call when you want to launch haptic in the controller of the user.")
        {
            if (hand.Equals(EHand.GAZE) || hand.Equals(EHand.NONE))
            {
                UnityEngine.Debug.LogError("The specified hand for the Haptic Request is not vqlid. Please give right or left hand as parameter of the event. Returning.");
                return;
            }

            Hand = hand;
            HapticDuration = GetBaseDuration(hapticDuration);
            HapticFrequency = (float)hapticFrequency;
            HapticAmplitude = GetBaseAmplitude(hapticAmplitude);
            FireEvent(this);
        }

        public OnHapticRequestedEvent(EHand hand, float hapticDuration = 0.5f, float hapticAmplitude = 0.5f, float hapticFrequency = 150) : base("Event to call when you want to launch haptic in the controller of the user.")
        {
            if (hand.Equals(EHand.GAZE) || hand.Equals(EHand.NONE))
            {
                UnityEngine.Debug.LogError("The specified hand for the Haptic Request is not vqlid. Please give right or left hand as parameter of the event. Returning.");
                return;
            }

            Hand = hand;
            HapticDuration = hapticDuration;
            HapticFrequency = hapticFrequency;
            HapticAmplitude = hapticAmplitude;
            FireEvent(this);
        }

        private float GetBaseAmplitude(EHapticAmplitude hapticAmplitude)
        {
            switch (hapticAmplitude)
            {
                case EHapticAmplitude.LIGHT:
                    return 0.1f;
                case EHapticAmplitude.MEDIUM:
                    return 0.5f;
                default:
                    return 1.0f;
            }
        }

        private float GetBaseDuration(EHapticDuration hapticDuration)
        {
            switch (hapticDuration)
            {
                case EHapticDuration.SHORT:
                    return 0.1f;
                case EHapticDuration.MEDIUM:
                    return 0.5f;
                default:
                    return 1.0f;
            }
        }
    }
}