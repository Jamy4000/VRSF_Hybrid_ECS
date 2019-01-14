using UnityEngine;
using VRSF.Utils;

namespace VRSF.MoveAround.Teleport.Systems
{
    /// <summary>
    /// Calculations helper for the Step by Step feature.
    /// </summary>
    public static class SBSCalculationsHelper
    {
        public static bool UserIsOnNavMesh(StepByStepSystem.Filter e, out Vector3 newCameraRigPos, LayerMask excludedLayer)
        {
            // We calculate the direction and the distance Vectors
            var directionVector = e.RayComp.RayVar.Value.direction;
            float distanceVector = VRSF_Components.CameraRig.transform.localScale.y * e.SBS_Comp.DistanceStepByStep;

            // Check if we hit a collider on the way. If it's the case, we reduce the distance.
            if (Physics.Raycast(VRSF_Components.VRCamera.transform.position, directionVector, out RaycastHit hit, distanceVector, ~e.RayComp.IgnoredLayers))
                distanceVector = 0;

            // We multiply the direction vector by the distance to which the user should be going
            directionVector *= distanceVector;

            // We check the theoritic new user pos
            var newCameraPos = GetNewTheoriticPos(directionVector, false);
            // We check the theoritic position for the cameraRig
            newCameraRigPos = GetNewTheoriticPos(directionVector, true);

            // We calculate a vector down based on the new Camera Pos. 
            var downVectorDistance = Mathf.Abs(VRSF_Components.VRCamera.transform.localPosition.y) + e.SBS_Comp.StepHeight;
            var downVector = newCameraPos + (Vector3.down * downVectorDistance);

            // We calculate the linecast between the newUserPos and the downVector and check if it hits the NavMesh
            TeleportNavMeshHelper.Linecast
            (
                newCameraPos,
                downVector,
                out bool endOnNavmesh,
                excludedLayer,
                out newCameraPos,
                out Vector3 norm,
                e.SceneObjects._TeleportNavMesh
            );

            // We substract the camera pos in y to the CameraRig pos in y
            newCameraRigPos.y = newCameraPos.y;

            return endOnNavmesh;
        }

        /// <summary>
        /// We get the theoritic position for the CameraRig and the VRCamera based on the scaled direction (direction * distance)
        /// </summary>
        /// <param name="scaledDirection">The direction multiplied by the distance to go to</param>
        /// <param name="isCheckingCameraRig">Whether the check is for the CameraRig or the VRCamera</param>
        /// <returns>The new Theoritic position</returns>
        private static Vector3 GetNewTheoriticPos(Vector3 scaledDirection, bool isCheckingCameraRig)
        {
            var origin = isCheckingCameraRig ? VRSF_Components.CameraRig.transform.position : VRSF_Components.VRCamera.transform.position;
            return origin + new Vector3(scaledDirection.x, 0.0f, scaledDirection.z);
        }
    }
}