using Unity.Entities;
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
    public class BorderRendererSystem : ComponentSystem
    {
        struct Filter
        {
            public BorderRendererComponent BorderRenderer;
        }

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
        }

        protected override void OnUpdate()
        {
            foreach (var e in GetEntities<Filter>())
            {
                if (e.BorderRenderer.MeshNeedRegeneration)
                {
                    RegenerateMesh(e.BorderRenderer);
                }

                if (e.BorderRenderer.BorderAreShown)
                {
                    if (e.BorderRenderer.CachedMeshes == null || e.BorderRenderer.BorderMaterial == null)
                        return;

                    if (e.BorderRenderer.LastBorderAlpha != e.BorderRenderer.BorderAlpha && e.BorderRenderer.BorderMaterial != null)
                    {
                        e.BorderRenderer.BorderMaterial.SetFloat("_Alpha", e.BorderRenderer.BorderAlpha);
                        e.BorderRenderer.LastBorderAlpha = e.BorderRenderer.BorderAlpha;
                    }

                    foreach (Mesh m in e.BorderRenderer.CachedMeshes)
                        Graphics.DrawMesh(m, e.BorderRenderer.Transpose, e.BorderRenderer.BorderMaterial, e.BorderRenderer.gameObject.layer, null, 0, null, false, false);
                }
            }
        }

        /// <summary>
        /// Regenerate the Borders Meshes based on the Borders Points provided in the Editor
        /// </summary>
        private void RegenerateMesh(BorderRendererComponent borderRenderer)
        {
            // We check that the points are not null. If so, we set the Cahced Meshes to an empty Collection
            if (borderRenderer.Points == null)
            {
                borderRenderer.CachedMeshes = new Mesh[0];
            }
            else
            {
                borderRenderer.CachedMeshes = new Mesh[borderRenderer.Points.Length];

                // For each points in the ChachedMeshes, we check that the point is not null, and generate a new mesh
                // based on the Borders Points
                for (int x = 0; x < borderRenderer.CachedMeshes.Length; x++)
                {
                    if (borderRenderer.Points[x] == null || borderRenderer.Points[x].Points == null)
                        borderRenderer.CachedMeshes[x] = new Mesh();
                    else
                        borderRenderer.CachedMeshes[x] = GenerateMeshForPoints(borderRenderer.Points[x].Points, borderRenderer.BorderHeight);
                }
            }
        }


        /// <summary>
        /// Generate a new Mesh based on the Polylines provided
        /// </summary>
        /// <param name="Points">The Polylines where the border should be</param>
        /// <returns>The mesh generated</returns>
        private Mesh GenerateMeshForPoints(Vector3[] Points, float borderHeight)
        {
            if (Points.Length <= 1)
                return new Mesh();

            Vector3[] verts = new Vector3[Points.Length * 2];
            Vector2[] uv = new Vector2[Points.Length * 2];

            // We calculate the mesh vertices and UVs based on the points positions and the Border Height
            for (int x = 0; x < Points.Length; x++)
            {
                verts[2 * x] = Points[x];
                verts[2 * x + 1] = Points[x] + Vector3.up * borderHeight;

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
    }
}