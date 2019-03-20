using UnityEngine;

namespace VRSF.Core.Inputs
{
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity))]
    public class SimulatorMovementComponent : MonoBehaviour
    {
        [Header("Movement Settings")]
        [Tooltip("Exponential boost factor on translation, controllable by mouse wheel.")]
        public float boost = 2.5f;

        [Tooltip("Time it takes to interpolate camera position 99% of the way to the target."), Range(0.001f, 1f)]
        public float positionLerpTime = 0.2f;

        [Header("Rotation Settings")]
        [Tooltip("X = Change in mouse position.\nY = Multiplicative factor for camera rotation.")]
        public AnimationCurve mouseSensitivityCurve = new AnimationCurve(new Keyframe(0f, 0.5f, 0f, 5f), new Keyframe(1f, 2.5f, 0f, 0f));

        [Tooltip("Time it takes to interpolate camera rotation 99% of the way to the target."), Range(0.001f, 1f)]
        public float rotationLerpTime = 0.01f;

        [HideInInspector]
        public CameraState m_TargetCameraState = new CameraState();
        [HideInInspector]
        public CameraState m_InterpolatingCameraState = new CameraState();
    }
}
