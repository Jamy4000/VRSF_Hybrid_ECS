using UnityEngine;
using VRSF.Utils.Components;

namespace VRSF.MoveAround.Components
{
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity), typeof(ScriptableRaycastComponent), typeof(FlyAccelerationComponent))]
    public class FlyVelocityComponent : MonoBehaviour
    {
        [HideInInspector] public float CurrentFlightVelocity = 0.0f;
    }
}