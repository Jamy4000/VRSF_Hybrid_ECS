using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Rendering;
using System.Collections.Generic;
using Unity.Entities;

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
    [RequireComponent(typeof(GameObjectEntity))]
    [ExecuteInEditMode]
    public class TeleportNavMeshComponent : MonoBehaviour
    {
        /// <summary>
        /// The alpha (transparency) value of the rendered ground mesh
        /// </summary>
        [Range(0, 1)]
        [SerializeField]
        [HideInInspector] public float GroundAlpha = 1.0f;

        public bool IgnoreSlopedSurfaces { get { return _IgnoreSlopedSurfaces; } }

        public ENavmeshDewarpingMethod DewarpingMethod { get { return _DewarpingMethod; } }

        
        [FormerlySerializedAs("_GroundMaterial")]
        [SerializeField] public Material _GroundMaterialSource;
        
        [System.NonSerialized] public int AlphaShaderID = -1;
        
        [SerializeField] public int _QueryTriggerInteraction = 0;

        [SerializeField]
        [HideInInspector] public Mesh _SelectableMesh;

        [SerializeField]
        [HideInInspector] public int _NavAreaMask = ~0; // Initialize to all

        [SerializeField] public bool _IgnoreSlopedSurfaces = true;

        [SerializeField] public float _SampleRadius = 0.25f;

        [SerializeField] public ENavmeshDewarpingMethod _DewarpingMethod = ENavmeshDewarpingMethod.None;

        [System.NonSerialized] public Dictionary<Camera, CommandBuffer> _Cameras = new Dictionary<Camera, CommandBuffer>();

        public void OnEnable()
        {
            TeleportNavMeshHelper.Cleanup(this);
        }

        public void OnDisable()
        {
            TeleportNavMeshHelper.Cleanup(this);
        }

        private void OnRenderObject()
        {
            // We have to use command buffers instead of Graphics.DrawMesh because of strange depth issues that I am experiencing
            // with Graphics.Drawmesh (perhaps Graphics.DrawMesh is called before all opaque objects are rendered?)
            var act = gameObject.activeInHierarchy && enabled;
            if (!act)
            {
                TeleportNavMeshHelper.Cleanup(this);
                return;
            }
            
            // We set the alpha of the Ground
            if (_GroundMaterialSource != null)
                _GroundMaterialSource.SetFloat(AlphaShaderID, GroundAlpha);

            var cam = Camera.current;
            if (!cam || cam.cameraType == CameraType.Preview || ((1 << gameObject.layer) & Camera.current.cullingMask) == 0)
                return;
            
           // CommandBuffer buf = null; 
            if (_Cameras.ContainsKey(cam))
                return;

            CommandBuffer buf = new CommandBuffer();
            // Note: Mesh is drawn slightly pushed upwards to avoid z-fighting issues
            buf.DrawMesh(_SelectableMesh, Matrix4x4.TRS(Vector3.up * 0.005f, Quaternion.identity, Vector3.one), GroundMaterial, 0);
            _Cameras[cam] = buf;
            cam.AddCommandBuffer(CameraEvent.AfterForwardOpaque, buf);
        }

        private void OnValidate()
        {
           // GetComponent<BorderRendererComponent>().Points = SelectableMeshBorder;

            if (AlphaShaderID == -1)
                AlphaShaderID = Shader.PropertyToID("_Alpha");
        }

        #region GETTERS_SETTERS
        /// <summary>
        /// Material used for the floor mesh when the user is selecting a point to teleport to
        /// </summary>
        public Material GroundMaterial
        {
            get { return _GroundMaterialSource; }
            set
            {
                Material old = _GroundMaterialSource;
                _GroundMaterialSource = value;
                if (_GroundMaterialSource != null)
                    _GroundMaterialSource.SetFloat(AlphaShaderID, GroundAlpha);
                if (old != _GroundMaterialSource)
                    TeleportNavMeshHelper.Cleanup(this);
            }
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
            set
            {
                _SelectableMesh = value;
                // Cleanup because we need to change the mesh inside command buffers
                TeleportNavMeshHelper.Cleanup(this);
            }
        }

        public float SampleRadius
        {
            get { return _SampleRadius; }
        }
        #endregion GETTERS_SETTERS
    }
}