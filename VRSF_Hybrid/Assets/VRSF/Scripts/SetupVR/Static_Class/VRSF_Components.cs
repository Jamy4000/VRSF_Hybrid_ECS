using UnityEngine;
using VRSF.Controllers;

namespace VRSF.Utils
{
    /// <summary>
    /// Contains all the static references for the VRSF Objects
    /// </summary>
	public static class VRSF_Components
    {
        #region PUBLIC_VARIABLES
        [Header("A reference to the CameraRig object")]
        public static GameObject CameraRig;

        [Header("A reference to the Controllers")]
        public static GameObject RightController;
        public static GameObject LeftController;

        [Header("A reference to the Camera")]
        public static GameObject VRCamera;

        [Header("The name of the SDK that has been loaded. Not necessary if you're using a Starting Screen.")]
        public static EDevice DeviceLoaded = EDevice.NULL;
        #endregion


        #region PRIVATE_VARIABLES
        private static bool _setupVRIsReady = false;
        #endregion


        #region PUBLIC_METHODS
        /// <summary>
        /// Setup the VRSF Objects based on the Scripts Container in scene
        /// </summary>
        /// <param name="scriptsContainer">The GameObject to copy</param>
        /// <param name="newInstance">The new Instance of the VRSF Object</param>
        public static void SetupTransformFromContainer(GameObject scriptsContainer, ref GameObject newInstance)
        {
            if (newInstance.name.Contains("CameraRig"))
            {
                SetCameraRigPosition(scriptsContainer.transform.position, true, (DeviceLoaded == EDevice.OPENVR));
            }
            else
            {
                newInstance.transform.position = scriptsContainer.transform.position;
            }

            // We copy the transform values of the scriptsContainer to the newInstance
            newInstance.transform.localScale = scriptsContainer.transform.localScale;
            newInstance.transform.rotation = scriptsContainer.transform.rotation;

            // We set the script container as child of the new Instance object
            scriptsContainer.transform.SetParent(newInstance.transform);
        }

        /// <summary>
        /// Method to set the CameraRig position by taking account of the offset of the VRCamera's position
        /// </summary>
        /// <param name="newPos">The new Pos where the user should be in World coordinate</param>
        /// <param name="setYPos">Wheter we have to change the Y position</param>
        /// <param name="useYOffset">Whether we need to check the offset of the Y localPosition of the VRCamera</param>
        public static void SetCameraRigPosition(Vector3 newPos, bool setYPos = true, bool useYOffset = false)
        {
            if (useYOffset)
            {
                newPos -= VRCamera.transform.localPosition;
            }
            else
            {
                newPos.x -= VRCamera.transform.localPosition.x;
                newPos.z -= VRCamera.transform.localPosition.z;
            }

            CameraRig.transform.position = setYPos ? newPos : new Vector3(newPos.x, CameraRig.transform.position.y, newPos.z);
        }
        #endregion


        // EMPTY
        #region PRIVATE_METHODS

        #endregion


        #region GETTERS_SETTERS
        public static bool SetupVRIsReady
        {
            get
            {
                // If we use the controller, we check the object in this class. If not, we only check the VRCamera and the CameraRig. 
                return ControllersParametersVariable.Instance.UseControllers ?
                    (CameraRig != null && VRCamera != null) :
                    (CameraRig != null && VRCamera != null && LeftController != null && RightController != null);
            }


            set => _setupVRIsReady = value;
        }
        #endregion
    }
}