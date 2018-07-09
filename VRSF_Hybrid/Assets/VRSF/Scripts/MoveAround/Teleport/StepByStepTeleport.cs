using UnityEngine;
using VRSF.Utils;

namespace VRSF.MoveAround.Teleport
{
	public class StepByStepTeleport : ButtonActionChoser 
	{
        // EMPTY
        #region PUBLIC_VARIABLES

        #endregion

            
        #region PRIVATE_VARIABLES
        // Scriptable Parameter for the teleport script
        private TeleportParametersVariable _teleportParameters;
        #endregion


        #region MONOBEHAVIOUR_METHODS
        // Use this for initialization
        public void Awake()
        {
            _teleportParameters = TeleportParametersVariable.Instance;
        }
        #endregion

        
        #region PUBLIC_METHODS
        /// <summary>
        /// Method to call from StartClicking or StartTouching, teleport the user one meter away.
        /// </summary>
        public void TeleportSBS()
        {
            // We check where the user should be when teleported one meter away.
            Vector3 newPos = CheckHandForward();

            // If the new pos returned is null, an error as occured, so we stop the method
            if (newPos == Vector3.zero)
                return;

            // If we want to stay on the same vertical axis, we set the y in newPos to 0
            if (!_teleportParameters.MoveOnVerticalAxis)
                newPos = new Vector3(newPos.x, 0.0f, newPos.z);

            // If we use boundaries, we check if the user is not going to far away
            if (_teleportParameters.UseBoundaries)
                newPos = CheckNewPosSBSBoundaries(newPos);

            // We set the cameraRig position
            VRSF_Components.CameraRig.transform.position += newPos;
        }
        #endregion

        
        #region PRIVATE_METHODS
        /// <summary>
        /// Check, depending on the RayOrigin and the User's size, the forward vector to use.
        /// </summary>
        /// <returns>The new theoritical position of the user</returns>
        private Vector3 CheckHandForward()
        {
            float distanceWithScale = VRSF_Components.CameraRig.transform.localScale.x * _teleportParameters.DistanceStepByStep;
            switch (RayOrigin)
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


        /// <summary>
        /// Check the newPos for theStep by Step feature depending on the Teleport Boundaries
        /// </summary>
        private Vector3 CheckNewPosSBSBoundaries(Vector3 PosToCheck)
        {
            Vector3 avatarNewPos = VRSF_Components.CameraRig.transform.position + PosToCheck;

            float newX = CheckAxisValue(PosToCheck.x, avatarNewPos.x, _teleportParameters.MinAvatarPosition.x, _teleportParameters.MaxAvatarPosition.x);
            float newY = CheckAxisValue(PosToCheck.y, avatarNewPos.y, _teleportParameters.MinAvatarPosition.y, _teleportParameters.MaxAvatarPosition.y);
            float newZ = CheckAxisValue(PosToCheck.z, avatarNewPos.z, _teleportParameters.MinAvatarPosition.z, _teleportParameters.MaxAvatarPosition.z);

            return new Vector3(newX, newY, newZ);
        }


        /// <summary>
        /// Kind of like a Clampf method, but adjust for our purpose
        /// </summary>
        /// <param name="baseValue">The position to check</param>
        /// <param name="avatarAxisPos">The new position of the avatar after adding the position to check to it</param>
        /// <param name="minVal">the minimum value in the scene</param>
        /// <param name="maxVal">The maximum value for teleporting in the scene</param>
        /// <returns>the new position in scene for the avatar, on one axis</returns>
        private float CheckAxisValue(float baseValue, float avatarAxisPos, float minVal, float maxVal)
        {
            if (avatarAxisPos > maxVal)
                return maxVal - avatarAxisPos;
            else if (avatarAxisPos < minVal)
                return minVal - avatarAxisPos;
            else
                return baseValue;
        }
        #endregion


        // EMPTY
        #region GETTERS_SETTERS

        #endregion
    }
}