using System.Collections.Generic;
using UnityEngine;
using VRSF.Utils.Components;
using VRSF.Utils.Components.ButtonActionChoser;

namespace VRSF.MoveAround.Teleport.Components
{
    /// <summary>
    /// Contains all variable necessary for the BezierTeleportSystems to work.
    /// </summary>
    [RequireComponent(typeof(BezierTeleportParametersComponent), typeof(BACGeneralVariablesComponents), typeof(ScriptableRaycastComponent))]
    public class BezierTeleportCalculationComponent : MonoBehaviour
    {
        [HideInInspector] public Transform _CurveOrigin;
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
    }
}