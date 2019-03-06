using Unity.Entities;
using UnityEngine;
using VRSF.Controllers;
using VRSF.Core.SetupVR;
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
            public ScriptableRaycastComponent RaycastComp;
        }
        
        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            OnSetupVRReady.RegisterListener(SetupVariables);
        }
        
        protected override void OnUpdate() {}

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
            OnSetupVRReady.UnregisterListener(SetupVariables);
        }
        #endregion


        #region PRIVATES_METHODS
        /// <summary>
        /// Check which RaycastHitVariable is used depending on the RayOrigin specified
        /// </summary>
        private void SetupVariables(OnSetupVRReady setupVRReady)
        {
            var controllersParameters = ControllersParametersVariable.Instance;
            var interactionsContainer = InteractionVariableContainer.Instance;

            foreach (var e in GetEntities<Filter>())
            {
                // We check the raycast if we use the controllers or we use the gaze
                e.RaycastComp.CheckRaycast = controllersParameters.UseControllers || Gaze.GazeParametersVariable.Instance.UseGaze;

                switch (e.RaycastComp.RayOrigin)
                {
                    case (EHand.LEFT):

                        e.RaycastComp.RayVar = interactionsContainer.LeftRay;
                        e.RaycastComp.RaycastHitVar = interactionsContainer.LeftHit;
                        e.RaycastComp.IgnoredLayers = controllersParameters.GetExclusionsLayer(EHand.LEFT);
                        e.RaycastComp.RayOriginTransform = VRSF_Components.LeftController.transform;
                        e.RaycastComp.RaycastMaxDistance = controllersParameters.MaxDistancePointerLeft;
                        break;

                    case (EHand.RIGHT):
                        e.RaycastComp.RayVar = interactionsContainer.RightRay;
                        e.RaycastComp.RaycastHitVar = interactionsContainer.RightHit;
                        e.RaycastComp.IgnoredLayers = controllersParameters.GetExclusionsLayer(EHand.RIGHT);
                        e.RaycastComp.RayOriginTransform = VRSF_Components.RightController.transform;
                        e.RaycastComp.RaycastMaxDistance = controllersParameters.MaxDistancePointerRight;
                        break;

                    case (EHand.GAZE):
                        e.RaycastComp.RayVar = interactionsContainer.GazeRay;
                        e.RaycastComp.RaycastHitVar = interactionsContainer.GazeHit;
                        e.RaycastComp.IgnoredLayers = ~Gaze.GazeParametersVariable.Instance.GazeExclusionLayer;
                        e.RaycastComp.RayOriginTransform = VRSF_Components.VRCamera.transform;
                        e.RaycastComp.RaycastMaxDistance = Gaze.GazeParametersVariable.Instance.DefaultDistance;
                        break;
                }

                e.RaycastComp.IsSetup = true;
            }
        }
        #endregion PRIVATES_METHODS
    }
}