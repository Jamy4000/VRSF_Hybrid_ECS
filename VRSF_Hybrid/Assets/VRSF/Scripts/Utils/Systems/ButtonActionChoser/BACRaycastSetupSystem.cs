using Unity.Entities;
using UnityEngine;
using VRSF.Controllers;
using VRSF.Utils.Components;

namespace VRSF.Utils.Systems.ButtonActionChoser
{
    /// <summary>
    /// Setup the references for the left, right or gaze Ray and RaycastHit Scriptable Variables
    /// </summary>
    public class BACRaycastSetupSystem : ComponentSystem
    {
        struct Filter
        {
            public ButtonActionChoserComponents ButtonComponents;
        }

        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            foreach (var entity in GetEntities<Filter>())
            {
                // We check which hit to use for this feature with the RayOrigin
                SetupRayAndHit(entity.ButtonComponents);
            }
        }


        protected override void OnUpdate() {}
        #endregion


        #region PRIVATES_METHODS
        /// <summary>
        /// Check which RaycastHitVariable is used depending on the RayOrigin specified
        /// </summary>
        private void SetupRayAndHit(ButtonActionChoserComponents comp)
        {
            switch (comp.RayOrigin)
            {
                case (EHand.LEFT):
                    comp.HitVar = comp.InteractionsContainer.LeftHit;
                    comp.RayVar = comp.InteractionsContainer.LeftRay;
                    break;

                case (EHand.RIGHT):
                    comp.HitVar = comp.InteractionsContainer.RightHit;
                    comp.RayVar = comp.InteractionsContainer.RightRay;
                    break;

                case (EHand.GAZE):
                    comp.HitVar = comp.InteractionsContainer.GazeHit;
                    comp.RayVar = comp.InteractionsContainer.GazeRay;
                    break;

                default:
                    Debug.LogError("VRSF : You need to specify the RayOrigin for the " + GetType().Name + " script. Setting CanBeUsed of ButtonActionChoserComponents to false.");
                    comp.CanBeUsed = false;
                    break;
            }
        }
        #endregion PRIVATES_METHODS
    }
}