using UnityEngine;

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

        [System.NonSerialized]
        public float _Timer = 0.0f;
    }
}