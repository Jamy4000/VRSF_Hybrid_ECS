using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRSF.Core.Gaze;
using VRSF.Core.SetupVR;

namespace VRSF.Gaze
{
    public class GazeSetupSystem : ComponentSystem
    {
        struct Filter
        {
            public GazeParametersComponent GazeParameters;
            public GazeCalculationsComponent GazeCalculations;
        }


        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        protected override void OnCreateManager()
        {
            OnSetupVRReady.Listeners += GazeCalculationsSetup;
            SceneManager.sceneLoaded += OnSceneLoaded;
            base.OnCreateManager();
        }
        
        protected override void OnUpdate() {}

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
            SceneManager.sceneLoaded -= OnSceneLoaded;
            OnSetupVRReady.Listeners -= GazeCalculationsSetup;
        }
        #endregion ComponentSystem_Methods


        #region PRIVATE_METHODS
        /// <summary>
        /// Setup the Gaze Parameters based on the ScriptableSingleton, set in the VRSF Interaction Parameters Window
        /// </summary>
        private void GazeCalculationsSetup(OnSetupVRReady setupVRReady)
        {
            if (GazeParametersVariable.Instance.UseGaze)
            {
                foreach (var e in GetEntities<Filter>())
                {
                    // Store the original scale and rotation.
                    e.GazeCalculations._OriginalScale = e.GazeParameters.ReticleTransform.localScale;
                    e.GazeCalculations._OriginalRotation = e.GazeParameters.ReticleTransform.localRotation;
                    e.GazeCalculations._VRCamera = VRSF_Components.VRCamera.transform;
                    e.GazeCalculations._IsSetup = true;
                }
            }
            else
            {
                this.Enabled = false;
            }
        }


        /// <summary>
        /// Reactivate the System when switching to another Scene.
        /// </summary>
        /// <param name="oldScene">The previous scene before switching</param>
        private void OnSceneLoaded(Scene oldScene, LoadSceneMode loadMode)
        {
            this.Enabled = loadMode == LoadSceneMode.Single;
        }
        #endregion PRIVATE_METHODS
    }
}