using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRSF.Controllers;
using VRSF.Utils;

/// <summary>
/// This script was largely inspired by the one provided by I_JEMIN and its Simple VR Teleporter Assets.
/// You can check it out at this adress : https://assetstore.unity.com/packages/tools/input-management/simple-vr-teleporter-115996
/// </summary>
namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// Create a Bezier Curve to teleport the User
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class BezierTeleporter : ButtonActionChoser
    {
        #region PUBLIC_VARIABLES
        [Header("Teleport Target")]
        [Tooltip("Specify the GameObject corresponding to the Target of the Bezier Curve.\n" +
            "It will be used to display the ground position.")]
        public GameObject TargetMarker;
        #endregion PUBLIC_VARIABLES


        #region PRIVATE_VARIABLES
        // VRSF Parameters
        private TeleportParametersVariable _teleportParameters;
        private Transform _cameraRig;

        private LayerMask _GroundLayer = -1;             // The Layer for the Ground
        private Transform _RayOrigin;
        private int _ExclusionLayer = -1;
        private LineRenderer _ControllerPointer;

        // Bezier calculation Parameters
        private int _MaxVertexcount = 1000; // limitation of vertices for performance. 

        private float _VertexDelta = 0.08f; // Delta between each Vertex on arc. Decresing this value may cause performance problem.

        private LineRenderer _ArcRenderer;

        private Vector3 _Velocity; // Velocity of latest vertex

        private Vector3 _GroundPos; // detected ground position

        private Vector3 _LastNormal; // detected surface normal

        private bool _GroundDetected = false;

        private List<Vector3> _VertexList = new List<Vector3>(); // vertex on arc

        private bool _DisplayActive = false; // don't update path when it's false.

        private bool _LimitDetected = false;

        private bool _IsSetup = false;
        #endregion PRIVATE_VARIABLES


        #region MONOBEHAVIOUR_METHODS
        public override void Start()
        {
            base.Start();
            InitializeValues();
        }

        private void FixedUpdate()
        {
            if (!_IsSetup)
            {
                InitializeValues();
                return;
            }

            if (_DisplayActive)
            {
                UpdatePath();
            }
        }
        #endregion MONOBEHAVIOUR_METHODS


        #region PUBLIC_METHODS
        /// <summary>
        /// Teleport the user to the ground position
        /// </summary>
        public void Teleport()
        {
            if (_GroundDetected || _LimitDetected)
            {
                _cameraRig.position = _GroundPos + new Vector3(0, _teleportParameters.HeightAboveGround + _cameraRig.localScale.x, 0) + _LastNormal * 0.1f;
            }
            ToggleDisplay(false);
        }

        /// <summary>
        /// Active Teleporter Arc Path
        /// </summary>
        /// <param name="active"></param>
        public void ToggleDisplay(bool active)
        {
            _ArcRenderer.enabled = active;
            TargetMarker.SetActive(active);
            _DisplayActive = active;

            // Change pointer activation if the user is using it
            if ((RayOrigin == EHand.LEFT && ControllersParameters.UsePointerLeft) || (RayOrigin == EHand.RIGHT && ControllersParameters.UsePointerRight))
                _ControllerPointer.enabled = !active;
        }
        #endregion PUBLIC_METHODS


        #region PRIVATE_METHODS
        /// <summary>
        /// Display the TeleportPath and check if it touch the ground
        /// </summary>
        private void UpdatePath()
        {
            float strength = _teleportParameters.BezierStrength;
            _ArcRenderer.startWidth = _cameraRig.localScale.x / 10;
            _ArcRenderer.endWidth = _cameraRig.localScale.x / 10;

            if (_cameraRig.localScale.y > 1.0f)
            {
                // Setting lineRenderer size according to cameraRig size
                strength *= _cameraRig.localScale.x / 10;
                _ArcRenderer.textureMode = LineTextureMode.DistributePerSegment;
            }
            else
            {
                _ArcRenderer.textureMode = LineTextureMode.RepeatPerSegment;
            }

            // Init
            _GroundDetected = false;
            _VertexList.Clear(); // delete all previouse vertices
            _LimitDetected = false;

            // Check the Velocity
            _Velocity = Quaternion.AngleAxis(-_teleportParameters.BezierAngle, transform.right) * transform.forward * strength;

            DisplayArc();
            CheckHitPointArc();

            // Update Line Renderer
            _ArcRenderer.positionCount = _VertexList.Count;
            _ArcRenderer.SetPositions(_VertexList.ToArray());
        }


        /// <summary>
        /// Start while loop to check if the ground was hit. During that, register all new vertec position for Line Renderer.
        /// </summary>
        private void DisplayArc()
        {
            // Init
            RaycastHit hit = new RaycastHit();
            Vector3 pos = _RayOrigin.position; // take off position
            _VertexList.Add(pos);

            // Start the loop to check the ground
            while (!_GroundDetected && _VertexList.Count < _MaxVertexcount)
            {
                Vector3 newPos = pos + _Velocity * _VertexDelta
                    + 0.5f * Physics.gravity * _VertexDelta * _VertexDelta;

                Vector3 boundedPos = CheckNewPosLongTeleportBoundaries(newPos);

                _Velocity += Physics.gravity * _VertexDelta;

                // add new calculated vertex
                _VertexList.Add(boundedPos);

                // if linecast between last vertex and current vertex hit something
                if (Physics.Linecast(pos, boundedPos, out hit, _ExclusionLayer))
                {
                    _GroundDetected = true;
                    _GroundPos = hit.point;
                    _LastNormal = hit.normal;
                    _VertexList.RemoveAt(_VertexList.Count - 1);
                }
                // if the boundedPos is different from the theoritical newPos
                else if (boundedPos != newPos)
                {
                    _LimitDetected = true;
                    _GroundPos = boundedPos;
                    _LastNormal = hit.normal;
                    _VertexList.RemoveAt(_VertexList.Count - 1);
                    break;
                }
                pos = boundedPos; // update current vertex as last vertex
            }
        }


        /// <summary>
        /// Check if the ground or the boundaries were hit, and set the Target position and color and the arc color accordingly
        /// </summary>
        private void CheckHitPointArc()
        {
            if (_GroundDetected || _LimitDetected)
            {
                TargetMarker.SetActive(true);

                TargetMarker.transform.position = _GroundPos;
                TargetMarker.transform.rotation = Quaternion.FromToRotation(Vector3.forward, _LastNormal);
                
                _ArcRenderer.material.color = _teleportParameters.ColorBezierHitting;
                TargetMarker.GetComponent<Image>().color = _teleportParameters.ColorBezierHitting;
            }
            else
            {
                TargetMarker.SetActive(false);

                _ArcRenderer.material.color = _teleportParameters.ColorBezierNotHitting;
                TargetMarker.GetComponent<Image>().color = _teleportParameters.ColorBezierNotHitting;
            }
        }


        /// <summary>
        /// Check the newPos for the Long Teleport feature depending on the Teleport Boundaries
        /// </summary>
        private Vector3 CheckNewPosLongTeleportBoundaries(Vector3 PosToCheck)
        {
            if (_teleportParameters.UseBoundaries)
            {
                float newX = Mathf.Clamp(PosToCheck.x, _teleportParameters.MinAvatarPosition.x, _teleportParameters.MaxAvatarPosition.x);
                float newY = Mathf.Clamp(PosToCheck.y, _teleportParameters.MinAvatarPosition.y, _teleportParameters.MaxAvatarPosition.y);
                float newZ = Mathf.Clamp(PosToCheck.z, _teleportParameters.MinAvatarPosition.z, _teleportParameters.MaxAvatarPosition.z);
                PosToCheck = new Vector3(newX, newY, newZ);
            }
            return PosToCheck;
        }


        /// <summary>
        /// Initialize the variable for this script
        /// </summary>
        private void InitializeValues()
        {
            try
            {
                _teleportParameters = TeleportParametersVariable.Instance;
                _cameraRig = VRSF_Components.CameraRig.transform;
                
                if (RayOrigin == EHand.GAZE)
                {
                    _ExclusionLayer = GazeParameters.GetGazeExclusionsLayer();
                }
                else
                {
                    _ExclusionLayer = ControllersParameters.GetExclusionsLayer(RayOrigin);
                }

                CheckHand();

                _GroundLayer = LayerMask.NameToLayer("Teleport");

                if (_GroundLayer == -1)
                {
                    Debug.Log("VRSF : You won't be able to teleport on the floor, as you didn't set the Ground Layer");
                }

                _ArcRenderer = GetComponent<LineRenderer>();
                _ArcRenderer.enabled = false;
                TargetMarker.SetActive(false);

                _IsSetup = true;
            }
            catch (System.Exception e)
            {
                Debug.Log("VRSF : VR Components not setup yet, waiting for next frame.\n" + e);
            }
        }


        /// <summary>
        /// Set the RayOrigin Transform reference depending on the Hand holding the script.
        /// </summary>
        private void CheckHand()
        {
            switch (RayOrigin)
            {
                case (EHand.LEFT):
                    _RayOrigin = VRSF_Components.LeftController.transform;

                    if (ControllersParameters.UsePointerLeft)
                        _ControllerPointer = VRSF_Components.LeftController.GetComponent<LineRenderer>();
                    break;

                case (EHand.RIGHT):
                    _RayOrigin = VRSF_Components.RightController.transform;

                    if (ControllersParameters.UsePointerRight)
                        _ControllerPointer = VRSF_Components.RightController.GetComponent<LineRenderer>();
                    break;

                case (EHand.GAZE):
                    _RayOrigin = VRSF_Components.VRCamera.transform;
                    break;

                default:
                    Debug.LogError("Please specify a valid hand in the BezierTeleport script. The Gaze cannot be used.");
                    break;
            }
        }
        #endregion PRIVATE_METHODS
    }
}