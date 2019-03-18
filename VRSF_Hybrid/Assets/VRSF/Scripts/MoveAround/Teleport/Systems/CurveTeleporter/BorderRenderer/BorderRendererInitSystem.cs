using Unity.Entities;
using UnityEngine;
using VRSF.Core.SetupVR;

namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// A generic System that renders a border using the given polylines.  
    /// The borders are double sided and are oriented upwards (ie normals are parallel to the XZ plane)
    /// 
    /// Disclaimer : This script is based on the Flafla2 Vive-Teleporter Repository. You can check it out here :
    /// https://github.com/Flafla2/Vive-Teleporter
    /// </summary>
    public class BorderRendererInitSystem : ComponentSystem
    {
        struct Filter
        {
            public BorderRendererComponent BorderRenderer;
        }

        protected override void OnStartRunning()
        {
            OnSetupVRReady.RegisterListener(Init);
            base.OnStartRunning();
        }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
            OnSetupVRReady.UnregisterListener(Init);
        }

        protected override void OnUpdate() { }

        private void Init(OnSetupVRReady setupVRReady)
        {
            if (VRSF_Components.DeviceLoaded == EDevice.SIMULATOR)
            {
                foreach (var e in GetEntities<Filter>())
                {
                    e.BorderRenderer.Points = new Vector3[0];
                }
                // We deactivate the system if we load the simulator, as we cannot have access to any chaperone
                OnSetupVRReady.UnregisterListener(Init);
                this.Enabled = false;
                return;
            }
            
            foreach (var e in GetEntities<Filter>())
            {
                // We check if a Play Area was set by the user
                bool success = GetOpenVRChaperone(out Vector3 p0, out Vector3 p1, out Vector3 p2, out Vector3 p3);

                // If we didn't set the chaperone yet and we could correctly get it
                if (!e.BorderRenderer.ChaperoneIsSetup && success)
                {
                    // We Rotate to match camera rig rotation
                    var originRotationMatrix = Matrix4x4.TRS(Vector3.zero, VRSF_Components.CameraRig.transform.rotation, Vector3.one);

                    // We set the points of the borders
                    e.BorderRenderer.Points = new Vector3[]
                    {
                        originRotationMatrix * p0,
                        originRotationMatrix * p1,
                        originRotationMatrix * p2,
                        originRotationMatrix * p3,
                        originRotationMatrix * p0,
                    };

                    e.BorderRenderer.ChaperoneIsSetup = true;
                    e.BorderRenderer.BorderAreShown = false;
                }
                else
                {
                    Debug.LogError("<b>[VRSF] :</b> Couldn't set the chaperonne for the teleport properly. Returning.");
                }
            }
        }

        /// <summary>
        /// Get the OpenVR Boundaries to display the borders
        /// </summary>
        /// <param name="p0">The first point of the border</param>
        /// <param name="p1">The second point of the border</param>
        /// <param name="p2">The third point of the border</param>
        /// <param name="p3">The fourth point of the border</param>
        /// <returns>true if the boundaries of OpenVR could be correctly get</returns>
        private bool GetOpenVRChaperone(out Vector3 p0, out Vector3 p1, out Vector3 p2, out Vector3 p3)
        {
            //var initOpenVR = !SteamVR.active && !SteamVR.usingNativeSupport;
            //if (initOpenVR)
            //{
            //    var error = EVRInitError.None;
            //    OpenVR.Init(ref error, EVRApplicationType.VRApplication_Other);
            //}

            //var chaperone = OpenVR.Chaperone;
            //HmdQuad_t rect = new HmdQuad_t();
            //bool success = (chaperone != null) && chaperone.GetPlayAreaRect(ref rect);
            //p0 = new Vector3(rect.vCorners0.v0, rect.vCorners0.v1, rect.vCorners0.v2);
            //p1 = new Vector3(rect.vCorners1.v0, rect.vCorners1.v1, rect.vCorners1.v2);
            //p2 = new Vector3(rect.vCorners2.v0, rect.vCorners2.v1, rect.vCorners2.v2);
            //p3 = new Vector3(rect.vCorners3.v0, rect.vCorners3.v1, rect.vCorners3.v2);

            //if (!success)
            //    Debug.LogWarning("Failed to get Calibrated Play Area bounds!  Make sure you have tracking first, and that your space is calibrated.");

            //if (initOpenVR)
            //    OpenVR.Shutdown();

            //return success;
            p0 = Vector3.back;
            p1 = Vector3.back;
            p2 = Vector3.back;
            p3 = Vector3.back;
            return true;
        }
    }
}