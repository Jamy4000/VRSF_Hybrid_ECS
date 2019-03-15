using UnityEngine;

namespace VRSF.MoveAround.Fly
{
    public class FlyAccelerationComponent : MonoBehaviour
    {
        /// <summary>
        /// If you want to have an acceleration and deceleration effect when starting and stopping to fly
        /// </summary>
        [Tooltip("If you want to have an acceleration and deceleration effect when starting and stopping to fly.")]
        [HideInInspector] public bool AccelerationDecelerationEffect = true;

        /// <summary>
        /// The factor for the acceleration effect. Set to 0 to remove this effect.
        /// </summary>
        [Tooltip("The factor for the acceleration effect. Set to 0 to remove this effect.")]
        [HideInInspector] public float AccelerationEffectFactor = 1.0f;

        /// <summary>
        /// The factor for the deceleration effect. Set to 0 to remove this effect.
        /// </summary>
        [Tooltip("The factor for the deceleration effect. Set to 0 to remove this effect.")]
        [HideInInspector] public float DecelerationEffectFactor = 1.0f;
    }
}