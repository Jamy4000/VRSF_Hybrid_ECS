using UnityEngine;

namespace VRSF.MoveAround.Components
{
    [RequireComponent(typeof(FlyBoundariesComponent))]
    public class FlyDirectionComponent : MonoBehaviour
    {
        [HideInInspector] public float FlightDirection = 0.0f;

        [HideInInspector] public Vector3 NormalizedDir = new Vector3();
        [HideInInspector] public Vector3 FinalDirection = new Vector3();
    }
}