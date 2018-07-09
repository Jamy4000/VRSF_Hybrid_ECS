﻿using System.Collections;
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
        private InteractionVariableContainer _interactionsContainer;
        #endregion


        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            _interactionsContainer = InteractionVariableContainer.Instance;

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
                    comp.HitVar = _interactionsContainer.LeftHit;
                    comp.RayVar = _interactionsContainer.LeftRay;
                    break;

                case (EHand.RIGHT):
                    comp.HitVar = _interactionsContainer.RightHit;
                    comp.RayVar = _interactionsContainer.RightRay;
                    break;

                case (EHand.GAZE):
                    comp.HitVar = _interactionsContainer.GazeHit;
                    comp.RayVar = _interactionsContainer.GazeRay;
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