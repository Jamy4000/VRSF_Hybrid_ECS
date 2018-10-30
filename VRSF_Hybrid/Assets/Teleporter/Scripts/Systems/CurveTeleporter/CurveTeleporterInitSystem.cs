using Unity.Entities;
using UnityEngine;
using Valve.VR;
using VRSF.Utils;
using VRSF.Utils.Components.ButtonActionChoser;

namespace VRSF.MoveAround.Teleport
{
    public class CurveTeleporterInitSystem : ComponentSystem
    {
        private struct Filter
        {
            public SceneObjectsComponent SceneObjects;
            public TeleportCalculationsComponent TeleportCalculations;
            public TeleportFadeComponent FadeComp;
            public NavMeshAnimatorComponent NavMeshAnim;
            public BACGeneralComponent BACGeneral;
        }

        protected override void OnUpdate()
        {
            if (VRSF_Components.SetupVRIsReady)
            {
                foreach (var e in GetEntities<Filter>())
                {
                    InitVariables(e);
                }
            }
        }

        /// <summary>
        /// Initialize all variables necessary to use the teleport system.
        /// </summary>
        private void InitVariables(Filter e)
        {
            // We set the references for the CameraRig and VRCamera Transforms
            e.SceneObjects._originTransform = VRSF_Components.CameraRig.transform;
            e.SceneObjects._headTransform = VRSF_Components.VRCamera.transform;

            // We set the active controller to the RayOrigin of this teleporter
            e.SceneObjects._activeController = e.BACGeneral.ButtonHand == Controllers.EHand.RIGHT ?
                VRSF_Components.RightController : VRSF_Components.LeftController;
            
            // Ensure we mark the player as not teleporting
            e.TeleportCalculations.CurrentTeleportState = ETeleportState.None;

            Vector3[] verts = new Vector3[]
            {
                new Vector3(-1, -1, 0),
                new Vector3(-1, 1, 0),
                new Vector3(1, 1, 0),
                new Vector3(1, -1, 0)
            };

            int[] elts = new int[] { 0, 1, 2, 0, 2, 3 };

            // Standard plane mesh used for "fade out" graphic when you teleport
            // This way you don't need to supply a simple plane mesh in the inspector
            e.FadeComp._planeMesh = new Mesh
            {
                vertices = verts,
                triangles = elts
            };
            e.FadeComp._planeMesh.RecalculateBounds();

            // Set some standard variables for the FadeComponent
            if (e.FadeComp._fadeMaterial != null)
            {
                e.FadeComp._fadeMaterialInstance = new Material(e.FadeComp._fadeMaterial);
            }
            e.FadeComp._materialFadeID = Shader.PropertyToID("_Fade");
            e.FadeComp._TeleportCalculations = e.TeleportCalculations;

            // Set some standard variables for the TeleportNavMeshComponent
            e.NavMeshAnim._navmeshAnimator = e.NavMeshAnim.GetComponent<Animator>();
            e.NavMeshAnim._enabledAnimatorID = Animator.StringToHash("Enabled");

            // Set some standard variables for the SceneObjectsComponent
            e.SceneObjects.Pointer = e.SceneObjects.GetComponent<PointerCalculationsComponent>();
            e.SceneObjects._roomBorder = e.SceneObjects.GetComponent<BorderRendererComponent>();

            // We check if a Play Area was set by the user
            if (GetChaperoneBounds(out Vector3 p0, out Vector3 p1, out Vector3 p2, out Vector3 p3))
            {
                // Rotate to match camera rig rotation
                var originRotationMatrix = Matrix4x4.TRS(Vector3.zero, e.SceneObjects._originTransform.rotation, Vector3.one);

                BorderPointSet p = new BorderPointSet(new Vector3[]
                {
                    originRotationMatrix * p0,
                    originRotationMatrix * p1,
                    originRotationMatrix * p2,
                    originRotationMatrix * p3,
                    originRotationMatrix * p0,
                });

                e.SceneObjects._roomBorder.Points = new BorderPointSet[]
                {
                    p
                };
            }

            e.SceneObjects._roomBorder.enabled = false;
            e.TeleportCalculations._IsSetup = true;
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
            bool toReturn = false;

            switch (VRSF_Components.DeviceLoaded)
            {
                case EDevice.OPENVR:
                    toReturn = GetViveChaperone(out p0, out p1, out p2, out p3);
                    break;
                case EDevice.OCULUS_RIFT:
                case EDevice.PORTABLE_OVR:
                    toReturn = GetOVRBoundaries(out p0, out p1, out p2, out p3);
                    break;
                default:
                    p0 = Vector3.zero;
                    p1 = Vector3.zero;
                    p2 = Vector3.zero;
                    p3 = Vector3.zero;
                    return toReturn;
            }

            return toReturn;
        }
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

        private bool GetOVRBoundaries(out Vector3 p0, out Vector3 p1, out Vector3 p2, out Vector3 p3)
        {
            var chaperone = OVRManager.boundary;

            if (chaperone.GetConfigured())
            {
                var geometry = chaperone.GetGeometry(OVRBoundary.BoundaryType.PlayArea);
                bool success = (chaperone != null) && (geometry.Length > 0);

                p0 = geometry[0];
                p1 = geometry[1];
                p2 = geometry[2];
                p3 = geometry[3];

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
    }
}