using System;
using System.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SpatialTracking;
using UnityEngine.XR;
using VRSF.Core.Controllers;
using VRSF.Core.Inputs;

namespace VRSF.Core.SetupVR
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
        protected override void OnCreateManager()
        {
            base.OnCreateManager();
            SceneManager.sceneLoaded += OnSceneLoaded;
            OnSetupVRNeedToBeReloaded.Listeners += ReloadSetupVR;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            _controllersParameters = ControllersParametersVariable.Instance;

            foreach (var e in GetEntities<Filter>())
            {
                LoadCorrespondingSDK(e.SetupVR);
                SetupVRInScene(e.SetupVR);
            }
            
            this.Enabled = !VRSF_Components.SetupVRIsReady;
        }

        protected override void OnUpdate()
        {
            foreach (var e in GetEntities<Filter>())
            {
                if (!e.SetupVR.IsLoaded && XRDevice.model != String.Empty)
                    LoadCorrespondingSDK(e.SetupVR);
                else if (e.SetupVR.IsLoaded && !VRSF_Components.SetupVRIsReady)
                    SetupVRInScene(e.SetupVR);

                this.Enabled = !VRSF_Components.SetupVRIsReady;
            }
        }
        
        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
            SceneManager.sceneLoaded -= OnSceneLoaded;
            OnSetupVRNeedToBeReloaded.Listeners -= ReloadSetupVR;
        }
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Will Instantiate and reference the SDK prefab to load thanks to the string field.
        /// </summary>
        void LoadCorrespondingSDK(SetupVRComponents setupVR)
        {
            if (setupVR.CheckDeviceAtRuntime)
            {
                setupVR.DeviceToLoad = CheckDeviceConnected();
            }
            else if (setupVR.DeviceToLoad == EDevice.NULL)
            {
                Debug.LogError("<b>[VRSF] :</b> Device is null, Checking runtime device.");
                setupVR.DeviceToLoad = CheckDeviceConnected();
            }

            VRSF_Components.DeviceLoaded = setupVR.DeviceToLoad;

            switch (VRSF_Components.DeviceLoaded)
            {
                case (EDevice.OCULUS_RIFT):
                    GameObject.Destroy(setupVR.GetComponent<ViveControllersInputCaptureComponent>());
                    GameObject.Destroy(setupVR.GetComponent<WMRControllersInputCaptureComponent>());
                    RemoveSimulatorStuffs();
                    CheckControllersReferences(setupVR, setupVR.Rift_Controllers);
                    setupVR.FloorOffset.localPosition = new Vector3(0, 1.7f, 0);
                    break;
                case (EDevice.HTC_VIVE):
                    GameObject.Destroy(setupVR.GetComponent<RiftControllersInputCaptureComponent>());
                    GameObject.Destroy(setupVR.GetComponent<WMRControllersInputCaptureComponent>());
                    RemoveSimulatorStuffs();
                    CheckControllersReferences(setupVR, setupVR.Vive_Controllers);
                    setupVR.FloorOffset.localPosition = Vector3.zero;
                    break;
                case (EDevice.WMR):
                    GameObject.Destroy(setupVR.GetComponent<RiftControllersInputCaptureComponent>());
                    GameObject.Destroy(setupVR.GetComponent<ViveControllersInputCaptureComponent>());
                    RemoveSimulatorStuffs();
                    CheckControllersReferences(setupVR, setupVR.WMR_Controllers);
                    setupVR.FloorOffset.localPosition = Vector3.zero;
                    break;
                default:
                    GameObject.Destroy(setupVR.GetComponent<ViveControllersInputCaptureComponent>());
                    GameObject.Destroy(setupVR.GetComponent<RiftControllersInputCaptureComponent>());
                    GameObject.Destroy(setupVR.GetComponent<WMRControllersInputCaptureComponent>());
                    CheckControllersReferences(setupVR, setupVR.Simulator_Controllers);
                    setupVR.FloorOffset.localPosition = new Vector3(0, 1.7f, 0);
                    setupVR.StartCoroutine(ResetVRCamera());
                    break;
            }
            
            setupVR.CameraRig.transform.name = "[VRSF] " + VRSF_Components.DeviceLoaded.ToString();
            setupVR.CameraRig.transform.SetParent(null);
            setupVR.IsLoaded = true;


            /// <summary>
            /// Check which device is connected, and set the DeviceToLoad to the right name.
            /// </summary>
            EDevice CheckDeviceConnected()
            {
                if (XRDevice.isPresent)
                {
                    string detectedHmd = XRDevice.model;

                    Debug.Log("<b>[VRSF] :</b> " + detectedHmd + " is connected");

                    if (detectedHmd.ToLower().Contains("vive"))
                    {
                        return EDevice.HTC_VIVE;
                    }
                    else if (detectedHmd.ToLower().Contains("rift"))
                    {
                        return EDevice.OCULUS_RIFT;
                    }
                    else if (detectedHmd.ToLower().Contains("windows"))
                    {
                        return EDevice.WMR;
                    }
                    else
                    {
                        Debug.LogError("<b>[VRSF] :</b> " + detectedHmd + " is not supported yet, loading Simulator.");
                        return EDevice.SIMULATOR;
                    }
                }
                else
                {
                    Debug.Log("<b>[VRSF] :</b> No XRDevice present, loading Simulator");
                    return EDevice.SIMULATOR;
                }
            }

            void RemoveSimulatorStuffs()
            {
                GameObject.Destroy(setupVR.CameraRig.GetComponent<SimulatorMovementComponent>());
                GameObject.Destroy(setupVR.GetComponent<SimulatorInputCaptureComponent>());
            }

            IEnumerator ResetVRCamera()
            {
                XRSettings.enabled = false;
                GameObject.Destroy(setupVR.VRCamera.GetComponent<TrackedPoseDriver>());
                GameObject.Destroy(setupVR.LeftController.GetComponent<TrackedPoseDriver>());
                GameObject.Destroy(setupVR.RightController.GetComponent<TrackedPoseDriver>());
                yield return new WaitForEndOfFrame();
                setupVR.VRCamera.transform.localPosition = Vector3.zero;
                setupVR.VRCamera.transform.localRotation = Quaternion.identity;
            }
        }


        /// <summary>
        /// Method called on Awake and in Update, if the setup is not finished, 
        /// to load the VR SDK Prefab and set its parameters.
        /// </summary>
        private void SetupVRInScene(SetupVRComponents setupVR)
        {
            if (setupVR.FloorOffset == null)
            {
                Debug.LogError("<b>[VRSF] :</b> No Floor Offset was references in SetupVR. Trying to fetch it using the name Floor_Offset.");
                setupVR.FloorOffset = GameObject.Find("Floor_Offset").transform;
                return;
            }

            VRSF_Components.FloorOffset = setupVR.FloorOffset;

            if (setupVR.CameraRig == null)
            {
                Debug.LogError("<b>[VRSF] :</b> No CameraRig was references in SetupVR. Trying to fetch it using tag RESERVED_CameraRig.");
                setupVR.CameraRig = GameObject.FindGameObjectWithTag("RESERVED_CameraRig");
                return;
            }

            VRSF_Components.CameraRig = setupVR.CameraRig;

            // We set the references to the VRCamera
            if (setupVR.VRCamera == null)
            {
                Debug.LogError("<b>[VRSF] :</b> No VRCamera was references in SetupVR. Trying to fetch it using tag MainCamera.");
                setupVR.VRCamera = GameObject.FindGameObjectWithTag("MainCamera");
                return;
            }

            VRSF_Components.VRCamera = setupVR.VRCamera;

            // If the user is not using the controllers and we cannot disable them
            if (!_controllersParameters.UseControllers && !DisableControllers(setupVR))
            {
                setupVR.LeftController.SetActive(false);
                setupVR.RightController.SetActive(false);
            }

            VRSF_Components.LeftController = setupVR.LeftController;
            VRSF_Components.RightController = setupVR.RightController;

            VRSF_Components.SetupVRIsReady = true;
            new OnSetupVRReady();
        }


        /// <summary>
        /// To setup the controllers reference and instantiate the corresponding models
        /// </summary>
        void CheckControllersReferences(SetupVRComponents setupVR, VRController[] Controllers)
        {
            if (setupVR.LeftController == null)
            {
                Debug.LogError("<b>[VRSF] :</b> No Left Controller was references in SetupVR. Trying to fetch it using tag RESERVED_LeftController.");
                setupVR.LeftController = GameObject.FindGameObjectWithTag("RESERVED_LeftController");
            }

            if (setupVR.RightController == null)
            {
                Debug.LogError("<b>[VRSF] :</b> No Right Controller was references in SetupVR. Trying to fetch it using tag RESERVED_RightController.");
                setupVR.RightController = GameObject.FindGameObjectWithTag("RESERVED_RightController");
            }

            if (setupVR.LeftController != null)
                GameObject.Instantiate(Controllers[0].Hand == EHand.LEFT ? Controllers[0].ControllerPrefab : Controllers[1].ControllerPrefab, setupVR.LeftController.transform);

            if (setupVR.RightController != null)
                GameObject.Instantiate(Controllers[0].Hand == EHand.RIGHT ? Controllers[0].ControllerPrefab : Controllers[1].ControllerPrefab, setupVR.RightController.transform);
        }


        /// <summary>
        /// Disable the two controllers if we don't use them
        /// </summary>
        /// <returns>true if the controllers were disabled correctly</returns>
        bool DisableControllers(SetupVRComponents setupVR)
        {
            try
            {
                switch (VRSF_Components.DeviceLoaded)
                {
                    case (EDevice.OCULUS_RIFT):
                        setupVR.CameraRig.GetComponent<RiftControllersInputCaptureComponent>().enabled = false;
                        break;
                    case (EDevice.HTC_VIVE):
                        setupVR.CameraRig.GetComponent<ViveControllersInputCaptureComponent>().enabled = false;
                        break;
                    case (EDevice.WMR):
                        setupVR.CameraRig.GetComponent<WMRControllersInputCaptureComponent>().enabled = false;
                        break;
                    case (EDevice.SIMULATOR):
                        setupVR.CameraRig.GetComponent<SimulatorInputCaptureComponent>().enabled = false;
                        break;
                    default:
                        Debug.LogError("<b>[VRSF] :</b> Device Loaded is not set to a valid value : " + VRSF_Components.DeviceLoaded);
                        return false;
                }

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("<b>[VRSF] :</b> Can't disable Left and Right Controllers.\n" + e);
                return false;
            }
        }


        /// <summary>
        /// Reactivate the System when switching to another Scene.
        /// </summary>
        /// <param name="oldScene">The previous scene before switching</param>
        private void OnSceneLoaded(Scene oldScene, LoadSceneMode loadMode)
        {
            if (loadMode == LoadSceneMode.Single)
                new OnSetupVRNeedToBeReloaded();
        }


        private void ReloadSetupVR(OnSetupVRNeedToBeReloaded info)
        {
            VRSF_Components.SetupVRIsReady = false;
            this.Enabled = true;
        }
        #endregion
    }
}