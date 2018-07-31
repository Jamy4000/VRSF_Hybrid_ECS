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
    public class BorderRenderer : MonoBehaviour
    {
        #region PRIVATE_VARIABLES
        private Mesh[] CachedMeshes;
        
        private float LastBorderAlpha = 1.0f;

        [Tooltip("Layer to render the mesh at.")]
        private int AlphaShaderID = -1;
        
        private BorderPointSet[] _Points;
        
        [SerializeField]
        [Tooltip("Height of the border mesh, in meters.")]
        private float _BorderHeight = 0.2f;
        #endregion PRIVATE_VARIABLES


        #region PUBLIC_VARIABLES
        [SerializeField]
        [Range(0, 1)]
        [Tooltip("Alpha (transparency) of the border mesh.")]
        public float BorderAlpha = 1.0f;

        [Tooltip("Material used to render the border mesh.  UV's are set up so that v=0->bottom and v=1->top.  u is stretched along each edge.")]
        public Material BorderMaterial;

        [System.NonSerialized]
        public Matrix4x4 Transpose = Matrix4x4.identity;
        #endregion PUBLIC_VARIABLES


        #region MONOBEHAVIOURS_METHODS
        void Update()
        {
            if (CachedMeshes == null || BorderMaterial == null)
                return;

            if (LastBorderAlpha != BorderAlpha && BorderMaterial != null)
            {
                BorderMaterial.SetFloat("_Alpha", BorderAlpha);
                LastBorderAlpha = BorderAlpha;
            }

            foreach (Mesh m in CachedMeshes)
                Graphics.DrawMesh(m, Transpose, BorderMaterial, gameObject.layer, null, 0, null, false, false);
        }

        void OnValidate()
        {
            RegenerateMesh();

            if (AlphaShaderID == -1)
                AlphaShaderID = Shader.PropertyToID("_Alpha");
            if (BorderMaterial != null)
                BorderMaterial.SetFloat(AlphaShaderID, BorderAlpha);
        }
        #endregion MONOBEHAVIOURS_METHODS


        #region PUBLIC_METHODS
        /// <summary>
        /// Regenerate the Borders Meshes based on the Borders Points provided in the Editor
        /// </summary>
        public void RegenerateMesh()
        {
            // We check that the points are not null. If so, we set the Cahced Meshes to an empty Collection
            if (Points == null)
            {
                CachedMeshes = new Mesh[0];
            }
            else
            {
                CachedMeshes = new Mesh[Points.Length];

                // For each points in the ChachedMeshes, we check that the point is not null, and generate a new mesh
                // based on the Borders Points
                for (int x = 0; x < CachedMeshes.Length; x++)
                {
                    if (Points[x] == null || Points[x].Points == null)
                        CachedMeshes[x] = new Mesh();
                    else
                        CachedMeshes[x] = GenerateMeshForPoints(Points[x].Points);
                }
            }
        }
        #endregion PUBLIC_METHODS


        #region PRIVATE_METHODS
        /// <summary>
        /// Generate a new Mesh based on the Polylines provided
        /// </summary>
        /// <param name="Points">The Polylines where the border should be</param>
        /// <returns>The mesh generated</returns>
        private Mesh GenerateMeshForPoints(Vector3[] Points)
        {
            if (Points.Length <= 1)
                return new Mesh();

            Vector3[] verts = new Vector3[Points.Length * 2];
            Vector2[] uv = new Vector2[Points.Length * 2];

            // We calculate the mesh vertices and UVs based on the points positions and the Border Height
            for (int x = 0; x < Points.Length; x++)
            {
                verts[2 * x] = Points[x];
                verts[2 * x + 1] = Points[x] + Vector3.up * BorderHeight;

                uv[2 * x] = new Vector2(x % 2, 0);
                uv[2 * x + 1] = new Vector2(x % 2, 1);
            }

            int[] indices = new int[2 * 3 * (verts.Length - 2)];

            // We calculate the indices based on the amount of vertices
            for (int x = 0; x < verts.Length / 2 - 1; x++)
            {
                int p1 = 2 * x;
                int p2 = 2 * x + 1;
                int p3 = 2 * x + 2;
                int p4 = 2 * x + 3;

                indices[12 * x] = p1;
                indices[12 * x + 1] = p2;
                indices[12 * x + 2] = p3;
                indices[12 * x + 3] = p3;
                indices[12 * x + 4] = p2;
                indices[12 * x + 5] = p4;

                indices[12 * x + 6] = p3;
                indices[12 * x + 7] = p2;
                indices[12 * x + 8] = p1;
                indices[12 * x + 9] = p4;
                indices[12 * x + 10] = p2;
                indices[12 * x + 11] = p3;
            }

            // We create the new mesh based on the Vertices, UVs and indices/triangles calculated
            Mesh m = new Mesh
            {
                vertices = verts,
                uv = uv,
                triangles = indices
            };

            // We recalculate the bounds and normals of the generated Mesh
            m.RecalculateBounds();
            m.RecalculateNormals();

            return m;
        }
        #endregion PRIVATE_METHODS


        #region GETTERS_SETTERS
        /// <summary>
        /// Polylines that will be drawn.
        /// </summary>
        public BorderPointSet[] Points
        {
            get
            {
                return _Points;
            }
            set
            {
                _Points = value;
                RegenerateMesh();
            }
        }

        /// <summary>
        /// The Borders Height, ie How high goes the Mesh for the border
        /// </summary>
        public float BorderHeight
        {
            get
            {
                return _BorderHeight;
            }
            set
            {
                _BorderHeight = value;
                RegenerateMesh();
            }
        }
        #endregion GETTERS_SETTERS
    }
}