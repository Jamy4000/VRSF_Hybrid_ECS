using System.Collections.Generic;
using UnityEngine;
using VRSF.MoveAround.Teleport.Interfaces;

namespace VRSF.MoveAround.Teleport.Components
{
    /// <summary>
    /// Contains all variable necessary for the BezierTeleportSystems to work.
    /// It implements the ITeleportComponent to be able to display the boundaries limits with TeleportBoundaries.
    /// </summary>
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity), typeof(Utils.Components.ButtonActionChoserComponents))]
    public class BezierTeleportComponent : MonoBehaviour, ITeleportComponent
    {
        #region For_User_Variable
        [Header("Bezier Curve Parameters")]

        [Tooltip("The height at which the user is teleported above the ground.")]
        public float HeightAboveGround = 1.8f;

        [Tooltip("The value corresponding to the Arc take off angle.")]
        public float BezierAngle = 45f;
        [Tooltip("The value corresponding to the overall arc length.\n" +
            "Increasing this value will increase overall arc length")]
        public float BezierStrength = 10f;

        [Tooltip("The color of the bézier ray when it's not hitting anything.")]
        public Color ColorBezierNotHitting = Color.red;
        [Tooltip("The color of the bézier ray when it's hitting something.")]
        public Color ColorBezierHitting = Color.green;


        [Header("Teleport Target")]
        [Tooltip("Specify the GameObject corresponding to the Target of the Bezier Curve.\n" +
            "It will be used to display the ground position.")]
        public GameObject TargetMarker;
        #endregion For_User_Variable


        #region Not_For_User_Variable
        [HideInInspector] public LayerMask _TeleportLayer = -1;             // The Layer for the Ground
        [HideInInspector] public Transform _CurveOrigin;
        [HideInInspector] public int _ExclusionLayer = -1;
        [HideInInspector] public LineRenderer _ControllerPointer;

        // Bezier calculation Parameters
        [HideInInspector] public int _MaxVertexcount = 1000; // limitation of vertices for performance. 

        [HideInInspector] public float _VertexDelta = 0.08f; // Delta between each Vertex on arc. Decresing this value may cause performance problem.

        [HideInInspector] public LineRenderer _ArcRenderer;

        [HideInInspector] public Vector3 _Velocity; // Velocity of latest vertex

        [HideInInspector] public Vector3 _GroundPos; // detected ground position

        [HideInInspector] public Vector3 _LastNormal; // detected surface normal

        [HideInInspector] public bool _GroundDetected = false;

        [HideInInspector] public List<Vector3> _VertexList = new List<Vector3>(); // vertex on arc

        [HideInInspector] public bool _DisplayActive = false; // don't update path when it's false.

        [HideInInspector] public bool _LimitDetected = false;

        [HideInInspector] public bool _IsSetup = false;
        #endregion Not_For_User_Variable


        #region Boundaries
        [Tooltip("Wheter you wanna use boundaries for the flying mode or not.")]
        [HideInInspector] public bool _UseBoundaries = false;

        [Tooltip("The minimun position at which the user can go if UseHorizontalBoundaries is at true.")]
        [HideInInspector] public Vector3 _MinUserPosition = new Vector3(-100.0f, -1.0f, -100.0f);

        [Tooltip("The maximum position at which the user can go if UseHorizontalBoundaries is at true.")]
        [HideInInspector] public Vector3 _MaxUserPosition = new Vector3(100.0f, 100.0f, 100.0f);
        #endregion Boundaries


        #region Getters_ITeleportComponent
        public bool UseBoundaries()
        {
            return _UseBoundaries;
        }

        public Vector3 MaxPosBoundaries()
        {
            return _MaxUserPosition;
        }

        public Vector3 MinPosBoundaries()
        {
            return _MinUserPosition;
        }
        #endregion Getters_ITeleportComponent
    }
}