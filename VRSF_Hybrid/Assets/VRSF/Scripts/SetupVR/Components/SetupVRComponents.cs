using ScriptableFramework.Events;
using UnityEngine;

namespace VRSF.Utils.Components
{
    [RequireComponent(typeof(EventInspectorStandIn))]
    public class SetupVRComponents : MonoBehaviour
    {
        #region SDKS_PREFABS
        [Header("The 3 prefabs to load for the Vive, Oculus and Simulator.")]

        [Tooltip("If you don't want to use the VR Template under SetupVR, you can still drag the prefabs in your scene\n" +
            "and add the scripts to the prefabs directly. Don't forget to Apply the changes to the prefab and then to Remove it from the scene.")]
        [HideInInspector] public GameObject OpenVR_SDK;

        [Tooltip("If you don't want to use the VR Template under SetupVR, you can still drag the prefabs in your scene\n" +
            "and add the scripts to the prefabs directly. Don't forget to Apply the changes to the prefab and then to Remove it from the scene.")]
        [HideInInspector] public GameObject OVR_SDK;

        [Tooltip("If you don't want to use the VR Template under SetupVR, you can still drag the prefabs in your scene\n" +
            "and add the scripts to the prefabs directly. Don't forget to Apply the changes to the prefab and then to Remove it from the scene.")]
        [HideInInspector] public GameObject Simulator_SDK;
        #endregion SDKS_PREFABS


        #region SERIALIZED_FIELDS
        [Header("VR Device Parameters.")]
        [Tooltip("The Device you want to load.")]
        [SerializeField]
        public EDevice DeviceToLoad = EDevice.NULL;

        [Tooltip("If false, the device to load will be set with your Editor choice or with a potential starting screen choice.")]
        [SerializeField]
        public bool CheckDeviceAtRuntime = true;
        #endregion SERIALIZED_FIELDS


        #region SCRIPTS_CONTAINERS
        [Header("The references to the Scripts and Transform Containers.")]
        public GameObject CameraRigScripts;
        public GameObject LeftControllerScripts;
        public GameObject RightControllerScripts;
        public GameObject VRCameraScripts;
        #endregion SCRIPTS_CONTAINERS


        // Check if we already instantiated the SDK in the past, useful if the SDK is re-instantiated after a new scene has been loaded
        [HideInInspector] public bool SDKHasBeenInstantiated;
        [HideInInspector] public bool Loaded;
    }
}