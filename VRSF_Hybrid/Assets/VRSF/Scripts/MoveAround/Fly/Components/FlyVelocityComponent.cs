using UnityEngine;
using VRSF.Core.Raycast;

namespace VRSF.MoveAround.Fly
{
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity), typeof(FlyAccelerationComponent))]
    public class FlyVelocityComponent : MonoBehaviour
    {
        [HideInInspector] public float CurrentFlightVelocity = 0.0f;
    }
}