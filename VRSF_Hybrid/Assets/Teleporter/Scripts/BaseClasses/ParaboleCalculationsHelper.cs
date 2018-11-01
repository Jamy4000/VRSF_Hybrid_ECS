using System.Collections.Generic;
using UnityEngine;

namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// A helper class containing the mathematics to calculate a parabole curve.
    /// 
    /// Disclaimer : This script is based on the Flafla2 Vive-Teleporter Repository. You can check it out here :
    /// https://github.com/Flafla2/Vive-Teleporter
    /// </summary>
    public static class ParaboleCalculationsHelper
    {
        /// <summary>
        /// Sample a bunch of points along a parabolic curve until you hit gnd.  At that point, cut off the parabola
        /// </summary>
        /// 
        /// <param name="p0">starting point of parabola</param>
        /// <param name="v0">initial parabola velocity</param>
        /// <param name="a">initial acceleration</param>
        /// <param name="dist">distance between sample points</param>
        /// <param name="points">number of sample points</param>
        /// <param name="nav">Vive Nav Mesh used to teleport</param>
        /// <param name="outPts">List that will be populated by new points</param>
        /// <param name="normal">normal of hit point</param>
        /// 
        /// <returns>true if the the parabole is at the end of the NavMesh</returns>
        public static bool CalculateParabolicCurve(Vector3 p0, Vector3 v0, Vector3 a, float dist, int points, TeleportNavMeshComponent nav, int excludedLayer, List<Vector3> outPts, out Vector3 normal)
        {
            outPts.Clear();
            outPts.Add(p0);

            Vector3 last = p0;
            float t = 0;

            for (int i = 0; i < points; i++)
            {
                t += dist / ParabolicCurveDeriv(v0, a, t).magnitude;
                Vector3 next = ParabolicCurve(p0, v0, a, t);

                if (TeleportNavMeshHelper.Linecast(last, next, out bool endOnNavmesh, excludedLayer, out Vector3 castHit, out Vector3 norm, nav))
                {
                    outPts.Add(castHit);
                    normal = norm;
                    return endOnNavmesh;
                }
                else
                {
                    outPts.Add(next);
                }

                last = next;
            }

            normal = Vector3.up;
            return false;
        }

        /// <summary>
        /// Project a point on a normalized plane
        /// </summary>
        /// <param name="planeNormal">The normalized Vector</param>
        /// <param name="point">The point to project</param>
        /// <returns>The new projected Vector</returns>
        public static Vector3 ProjectVectorOntoPlane(Vector3 planeNormal, Vector3 point)
        {
            Vector3 d = Vector3.Project(point, planeNormal.normalized);
            return point - d;
        }

        /// <summary>
        /// Generate the mesh of the parabole
        /// </summary>
        /// <param name="m">The mesh to generate, pass as reference</param>
        /// <param name="points">The list of points on the path of the parabole</param>
        /// <param name="fwd">The forward Vector for the parabole</param>
        /// <param name="uvoffset">The offset for the UV</param>
        /// <param name="graphicThickness">The thickness of the parabole to dispaly</param>
        public static void GenerateMesh(ref Mesh m, List<Vector3> points, Vector3 fwd, float uvoffset, float graphicThickness)
        {
            Vector3[] verts = new Vector3[points.Count * 2];
            Vector2[] uv = new Vector2[points.Count * 2];

            Vector3 right = Vector3.Cross(fwd, Vector3.up).normalized;

            for (int x = 0; x < points.Count; x++)
            {
                verts[2 * x] = points[x] - right * graphicThickness / 2;
                verts[2 * x + 1] = points[x] + right * graphicThickness / 2;

                float uvoffset_mod = uvoffset;
                if (x == points.Count - 1 && x > 1)
                {
                    float dist_last = (points[x - 2] - points[x - 1]).magnitude;
                    float dist_cur = (points[x] - points[x - 1]).magnitude;
                    uvoffset_mod += 1 - dist_cur / dist_last;
                }

                uv[2 * x] = new Vector2(0, x - uvoffset_mod);
                uv[2 * x + 1] = new Vector2(1, x - uvoffset_mod);
            }

            int[] indices = new int[2 * 3 * (verts.Length - 2)];
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

            m.Clear();
            m.vertices = verts;
            m.uv = uv;
            m.triangles = indices;
            m.RecalculateBounds();
            m.RecalculateNormals();
        }


        /// <summary>
        /// Clamps the given velocity vector so that it can't be more than 45 degrees above the horizontal.
        /// This is done so that it is easier to leverage the maximum distance (at the 45 degree angle) of
        /// parabolic motion.
        /// </summary>
        /// <returns>angle with reference to the XZ plane</returns>
        public static float ClampInitialVelocity(ref Vector3 velocity, out Vector3 velocity_normalized, Vector3 initialVelocity)
        {
            // Project the initial velocity onto the XZ plane.  This gives us the "forward" direction
            Vector3 velocity_fwd = ParaboleCalculationsHelper.ProjectVectorOntoPlane(Vector3.up, velocity);

            // Find the angle between the XZ plane and the velocity
            float angle = Vector3.Angle(velocity_fwd, velocity);
            // Calculate positivity/negativity of the angle using the cross product
            // Below is "right" from controller's perspective (could also be left, but it doesn't matter for our purposes)
            Vector3 right = Vector3.Cross(Vector3.up, velocity_fwd);
            // If the cross product between forward and the velocity is in the same direction as right, then we are below the vertical
            if (Vector3.Dot(right, Vector3.Cross(velocity_fwd, velocity)) > 0)
                angle *= -1;

            // Clamp the angle if it is greater than 45 degrees
            //if (angle > 45)
            //{
            //    velocity = Vector3.Slerp(velocity_fwd, velocity, 45f / angle);
            //    velocity /= velocity.magnitude;
            //    velocity_normalized = velocity;
            //    velocity *= initialVelocity.magnitude;
            //    angle = 45;
            //}
            //else
            velocity_normalized = velocity.normalized;

            return angle;
        }

        /// <summary>
        /// Parabolic motion equation, y = p0 + v0*t + 1/2at^2
        /// </summary> 
        private static float ParabolicCurve(float p0, float v0, float a, float t)
        {
            return p0 + v0 * t + 0.5f * a * t * t;
        }

        /// <summary>
        /// Derivative of parabolic motion equation
        /// </summary> 
        private static float ParabolicCurveDeriv(float v0, float a, float t)
        {
            return v0 + a * t;
        }

        /// <summary>
        /// Parabolic motion equation applied to 3 dimensions
        /// </summary> 
        private static Vector3 ParabolicCurve(Vector3 p0, Vector3 v0, Vector3 a, float t)
        {
            Vector3 ret = new Vector3();
            for (int x = 0; x < 3; x++)
                ret[x] = ParabolicCurve(p0[x], v0[x], a[x], t);
            return ret;
        }

        /// <summary>
        /// Parabolic motion derivative applied to 3 dimensions
        /// </summary> 
        private static Vector3 ParabolicCurveDeriv(Vector3 v0, Vector3 a, float t)
        {
            Vector3 ret = new Vector3();
            for (int x = 0; x < 3; x++)
                ret[x] = ParabolicCurveDeriv(v0[x], a[x], t);
            return ret;
        }
    }
}