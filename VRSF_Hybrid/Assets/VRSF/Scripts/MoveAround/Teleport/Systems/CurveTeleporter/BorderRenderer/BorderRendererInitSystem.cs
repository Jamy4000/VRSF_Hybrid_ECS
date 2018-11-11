using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;
using VRSF.Utils;

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
            base.OnStartRunning();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        protected override void OnUpdate()
        {
            if (VRSF_Components.DeviceLoaded == EDevice.SIMULATOR)
            {
                // We deactivate the system if we load the simulator, as we cannot have access to any chaperone
                this.Enabled = false;
                return;
            }

            bool systemStillRunning = false;

            foreach (var e in GetEntities<Filter>())
            {
                // We check if a Play Area was set by the user
                bool success = GetChaperoneBounds(out Vector3 p0, out Vector3 p1, out Vector3 p2, out Vector3 p3);

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
                else if (!success)
                {
                    systemStillRunning = true;
                }
            }

            this.Enabled = systemStillRunning;
        }

        /// <summary>
        /// Requests the chaperone boundaries of the SteamVR play area.  This doesn't work if you haven't performed Room Setup.
        /// </summary>
        /// <param name="p0">Point 0 that make up the chaperone boundaries.</param>
        /// <param name="p1">Point 1 that make up the chaperone boundaries.</param>
        /// <param name="p2">Point 2 that make up the chaperone boundaries.</param>
        /// <param name="p3">Point 3 that make up the chaperone boundaries.</param>
        /// <returns>If the play area retrieval was successful</returns>
        public bool GetChaperoneBounds(out Vector3 p0, out Vector3 p1, out Vector3 p2, out Vector3 p3)
        {
            switch (VRSF_Components.DeviceLoaded)
            {
                case EDevice.OPENVR:
                    return GetViveChaperone(out p0, out p1, out p2, out p3);
                case EDevice.OCULUS_RIFT:
                case EDevice.PORTABLE_OVR:
                    return GetOVRBoundaries(out p0, out p1, out p2, out p3);
                default:
                    p0 = Vector3.zero;
                    p1 = Vector3.zero;
                    p2 = Vector3.zero;
                    p3 = Vector3.zero;
                    return false;
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
        private bool GetViveChaperone(out Vector3 p0, out Vector3 p1, out Vector3 p2, out Vector3 p3)
        {
            var initOpenVR = (!SteamVR.active && !SteamVR.usingNativeSupport);
            if (initOpenVR)
            {
                var error = EVRInitError.None;
                OpenVR.Init(ref error, EVRApplicationType.VRApplication_Other);
            }

            var chaperone = OpenVR.Chaperone;
            HmdQuad_t rect = new HmdQuad_t();
            bool success = (chaperone != null) && chaperone.GetPlayAreaRect(ref rect);
            p0 = new Vector3(rect.vCorners0.v0, rect.vCorners0.v1, rect.vCorners0.v2);
            p1 = new Vector3(rect.vCorners1.v0, rect.vCorners1.v1, rect.vCorners1.v2);
            p2 = new Vector3(rect.vCorners2.v0, rect.vCorners2.v1, rect.vCorners2.v2);
            p3 = new Vector3(rect.vCorners3.v0, rect.vCorners3.v1, rect.vCorners3.v2);

            if (!success)
                Debug.LogWarning("Failed to get Calibrated Play Area bounds!  Make sure you have tracking first, and that your space is calibrated.");

            if (initOpenVR)
                OpenVR.Shutdown();

            return success;
        }

        /// <summary>
        /// Get the OVR (Oculus) Boundaries to display the borders
        /// </summary>
        /// <param name="p0">The first point of the border</param>
        /// <param name="p1">The second point of the border</param>
        /// <param name="p2">The third point of the border</param>
        /// <param name="p3">The fourth point of the border</param>
        /// <returns>true if the boundaries of OVR could be correctly get</returns>
        private bool GetOVRBoundaries(out Vector3 p0, out Vector3 p1, out Vector3 p2, out Vector3 p3)
        {
            var chaperone = OVRManager.boundary;

            if (chaperone.GetConfigured())
            {
                var geometry = chaperone.GetGeometry(OVRBoundary.BoundaryType.PlayArea);
                bool success = (chaperone != null) && (geometry.Length > 0);

                p0 = new Vector3(geometry[0].x, geometry[0].y + 1.8f, geometry[0].z);
                p1 = new Vector3(geometry[1].x, geometry[1].y + 1.8f, geometry[1].z);
                p2 = new Vector3(geometry[2].x, geometry[2].y + 1.8f, geometry[2].z);
                p3 = new Vector3(geometry[3].x, geometry[3].y + 1.8f, geometry[3].z);

                if (!success)
                    Debug.LogWarning("Failed to get Calibrated Play Area bounds!  Make sure you have tracking first, and that your space is calibrated.");

                return success;
            }
            else
            {
                p0 = Vector3.zero;
                p1 = Vector3.zero;
                p2 = Vector3.zero;
                p3 = Vector3.zero;
                return true;
            }
        }

        /// <summary>
        /// Relaunch the system everytime we enter in a new scene
        /// </summary>
        /// <param name="sceneLoaded"></param>
        /// <param name="sceneMode"></param>
        private void OnSceneLoaded(Scene sceneLoaded, LoadSceneMode sceneMode)
        {
            // We remove the previous vallback to avoid adding multiple callback
            SceneManager.sceneLoaded -= OnSceneLoaded;
            this.Enabled = true;
        }
    }
}