using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using VRSF.Inputs;
using VRSF.Controllers;
using System;

namespace VRSF.Utils
{
    /// <summary>
    /// Script placed on the SetupVR Prefab.
    /// Load the selected SDK at runtime, and stock all the references to the important VR components.
    /// </summary>
    [RequireComponent(typeof(ScriptableFramework.Events.EventInspectorStandIn))]
    public class SetupVR : MonoBehaviour
    {
        #region PUBLIC_VARIABLES

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


        #region SCRIPTS_CONTAINERS
        [Header("The references to the Scripts Containers, children of SetupVR.")]

        [HideInInspector] public GameObject CameraRigScripts;

        [HideInInspector] public GameObject LeftControllerScripts;

        [HideInInspector] public GameObject RightControllerScripts;

        [HideInInspector] public GameObject VRCameraScripts;
        #endregion SCRIPTS_TEMPLATES


        #region SERIALIZED_FIELDS
        [Header("VR Device Parameters.")]
        [Tooltip("The Device you want to load.")]
        [SerializeField]
        public EDevice DeviceToLoad = EDevice.NULL;

        [Tooltip("If false, the device to load will be set with your Editor choice or with a potential starting screen choice.")]
        [SerializeField]
        public bool CheckDeviceAtRuntime = true;
        #endregion SERIALIZED_FIELDS


        [Header("SDK Changes in Hierarchy")]
        [Tooltip("If you change the VR SDK in scene from DontDestroyOnLoad (For example, to place it as a child of another object in the scene) and you change the scene, the SDK will be deleted." +
        "This option let you reinstantiate the SDK when you change the scene. WARNING : This option won't look if the current scene is the one specified in the SceneToUse variable.")]
        public bool CheckSDKBetweenScene = false;

        [Header("OPTIONAL")]
        [Tooltip("The Scene name in which to load the VR SDK. If Empty, the current scene will be use")]
        public string SceneToUse;
        #endregion


        #region PRIVATE_VARIABLES
        // Check if we already instantiated the SDK in the past, useful if the SDK is re-instantiated after a new scene has been loaded
        private bool _sdkHasBeenInstantiated;
        private bool _loaded;
        private bool _setupEnded;

        private GameObject _sdk;

        private ControllersParametersVariable _controllersParameter;     // The Controllers and Gaze Parameters
        #endregion


        #region MONOBEHAVIOUR_METHODS
        private void Awake()
        {
            // Check if there's any other instance of the SetupVR in the scene.
            if (!CheckSetupVRInstance())
            {
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            _controllersParameter = ControllersParametersVariable.Instance;
            
            SceneManager.activeSceneChanged += delegate { CheckVRSdkIsInScene(); };

            SetupVRInScene();
        }

        private void Update()
        {
            if (!_setupEnded)
            {
                SetupVRInScene();
            }
        }

        private void OnApplicationQuit()
        {
            SceneManager.activeSceneChanged -= delegate { CheckVRSdkIsInScene(); };
        }
        #endregion


        // EMPTY
        #region PUBLIC_METHODS
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Method called on Awake and in Update, if the setup is not finished, 
        /// to load the VR SDK Prefab and set some of its parameters.
        /// </summary>
        private void SetupVRInScene()
        {
            // We Check if we are currently in the scene in which we wanna load the VR SDK
            if (!_sdkHasBeenInstantiated && !string.IsNullOrEmpty(SceneToUse))
            {
                var sceneName = SceneManager.GetActiveScene().name;
                if (!sceneName.Contains(SceneToUse))
                    return;
            }

            // If the SDK is not loaded, we load it
            if (!_loaded)
            {
                LoadCorrespondingSDK();
            }

            // We check if the ActiveSDK is correctly set (set normally in LoadCorrespondingSDK())
            if (VRSF_Components.CameraRig == null)
            {
                _loaded = false;
                return;
            }

            // Check references for the controllers
            if (!CheckControllersReferences())
                return;

            // If the user is not using the controllers and we cannot disable them
            if (!_controllersParameter.UseControllers && !DisableControllers())
                return;

            // We set the references to the VRCamera
            if (!CheckCameraReference())
                return;

            // We copy the transform of the Scripts Container and add them as children of the corresponding SDKs objects
            VRSF_Components.SetupTransformFromContainer(CameraRigScripts, ref VRSF_Components.CameraRig);
            VRSF_Components.SetupTransformFromContainer(VRCameraScripts, ref VRSF_Components.VRCamera);

            if (_controllersParameter.UseControllers)
            {
                VRSF_Components.SetupTransformFromContainer(LeftControllerScripts, ref VRSF_Components.LeftController);
                VRSF_Components.SetupTransformFromContainer(RightControllerScripts, ref VRSF_Components.RightController);
            }

            _sdkHasBeenInstantiated = true;
            _setupEnded = true;
        }

        /// <summary>
        /// Check if there is already a setupVR instance in the scene
        /// </summary>
        /// <returns>true if there's multiple instances</returns>
        bool CheckSetupVRInstance()
        {
            SetupVR[] setupVRInstance = FindObjectsOfType<SetupVR>();
            return (setupVRInstance.Length > 1);
        }

        /// <summary>
        /// Will Instantiate and reference the SDK prefab to load thanks to the string field.
        /// </summary>
        void LoadCorrespondingSDK()
        {
            if (CheckDeviceAtRuntime)
                DeviceToLoad = CheckDeviceConnected();

            switch (DeviceToLoad)
            {
                case (EDevice.OVR):
                    XRSettings.enabled = true;
                    _sdk = Instantiate(OVR_SDK);
                    _sdk.transform.name = OVR_SDK.name;
                    VRSF_Components.DeviceLoaded = EDevice.OVR;
                    break;

                case (EDevice.OPENVR):
                    XRSettings.enabled = true;
                    _sdk = Instantiate(OpenVR_SDK);
                    _sdk.transform.name = OpenVR_SDK.name;
                    VRSF_Components.DeviceLoaded = EDevice.OPENVR;
                    break;

                case (EDevice.SIMULATOR):
                    XRSettings.enabled = false;
                    _sdk = Instantiate(Simulator_SDK);
                    _sdk.transform.name = Simulator_SDK.name;
                    VRSF_Components.DeviceLoaded = EDevice.SIMULATOR;
                    break;

                default:
                    Debug.LogError("VRSF : Device is null, loading Simulator.");
                    XRSettings.enabled = false;
                    _sdk = Instantiate(Simulator_SDK);
                    _sdk.transform.name = Simulator_SDK.name;
                    VRSF_Components.DeviceLoaded = EDevice.SIMULATOR;
                    break;
            }

            //Active SDK is set to the cameraRig, as it's the only object that will be moved
            VRSF_Components.CameraRig = _sdk.transform.gameObject;

            DontDestroyOnLoad(_sdk);
            _loaded = true;
        }


        /// <summary>
        /// Check which device is connected, and set the DeviceToLoad to the right name.
        /// </summary>
        EDevice CheckDeviceConnected()
        {
            if (XRDevice.isPresent)
            {
                string detectedHmd = XRDevice.model;
                Debug.Log("VRSF : " + detectedHmd + " is connected");

                if (detectedHmd.ToLower().Contains("vive"))
                {
                    return EDevice.OPENVR;
                }
                else if (detectedHmd.ToLower().Contains("oculus"))
                {
                    return EDevice.OVR;
                }
                else
                {
                    Debug.LogError("VRSF : " + detectedHmd + " is not supported yet, loading Simulator.");
                    return EDevice.SIMULATOR;
                }
            }
            else
            {
                Debug.Log("VRSF : No XRDevice present, loading Simulator");
                return EDevice.SIMULATOR;
            }
        }


        /// <summary>
        /// To setup the controllers reference
        /// </summary>
        bool CheckControllersReferences()
        {
            if (_loaded && (VRSF_Components.RightController == null || VRSF_Components.LeftController == null))
            {
                try
                {
                    VRSF_Components.LeftController = GameObject.FindGameObjectWithTag("LeftController");
                    VRSF_Components.RightController = GameObject.FindGameObjectWithTag("RightController");

                    return (VRSF_Components.LeftController != null && VRSF_Components.RightController != null);
                }
                catch (Exception e)
                {
                    Debug.LogError("VRSF : Can't setup Left and Right Controllers. Waiting for next frame.\n" + e);
                    return false;
                }
            }
            else
            {
                return true;
            }
        }


        private bool CheckCameraReference()
        {
            try
            {
                VRSF_Components.VRCamera = GameObject.FindGameObjectWithTag("MainCamera");
                return VRSF_Components.VRCamera;
            }
            catch (Exception e)
            {
                Debug.LogError("VRSF : Can't setup the VRCamera. Waiting for next frame.\n" + e);
                return false;
            }
        }


        /// <summary>
        /// Disable the two controllers if we don't use them
        /// </summary>
        /// <returns>true if the controllers were disabled correctly</returns>
        bool DisableControllers()
        {
            try
            {
                switch (VRSF_Components.DeviceLoaded)
                {
                    case (EDevice.OPENVR):
                        _sdk.GetComponent<ViveInputCapture>().enabled = false;
                        _sdk.GetComponent<SteamVR_ControllerManager>().enabled = false;
                        break;
                    case (EDevice.OVR):
                        _sdk.GetComponent<OculusInputCapture>().enabled = false;
                        break;
                    case (EDevice.SIMULATOR):
                        _sdk.GetComponent<SimulatorInputCapture>().enabled = false;
                        break;
                    default:
                        Debug.LogError("VRSF : Device Loaded is not set to a valid value : " + VRSF_Components.DeviceLoaded);
                        return false;
                }

                GameObject.FindGameObjectWithTag("LeftController").SetActive(false);
                GameObject.FindGameObjectWithTag("RightController").SetActive(false);

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("VRSF : Can't disable Left and Right Controllers.\n" + e);
                return false;
            }
        }


        /// <summary>
        /// Check if the VR Sdk is still instantiated when a new scene is loaded.
        /// </summary>
        void CheckVRSdkIsInScene()
        {
            if (CheckSDKBetweenScene && VRSF_Components.CameraRig == null)
            {
                _loaded = false;
                SetupVRInScene();
            }
        }
        #endregion


        // EMPTY
        #region GETTERS_SETTERS
        #endregion
    }
}