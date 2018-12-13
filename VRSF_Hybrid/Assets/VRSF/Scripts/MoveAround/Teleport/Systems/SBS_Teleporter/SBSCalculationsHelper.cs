using UnityEngine;
using VRSF.Utils;

namespace VRSF.MoveAround.Teleport.Systems
{
    public static class SBSCalculationsHelper
    {
        public static bool UserIsOnNavMesh(StepByStepSystem.Filter e, out Vector3 newTheoriticPos, LayerMask excludedLayer)
        {
            // We check the theoritic new user pos
            newTheoriticPos = GetNewPosWithObstacle(e, excludedLayer);
                
            // We calculate a vector down based on the new User Pos. 
            var downVector = newTheoriticPos + (Vector3.down * Mathf.Abs(newTheoriticPos.y));
            
            // We calculate the linecast between the newUserPos and the downVector and check if it hits the NavMesh
            TeleportNavMeshHelper.Linecast
            (
                newTheoriticPos,
                downVector,
                out bool endOnNavmesh,
                excludedLayer,
                out e.TeleportGeneral.PointToGoTo,
                out Vector3 norm,
                e.SceneObjects._TeleportNavMesh
            );

            //As the CameraRig is placed on the NavMesh (it's basically the floor), we set the newUser Pos to the downVector
            newTheoriticPos = e.TeleportGeneral.PointToGoTo;

            return endOnNavmesh;
        }

        private static Vector3 GetNewPosWithObstacle(StepByStepSystem.Filter e, LayerMask excludedLayer)
        {
            var origin = VRSF_Components.VRCamera.transform.position;
            var directionVector = CheckHandForward(e);
            float distanceWithScale = VRSF_Components.CameraRig.transform.localScale.y * e.SBS_Comp.DistanceStepByStep;

            if (Physics.Raycast(origin, directionVector, out RaycastHit hit, distanceWithScale, excludedLayer))
            {
                return hit.point - (directionVector / 2);
            }
            else
            {
                directionVector *= distanceWithScale;
                return origin + new Vector3(directionVector.x, 0.0f, directionVector.z);
            }
        }

        /// <summary>
        /// Check, depending on the RayOrigin and the User's size, the forward vector to use.
        /// </summary>
        /// <returns>The new theoritical position of the user</returns>
        private static Vector3 CheckHandForward(StepByStepSystem.Filter entity)
        {
            switch (entity.BAC_Comp.ButtonHand)
            {
                case Controllers.EHand.LEFT:
                    return VRSF_Components.LeftController.transform.forward;
                case Controllers.EHand.RIGHT:
                    return VRSF_Components.RightController.transform.forward;
                case Controllers.EHand.GAZE:
                    return VRSF_Components.VRCamera.transform.forward;
                default:
                    Debug.LogError("Please specify a valid RayOrigin in the Inspector to be able to use the Teleport StepByStep feature.");
                    return Vector3.zero;
            }
        }
    }
}