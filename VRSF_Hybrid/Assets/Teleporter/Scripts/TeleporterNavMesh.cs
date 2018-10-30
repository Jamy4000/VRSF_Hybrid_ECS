using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.Rendering;
using System.Collections.Generic;

namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// Disclaimer : This script is based on the Flafla2 Vive-Teleporter Repository. You can check it out here :
    /// https://github.com/Flafla2/Vive-Teleporter
    /// 
    /// A version of Unity's baked navmesh that is converted to a (serializable) component.  This allows the navmesh 
    /// used for Vive navigation to be separated form the AI Navmesh.  ViveNavMesh also handles the rendering of the 
    /// NavMesh grid in-game.
    /// </summary>
    [AddComponentMenu("VRSF/Teleport/Teleport Nav Mesh")]
    [RequireComponent(typeof(BorderRendererComponent))]
    [ExecuteInEditMode]
    public class TeleporterNavMesh : MonoBehaviour
    {
        #region PUBLIC_VARIABLES
        /// <summary>
        /// The alpha (transparency) value of the rendered ground mesh
        /// </summary>
        [Range(0, 1)]
        public float GroundAlpha = 1.0f;

        public bool IgnoreSlopedSurfaces { get { return _IgnoreSlopedSurfaces; } }

        public ENavmeshDewarpingMethod DewarpingMethod { get { return _DewarpingMethod; } }
        #endregion PUBLIC_VARIABLES


        #region PRIVATE_VARIABLES
        [FormerlySerializedAs("_GroundMaterial")]
        [SerializeField] private Material _GroundMaterialSource;
        private Material _GroundMaterial;
        
        private float LastGroundAlpha = 1.0f;
        private int AlphaShaderID = -1;

        [SerializeField] private int _LayerMask = 0;
        [SerializeField] private bool _IgnoreLayerMask = true;

        [SerializeField] private int _QueryTriggerInteraction = 0;

        [SerializeField]
        [HideInInspector] private Mesh _SelectableMesh;

        [SerializeField]
        [HideInInspector] private BorderPointSet[] _SelectableMeshBorder;

        [SerializeField]
        [HideInInspector] private int _NavAreaMask = ~0; // Initialize to all

        [SerializeField] private bool _IgnoreSlopedSurfaces = true;

        [SerializeField] private float _SampleRadius = 0.25f;

        [SerializeField] private ENavmeshDewarpingMethod _DewarpingMethod = ENavmeshDewarpingMethod.None;

        private BorderRendererComponent Border;

        private Dictionary<Camera, CommandBuffer> cameras = new Dictionary<Camera, CommandBuffer>();
        #endregion PRIVATE_VARIABLES


        #region MONOBEHAVIOUR_METHODS

        private void Start()
        {
            if (SelectableMesh == null)
                SelectableMesh = new Mesh();
            if (_SelectableMeshBorder == null)
                _SelectableMeshBorder = new BorderPointSet[0];

            Border = GetComponent<BorderRendererComponent>();
            Border.Points = SelectableMeshBorder;

            AlphaShaderID = Shader.PropertyToID("_Alpha");

            if (_GroundMaterialSource != null)
                GroundMaterial = new Material(_GroundMaterialSource);

            if (GroundAlpha != LastGroundAlpha && GroundMaterial != null)
            {
                GroundMaterial.SetFloat(AlphaShaderID, GroundAlpha);
                LastGroundAlpha = GroundAlpha;
            }

#if UNITY_EDITOR
            UnityEditor.SceneView.RepaintAll();
#endif
        }

        private void Update()
        {
            // We have to detect changes this way instead of using properties because
            // we want to be able to animate the alpha value with a Unity animator.
            if (GroundAlpha != LastGroundAlpha && GroundMaterial != null)
            {
                GroundMaterial.SetFloat(AlphaShaderID, GroundAlpha);
                LastGroundAlpha = GroundAlpha;
            }
        }

        public void OnEnable()
        {
            Cleanup();
        }

        public void OnDisable()
        {
            Cleanup();
        }

        private void OnRenderObject()
        {
            // We have to use command buffers instead of Graphics.DrawMesh because of strange depth issues that I am experiencing
            // with Graphics.Drawmesh (perhaps Graphics.DrawMesh is called before all opaque objects are rendered?)
            var act = gameObject.activeInHierarchy && enabled;
            if (!act)
            {
                Cleanup();
                return;
            }

            // If _SelectableMesh == null there is a crash in Unity 5.4 beta (apparently you can't pass null to CommandBuffer::DrawMesh now).
            if (!_SelectableMesh || !GroundMaterial)
                return;

            var cam = Camera.current;
            if (!cam || cam.cameraType == CameraType.Preview || ((1 << gameObject.layer) & Camera.current.cullingMask) == 0)
                return;

            CommandBuffer buf = null;
            if (cameras.ContainsKey(cam))
                return;

            buf = new CommandBuffer();
            // Note: Mesh is drawn slightly pushed upwards to avoid z-fighting issues
            buf.DrawMesh(_SelectableMesh, Matrix4x4.TRS(Vector3.up * 0.005f, Quaternion.identity, Vector3.one), GroundMaterial, 0);
            cameras[cam] = buf;
            cam.AddCommandBuffer(CameraEvent.AfterForwardOpaque, buf);
        }

        private void OnValidate()
        {
            Border = GetComponent<BorderRendererComponent>();
            Border.Points = SelectableMeshBorder;

            if (AlphaShaderID == -1)
                AlphaShaderID = Shader.PropertyToID("_Alpha");
        }
        #endregion MONOBEHAVIOUR_METHODS


        #region PUBLIC_METHODS
        
        /// <summary>
        /// Casts a ray against the Navmesh and attempts to calculate the ray's worldspace intersection with it.
        /// This uses Physics raycasts to perform the raycast calculation, so the teleport surface must have a collider on it.
        /// </summary>
        /// 
        /// <param name="p1">First (origin) point of ray</param>
        /// <param name="p2">Last (end) point of ray</param>
        /// <param name="pointOnNavmesh">If the raycast hit something on the navmesh</param>
        /// <param name="hitPoint">If hit, the point of the hit. Otherwise zero.</param>
        /// <param name="normal">If hit, the normal of the hit surface.  Otherwise (0, 1, 0)</param>
        /// 
        /// <returns>If the raycast hit something.</returns>
        public bool Linecast(Vector3 p1, Vector3 p2, out bool pointOnNavmesh, out Vector3 hitPoint, out Vector3 normal)
        {
            RaycastHit hit;
            Vector3 dir = p2 - p1;
            float dist = dir.magnitude;
            dir /= dist;
            if (Physics.Raycast(p1, dir, out hit, dist, _IgnoreLayerMask ? ~_LayerMask : _LayerMask, (QueryTriggerInteraction)_QueryTriggerInteraction))
            {
                normal = hit.normal;
                if (Vector3.Dot(Vector3.up, hit.normal) < 0.99f && _IgnoreSlopedSurfaces)
                {
                    pointOnNavmesh = false;
                    hitPoint = hit.point;

                    return true;
                }

                hitPoint = hit.point;
                NavMeshHit navHit;
                pointOnNavmesh = NavMesh.SamplePosition(hitPoint, out navHit, _SampleRadius, _NavAreaMask);

                return true;
            }
            pointOnNavmesh = false;
            hitPoint = Vector3.zero;
            normal = Vector3.up;
            return false;
        }
        #endregion PUBLIC_METHODS


        #region PRIVATE_METHODS
        /// <summary>
        /// Remove the Command Buffer from the Cameras and Clear the Cameras Dictionnary
        /// </summary>
        private void Cleanup()
        {
            foreach (var cam in cameras)
            {
                if (cam.Key)
                {
                    cam.Key.RemoveCommandBuffer(CameraEvent.AfterForwardOpaque, cam.Value);
                }
            }
            cameras.Clear();
        }
        #endregion PRIVATE_METHODS


        #region GETTERS_SETTERS
        /// <summary>
        /// Material used for the floor mesh when the user is selecting a point to teleport to
        /// </summary>
        public Material GroundMaterial
        {
            get { return _GroundMaterial; }
            set
            {
                Material old = _GroundMaterial;
                _GroundMaterial = value;
                if (_GroundMaterial != null)
                    _GroundMaterial.SetFloat(AlphaShaderID, GroundAlpha);
                if (old != _GroundMaterial)
                    Cleanup();
            }
        }

        public int LayerMask
        {
            get { return _LayerMask; }
            set { _LayerMask = value; }
        }

        public bool IgnoreLayerMask
        {
            get { return _IgnoreLayerMask; }
            set { _IgnoreLayerMask = value; }
        }

        public int QueryTriggerInteraction
        {
            get { return _QueryTriggerInteraction; }
            set { _QueryTriggerInteraction = value; }
        }

        /// A Mesh that represents the "Selectable" area of the world.  This is converted from Unity's NavMesh in ViveNavMeshEditor
        public Mesh SelectableMesh
        {
            get { return _SelectableMesh; }
            set { _SelectableMesh = value; Cleanup(); } // Cleanup because we need to change the mesh inside command buffers
        }

        /// <summary>
        /// The border points of SelectableMesh.  This is automatically generated in ViveNavMeshEditor.
        /// 
        /// This is an array of Vector3 arrays, where each Vector3 array is the points in a polyline. 
        /// These polylines combined describe the borders of SelectableMesh.  
        /// We have to use BorderPointSets instead of a jagged Vector3[][] array because
        /// Unity can't serialize jagged arrays for some reason.
        /// </summary>
        public BorderPointSet[] SelectableMeshBorder
        {
            get { return _SelectableMeshBorder; }
            set { _SelectableMeshBorder = value; Border.Points = _SelectableMeshBorder; }
        }

        public float SampleRadius
        {
            get { return _SampleRadius; }
        }
        #endregion GETTERS_SETTERS
    }
}