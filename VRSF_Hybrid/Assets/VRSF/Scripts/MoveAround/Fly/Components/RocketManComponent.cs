using UnityEngine;
using VRSF.MoveAround.Components;

namespace HPA_Boat.VR.Component
{
    [RequireComponent(typeof(FlyParametersComponent))]
    public class RocketManComponent : MonoBehaviour
    {
        [HideInInspector] public bool SpeedHasBeenSet = false;
        [HideInInspector] public bool RocketSlowingDown = false;

        [HideInInspector] public float BaseSpeedValue = 1.0f;

        [HideInInspector] public float MaxRocketSpeed = 50.0f;
    }
}