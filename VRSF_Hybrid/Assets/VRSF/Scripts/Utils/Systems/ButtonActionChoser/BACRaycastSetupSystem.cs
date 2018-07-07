using System.Collections;
using Unity.Entities;
using UnityEngine;
using VRSF.Controllers;
using VRSF.Interactions;
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

        #region PRIVATE_VARIABLES
        private ButtonActionChoserComponents _currentComp;

        private InteractionVariableContainer _interactionsContainer;
        #endregion


        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            foreach (var entity in GetEntities<Filter>())
            {
                _currentComp = entity.ButtonComponents;

                if (_currentComp.SOsAreReady)
                {
                    // We check which hit to use for this feature with the RayOrigin
                    SetupRayAndHit();
                }
                else
                {
                    _currentComp.StartCoroutine(WaitForScriptableSingletons());
                }
            }
        }


        protected override void OnUpdate() {}
        #endregion


        #region PRIVATES_METHODS
        /// <summary>
        /// Check which RaycastHitVariable is used depending on the RayOrigin specified
        /// </summary>
        private void SetupRayAndHit()
        {
            switch (_currentComp.RayOrigin)
            {
                case (EHand.LEFT):
                    _currentComp.HitVar = _interactionsContainer.LeftHit;
                    _currentComp.RayVar = _interactionsContainer.LeftRay;
                    break;

                case (EHand.RIGHT):
                    _currentComp.HitVar = _interactionsContainer.RightHit;
                    _currentComp.RayVar = _interactionsContainer.RightRay;
                    break;

                case (EHand.GAZE):
                    _currentComp.HitVar = _interactionsContainer.GazeHit;
                    _currentComp.RayVar = _interactionsContainer.GazeRay;
                    break;

                default:
                    Debug.LogError("VRSF : You need to specify the RayOrigin for the " + GetType().Name + " script. Setting CanBeUsed of ButtonActionChoserComponents to false.");
                    _currentComp.CanBeUsed = false;
                    break;
            }
        }

        /// <summary>
        /// As some values are initialized in other Systems, we just want to be sure that everything is setup before setting up everything.
        /// </summary>
        /// <returns></returns>
        private IEnumerator WaitForScriptableSingletons()
        {
            while (!_currentComp.SOsAreReady)
            {
                yield return new WaitForEndOfFrame();
            }
            // We check which hit to use for this feature with the RayOrigin
            SetupRayAndHit();
        }
        #endregion PRIVATES_METHODS
    }
}