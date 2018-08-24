using System;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using VRSF.Controllers;
using VRSF.Inputs.Components;
using VRSF.Inputs.Components.Vive;
using VRSF.Utils.Components;

namespace VRSF.Utils.Systems
{
    public class SetupVRSystems : ComponentSystem
    {
        /// <summary>
        /// The filter to find SetupVR entity
        /// </summary>
        struct Filter
        {
            public SetupVRComponents SetupVR;
        }

        private ControllersParametersVariable _controllersParameters;

        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            
            _controllersParameters = ControllersParametersVariable.Instance;

            SetupVRInScene(GetEntities<Filter>()[0].SetupVR);
            
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        protected override void OnUpdate()
        {
            var e = GetEntities<Filter>()[0];

            if (!VRSF_Components.SetupVRIsReady)
            {
                SetupVRInScene(e.SetupVR);
            }
            else
            {
                this.Enabled = false;
            }
        }


        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Method called on Awake and in Update, if the setup is not finished, 
        /// to load the VR SDK Prefab and set its parameters.
        /// </summary>
        private void SetupVRInScene(SetupVRComponents setupVR)
        {
            // If the SDK is not loaded, we load it
            if (!setupVR.Loaded)
            {
                LoadCorrespondingSDK(setupVR);
            }

            // We check if the ActiveSDK is correctly set (set normally in LoadCorrespondingSDK())
            if (VRSF_Components.CameraRig == null)
            {
                setupVR.Loaded = false;
                return;
            }

            // Check references for the controllers
            if (!CheckControllersReferences(setupVR))
                return;

            // If the user is not using the controllers and we cannot disable them
            if (!_controllersParameters.UseControllers && !DisableControllers())
                return;

            // We set the references to the VRCamera
            if (!CheckCameraReference())
                return;

            // We copy the transform of the Scripts Container and add them as children of the corresponding SDKs objects
            VRSF_Components.SetupTransformFromContainer(setupVR.CameraRigScripts, ref VRSF_Components.CameraRig);
            VRSF_Components.SetupTransformFromContainer(setupVR.VRCameraScripts, ref VRSF_Components.VRCamera);

            if (_controllersParameters.UseControllers)
            {
                VRSF_Components.SetupTransformFromContainer(setupVR.LeftControllerScripts, ref VRSF_Components.LeftController);
                VRSF_Components.SetupTransformFromContainer(setupVR.RightControllerScripts, ref VRSF_Components.RightController);
            }

            setupVR.SDKHasBeenInstantiated = true;
            VRSF_Components.SetupVRIsReady = true;
        }

        /// <summary>
        /// Will Instantiate and reference the SDK prefab to load thanks to the string field.
        /// </summary>
        void LoadCorrespondingSDK(SetupVRComponents setupVR)
        {
            if (setupVR.CheckDeviceAtRuntime)
                setupVR.DeviceToLoad = CheckDeviceConnected();

            switch (setupVR.DeviceToLoad)
            {
                case (EDevice.OCULUS_RIFT):
                    XRSettings.enabled = true;
                    VRSF_Components.CameraRig = GameObject.Instantiate(setupVR.Rift_SDK);
                    VRSF_Components.CameraRig.transform.name = setupVR.Rift_SDK.name;
                    VRSF_Components.DeviceLoaded = EDevice.OCULUS_RIFT;
                    break;

                case (EDevice.OCULUS_GO):
                    XRSettings.enabled = true;
                    VRSF_Components.CameraRig = GameObject.Instantiate(setupVR.OculusGo_SDK);
                    VRSF_Components.CameraRig.transform.name = setupVR.OculusGo_SDK.name;
                    VRSF_Components.DeviceLoaded = EDevice.OCULUS_GO;
                    break;

                case (EDevice.GEAR_VR):
                    XRSettings.enabled = true;
                    VRSF_Components.CameraRig = GameObject.Instantiate(setupVR.GearVR_SDK);
                    VRSF_Components.CameraRig.transform.name = setupVR.GearVR_SDK.name;
                    VRSF_Components.DeviceLoaded = EDevice.GEAR_VR;
                    break;

                case (EDevice.OPENVR):
                    XRSettings.enabled = true;
                    VRSF_Components.CameraRig = GameObject.Instantiate(setupVR.OpenVR_SDK);
                    VRSF_Components.CameraRig.transform.name = setupVR.OpenVR_SDK.name;
                    VRSF_Components.DeviceLoaded = EDevice.OPENVR;
                    break;

                case (EDevice.SIMULATOR):
                    XRSettings.enabled = false;
                    VRSF_Components.CameraRig = GameObject.Instantiate(setupVR.Simulator_SDK);
                    VRSF_Components.CameraRig.transform.name = setupVR.Simulator_SDK.name;
                    VRSF_Components.DeviceLoaded = EDevice.SIMULATOR;
                    break;

                default:
                    Debug.LogError("VRSF : Device is null, loading Simulator.");
                    XRSettings.enabled = false;
                    VRSF_Components.CameraRig = GameObject.Instantiate(setupVR.Simulator_SDK);
                    VRSF_Components.CameraRig.transform.name = setupVR.Simulator_SDK.name;
                    VRSF_Components.DeviceLoaded = EDevice.SIMULATOR;
                    break;
            }

            setupVR.Loaded = true;
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
                else if (detectedHmd.ToLower().Contains("rift"))
                {
                    return EDevice.OCULUS_RIFT;
                }
                else if (detectedHmd.ToLower().Contains("go"))
                {
                    return EDevice.OCULUS_GO;
                }
                else if (detectedHmd.ToLower().Contains("gear"))
                {
                    return EDevice.GEAR_VR;
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
        bool CheckControllersReferences(SetupVRComponents setupVR)
        {
            if (setupVR.Loaded && (VRSF_Components.RightController == null || VRSF_Components.LeftController == null))
            {
                try
                {
                    VRSF_Components.LeftController = GameObject.FindGameObjectWithTag("LeftController");
                    VRSF_Components.RightController = GameObject.FindGameObjectWithTag("RightController");

                    return VRSF_Components.LeftController != null && VRSF_Components.RightController != null;
                }
                catch (Exception e)
                {
                    Debug.LogError("VRSF : Can't setup Left or Right Controllers. Waiting for next frame.\n" + e);
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Find the main camera in scene and set the VRSF_Component reference
        /// </summary>
        /// <returns></returns>
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
                        VRSF_Components.CameraRig.GetComponent<ViveControllersInputCaptureComponent>().enabled = false;
                        VRSF_Components.CameraRig.GetComponent<SteamVR_ControllerManager>().enabled = false;
                        break;
                    case (EDevice.OCULUS_RIFT):
                        VRSF_Components.CameraRig.GetComponent<RiftControllersInputCaptureComponent>().enabled = false;
                        break;
                    case (EDevice.SIMULATOR):
                        VRSF_Components.CameraRig.GetComponent<SimulatorControllersInputCaptureComponent>().enabled = false;
                        break;
                    default:
                        Debug.LogError("VRSF : Device Loaded is not set to a valid value : " + VRSF_Components.DeviceLoaded);
                        return false;
                }

                VRSF_Components.LeftController.SetActive(false);
                VRSF_Components.RightController.SetActive(false);

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("VRSF : Can't disable Left and Right Controllers.\n" + e);
                return false;
            }
        }


        /// <summary>
        /// Reactivate the System when switching to another Scene.
        /// </summary>
        /// <param name="oldScene">The previous scene before switching</param>
        private void OnSceneUnloaded(Scene oldScene)
        {
            VRSF_Components.SetupVRIsReady = false;
            this.Enabled = true;
        }
        #endregion

    }
}