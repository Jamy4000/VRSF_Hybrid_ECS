using UnityEngine;
using VRSF.Core.Utils.ButtonActionChoser;

namespace VRSF.MoveAround.Rotate
{
    /// <summary>
    /// Component used by the rotation systems to rotate the user using the BAC Components
    /// </summary>
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity), typeof(BACGeneralComponent))]
    public class CameraRotationComponent : MonoBehaviour
    {
        [Header("Camera Rotation Parameters")]
        public float MaxSpeed = 1.0f;
        public float DegreesToTurn = 30.0f;
        public bool UseAccelerationEffect = false;
        
        [HideInInspector] public bool IsRotating;
        [HideInInspector] public float CurrentSpeed = 0.0f;
        [HideInInspector] public float LastThumbPos;
    }
}