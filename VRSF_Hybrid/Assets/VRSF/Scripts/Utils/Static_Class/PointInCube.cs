using UnityEngine;

namespace VRSF.Utils
{
    /// <summary>
    /// The method below is based on the math provided in this answer : https://math.stackexchange.com/a/1472080
    /// </summary>
    public static class PointInCube
    {
        /// <summary>
        /// Calculate if a Point is within a cube based on its vertices.
        /// To use this functiun, you need to follow this vertices order :
        /// vertices[0] = origin (0, 0, 0)
        /// vertices[1] = yMax (0, 1, 0)
        /// vertices[3] = zMax (0, 0, 1)
        /// vertices[4] = xMax (1, 0, 0)
        /// </summary>
        /// <param name="point">The Point to test</param>
        /// <param name="vertices">The Vertices of the cube</param>
        /// <returns>true if the point is within the cube</returns>
        public static bool IsPointWithtinACube(Vector3 point, Vector3[] vertices)
        {
            // We get the Point of Interest in the cube
            Vector3 origin = vertices[0];
            Vector3 xMax = vertices[4];
            Vector3 yMax = vertices[1];
            Vector3 zMax = vertices[3];

            // We calculate the 3 vector for the distance between origin and the Point of Interests
            Vector3 u = origin - zMax;
            Vector3 v = origin - xMax;
            Vector3 w = origin - yMax;

            // We calculate the dot product of the vectors for the boundaries of the cube
            float uMin = Vector3.Dot(u, origin);
            float uMax = Vector3.Dot(u, zMax);

            float vMin = Vector3.Dot(v, origin);
            float vMax = Vector3.Dot(v, xMax);

            float wMin = Vector3.Dot(w, origin);
            float wMax = Vector3.Dot(w, yMax);

            // We calculate the dot product for our point
            float uPoint = Vector3.Dot(u, point);
            float vPoint = Vector3.Dot(v, point);
            float wPoint = Vector3.Dot(w, point);

            // We check if the dot product of each points is within the boundaries calculated
            return (uPoint < uMax && uPoint > uMin &&
                vPoint < vMax && vPoint > vMin &&
                wPoint < wMax && wPoint > wMin);
        }

        public static Vector3 GetClosestPoint(Vector3 point, Vector3[] vertices)
        {

            return point;
        }
    }
}