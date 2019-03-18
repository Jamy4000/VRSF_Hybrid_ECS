using UnityEngine;
using VRSF.Core.Controllers;

namespace VRSF.Core.SetupVR
{
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity))]
    public class SetupVRComponents : MonoBehaviour
    {
        #region SERIALIZED_FIELDS
        [Header("VR Device Parameters.")]
        [Tooltip("The Device you want to load.")]
        [SerializeField]
        public EDevice DeviceToLoad = EDevice.NULL;

        [Tooltip("If false, the device to load will be set with your Editor choice or with a potential starting screen choice.")]
        [SerializeField]
        public bool CheckDeviceAtRuntime = true;
        #endregion SERIALIZED_FIELDS

        #region CONTROLLERS
        [Header("The prefabs to load for the Vive and Rift controllers.")]

        [Tooltip("If you don't want to use the VR Template under SetupVR, you can still drag the prefabs in your scene\n" +
            "and add the scripts to the prefabs directly. Don't forget to Apply the changes to the prefab and then to Remove it from the scene.")]
        public VRController[] Vive_Controllers;

        [Tooltip("If you don't want to use the VR Template under SetupVR, you can still drag the prefabs in your scene\n" +
            "and add the scripts to the prefabs directly. Don't forget to Apply the changes to the prefab and then to Remove it from the scene.")]
        public VRController[] Rift_Controllers;
        #endregion CONTROLLERS

        [Header("The references to the VR Transforms.")]
        public GameObject CameraRig;
        public GameObject LeftController;
        public GameObject RightController;
        public GameObject VRCamera;


        // Check if we already instantiated the SDK in the past, useful if the SDK is re-instantiated after a new scene has been loaded
        [HideInInspector] public bool IsLoaded;
    }
}