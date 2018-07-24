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
            SceneManager.activeSceneChanged += OnSceneChanged;

            foreach (var entity in GetEntities<Filter>())
            {
                // We check which hit to use for this feature with the RayOrigin
                SetupRayAndHit(entity);
            }

            this.Enabled = false;
        }


        protected override void OnUpdate() {}
        #endregion


        #region PRIVATES_METHODS
        /// <summary>
        /// Check which RaycastHitVariable is used depending on the RayOrigin specified
        /// </summary>
        private void SetupRayAndHit(Filter entity)
        {
            switch (entity.RayVarComp.RayOrigin)
            {
                case (EHand.LEFT):
                    entity.RayVarComp.RaycastHitVar = _interactionsContainer.LeftHit;
                    entity.RayVarComp.RayVar = _interactionsContainer.LeftRay;
                    break;

                case (EHand.RIGHT):
                    entity.RayVarComp.RaycastHitVar = _interactionsContainer.RightHit;
                    entity.RayVarComp.RayVar = _interactionsContainer.RightRay;
                    break;

                case (EHand.GAZE):
                    entity.RayVarComp.RaycastHitVar = _interactionsContainer.GazeHit;
                    entity.RayVarComp.RayVar = _interactionsContainer.GazeRay;
                    break;
            }
        }


        /// <summary>
        /// Reactivate the System when switching to another Scene.
        /// </summary>
        /// <param name="oldScene">The previous scene before switching</param>
        /// <param name="newScene">The new scene after switching</param>
        private void OnSceneChanged(Scene oldScene, Scene newScene)
        {
            this.Enabled = true;
        }
        #endregion PRIVATES_METHODS
    }
}