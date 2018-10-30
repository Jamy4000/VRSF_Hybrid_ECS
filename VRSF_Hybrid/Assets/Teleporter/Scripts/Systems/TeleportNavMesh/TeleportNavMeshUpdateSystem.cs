using Unity.Entities;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// Init the values for the Teleport Nav Mesh Classes
    /// 
    /// Disclaimer : This script is based on the Flafla2 Vive-Teleporter Repository. You can check it out here :
    /// https://github.com/Flafla2/Vive-Teleporter
    /// </summary>
    public class TeleportNavMeshUpdateSystem : ComponentSystem
    {

        private struct Filter
        {
            public TeleportNavMeshComponent TeleportNavMesh;
            public BorderRendererComponent Border;
        }

        protected override void OnUpdate()
        {
            foreach (var e in GetEntities<Filter>())
            {
                if (e.TeleportNavMesh.NeedCleanUp)
                {
                    Cleanup(e.TeleportNavMesh);
                }
            }
        }

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
        public static bool Linecast(Vector3 p1, Vector3 p2, out bool pointOnNavmesh, out Vector3 hitPoint, out Vector3 normal, TeleportNavMeshComponent teleportNavMesh)
        {
            Vector3 dir = p2 - p1;
            float dist = dir.magnitude;
            dir /= dist;

            if (Physics.Raycast(p1, dir, out RaycastHit hit, dist, teleportNavMesh._IgnoreLayerMask ? ~teleportNavMesh._LayerMask : teleportNavMesh._LayerMask, (QueryTriggerInteraction)teleportNavMesh._QueryTriggerInteraction))
            {
                normal = hit.normal;
                if (Vector3.Dot(Vector3.up, hit.normal) < 0.99f && teleportNavMesh._IgnoreSlopedSurfaces)
                {
                    pointOnNavmesh = false;
                    hitPoint = hit.point;

                    return true;
                }

                hitPoint = hit.point;
                pointOnNavmesh = NavMesh.SamplePosition(hitPoint, out NavMeshHit navHit, teleportNavMesh._SampleRadius, teleportNavMesh._NavAreaMask);

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
        private void Cleanup(TeleportNavMeshComponent teleportNavMesh)
        {
            foreach (var cam in teleportNavMesh.cameras)
            {
                if (cam.Key)
                {
                    cam.Key.RemoveCommandBuffer(CameraEvent.AfterForwardOpaque, cam.Value);
                }
            }
            teleportNavMesh.cameras.Clear();
            teleportNavMesh.NeedCleanUp = false;
        }
        #endregion PRIVATE_METHODS
    }
}