using Unity.Entities;
using UnityEngine.SceneManagement;
using VRSF.Core.Controllers;
using VRSF.Core.Gaze;
using VRSF.Core.Inputs;
using VRSF.Interactions;
using VRSF.Core.SetupVR;

namespace VRSF.Utils.Systems
{
    public class ScriptableSingletonsInitSystem : ComponentSystem
    {
        struct Filter
        {
            public ScriptableSingletonsComponent ScriptableSingletons;
        }
        
        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            SceneManager.sceneLoaded += OnSceneLoaded;

            foreach (var entity in GetEntities<Filter>())
            {
                entity.ScriptableSingletons.GazeParameters = GazeParametersVariable.Instance;
                entity.ScriptableSingletons.ControllersParameters = ControllersParametersVariable.Instance;
                entity.ScriptableSingletons.InteractionsContainer = InteractionVariableContainer.Instance;
                entity.ScriptableSingletons.InputsContainer = InputVariableContainer.Instance;

                entity.ScriptableSingletons._IsSetup = true;
            }

            this.Enabled = false;
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }


        /// <summary>
        /// Reactivate the System when switching to another Scene.
        /// </summary>
        /// <param name="oldScene">The previous scene before switching</param>
        private void OnSceneLoaded(Scene oldScene, LoadSceneMode loadMode)
        {
            this.Enabled = loadMode == LoadSceneMode.Single;
        }
    }
}