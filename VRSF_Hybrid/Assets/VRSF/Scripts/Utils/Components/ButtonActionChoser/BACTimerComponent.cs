using UnityEngine;
using UnityEngine.Events;

namespace VRSF.Utils.Components.ButtonActionChoser
{
    /// <summary>
    /// Timer component to activate a feature after a certain amount of time using the Button Action Choser.
    /// </summary>
    [RequireComponent(typeof(BACGeneralComponent), typeof(Unity.Entities.GameObjectEntity))]
    public class BACTimerComponent : MonoBehaviour
    {
        [Tooltip("Whether this BAC Component is called before or after the timer threshold.")]
        public bool IsUpdatedBeforeThreshold;

        [Tooltip("Threshold before or after which the feature is launched")]
        public float TimerThreshold = 1.0f;

        [Tooltip("Whether you want to fire the Stop interacting Events  when the user is not respective the timer.")]
        public bool StopEventShouldBeFired = true;

        [System.NonSerialized]
        public float _Timer = 0.0f;

        /// <summary>
        /// Event used when the user is using a Thumbstick for he's feature
        /// </summary>
        [System.NonSerialized]
        public UnityEvent ThumbCheckEvent;
    }
}