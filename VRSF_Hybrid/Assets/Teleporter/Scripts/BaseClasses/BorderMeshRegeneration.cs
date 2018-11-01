using UnityEngine;
using VRSF.Utils;

namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// Disclaimer : This script is based on the Flafla2 Vive-Teleporter Repository. You can check it out here :
    /// https://github.com/Flafla2/Vive-Teleporter
    /// 
    /// A generic System that renders a border using the given polylines.  
    /// The borders are double sided and are oriented upwards (ie normals are parallel to the XZ plane)
    /// </summary>
    public static class BorderMeshRegeneration
    {
        /// <summary>
        /// Regenerate the Borders Meshes based on the Borders Points provided in the Editor
        /// </summary>
        public static void RegenerateMesh(BorderRendererComponent borderRenderer, Vector3 selectedPoint)
        {
            // We check that the points are not null. 
            if (borderRenderer.Points != null || borderRenderer.Points.Length > 0)
            {
                // The user is still deciding where to teleport and has the touchpad held down.
                // Note: rendering of the parabolic pointer / marker is done in ParabolicPointer
                Vector3 offset = VRSF_Components.VRCamera.transform.position - VRSF_Components.CameraRig.transform.position;
                offset.y = 0.0f;
                
                // Render representation of where the chaperone bounds will be after teleporting
                borderRenderer.Transpose = Matrix4x4.TRS(selectedPoint - offset, Quaternion.identity, Vector3.one);

                borderRenderer.CachedBorderMesh = GenerateMeshForPoints(borderRenderer.Points, borderRenderer.BorderHeight);
            }
        }


        /// <summary>
        /// Generate a new Mesh based on the Polylines provided
        /// </summary>
        /// <param name="Points">The Polylines where the border should be</param>
        /// <returns>The mesh generated</returns>
        private static Mesh GenerateMeshForPoints(Vector3[] Points, float borderHeight)
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