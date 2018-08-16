﻿using Unity.Entities;
using UnityEngine.SceneManagement;
using VRSF.Controllers;
using VRSF.Gaze;
using VRSF.Inputs;
using VRSF.Interactions;
using VRSF.Utils.Components;

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

            SceneManager.sceneUnloaded += OnSceneUnloaded;

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
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }


        /// <summary>
        /// Reactivate the System when switching to another Scene.
        /// </summary>
        /// <param name="oldScene">The previous scene before switching</param>
        private void OnSceneUnloaded(Scene oldScene)
        {
            this.Enabled = true;
        }
    }
}