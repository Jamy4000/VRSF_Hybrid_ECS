﻿using UnityEngine;
using VRSF.Inputs;
using VRSF.MoveAround.Teleport.Components;
using VRSF.MoveAround.Teleport.Interfaces;
using VRSF.Utils;
using VRSF.Utils.Components;
using VRSF.Utils.Components.ButtonActionChoser;
using VRSF.Utils.Systems.ButtonActionChoser;

namespace VRSF.MoveAround.Teleport.Systems
{
    public class StepByStepSystem : BACUpdateSystem, ITeleportSystem
    {
        struct Filter : ITeleportFilter
        {
            public ButtonActionChoserComponents BAC_Comp;
            public ScriptableRaycastComponent RayComp;
            public StepByStepComponent SBS_Comp;
            public TeleportBoundariesComponent TeleportBoundaries;
        }


        #region PRIVATE_VARIABLES
        private Filter _currentSetupEntity;
        #endregion PRIVATE_VARIABLES


        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnStartRunning()
        {
            if (GetEntities<Filter>().Length == 0)
            {
                this.Enabled = false;
                return;
            }

            base.OnStartRunning();
            
            foreach (var e in GetEntities<Filter>())
            {
                _currentSetupEntity = e;
                SetupListenersResponses();
            }
        }

        protected override void OnStopRunning()
        {
            base.OnStopRunning();

            foreach (var e in GetEntities<Filter>())
            {
                _currentSetupEntity = e;
                RemoveListenersOnEndApp();
            }
        }
        #endregion ComponentSystem_Methods


        #region PUBLIC_METHODS

        #region Listeners_Setup
        public override void SetupListenersResponses()
        {
            if ((_currentSetupEntity.BAC_Comp.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                _currentSetupEntity.BAC_Comp.OnButtonStartClicking.AddListener(delegate { TeleportUser(_currentSetupEntity); });
            }

            if ((_currentSetupEntity.BAC_Comp.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                _currentSetupEntity.BAC_Comp.OnButtonStartTouching.AddListener(delegate { TeleportUser(_currentSetupEntity); });
            }
        }

        public override void RemoveListenersOnEndApp()
        {
            if ((_currentSetupEntity.BAC_Comp.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                _currentSetupEntity.BAC_Comp.OnButtonStartClicking.RemoveAllListeners();
            }

            if ((_currentSetupEntity.BAC_Comp.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                _currentSetupEntity.BAC_Comp.OnButtonStartTouching.RemoveAllListeners();
            }
        }
        #endregion Listeners_Setup


        #region Teleport_Interface
        /// <summary>
        /// Method to call from StartClicking or StartTouching, teleport the user one meter away.
        /// </summary>
        public void TeleportUser(ITeleportFilter teleportFilter)
        {
            Filter entity = (Filter)teleportFilter;

            // We check where the user should be when teleported one meter away.
            Vector3 newPos = CheckHandForward(entity);

            // If the new pos returned is null, an error as occured, so we stop the method
            if (newPos == Vector3.zero)
                return;

            // If we want to stay on the same vertical axis, we set the y in newPos to 0
            if (!entity.SBS_Comp.MoveOnVerticalAxis)
                newPos = new Vector3(newPos.x, 0.0f, newPos.z);

            // If we use boundaries, we check if the user is not going to far away
            if (entity.TeleportBoundaries._UseBoundaries)
                newPos = CheckNewPosWithBoundaries(entity, newPos);

            // We set the cameraRig position
            VRSF_Components.CameraRig.transform.position += newPos;
        }


        /// <summary>
        /// Check the newPos for theStep by Step feature depending on the Teleport Boundaries
        /// </summary>
        public Vector3 CheckNewPosWithBoundaries(ITeleportFilter teleportFilter, Vector3 posToCheck)
        {
            Filter entity = (Filter)teleportFilter;

            Vector3 minPos = entity.TeleportBoundaries._MinUserPosition;
            Vector3 maxPos = entity.TeleportBoundaries._MaxUserPosition;

            posToCheck.x = Mathf.Clamp(posToCheck.x, minPos.x, maxPos.x);
            posToCheck.y = Mathf.Clamp(posToCheck.y, minPos.y, maxPos.y);
            posToCheck.z = Mathf.Clamp(posToCheck.z, minPos.z, maxPos.z);
            
            return posToCheck;
        }
        #endregion

        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Check, depending on the RayOrigin and the User's size, the forward vector to use.
        /// </summary>
        /// <returns>The new theoritical position of the user</returns>
        private Vector3 CheckHandForward(Filter entity)
        {
            float distanceWithScale = VRSF_Components.CameraRig.transform.localScale.x * entity.SBS_Comp.DistanceStepByStep;

            switch (entity.RayComp.RayOrigin)
            {
                case Controllers.EHand.LEFT:
                    return VRSF_Components.LeftController.transform.forward * distanceWithScale;
                case Controllers.EHand.RIGHT:
                    return VRSF_Components.RightController.transform.forward * distanceWithScale;
                case Controllers.EHand.GAZE:
                    return VRSF_Components.VRCamera.transform.forward * distanceWithScale;
                default:
                    Debug.LogError("Please specify a valid RayOrigin in the Inspector to be able to use the Teleport StepByStep feature.");
                    return Vector3.zero;
            }
        }
        #endregion
    }
}