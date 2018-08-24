using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRSF.Gaze.Components;
using VRSF.Inputs.Components;
using VRSF.Utils;

namespace VRSF.Gaze.Systems
{
    public class GazeSetupSystem : ComponentSystem
    {
        struct Filter
        {
            public GazeParametersComponent GazeParameters;
            public GazeCalculationsComponent GazeCalculations;
        }


        #region PRIVATE_VARIABLES
        private GazeParametersVariable _gazeParameters;
        #endregion PRIVATE_VARIABLES


        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            _gazeParameters = GazeParametersVariable.Instance;
            SceneManager.sceneUnloaded += OnSceneUnloaded;

            if (_gazeParameters.UseGaze)
            {
                foreach (var e in GetEntities<Filter>())
                {
                    if (VRSF_Components.CameraRig != null)
                    {
                        CheckGazeParameters();
                        GeneralGazeSetup(e);
                    }
                }
            }
            else
            {
                this.Enabled = false;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        protected override void OnUpdate()
        {
            if (VRSF_Components.CameraRig != null)
            {
                foreach (var e in GetEntities<Filter>())
                {
                    if (!e.GazeCalculations._IsSetup)
                    {
                        CheckGazeParameters();
                        GeneralGazeSetup(e);
                    }
                    else
                    {
                        // AS there is only one gaze, we disable this system when it's setup.
                        this.Enabled = false;
                    }
                }
            }
        }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();

            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }
        #endregion ComponentSystem_Methods


        #region PRIVATE_METHODS
        private bool CheckGazeParameters()
        {
            // If we don't use the gaze, no need to check the rest of the Objects
            if (!_gazeParameters.UseGaze)
                return true;

            if (GameObject.FindObjectsOfType<GazeParametersComponent>().Length == 0)
            {
                Debug.LogError("VRSF : If you want to use the Gaze feature, please add a GazeParametersComponent on a GameObject, or place the Gaze Prefab in the scene." +
                    "(Assets/VRSF/Prefabs/UI/ReticleCanvas.prefab).\n Setting UseGaze in GazeParameters to false.");
                _gazeParameters.UseGaze = false;
                return false;
            }

            CheckGazeInputComponent();

            return true;
        }


        private void CheckGazeInputComponent()
        {
            switch (VRSF_Components.DeviceLoaded)
            {
                case EDevice.OPENVR:
                    if (VRSF_Components.CameraRig.GetComponent<ViveGazeInputCaptureComponent>() == null)
                    {
                        VRSF_Components.CameraRig.AddComponent<ViveGazeInputCaptureComponent>();
                    }
                    break;

                case EDevice.OCULUS_RIFT:
                    if (VRSF_Components.CameraRig.GetComponent<RiftGazeInputCaptureComponent>() == null)
                    {
                        VRSF_Components.CameraRig.AddComponent<RiftGazeInputCaptureComponent>();
                    }
                    break;
                case EDevice.SIMULATOR:
                    if (VRSF_Components.CameraRig.GetComponent<SimulatorGazeInputCaptureComponent>() == null)
                    {
                        VRSF_Components.CameraRig.AddComponent<SimulatorGazeInputCaptureComponent>();
                    }
                    break;
            }
        }


        /// <summary>
        /// Setup the Gaze Parameters based on the ScriptableSingleton, set in the VRSF Interaction Parameters Window
        /// </summary>
        private void GeneralGazeSetup(Filter entity)
        {
            try
            {
                if (_gazeParameters.ReticleSprite != null)
                {
                    entity.GazeParameters.ReticleBackground.sprite = _gazeParameters.ReticleSprite;
                }

                if (_gazeParameters.ReticleTargetSprite != null)
                {
                    entity.GazeParameters.ReticleTarget.sprite = _gazeParameters.ReticleTargetSprite;
                }

                entity.GazeParameters.transform.localScale = _gazeParameters.ReticleSize;

                // Store the original scale and rotation.
                entity.GazeCalculations._OriginalScale = entity.GazeParameters.ReticleTransform.localScale;
                entity.GazeCalculations._OriginalRotation = entity.GazeParameters.ReticleTransform.localRotation;
                entity.GazeCalculations._VRCamera = VRSF_Components.VRCamera.transform;
                entity.GazeCalculations._IsSetup = true;
            }
            catch (System.Exception e)
            {
                Debug.Log("VRSF : The VR Components are not set in the scene yet, waiting for next frame.\n" + e);
            }
        }


        /// <summary>
        /// Reactivate the System when switching to another Scene.
        /// </summary>
        /// <param name="oldScene">The previous scene before switching</param>
        private void OnSceneUnloaded(Scene oldScene)
        {
            this.Enabled = true;
        }
        #endregion PRIVATE_METHODS
    }
}