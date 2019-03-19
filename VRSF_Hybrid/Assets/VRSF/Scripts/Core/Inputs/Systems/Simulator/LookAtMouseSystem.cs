using Unity.Entities;
using UnityEngine;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Script attached to the Camera rig of the SimulatorSDK.
    /// Allow the user to rotate the camera, which will follow the mouse position.
    /// To use this feature, press the Space bar when you want to rotate the camera.
    /// </summary>
    public class LookAtMouseSystem : ComponentSystem
    {
        struct Components
        {
            public Transform transform;
            public LookAtMouseComponent lookAtMouseComponent;
        }
        
        protected override void OnCreateManager()
        {
            OnSetupVRReady.Listeners += CheckDevice;
            base.OnCreateManager();
        }

        protected override void OnUpdate()
        {
            float time = Time.deltaTime;
            if (Input.GetKey(KeyCode.Space))
            {
                foreach (var entity in GetEntities<Components>())
                {
                    CheckRayDirection(entity, time);
                }
            }
        }

        protected override void OnDestroyManager()
        {
            OnSetupVRReady.Listeners -= CheckDevice;
            base.OnDestroyManager();
        }

        private void CheckRayDirection(Components entity, float time)
        {
            // Generate a ray from the cursor position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Get the point along the ray that hits the calculated distance.
            Vector3 targetPoint = ray.GetPoint(100);

            // Determine the target rotation.  This is the rotation if the transform looks at the target point.
            Quaternion targetRotation = Quaternion.LookRotation(targetPoint - entity.transform.position);

            // Smoothly rotate towards the target point.
            entity.transform.rotation = Quaternion.Slerp(entity.transform.rotation, targetRotation, time * entity.lookAtMouseComponent.Speed);
        }

        private void CheckDevice(OnSetupVRReady info)
        {
            this.Enabled = VRSF_Components.DeviceLoaded == EDevice.SIMULATOR;
        }
    }
}