using UnityEngine;
using VRSF.MoveAround.Components;

namespace HPA_Boat.VR.Component
{
    [RequireComponent(typeof(FlyParametersComponent))]
    public class RocketManComponent : MonoBehaviour
    {
        [HideInInspector] public bool _SpeedHasBeenSet = false;
        [HideInInspector] public bool _RocketSlowingDown = false;
        [HideInInspector] public bool _IsSetup = false;

        [HideInInspector] public float _BaseSpeedValue = 1.0f;

        public float MaxRocketManSpeed = 30.0f;
    }
}