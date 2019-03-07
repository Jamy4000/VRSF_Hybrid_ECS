using UnityEngine;

namespace VRSF.MoveAround.Fly
{
    public class FlyAccelerationComponent : MonoBehaviour
    {
        [Tooltip("If you want to have an acceleration and deceleration effect when starting and stopping to fly.")]
        [HideInInspector] public bool AccelerationDecelerationEffect = true;

        [Tooltip("The factor for the acceleration effect. Set to 0 to remove this effect.")]
        [HideInInspector] public float AccelerationEffectFactor = 1.0f;

        [Tooltip("The factor for the deceleration effect. Set to 0 to remove this effect.")]
        [HideInInspector] public float DecelerationEffectFactor = 1.0f;
    }
}