using UnityEngine;
using VRSF.Utils;

namespace VRSF.MoveAround.Teleport.Systems
{
    public static class SBSCalculationsHelper
    {

        public static bool UserIsOnNavMesh(StepByStepSystem.Filter e, out Vector3 newTheoriticPos, LayerMask excludedLayer)
        {
            var directionVector = CheckHandForward(e);
            
            // We check the theoritic new user pos
            newTheoriticPos = VRSF_Components.VRCamera.transform.position + new Vector3(directionVector.x, 0.0f, directionVector.z);
                
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