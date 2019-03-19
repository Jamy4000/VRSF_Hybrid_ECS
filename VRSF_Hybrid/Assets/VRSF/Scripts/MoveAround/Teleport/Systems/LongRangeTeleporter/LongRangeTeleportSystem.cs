using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.Inputs;
using VRSF.Core.Raycast;
using VRSF.Core.SetupVR;
using VRSF.Core.Utils.ButtonActionChoser;

namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// Using the ButtonActionChoser, this System allow the user to teleport where the Raycast of his controller is pointing
    /// </summary>
    public class LongRangeTeleportSystem : BACListenersSetupSystem, ITeleportSystem
    {
        struct Filter : ITeleportFilter
        {
            public LongRangeTeleportComponent LRT_Comp;
            public BACGeneralComponent BAC_Comp;
            public ScriptableRaycastComponent RaycastComp;
            public TeleportGeneralComponent TeleportGeneral;
            public SceneObjectsComponent SceneObjects;
        }

        /// <summary>
        /// The reference to the ScriptableSingleton containing the variables for the Controllers
        /// </summary>
        private ControllersParametersVariable _controllersVariable;

        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnStartRunning()
        {
            OnSetupVRReady.Listeners += Init;
            _controllersVariable = ControllersParametersVariable.Instance;
            base.OnStartRunning();
        }
        
        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
            OnSetupVRReady.Listeners -= Init;
            foreach (var e in GetEntities<Filter>())
            {
                RemoveListeners(e);
            }
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
                e.BAC_Comp.OnButtonIsClicking.AddListener(delegate { OnIsInteractingCallback(e); });
                e.BAC_Comp.OnButtonStopClicking.AddListener(delegate { OnStopInteractingCallback(e); });
            }

            if ((e.BAC_Comp.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                e.BAC_Comp.OnButtonStartTouching.AddListener(delegate { OnStartInteractingCallback(e); });
                e.BAC_Comp.OnButtonIsTouching.AddListener(delegate { OnIsInteractingCallback(e); });
                e.BAC_Comp.OnButtonStopTouching.AddListener(delegate { OnStopInteractingCallback(e); });
            }
        }

        public override void RemoveListeners(object entity)
        {
            var e = (Filter)entity;

            if ((e.BAC_Comp.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                e.BAC_Comp.OnButtonStartClicking.RemoveAllListeners();
                e.BAC_Comp.OnButtonIsClicking.RemoveAllListeners();
                e.BAC_Comp.OnButtonStopClicking.RemoveAllListeners();
            }

            if ((e.BAC_Comp.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                e.BAC_Comp.OnButtonStartTouching.RemoveAllListeners();
                e.BAC_Comp.OnButtonIsTouching.RemoveAllListeners();
                e.BAC_Comp.OnButtonStopTouching.RemoveAllListeners();
            }
        }
        #endregion Listeners_Setup


        #region Teleport_Interface
        /// <summary>
        /// Method to call from StopTOuching or StopClicking Callback, set the user position to where he's poiting
        /// </summary>
        public void TeleportUser(ITeleportFilter teleportFilter)
        {
            Filter entity = (Filter)teleportFilter;
            new OnTeleportUser(entity.TeleportGeneral, entity.SceneObjects);
        }
        #endregion

        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Method to call from the StartTouching or StartClicking Method, set the Loading Slider values if used.
        /// </summary>
        private void OnStartInteractingCallback(Filter entity)
        {
            // We set the current state as Selecting
            TeleportUserSystem.SetTeleportState(ETeleportState.Selecting, entity.TeleportGeneral);
        }

        /// <summary>
        /// To call from the IsClicking or IsTouching event
        /// </summary>
        private void OnIsInteractingCallback(Filter e)
        {
            e.LRT_Comp._LoadingTimer += Time.deltaTime;
            
            // If the raycast is hitting something and it's not a UI Element
            if (e.RaycastComp.RaycastHitVar.RaycastHitIsNotOnUI())
            {
                TeleportNavMeshHelper.Linecast(e.RaycastComp.RayVar.Value.origin, e.RaycastComp.RaycastHitVar.Value.point, out bool endOnNavmesh,
                                           _controllersVariable.GetExclusionsLayer(e.BAC_Comp.ButtonHand), out TeleportGeneralComponent.PointToGoTo, out Vector3 norm, e.SceneObjects._TeleportNavMesh);

                TeleportGeneralComponent.CanTeleport = e.LRT_Comp.UseLoadingTimer ? (endOnNavmesh && e.LRT_Comp._LoadingTimer > e.LRT_Comp.LoadingTime) : endOnNavmesh;
            }
            else
            {
                TeleportGeneralComponent.CanTeleport = false;
            }
        }


        /// <summary>
        /// Handle the Teleport when the user is releasing the button.
        /// </summary>
        private void OnStopInteractingCallback(Filter entity)
        {
            if (TeleportGeneralComponent.CanTeleport)
                TeleportUser(entity);
            else
                TeleportUserSystem.SetTeleportState(ETeleportState.None, entity.TeleportGeneral);
        }

        /// <summary>
        /// Reactivate the System when setup vr is ready
        /// </summary>
        private void Init(OnSetupVRReady setupVRReady)
        {
            foreach (var e in GetEntities<Filter>())
            {
                SetupListenersResponses(e);
            }
        }
        #endregion
    }
}