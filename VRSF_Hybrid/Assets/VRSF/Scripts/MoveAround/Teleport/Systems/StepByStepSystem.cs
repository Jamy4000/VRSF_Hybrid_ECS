using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRSF.Inputs;
using VRSF.MoveAround.Teleport.Components;
using VRSF.MoveAround.Teleport.Interfaces;
using VRSF.Utils;
using VRSF.Utils.Components;
using VRSF.Utils.Components.ButtonActionChoser;
using VRSF.Utils.Systems.ButtonActionChoser;

namespace VRSF.MoveAround.Teleport.Systems
{
    public class StepByStepSystem : BACUpdateSystem<StepByStepComponent>, ITeleportSystem
    {
        new struct Filter : ITeleportFilter
        {
            public BACGeneralComponent BAC_Comp;
            public ScriptableRaycastComponent RayComp;
            public StepByStepComponent SBS_Comp;
            public TeleportBoundariesComponent TeleportBoundaries;
        }


        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            SceneManager.sceneUnloaded += OnSceneUnloaded;

            foreach (var e in GetEntities<Filter>())
            {
                SetupListenersResponses(e);
            }
        }
        
        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();

            foreach (var e in GetEntities<Filter>())
            {
                RemoveListenersOnEndApp(e);
            }

            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }
        #endregion ComponentSystem_Methods


        #region PUBLIC_METHODS

        #region Listeners_Setup
        public override void SetupListenersResponses(object entity)
        {
            var e = (Filter)entity;
            if ((e.BAC_Comp.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                e.BAC_Comp.OnButtonStartClicking.AddListener(delegate { TeleportUser(e); });
            }

            if ((e.BAC_Comp.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                e.BAC_Comp.OnButtonStartTouching.AddListener(delegate { TeleportUser(e); });
            }
        }

        public override void RemoveListenersOnEndApp(object entity)
        {
            var e = (Filter)entity;
            if ((e.BAC_Comp.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                e.BAC_Comp.OnButtonStartClicking.RemoveAllListeners();
            }

            if ((e.BAC_Comp.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                e.BAC_Comp.OnButtonStartTouching.RemoveAllListeners();
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

            // If the user is aiming to the UI, we don't activate the system
            if (!entity.RayComp.RaycastHitVar.isNull && entity.RayComp.RaycastHitVar.Value.collider.gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                return;
            }
            else
            {
                // We check where the user should be when teleported one meter away.
                Vector3 newPos = CheckHandForward(entity);

                // If the new pos returned is null, an error as occured, so we stop the method
                if (newPos != Vector3.zero)
                {
                    // If we want to stay on the same vertical axis, we set the y in newPos to 0
                    if (!entity.SBS_Comp.MoveOnVerticalAxis)
                    {
                        newPos = new Vector3(newPos.x, 0.0f, newPos.z);
                    }

                    // If we use boundaries, we check if the user is not going to far away
                    if (entity.TeleportBoundaries.UseBoundaries())
                    {
                        newPos += VRSF_Components.VRCamera.transform.position;

                        CheckNewPosWithBoundaries(entity, ref newPos);

                        // We set the cameraRig position
                        VRSF_Components.CameraRig.transform.position += (newPos - VRSF_Components.VRCamera.transform.position);
                    }
                    else
                    {
                        // We set the cameraRig position
                        VRSF_Components.CameraRig.transform.position += newPos;
                    }
                }
            }
        }


        /// <summary>
        /// Check the newPos for theStep by Step feature depending on the Teleport Boundaries
        /// </summary>
        public void CheckNewPosWithBoundaries(ITeleportFilter teleportFilter, ref Vector3 posToCheck)
        {
            Filter entity = (Filter)teleportFilter;
            
            bool isInBoundaries = false;
            List<Vector3> closestDists = new List<Vector3>();

            foreach (Bounds bound in entity.TeleportBoundaries.Boundaries())
            {
                if (bound.Contains(posToCheck))
                {
                    isInBoundaries = true;
                    break;
                }
                else
                {
                    closestDists.Add(bound.ClosestPoint(posToCheck));
                }
            }

            // if the posToCheck is not in the boundaries, we check what's the closest point from it
            if (!isInBoundaries)
            {
                float closestDist = float.PositiveInfinity;
                Vector3 closestPoint = Vector3.positiveInfinity;

                foreach (var point in closestDists)
                {
                    var distance = (posToCheck - point).magnitude;

                    if (distance < closestDist)
                    {
                        closestDist = distance;
                        closestPoint = point;
                    }
                }

                posToCheck = closestPoint;
            }
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


        /// <summary>
        /// Reactivate the System when switching to another Scene.
        /// </summary>
        /// <param name="oldScene">The previous scene before switching</param>
        private void OnSceneUnloaded(Scene oldScene)
        {
            foreach (var e in GetEntities<Filter>())
            {
                SetupListenersResponses(e);
            }
        }
        #endregion
    }
}