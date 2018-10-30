using System.Collections;
using UnityEngine;
using Valve.VR;
using VRSF.Inputs;
using VRSF.Inputs.Events;
using VRSF.Utils;

namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// A generic component that renders a border using the given polylines.  
    /// The borders are double sided and are oriented upwards (ie normals are parallel to the XZ plane)
    /// 
    /// Disclaimer : This script is based on the Flafla2 Vive-Teleporter Repository. You can check it out here :
    /// https://github.com/Flafla2/Vive-Teleporter
    /// </summary>
    [AddComponentMenu("VRSF/Teleporter/Teleporter")]
    [RequireComponent(typeof(BorderRendererComponent))]
    public class TeleporterVR : MonoBehaviour
    {
        #region PUBLIC_VARIABLES
        //OK
        [Tooltip("Parabolic Pointer object to pull destination points from, and to assign to each controller.")]
        public ParabolicPointer Pointer;

        //OK
        /// How long, in seconds, the fade-in/fade-out animation should take
        [Tooltip("Duration of the \"blink\" animation (fading in and out upon teleport) in seconds.")]
        public float TeleportFadeDuration = 0.2f;

        //OK
        /// Measure in degrees of how often the controller should respond with a haptic click.  Smaller value=faster clicks
        [Tooltip("The player feels a haptic pulse in the controller when they raise / lower the controller by this many degrees.  Lower value = faster pulses.")]
        public float HapticClickAngleStep = 10;
        #endregion PUBLIC_VARIABLES


        #region PRIVATE_VARIABLES
        //OK
        /// Origin of the tracking space (basically the cameraRig)
        private Transform _originTransform;

        //OK
        /// Origin of the player's head
        private Transform _headTransform;

        //OK
        /// BorderRenderer to render the chaperone bounds (when choosing a location to teleport to)
        private BorderRendererComponent _roomBorder;

        //OK
        /// Animator used to fade in/out the teleport area.  This should have a boolean parameter "Enabled" where if true
        /// the selectable area is displayed on the ground.
        [Tooltip("Animator with a boolean \"Enabled\" parameter that is set to true when the player is choosing a place to teleport.")]
        [SerializeField] private Animator _navmeshAnimator;
        //OK
        private int _enabledAnimatorID;

        //OK
        /// Material used to render the fade in/fade out quad
        [Tooltip("Material used to render the fade in/fade out quad.")]
        [SerializeField] private Material _fadeMaterial;
        //OK
        private Material _fadeMaterialInstance;
        //OK
        private int _materialFadeID;
        
        //OK
        /// SteamVR controllers that should be polled.
        private GameObject _activeController;


        //OK
        private Vector3 _lastClickAngle = Vector3.zero;
        //OK
        private bool IsClicking = false;

        //OK
        private bool _fadingIn = false;

        //OK
        private float _teleportTimeMarker = -1;

        //OK
        private Mesh _planeMesh;
        #endregion PRIVATE_VARIABLES


        #region MONOBEHAVIOUR_METHODS
        private void Start()
        {
            StartCoroutine(InitVariables());
        }

        private void Update()
        {
            // If we are currently teleporting (ie handling the fade in/out transition)...
            if (CurrentTeleportState == ETeleportState.Teleporting)
            {
                HandleTeleportingState();
            }
            // At this point, we are NOT actively teleporting.  So now we care about controller input.
            else if (CurrentTeleportState == ETeleportState.Selecting)
            {
                HandleSelectingState();
            }
            else // if (CurrentTeleportState == ETeleportState.None)
            {
                HandleNoneState();
            }
        }

        // OK
        private void OnPostRender()
        {
            if (CurrentTeleportState == ETeleportState.Teleporting)
            {
                // Perform the fading in/fading out animation, if we are teleporting.  This is essentially a triangle wave
                // in/out, and the user teleports when it is fully black.
                float alpha = Mathf.Clamp01((Time.time - _teleportTimeMarker) / (TeleportFadeDuration / 2));
                if (_fadingIn)
                    alpha = 1 - alpha;

                Matrix4x4 local = Matrix4x4.TRS(Vector3.forward * 0.3f, Quaternion.identity, Vector3.one);
                _fadeMaterialInstance.SetPass(0);
                _fadeMaterialInstance.SetFloat(_materialFadeID, alpha);
                Graphics.DrawMeshNow(_planeMesh, transform.localToWorldMatrix * local);
            }
        }
        #endregion MONOBEHAVIOUR_METHODS


        #region PUBLIC_METHODS
        /// <summary>
        /// Requests the chaperone boundaries of the SteamVR play area.  This doesn't work if you haven't performed Room Setup.
        /// </summary>
        /// <param name="p0">Point 0 that make up the chaperone boundaries.</param>
        /// <param name="p1">Point 1 that make up the chaperone boundaries.</param>
        /// <param name="p2">Point 2 that make up the chaperone boundaries.</param>
        /// <param name="p3">Point 3 that make up the chaperone boundaries.</param>
        /// <returns>If the play area retrieval was successful</returns>
        //ok
        public static bool GetChaperoneBounds(out Vector3 p0, out Vector3 p1, out Vector3 p2, out Vector3 p3)
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
        #endregion PUBLIC_METHODS

        
        #region PRIVATE_METHODS
        //ok
        private static bool GetViveChaperone(out Vector3 p0, out Vector3 p1, out Vector3 p2, out Vector3 p3)
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

        //ok
        private static bool GetOVRBoundaries(out Vector3 p0, out Vector3 p1, out Vector3 p2, out Vector3 p3)
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

        /// <summary>
        /// Called in Update when the user is teleported.
        /// CHeck the Fading status and teleport the user when the Fading out is done.
        /// </summary>
        private void HandleTeleportingState()
        {
            Debug.Log("BoupBoup 3 ");
            // If we are currently teleporting (ie handling the fade in/out transition)...
           // if (User wasn't teleported yet)
           // {
                Debug.Log("BoupBoup 4");
                // Wait until half of the teleport time has passed before the next event (note: both the switch from fade
                // out to fade in and the switch from fade in to stop the animation is half of the fade duration)
                if (Time.time - _teleportTimeMarker >= TeleportFadeDuration / 2)
                {
                    if (_fadingIn)
                    {
                        // We have finished fading in
                        CurrentTeleportState = ETeleportState.None;
                    }
                    else
                    {
                        // We have finished fading out - time to teleport!
                        Vector3 offset = _originTransform.position - _headTransform.position;
                        offset.y = 0;
                        _originTransform.position = Pointer.SelectedPoint + offset;
                    }

                    _teleportTimeMarker = Time.time;
                    _fadingIn = !_fadingIn;
                }
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        private void HandleSelectingState()
        {
            Debug.Assert(_activeController != null);
            
            // If User stop clicking
            if (!InputVariableContainer.Instance.RightClickBoolean.Get("TriggerIsDown").Value)
            {
                // If the user has decided to teleport (ie lets go of touchpad) then remove all visual indicators
                // related to selecting things and actually teleport
                // If the user has decided to cancel (ie squeezes grip button) then remove visual indicators and do nothing
                if (Pointer.PointOnNavMesh)
                {
                    // Begin teleport sequence
                    CurrentTeleportState = ETeleportState.Teleporting;
                    _teleportTimeMarker = Time.time;
                }
                else
                {
                    CurrentTeleportState = ETeleportState.None;
                }

                // Reset active controller, disable pointer, disable visual indicators
                _activeController = null;
                Pointer.enabled = false;
                _roomBorder.enabled = false;
                //RoomBorder.Transpose = Matrix4x4.TRS(OriginTransform.position, Quaternion.identity, Vector3.one);
                if (_navmeshAnimator != null)
                    _navmeshAnimator.SetBool(_enabledAnimatorID, false);

                Pointer.transform.parent = null;
                Pointer.transform.position = Vector3.zero;
                Pointer.transform.rotation = Quaternion.identity;
                Pointer.transform.localScale = Vector3.one;
            }
            // If User is clicking
            else
            {
                CalculateRommBorderPos();
                // HandleHapticPulse(device);
            }
        }

        private void HandleNoneState()
        {
            Debug.Log("BoupBoup 1 ");
            // If User Start clicking
            // {
                // At this point the user is not holding down on the touchpad at all or has canceled a teleport and hasn't
                // let go of the touchpad.  So we wait for the user to press the touchpad and enable visual indicators
                // if necessary.
                Debug.Log("BoupBoup 2 ");
                // Set active controller to this controller, and enable the parabolic pointer and visual indicators
                // that the user can use to determine where they are able to teleport.
                _activeController = gameObject;

                Pointer.transform.parent = transform;
                Pointer.transform.localPosition = Vector3.zero;
                Pointer.transform.localRotation = Quaternion.identity;
                Pointer.transform.localScale = Vector3.one;
                Pointer.enabled = true;

                CurrentTeleportState = ETeleportState.Selecting;

                if (_navmeshAnimator != null)
                    _navmeshAnimator.SetBool(_enabledAnimatorID, true);

                Pointer.ForceUpdateCurrentAngle();
                _lastClickAngle = Pointer.CurrentPointVector;
                IsClicking = Pointer.PointOnNavMesh;
            // }
        }

        private void CalculateRommBorderPos()
        {
            // The user is still deciding where to teleport and has the touchpad held down.
            // Note: rendering of the parabolic pointer / marker is done in ParabolicPointer
            Vector3 offset = _headTransform.position - _originTransform.position;
            offset.y = 0;

            // Render representation of where the chaperone bounds will be after teleporting
            _roomBorder.enabled = Pointer.PointOnNavMesh;
            _roomBorder.Transpose = Matrix4x4.TRS(Pointer.SelectedPoint - offset, Quaternion.identity, Vector3.one);
        }

        private void HandleHapticPulse(SteamVR_Controller.Device device)
        {
            // Haptic feedback click every [HaptickClickAngleStep] degrees
            if (Pointer.CurrentParabolaAngleY >= 45) // Don't click when at max degrees
                _lastClickAngle = Pointer.CurrentPointVector;

            float angleClickDiff = Vector3.Angle(_lastClickAngle, Pointer.CurrentPointVector);
            if (IsClicking && Mathf.Abs(angleClickDiff) > HapticClickAngleStep)
            {
                _lastClickAngle = Pointer.CurrentPointVector;
                if (Pointer.PointOnNavMesh)
                    device.TriggerHapticPulse();
            }

            // Trigger a stronger haptic pulse when "entering" a teleportable surface
            if (Pointer.PointOnNavMesh && !IsClicking)
            {
                IsClicking = true;
                device.TriggerHapticPulse(750);
                _lastClickAngle = Pointer.CurrentPointVector;
            }
            else if (!Pointer.PointOnNavMesh && IsClicking)
                IsClicking = false;
        }

        /// <summary>
        /// Initialize all variables necessary to use the teleport system.
        /// </summary>
        //ok
        private IEnumerator InitVariables()
        {
            while (VRSF_Components.CameraRig == null)
            {
                yield return new WaitForEndOfFrame();
            }
            _originTransform = VRSF_Components.CameraRig.transform;
            _headTransform = VRSF_Components.VRCamera.transform;
            // Disable the pointer graphic (until the user holds down on the touchpad)
            Pointer.enabled = false;

            // Ensure we mark the player as not teleporting
            CurrentTeleportState = ETeleportState.None;

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
            _planeMesh = new Mesh
            {
                vertices = verts,
                triangles = elts
            };

            _planeMesh.RecalculateBounds();

            if (_fadeMaterial != null)
                _fadeMaterialInstance = new Material(_fadeMaterial);

            // Set some standard variables
            _materialFadeID = Shader.PropertyToID("_Fade");
            _enabledAnimatorID = Animator.StringToHash("Enabled");

            _roomBorder = GetComponent<BorderRendererComponent>();
            
            if (GetChaperoneBounds(out Vector3 p0, out Vector3 p1, out Vector3 p2, out Vector3 p3))
            {
                // Rotate to match camera rig rotation
                var originRotationMatrix = Matrix4x4.TRS(Vector3.zero, _originTransform.rotation, Vector3.one);

                BorderPointSet p = new BorderPointSet(new Vector3[]
                {
                    originRotationMatrix * p0,
                    originRotationMatrix * p1,
                    originRotationMatrix * p2,
                    originRotationMatrix * p3,
                    originRotationMatrix * p0,
                });

                _roomBorder.Points = new BorderPointSet[]
                {
                    p
                };
            }

            _roomBorder.enabled = false;
        }
        #endregion PRIVATE_METHODS


        #region GETTERS_SETTERS
        /// <summary>
        /// Indicates the current use of teleportation.
        /// None: The player is not using teleportation right now
        /// Selecting: The player is currently selecting a teleport destination (holding down on touchpad)
        /// Teleporting: The player has selected a teleport destination and is currently teleporting now (fading in/out)
        /// </summary>
        //ok
        public ETeleportState CurrentTeleportState { get; private set; }
        #endregion GETTERS_SETTERS
    }
}