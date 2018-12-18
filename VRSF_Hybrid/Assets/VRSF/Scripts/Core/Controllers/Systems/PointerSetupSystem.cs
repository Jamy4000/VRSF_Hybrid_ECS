using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRSF.Controllers.Components;
using VRSF.Utils;

namespace VRSF.Controllers.Systems
{
    public class PointerSetupSystem : ComponentSystem
    {
        struct Filter
        {
            public ControllerPointerComponents ControllerPointerComp;
        }

        #region PRIVATE_VARIABLES
        // VRSF Parameters references
        private ControllersParametersVariable _controllersParameters;
        #endregion PRIVATE_VARIABLES


        #region ComponentSystem_Methods
        // Use this for initialization
        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            _controllersParameters = ControllersParametersVariable.Instance;

            SceneManager.sceneUnloaded += OnSceneUnloaded;

            foreach (var e in GetEntities<Filter>())
            {
                SetupVRComponents(e.ControllerPointerComp);
            }
        }

        // Update is called once per frame
        protected override void OnUpdate()
        {
            bool controllersSettingUp = false;
            foreach (var e in GetEntities<Filter>())
            {
                // As the vive send errors if the controller are not seen on the first frame, we need to put that in the update method
                if (!e.ControllerPointerComp._IsSetup)
                {
                    controllersSettingUp = true;
                    SetupVRComponents(e.ControllerPointerComp);
                }
            }
            this.Enabled = controllersSettingUp;
        }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }
        #endregion ComponentSystem_Methods


        #region PRIVATE_METHODS
        /// <summary>
        /// Setup the VR Components thanks to the VRSF_Components static class
        /// </summary>
        private void SetupVRComponents(ControllerPointerComponents comp)
        {
            try
            {
                if (_controllersParameters.UseControllers)
                    SetupPointers(comp);

                comp._IsSetup = true;
            }
            catch (System.Exception e)
            {
                Debug.Log("VRSF : VR Components not setup yet, waiting for next frame.\n" + e);
            }
        }

        /// <summary>
        /// Setup the pointer with the values of the VRSF Interaction Parameters
        /// </summary>
        private void SetupPointers(ControllerPointerComponents comp)
        {
            // We setup the right pointer
            comp._RightHandPointer = VRSF_Components.RightController.GetComponentInChildren<LineRenderer>();
            comp._RightParticles = comp._RightHandPointer.GetComponentsInChildren<ParticleSystem>();

            if (!_controllersParameters.UsePointerRight || VRSF_Components.DeviceLoaded == EDevice.SIMULATOR)
                RemovePointer(comp._RightHandPointer, comp._RightParticles);

            // We setup the left pointer
            comp._LeftHandPointer = VRSF_Components.LeftController.GetComponentInChildren<LineRenderer>();
            comp._LeftParticles = comp._LeftHandPointer.GetComponentsInChildren<ParticleSystem>();

            if (!_controllersParameters.UsePointerLeft || VRSF_Components.DeviceLoaded == EDevice.SIMULATOR)
                RemovePointer(comp._LeftHandPointer, comp._LeftParticles);


            /// <summary>
            /// Disable the LineRenderer and the particle System used on the Pointer
            /// </summary>
            /// <param name="lineRenderer">The line Renderer to disable</param>
            /// <param name="particleSystems">The particle systems to stop</param>
            void RemovePointer(LineRenderer lineRenderer, ParticleSystem[] particleSystems)
            {
                lineRenderer.enabled = false;
                if (particleSystems != null)
                {
                    foreach (var particleSystem in particleSystems)
                    {
                        particleSystem.Stop();
                        particleSystem.Clear();
                    }
                }
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