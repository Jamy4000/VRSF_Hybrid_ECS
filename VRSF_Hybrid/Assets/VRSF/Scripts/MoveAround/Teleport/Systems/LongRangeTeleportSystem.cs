using UnityEngine;
using UnityEngine.SceneManagement;
using VRSF.Controllers;
using VRSF.Inputs;
using VRSF.MoveAround.Teleport.Components;
using VRSF.MoveAround.Teleport.Interfaces;
using VRSF.Utils.Components;
using VRSF.Utils.Components.ButtonActionChoser;
using VRSF.Utils.Systems.ButtonActionChoser;

namespace VRSF.MoveAround.Teleport.Systems
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
            base.OnStartRunning();

            SceneManager.sceneUnloaded += OnSceneUnloaded;

            _controllersVariable = ControllersParametersVariable.Instance;

            foreach (var e in GetEntities<Filter>())
            {
                SetupListenersResponses(e);
                DeactivateTeleportSlider(e);
            }
        }

        protected override void OnUpdate() { }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();

            foreach (var e in GetEntities<Filter>())
            {
                RemoveListeners(e);
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
            TeleportUserSystem.SetTeleportState(entity.TeleportGeneral, entity.SceneObjects, ETeleportState.Teleporting);
        }
        #endregion

        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Method to call from the StartTouching or StartClicking Method, set the Loading Slider values if used.
        /// </summary>
        private void OnStartInteractingCallback(Filter entity)
        {
            Initvariables(entity);
        }

        /// <summary>
        /// To call from the IsClicking or IsTouching event
        /// </summary>
        private void OnIsInteractingCallback(Filter e)
        {
            e.LRT_Comp._LoadingTimer += Time.deltaTime;

            // We check that the user can actually teleport himself
            CheckTeleport();
            
            // If the loading slider is still not full
            if (e.LRT_Comp.UseLoadingTimer && e.LRT_Comp.FillRect != null)
            {
                if (e.LRT_Comp.FillRect.fillAmount < 1.0f)
                    e.LRT_Comp.FillRect.fillAmount += Time.deltaTime / e.LRT_Comp.LoadingTime;
            }

            // If we use a text to give a feedback to the user
            if (e.LRT_Comp.TeleportText != null)
                e.LRT_Comp.TeleportText.text = e.TeleportGeneral.CanTeleport ? "Release To Teleport !" : "Waiting for ground ...";
            


            /// <summary>
            /// Check if the Teleport ray is on a Teleport Layer, and set the _canTeleport bool and the color of the Loading Slider accordingly.
            /// </summary>
            void CheckTeleport()
            {
                Color32 fillRectColor;
                bool endOnNavmesh = false;

                // If the raycast is hitting something and it's not a UI Element
                if (e.RaycastComp.RaycastHitVar.isNull || e.RaycastComp.RaycastHitVar.Value.collider.gameObject.layer != LayerMask.NameToLayer("UI"))
                {
                    TeleportNavMeshHelper.Linecast(e.RaycastComp.RayVar.Value.origin, e.RaycastComp.RaycastHitVar.Value.point, out endOnNavmesh,
                                               _controllersVariable.GetExclusionsLayer(e.BAC_Comp.ButtonHand), out e.TeleportGeneral.PointToGoTo, out Vector3 norm, e.SceneObjects._TeleportNavMesh);
                }

                e.TeleportGeneral.CanTeleport = e.LRT_Comp.UseLoadingTimer ? (endOnNavmesh && e.LRT_Comp._LoadingTimer > e.LRT_Comp.LoadingTime) : endOnNavmesh;
                fillRectColor = e.TeleportGeneral.CanTeleport ? new Color32(100, 255, 100, 255) : new Color32(0, 180, 255, 255);

                // If we use a loading slider and the fillRect to give the user a visual feedback is not null
                if (e.LRT_Comp.UseLoadingTimer && e.LRT_Comp.FillRect != null)
                    e.LRT_Comp.FillRect.color = fillRectColor;

                // If we use a loading slider and the Text to give the user a visual feedback is not null
                if (e.LRT_Comp.UseLoadingTimer && e.LRT_Comp.TeleportText != null)
                    e.LRT_Comp.TeleportText.color = fillRectColor;
            }
        }


        /// <summary>
        /// Handle the Teleport when the user is releasing the button.
        /// </summary>
        private void OnStopInteractingCallback(Filter entity)
        {
            entity.LRT_Comp._LoadingTimer = 0.0f;

            if (entity.TeleportGeneral.CanTeleport)
                TeleportUser(entity);
            else
                TeleportUserSystem.SetTeleportState(entity.TeleportGeneral, entity.SceneObjects, ETeleportState.None);

            DeactivateTeleportSlider(entity);
        }


        private void Initvariables(Filter entity)
        {
            // We set the current state as Selecting
            TeleportUserSystem.SetTeleportState(entity.TeleportGeneral, entity.SceneObjects, ETeleportState.Selecting);
            
            // If we use the loading slider, we set the fillRect value and the TeleportText value
            if (entity.LRT_Comp.FillRect != null)
            {
                entity.LRT_Comp.FillRect.gameObject.SetActive(true);
                entity.LRT_Comp.FillRect.fillAmount = 0.0f;
            }

            if (entity.LRT_Comp.TeleportText != null)
            {
                entity.LRT_Comp.TeleportText.gameObject.SetActive(true);
                entity.LRT_Comp.TeleportText.text = "Preparing Teleport ...";
            }
        }


        /// <summary>
        /// If used, we deactivate the Teleport Slider and Text when the user release the button.
        /// </summary>
        private void DeactivateTeleportSlider(Filter e)
        {
            e.LRT_Comp.FillRect?.gameObject.SetActive(false);
            e.LRT_Comp.TeleportText?.gameObject.SetActive(false);
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
                DeactivateTeleportSlider(e);
            }
        }
        #endregion
    }
}