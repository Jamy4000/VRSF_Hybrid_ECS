using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRSF.Controllers;
using VRSF.Interactions;
using VRSF.Utils.Components;

namespace VRSF.Utils.Systems
{
    /// <summary>
    /// Setup the references for the left, right or gaze Ray and RaycastHit Scriptable Variables depending on the RayOrigin Chosed in the ButtonActionChoserComponent
    /// </summary>
    public class ScriptableRaycastSetupSystem : ComponentSystem
    {
        struct Filter
        {
            public ScriptableRaycastComponent RayVarComp;
        }

        #region PRIVATE_VARIABLES
        private InteractionVariableContainer _interactionsContainer;
        #endregion


        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            _interactionsContainer = InteractionVariableContainer.Instance;
            SceneManager.sceneUnloaded += OnSceneUnloaded;

            foreach (var entity in GetEntities<Filter>())
            {
                // We check which hit to use for this feature with the RayOrigin
                SetupVariables(entity);
            }

            this.Enabled = false;
        }


        protected override void OnUpdate() { }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }
        #endregion


        #region PRIVATES_METHODS
        /// <summary>
        /// Check which RaycastHitVariable is used depending on the RayOrigin specified
        /// </summary>
        private void SetupVariables(Filter entity)
        {
            switch (entity.RayVarComp.RayOrigin)
            {
                case (EHand.LEFT):
                    entity.RayVarComp.RaycastHitVar = _interactionsContainer.LeftHit;
                    entity.RayVarComp.RayVar = _interactionsContainer.LeftRay;
                    entity.RayVarComp.IgnoredLayers = ControllersParametersVariable.Instance.LeftExclusionLayer;
                    break;

                case (EHand.RIGHT):
                    entity.RayVarComp.RaycastHitVar = _interactionsContainer.RightHit;
                    entity.RayVarComp.RayVar = _interactionsContainer.RightRay;
                    entity.RayVarComp.IgnoredLayers = ControllersParametersVariable.Instance.RightExclusionLayer;
                    break;

                case (EHand.GAZE):
                    entity.RayVarComp.RaycastHitVar = _interactionsContainer.GazeHit;
                    entity.RayVarComp.RayVar = _interactionsContainer.GazeRay;
                    entity.RayVarComp.IgnoredLayers = VRSF.Gaze.GazeParametersVariable.Instance.GazeExclusionLayer;
                    break;
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
        #endregion PRIVATES_METHODS
    }
}