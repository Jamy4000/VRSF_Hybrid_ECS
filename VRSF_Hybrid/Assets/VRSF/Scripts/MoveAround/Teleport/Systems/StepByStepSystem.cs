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
    public class StepByStepSystem : BACUpdateSystem<StepByStepComponent>, ITeleportSystem
    {
        new struct Filter : ITeleportFilter
        {
            public BACGeneralComponent BAC_Comp;
            public ScriptableRaycastComponent RayComp;
            public StepByStepComponent SBS_Comp;
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

            // We check where the user should be when teleported one meter away.
            Vector3 newPos = CheckHandForward(entity);

            // If the new pos returned is null, an error as occured, so we stop the method
            if (newPos != Vector3.zero)
            {
                // We check the theoritic new user pos
                var newUsersPos = VRSF_Components.CameraRig.transform.position + new Vector3(newPos.x, 0.0f, newPos.z);

                // We create a down vector based on the new theoritic position and the actual users's height
                var downVector = newUsersPos + (Vector3.down * newUsersPos.y);
                
                // If the linecast between the newUserPos and the downVector hit the TeleportNavMesh
                if (TeleportNavMeshHelper.Linecast(newUsersPos, downVector, out bool endOnNavmesh,
                                               entity.TeleportGeneral.ExclusionLayer, out entity.TeleportGeneral.PointToGoTo, out Vector3 norm, entity.SceneObjects._TeleportNavMesh))
                {
                    // We set the current State to Teleporting
                    entity.TeleportGeneral.CurrentTeleportState = ETeleportState.Teleporting;

                    // We set the current State of the FadeComponent to Teleporting if it exist. The teleport is then handled by the TeleportUserSystem
                    if (entity.SceneObjects.FadeComponent != null)
                    {
                        entity.SceneObjects.FadeComponent.TeleportState = ETeleportState.Teleporting;
                        entity.SceneObjects.FadeComponent._teleportTimeMarker = Time.time;
                    }
                }
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