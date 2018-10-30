using UnityEngine;

namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// Disclaimer : This script is based on the Flafla2 Vive-Teleporter Repository. You can check it out here :
    /// https://github.com/Flafla2/Vive-Teleporter
    /// 
    /// A generic component that renders a border using the given polylines.  
    /// The borders are double sided and are oriented upwards (ie normals are parallel to the XZ plane)
    /// </summary>
    [AddComponentMenu("VRSF/Teleport/Border Renderer")]
    [ExecuteInEditMode]
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity))]
    public class BorderRendererComponent : MonoBehaviour
    {
        #region PRIVATE_VARIABLES
        [SerializeField]
        [Tooltip("Height of the border mesh, in meters.")]
        private float _borderHeight = 0.2f;

        private BorderPointSet[] _points;
        #endregion PRIVATE_VARIABLES


        #region PUBLIC_VARIABLES
        [System.NonSerialized] public Mesh[] CachedMeshes;

        [System.NonSerialized] public float LastBorderAlpha = 1.0f;

        // Layer to render the mesh at
        [System.NonSerialized] public int AlphaShaderID = -1;

        [System.NonSerialized] public Matrix4x4 Transpose = Matrix4x4.identity;

        [System.NonSerialized] public bool MeshNeedRegeneration = false;

        [Range(0, 1)]
        [Tooltip("Alpha (transparency) of the border mesh.")]
        public float BorderAlpha = 1.0f;

        [Tooltip("Material used to render the border mesh.  UV's are set up so that v=0->bottom and v=1->top.  u is stretched along each edge.")]
        public Material BorderMaterial;
        #endregion PUBLIC_VARIABLES


        #region GETTERS_SETTERS
        /// <summary>
        /// Polylines that will be drawn.
        /// </summary>
        public BorderPointSet[] Points
        {
            get
            {
                return _points;
            }
            set
            {
                _points = value;
                MeshNeedRegeneration = true;
            }
        }

        /// <summary>
        /// The Borders Height, ie How high goes the Mesh for the border
        /// </summary>
        public float BorderHeight
        {
            get
            {
                return _borderHeight;
            }
            set
            {
                _borderHeight = value;
                MeshNeedRegeneration = true;
            }
        }
        #endregion GETTERS_SETTERS

#if UNITY_EDITOR
        void OnValidate()
        {
            MeshNeedRegeneration = true;

            if (AlphaShaderID == -1)
                AlphaShaderID = Shader.PropertyToID("_Alpha");
            if (BorderMaterial != null)
                BorderMaterial.SetFloat(AlphaShaderID, BorderAlpha);
        }
#endif
    }
}