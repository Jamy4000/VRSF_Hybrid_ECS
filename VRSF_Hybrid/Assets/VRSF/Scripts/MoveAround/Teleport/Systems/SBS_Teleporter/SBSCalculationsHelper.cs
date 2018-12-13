using UnityEngine;
using VRSF.Utils;

namespace VRSF.MoveAround.Teleport.Systems
{
    public static class SBSCalculationsHelper
    {

        public static bool UserIsOnNavMesh(StepByStepSystem.Filter e, out Vector3 newTheoriticPos)
        {
            var directionVector = CheckHandForward(e);

            // If the new theoritic position is null, an error as occured, so we stop the method
            if (directionVector != Vector3.zero)
            {
                // We check the theoritic new user pos
                newTheoriticPos = VRSF_Components.CameraRig.transform.position + new Vector3(directionVector.x, 0.0f, directionVector.z);

                // We launch a Vector directed to the floor to check if the new position is on the Teleport NavMesh
                // And multiply it by the current height of the user
                var vectorDownFactor = Vector3.down * newTheoriticPos.y;

                // We calculate a vector down based on the new User Pos
                var downVector = newTheoriticPos + vectorDownFactor;

                // We calculate the linecast between the newUserPos and the downVector and check if it hits the NavMesh
                TeleportNavMeshHelper.Linecast
                (
                    newTheoriticPos,
                    downVector,
                    out bool endOnNavmesh,
                    e.TeleportGeneral.ExclusionLayer,
                    out e.TeleportGeneral.PointToGoTo,
                    out Vector3 norm,
                    e.SceneObjects._TeleportNavMesh
                );

                return endOnNavmesh;
            }
            else
            {
                newTheoriticPos = Vector3.zero;
                return false;
            }
        }


        /// <summary>
        /// Check, depending on the RayOrigin and the User's size, the forward vector to use.
        /// </summary>
        /// <returns>The new theoritical position of the user</returns>
        private static Vector3 CheckHandForward(StepByStepSystem.Filter entity)
        {
            float distanceWithScale = VRSF_Components.CameraRig.transform.localScale.y * entity.SBS_Comp.DistanceStepByStep;

            switch (entity.BAC_Comp.ButtonHand)
            {
                case Controllers.EHand.LEFT:
                    return VRSF_Components.LeftController.transform.forward * distanceWithScale;
                case Controllers.EHand.RIGHT:
                    return VRSF_Components.RightController.transform.forward * distanceWithScale;
                case Controllers.EHand.GAZE:
                    return VRSF_Components.VRCamera.transform.forward * distanceWithScale;
                default:
                    Debug.LogError("Please specify a valid RayOrigin in the Inspector to be able to use the Teleport StepByStep feature.");
                    return Vector3.zero;
            }
        }
    }
}