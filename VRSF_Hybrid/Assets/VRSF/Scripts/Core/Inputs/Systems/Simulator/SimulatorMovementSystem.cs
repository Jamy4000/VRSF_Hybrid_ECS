using UnityEngine;
using Unity.Entities;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    public class SimulatorMovementSystem : ComponentSystem
    {
        struct Filter
        {
            public SimulatorMovementComponent cameraComponent;
        }

        protected override void OnCreateManager()
        {
            OnSetupVRReady.Listeners += CheckSystemState;
            base.OnCreateManager();
        }

        protected override void OnStartRunning()
        {
            base.OnStartRunning();

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
                // Rotation
                if (Input.GetMouseButton(1))
                {
                    EvaluateRotation(e.cameraComponent, mouse);
                }
                // Translation
                if (EvaluateTranslation(e.cameraComponent, ScrollDeltaY, dt))
                {
                    // Interpolate toward new position
                    Interpolate(e.cameraComponent, dt);
                }
            }
        }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
            OnSetupVRReady.Listeners -= CheckSystemState;
        }

        // Evaluate the camera rotation based on the mouse screen position
        private void EvaluateRotation(SimulatorMovementComponent cameraComp, Vector2 mouse)
        {
            var mouseSensitivityFactor = cameraComp.mouseSensitivityCurve.Evaluate(mouse.magnitude);
            cameraComp.m_TargetCameraState.yaw += mouse.x * mouseSensitivityFactor;
            cameraComp.m_TargetCameraState.pitch += mouse.y * mouseSensitivityFactor;
        }

        /// <summary>
        /// Evaluate the camera translation based on WASD and apply boost.
        /// </summary>
        /// <param name="cameraComp"></param>
        /// <param name="deltaY"></param>
        /// <param name="deltaTime"></param>
        /// <returns>return true if translation is not a vector3.zero</returns>
        private bool EvaluateTranslation(SimulatorMovementComponent cameraComp, float deltaY, float deltaTime)
        {
            var translation = GetInputTranslationDirection() * deltaTime;
            if (translation != Vector3.zero)
            {
                // Speed up movement when shift key held
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    translation *= 10.0f;
                }

                // Modify movement by a boost factor (defined in Inspector and modified in play mode through the mouse scroll wheel)
                cameraComp.boost += deltaY * 0.2f;
                translation *= Mathf.Pow(2.0f, cameraComp.boost);

                cameraComp.m_TargetCameraState.Translate(translation);

                return true;
            }
            return false;
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

        private Vector3 GetInputTranslationDirection()
        {
            Vector3 direction = Vector3.zero;
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


        private void CheckSystemState(OnSetupVRReady info)
        {
            this.Enabled = VRSF_Components.DeviceLoaded == EDevice.SIMULATOR;
        }
    }
}
