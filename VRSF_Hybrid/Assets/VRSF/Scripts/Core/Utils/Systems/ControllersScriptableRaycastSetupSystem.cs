using Unity.Entities;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.SetupVR;
using VRSF.Interactions;

namespace VRSF.Core.Raycast
{
    /// <summary>
    /// Setup the references for the left or right Ray and RaycastHit Scriptable Variables depending on the RayOrigin Chosed in the ButtonActionChoserComponent
    /// </summary>
    public class ControllersScriptableRaycastSetupSystem : ComponentSystem
    {
        struct Filter
        {
            public ControllersScriptableRaycastComponent RaycastComp;
        }

        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnCreateManager()
        {
            OnSetupVRReady.Listeners += SetupVariables;
            base.OnCreateManager();
        }

        protected override void OnUpdate() { }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
            OnSetupVRReady.Listeners -= SetupVariables;
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
                // We check the raycast if we use the controllers
                e.RaycastComp.CheckRaycast = controllersParameters.UseControllers;

                switch (e.RaycastComp.RayOrigin)
                {
                    case ERayOrigin.LEFT_HAND:

                        e.RaycastComp.RayVar = interactionsContainer.LeftRay;
                        e.RaycastComp.RaycastHitVar = interactionsContainer.LeftHit;
                        e.RaycastComp.RayOriginTransform = VRSF_Components.LeftController.transform;
                        e.RaycastComp.RaycastMaxDistance = controllersParameters.MaxDistancePointerLeft;
                        break;

                    case ERayOrigin.RIGHT_HAND:
                        e.RaycastComp.RayVar = interactionsContainer.RightRay;
                        e.RaycastComp.RaycastHitVar = interactionsContainer.RightHit;
                        e.RaycastComp.RayOriginTransform = VRSF_Components.RightController.transform;
                        e.RaycastComp.RaycastMaxDistance = controllersParameters.MaxDistancePointerRight;
                        break;

                    //case (EHand.GAZE):
                    //    e.RaycastComp.RayVar = interactionsContainer.GazeRay;
                    //    e.RaycastComp.RaycastHitVar = interactionsContainer.GazeHit;
                    //    e.RaycastComp.RayOriginTransform = VRSF_Components.VRCamera.transform;
                    //    e.RaycastComp.RaycastMaxDistance = Gaze.GazeParametersVariable.Instance.DefaultDistance;
                    //    break;
                }

                e.RaycastComp.IsSetup = true;
            }
        }
        #endregion PRIVATES_METHODS
    }
}