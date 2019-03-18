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


        #region PRIVATE_VARIABLES
        private GazeParametersVariable _gazeParameters;
        #endregion PRIVATE_VARIABLES


        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            _gazeParameters = GazeParametersVariable.Instance;
            SceneManager.sceneLoaded += OnSceneLoaded;

            if (_gazeParameters.UseGaze)
            {
                foreach (var e in GetEntities<Filter>())
                {
                    if (VRSF_Components.CameraRig != null)
                    {
                        GazeCalculationsSetup(e);
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
                        GazeCalculationsSetup(e);
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

            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        #endregion ComponentSystem_Methods


        #region PRIVATE_METHODS
        /// <summary>
        /// Setup the Gaze Parameters based on the ScriptableSingleton, set in the VRSF Interaction Parameters Window
        /// </summary>
        private void GazeCalculationsSetup(Filter entity)
        {
            try
            {
                // Store the original scale and rotation.
                entity.GazeCalculations._OriginalScale = entity.GazeParameters.ReticleTransform.localScale;
                entity.GazeCalculations._OriginalRotation = entity.GazeParameters.ReticleTransform.localRotation;
                entity.GazeCalculations._VRCamera = VRSF_Components.VRCamera.transform;
                entity.GazeCalculations._IsSetup = true;
            }
            catch (System.Exception e)
            {
                Debug.Log("<b>[VRSF] :</b> The VR Components are not set in the scene yet, waiting for next frame.\n" + e);
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