using UnityEngine;

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


        // EMPTY
        #region PRIVATE_VARIABLES

        #endregion


        #region PUBLIC_METHODS
        /// <summary>
        /// Setup the VRSF Objects based on the Scripts Container in scene
        /// </summary>
        /// <param name="scriptsContainer">The GameObject to copy</param>
        /// <param name="newInstance">The new Instance of the VRSF Object</param>
        public static void SetupTransformFromContainer(GameObject scriptsContainer, ref GameObject newInstance)
        {
            // We copy the transform values of the scriptsContainer to the newInstance
            newInstance.transform.position = scriptsContainer.transform.position;
            newInstance.transform.localScale = scriptsContainer.transform.localScale;
            newInstance.transform.rotation = scriptsContainer.transform.rotation;

            // We set the script container as child of the new Instance object
            scriptsContainer.transform.SetParent(newInstance.transform);
        }
        #endregion


        // EMPTY
        #region PRIVATE_METHODS

        #endregion


        // EMPTY
        #region GETTERS_SETTERS

        #endregion
    }
}