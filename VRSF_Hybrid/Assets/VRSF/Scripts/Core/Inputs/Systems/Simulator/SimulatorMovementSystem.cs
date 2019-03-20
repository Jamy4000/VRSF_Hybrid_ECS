using UnityEngine;
using Unity.Entities;

namespace VRSF.Core.Inputs
{
    public class SimulatorMovementSystem : ComponentSystem
    {
        struct Filter
        {
            public SimulatorMovementComponent cameraComponent;
        }

        private bool _isCursorLocked = true;

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            // Startin lock mode
            ChangeCursorState(true, CursorLockMode.Locked);
            foreach (var e in GetEntities<Filter>())
            {
                e.cameraComponent.m_TargetCameraState.SetFromTransform(e.cameraComponent.transform);
                e.cameraComponent.m_TargetCameraState.SetFromTransform(e.cameraComponent.transform);
            }
        }
        
        protected override void OnUpdate()
        {
            float dt = Time.deltaTime;
            Vector2 mouse = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            float ScrollDeltaY = Input.mouseScrollDelta.y;

            foreach (var e in GetEntities<Filter>())
            {
                // Change lock mode when escape is pressed
                if (Input.GetKey(KeyCode.Escape))
                {
                    if(_isCursorLocked)
                        ChangeCursorState(false, CursorLockMode.None);
                    else
                        ChangeCursorState(true, CursorLockMode.Locked);
                }
                // Rotation
                if (_isCursorLocked)
                {
                    EvaluateRotation(e.cameraComponent, mouse);
                }
                // Translation
                EvaluateTranslation(e.cameraComponent, ScrollDeltaY, dt);
                // Interpolate toward new position
                Interpolate(e.cameraComponent, dt);
            }
        }

        // Evaluate the camera rotation based on the mouse screen position
        private void EvaluateRotation(SimulatorMovementComponent cameraComp, Vector2 mouse)
        {
            var mouseSensitivityFactor = cameraComp.mouseSensitivityCurve.Evaluate(mouse.magnitude);
            cameraComp.m_TargetCameraState.yaw += mouse.x * mouseSensitivityFactor;
            cameraComp.m_TargetCameraState.pitch += mouse.y * mouseSensitivityFactor;
        }

        // Evaluate the camera translation based on WASD and apply boost.
        private void EvaluateTranslation(SimulatorMovementComponent cameraComp, float deltaY, float deltaTime)
        {
            var translation = GetInputTranslationDirection() * deltaTime;

            // Speed up movement when shift key held
            if (Input.GetKey(KeyCode.LeftShift))
            {
                translation *= 10.0f;
            }

            // Modify movement by a boost factor (defined in Inspector and modified in play mode through the mouse scroll wheel)
            cameraComp.boost += deltaY * 0.2f;
            translation *= Mathf.Pow(2.0f, cameraComp.boost);

            cameraComp.m_TargetCameraState.Translate(translation);
        }

        // Framerate-independent interpolation
        private void Interpolate(SimulatorMovementComponent cameraComp, float deltaTime)
        {
            // Calculate the lerp amount, such that we get 99% of the way to our target in the specified time
            var positionLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / cameraComp.positionLerpTime) * deltaTime);
            var rotationLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / cameraComp.rotationLerpTime) * deltaTime);
            cameraComp.m_InterpolatingCameraState.LerpTowards(cameraComp.m_TargetCameraState, positionLerpPct, rotationLerpPct);
            cameraComp.m_InterpolatingCameraState.UpdateTransform(cameraComp.transform);
        }

        // Change cursor lock mode
        private void ChangeCursorState(bool cursorLocked, CursorLockMode cursorMode)
        {
            _isCursorLocked = cursorLocked;
            Cursor.visible = !cursorLocked;
            Cursor.lockState = cursorMode;
        }

        private Vector3 GetInputTranslationDirection()
        {
            Vector3 direction = new Vector3();
            if (Input.GetKey(KeyCode.W))
            {
                direction += Vector3.forward;
            }
            if (Input.GetKey(KeyCode.S))
            {
                direction += Vector3.back;
            }
            if (Input.GetKey(KeyCode.A))
            {
                direction += Vector3.left;
            }
            if (Input.GetKey(KeyCode.D))
            {
                direction += Vector3.right;
            }
            if (Input.GetKey(KeyCode.Q))
            {
                direction += Vector3.down;
            }
            if (Input.GetKey(KeyCode.E))
            {
                direction += Vector3.up;
            }
            return direction;
        }
    }
}
