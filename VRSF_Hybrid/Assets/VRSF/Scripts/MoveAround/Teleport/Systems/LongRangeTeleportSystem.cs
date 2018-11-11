using System.Collections.Generic;
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
    public class LongRangeTeleportSystem : BACUpdateSystem, ITeleportSystem
    {
        new struct Filter : ITeleportFilter
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
                // Setting up teleport layer
                e.TeleportGeneral.TeleportLayer = LayerMask.NameToLayer("Teleport");

                if (e.TeleportGeneral.TeleportLayer == -1)
                {
                    Debug.LogError("VRSF : You won't be able to teleport on the floor, as you didn't set the Ground Layer");
                }
                
                SetupListenersResponses(e);
                DeactivateTeleportSlider(e);
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

        public override void RemoveListenersOnEndApp(object entity)
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
        /// Method to call from StartClicking or StartTouching, teleport the user one meter away.
        /// </summary>
        public void TeleportUser(ITeleportFilter teleportFilter)
        {
            Filter entity = (Filter)teleportFilter;

            SetTeleportState(entity, ETeleportState.Teleporting);
            if (entity.SceneObjects.FadeComponent != null)
                entity.SceneObjects.FadeComponent._teleportTimeMarker = Time.time;
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
        private void OnIsInteractingCallback(Filter entity)
        {
            // We check that the user can actually teleport himself
            CheckTeleport();
            
            // If the loading slider is still not full
            if (entity.LRT_Comp.UseLoadingSlider && entity.LRT_Comp.FillRect != null)
            {
                // We set the texts and fillrect based on the current fillAmount of the Timer
                var currentFillAmount = entity.LRT_Comp.FillRect.fillAmount * entity.LRT_Comp.LoadingTime;

                if (currentFillAmount < entity.LRT_Comp.LoadingTime)
                    entity.LRT_Comp.FillRect.fillAmount += Time.deltaTime / entity.LRT_Comp.LoadingTime;
            }
            // If we don't hit the ground with the laser
            else if (entity.LRT_Comp.TeleportText != null && !entity.TeleportGeneral.CanTeleport)
            {
                entity.LRT_Comp.TeleportText.text = "Waiting for ground ...";
            }
            // If we hit the ground with the laser
            else if (entity.LRT_Comp.TeleportText != null)
            {
                entity.LRT_Comp.TeleportText.text = "Release To Teleport !";
            }
            


            /// <summary>
            /// Check if the Teleport ray is on a Teleport Layer, and set the _canTeleport bool and the color of the Loading Slider accordingly.
            /// </summary>
            void CheckTeleport()
            {
                Color32 fillRectColor;
                bool endOnNavmesh = false;

                // If the raycast is hitting something and it's not a UI Element
                if (!entity.RaycastComp.RaycastHitVar.isNull && entity.RaycastComp.RaycastHitVar.Value.collider.gameObject.layer != LayerMask.NameToLayer("UI"))
                {
                    TeleportNavMeshHelper.Linecast(entity.RaycastComp.RayVar.Value.origin, entity.RaycastComp.RaycastHitVar.Value.point, out endOnNavmesh,
                                               entity.TeleportGeneral.ExclusionLayer, out entity.TeleportGeneral.PointToGoTo, out Vector3 norm, entity.SceneObjects._TeleportNavMesh);
                }

                entity.TeleportGeneral.CanTeleport = endOnNavmesh;
                fillRectColor = endOnNavmesh ? new Color32(100, 255, 100, 255) : new Color32(0, 180, 255, 255);

                // If we use a loading slider and the fillRect to give the user a visual feedback is not null
                if (entity.LRT_Comp.UseLoadingSlider && entity.LRT_Comp.FillRect != null)
                {
                    entity.LRT_Comp.FillRect.color = fillRectColor;
                }

                // If we use a loading slider and the Text to give the user a visual feedback is not null
                if (entity.LRT_Comp.UseLoadingSlider && entity.LRT_Comp.TeleportText != null)
                {
                    entity.LRT_Comp.TeleportText.color = fillRectColor;
                }
            }
        }


        /// <summary>
        /// Handle the Teleport when the user is releasing the button.
        /// </summary>
        private void OnStopInteractingCallback(Filter entity)
        {
            TeleportUser(entity);
            DeactivateTeleportSlider(entity);

            _controllersVariable.RightExclusionLayer = _controllersVariable.RightExclusionLayer.AddToMask(entity.TeleportGeneral.TeleportLayer);
            _controllersVariable.LeftExclusionLayer = _controllersVariable.LeftExclusionLayer.AddToMask(entity.TeleportGeneral.TeleportLayer);
        }

        private void Initvariables(Filter entity)
        {
            // We set the current state as Selecting
            entity.TeleportGeneral.CurrentTeleportState = ETeleportState.Selecting;

            // We do the same for the Fade component if it exists
            if (entity.SceneObjects.FadeComponent != null)
                entity.SceneObjects.FadeComponent.TeleportState = entity.TeleportGeneral.CurrentTeleportState;

            // We remove the Teleport Layer from the exclusion layer while the user is clicking on the teleport button
            _controllersVariable.RightExclusionLayer = _controllersVariable.RightExclusionLayer.RemoveFromMask(entity.TeleportGeneral.TeleportLayer);
            _controllersVariable.LeftExclusionLayer = _controllersVariable.LeftExclusionLayer.RemoveFromMask(entity.TeleportGeneral.TeleportLayer);

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
                // Setting up teleport layer
                e.TeleportGeneral.TeleportLayer = LayerMask.NameToLayer("Teleport");

                if (e.TeleportGeneral.TeleportLayer == -1)
                {
                    Debug.LogError("VRSF : You won't be able to teleport on the floor, as you didn't set the Ground Layer");
                }

                SetupListenersResponses(e);
                DeactivateTeleportSlider(e);
            }
        }
        #endregion
    }
}