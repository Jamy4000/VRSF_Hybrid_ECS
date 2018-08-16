using UnityEngine;
using VRSF.Utils.Components.ButtonActionChoser;

namespace VRSF.MoveAround.Components
{
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity), typeof(ButtonActionChoserComponents))]
    public class CameraRotationComponent : MonoBehaviour
    {
        [Header("Camera Rotation Parameters")]
        public float MaxSpeed = 1.0f;
        public float DegreesToTurn = 30.0f;
        public bool UseAccelerationEffect = true;
        
        [HideInInspector] public bool _IsRotating;
        [HideInInspector] public bool _HasRotated;   // Used for the rotation without acceleration
        [HideInInspector] public float _CurrentSpeed = 0.0f;
        [HideInInspector] public float _LastThumbPos;
    }
}