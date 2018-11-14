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
    /// <summary>
    /// Using the ButtonActionChoser, this System allow the user to move Step by Step, ie in the direction of its laser to which this feature is linked.
    /// </summary>
    public class StepByStepSystem : BACListenersSetupSystem, ITeleportSystem
    {
        struct Filter : ITeleportFilter
        {
            public StepByStepComponent SBS_Comp;
            public ScriptableRaycastComponent RayComp;
            public BACGeneralComponent BAC_Comp;
            public TeleportGeneralComponent TeleportGeneral;
            public SceneObjectsComponent SceneObjects;
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

        protected override void OnUpdate() { }

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
                e.BAC_Comp.OnButtonStartClicking.AddListener(delegate { OnStartInteractingCallback(e); });
                e.BAC_Comp.OnButtonStopClicking.AddListener(delegate { OnStopInteractingCallback(e); });
            }

            if ((e.BAC_Comp.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                e.BAC_Comp.OnButtonStartTouching.AddListener(delegate { OnStartInteractingCallback(e); });
                e.BAC_Comp.OnButtonStopTouching.AddListener(delegate { OnStopInteractingCallback(e); });
            }
        }

        public override void RemoveListenersOnEndApp(object entity)
        {
            var e = (Filter)entity;
            if ((e.BAC_Comp.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                e.BAC_Comp.OnButtonStartClicking.RemoveAllListeners();
                e.BAC_Comp.OnButtonStopClicking.RemoveAllListeners();
            }

            if ((e.BAC_Comp.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                e.BAC_Comp.OnButtonStartTouching.RemoveAllListeners();
                e.BAC_Comp.OnButtonStopTouching.RemoveAllListeners();
            }
        }
        #endregion Listeners_Setup


        #region Teleport_Interface
        /// <summary>
        /// Method to call from StopTouching, teleport the user in the direction of its controller.
        /// </summary>
        public void TeleportUser(ITeleportFilter teleportFilter)
        {
            Filter entity = (Filter)teleportFilter;

            // We check where the user should be when teleported one meter away.
            Vector3 newPos = CheckHandForward(entity);

            // If the new pos returned is null, an error as occured, so we stop the method
            if (newPos != Vector3.zero)
            {
                Debug.Log("newPos != Vector3.zero)");
                // We check the theoritic new user pos
                var newUsersPos = VRSF_Components.CameraRig.transform.position + new Vector3(newPos.x, 0.0f, newPos.z);

                // We launch a Vector directed to the floor to check if the new position is on the Teleport NavMesh
                // We use a 2.5 factor as a user's height is between 0.0 (SteamVR) and the height of a normal user (2 meter approximatively)
                // and we multiply it to the scale of the user
                var downVector = newUsersPos + (Vector3.down * 2.5f * VRSF_Components.CameraRig.transform.localScale.y);

                // We calculate the linecast between the newUserPos and the downVector and check if it hits the NavMesh
                TeleportNavMeshHelper.Linecast
                (
                    newUsersPos, 
                    downVector, 
                    out bool endOnNavmesh,      
                    entity.TeleportGeneral.ExclusionLayer, 
                    out entity.TeleportGeneral.PointToGoTo,
                    out Vector3 norm, 
                    entity.SceneObjects._TeleportNavMesh
                );

                Debug.Log("endOnNavmesh " + endOnNavmesh);

                if (endOnNavmesh)
                    SetTeleportState(entity, ETeleportState.Teleporting);
            }
        }
        #endregion

        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void OnStartInteractingCallback(Filter e)
        {
            SetTeleportState(e, ETeleportState.Selecting);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void OnStopInteractingCallback(Filter e)
        {
            Debug.Log("OnStopinteracting");
            // If the user is aiming to the UI, we don't activate the system
            if (!e.RayComp.RaycastHitVar.isNull && e.RayComp.RaycastHitVar.Value.collider.gameObject.layer == LayerMask.NameToLayer("UI"))
                return;

            TeleportUser(e);
        }

        /// <summary>
        /// Check, depending on the RayOrigin and the User's size, the forward vector to use.
        /// </summary>
        /// <returns>The new theoritical position of the user</returns>
        private Vector3 CheckHandForward(Filter entity)
        {
            float distanceWithScale = VRSF_Components.CameraRig.transform.localScale.x * entity.SBS_Comp.DistanceStepByStep;

            switch (entity.BAC_Comp.ButtonHand)
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


        private void SetTeleportState(Filter entity, ETeleportState newState)
        {
            // We set the teleporting state to teleporting
            entity.TeleportGeneral.CurrentTeleportState = newState;

            // We do the same for the Fading component if it exist. The TeleportUserSystem will handle the teleporting feature
            if (entity.SceneObjects.FadeComponent != null)
            {
                entity.SceneObjects.FadeComponent.TeleportState = newState;
                entity.SceneObjects.FadeComponent._teleportTimeMarker = Time.time;
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