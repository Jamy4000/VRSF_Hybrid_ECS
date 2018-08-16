using UnityEngine;

namespace VRSF.MoveAround.Components
{
    [RequireComponent(typeof(FlyBoundariesComponent))]
    public class FlyDirectionComponent : MonoBehaviour
    {
        [HideInInspector] public float _FlightDirection = 0.0f;

        [HideInInspector] public Vector3 _NormalizedDir = Vector3.zero;
        [HideInInspector] public Vector3 _FinalDirection = Vector3.zero;
    }
}