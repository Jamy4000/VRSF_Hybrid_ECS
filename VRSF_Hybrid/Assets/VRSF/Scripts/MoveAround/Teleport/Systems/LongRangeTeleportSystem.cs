﻿using UnityEngine;
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
    public class LongRangeTeleportSystem : BACUpdateSystem<LongRangeTeleportComponent>, ITeleportSystem
    {
        new struct Filter : ITeleportFilter
        {
            public LongRangeTeleportComponent LRT_Comp;
            public BACGeneralComponent BAC_Comp;
            public ScriptableRaycastComponent RaycastComp;
            public TeleportGeneralComponent GeneralTeleport;
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
                e.GeneralTeleport.TeleportLayer = LayerMask.NameToLayer("Teleport");

                if (e.GeneralTeleport.TeleportLayer == -1)
                {
                    Debug.LogError("VRSF : You won't be able to teleport on the floor, as you didn't set the Ground Layer");
                }
                
                SetupListenersResponses(e);
                DeactivateTeleportSlider(e.LRT_Comp);
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
                e.BAC_Comp.OnButtonStartClicking.AddListener(delegate { OnStartInteracting(e); });
                e.BAC_Comp.OnButtonIsClicking.AddListener(delegate { OnIsInteracting(e); });
                e.BAC_Comp.OnButtonStopClicking.AddListener(delegate { TeleportUser(e); });
            }

            if ((e.BAC_Comp.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                e.BAC_Comp.OnButtonStartTouching.AddListener(delegate { OnStartInteracting(e); });
                e.BAC_Comp.OnButtonIsTouching.AddListener(delegate { OnIsInteracting(e); });
                e.BAC_Comp.OnButtonStopTouching.AddListener(delegate { TeleportUser(e); });
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

            Teleport(entity);
            DeactivateTeleportSlider(entity.LRT_Comp);

            _controllersVariable.RightExclusionLayer = _controllersVariable.RightExclusionLayer.AddToMask(entity.GeneralTeleport.TeleportLayer);
            _controllersVariable.LeftExclusionLayer = _controllersVariable.LeftExclusionLayer.AddToMask(entity.GeneralTeleport.TeleportLayer);
        }
        #endregion

        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Method to call from the StartTouching or StartClicking Method, set the Loading Slider values if used.
        /// </summary>
        private void OnStartInteracting(Filter entity)
        {
            // If the user is aiming to the UI, we don't activate the system
            if (!entity.RaycastComp.RaycastHitVar.isNull && entity.RaycastComp.RaycastHitVar.Value.collider.gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                return;
            }

            // We set the current state as 
            entity.GeneralTeleport.CurrentTeleportState = ETeleportState.Selecting;
            if (entity.SceneObjects.FadeComponent != null)
                entity.SceneObjects.FadeComponent.TeleportState = entity.GeneralTeleport.CurrentTeleportState;

            _controllersVariable.RightExclusionLayer = _controllersVariable.RightExclusionLayer.RemoveFromMask(entity.GeneralTeleport.TeleportLayer);
            _controllersVariable.LeftExclusionLayer = _controllersVariable.LeftExclusionLayer.RemoveFromMask(entity.GeneralTeleport.TeleportLayer);

            if (entity.LRT_Comp.UseLoadingSlider)
            {
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
        }


        /// <summary>
        /// To call from the IsClicking or IsTouching event
        /// </summary>
        private void OnIsInteracting(Filter entity)
        {
            // We check that the user can actually teleport himself
            CheckTeleport();

            // If we use a loading slider
            if (entity.LRT_Comp.UseLoadingSlider)
            {
                // We set the texts and fillrect based on the current fillAmount of the Timer
                var currentFillAmount = entity.LRT_Comp.FillRect.fillAmount * entity.LRT_Comp.TimerBeforeTeleport;

                // If the loading slider is still not full
                if (entity.LRT_Comp.FillRect != null && currentFillAmount < entity.LRT_Comp.TimerBeforeTeleport)
                {
                    entity.LRT_Comp.FillRect.fillAmount += Time.deltaTime / entity.LRT_Comp.TimerBeforeTeleport;
                }
                // If we don't hit the ground with the laser
                else if (entity.LRT_Comp.TeleportText != null && !entity.GeneralTeleport.CanTeleport)
                {
                    entity.LRT_Comp.TeleportText.text = "Waiting for ground ...";
                }
                // If we hit the ground with the laser
                else if (entity.LRT_Comp.TeleportText != null)
                {
                    entity.LRT_Comp.TeleportText.text = "Release To Teleport !";
                }
            }


            /// <summary>
            /// Check if the Teleport ray is on a Teleport Layer, and set the _canTeleport bool and the color of the Loading Slider accordingly.
            /// </summary>
            void CheckTeleport()
            {
                Color32 fillRectColor;
                bool endOnNavmesh = false;

                // If the raycast is hitting something ad it's not a UI Element
                if (!entity.RaycastComp.RaycastHitVar.isNull && entity.RaycastComp.RaycastHitVar.Value.collider.gameObject.layer != LayerMask.NameToLayer("UI"))
                {
                    TeleportNavMeshHelper.Linecast(entity.RaycastComp.RayVar.Value.origin, entity.RaycastComp.RaycastHitVar.Value.point, out endOnNavmesh,
                                               entity.GeneralTeleport.ExclusionLayer, out entity.GeneralTeleport.PointToGoTo, out Vector3 norm, entity.SceneObjects._TeleportNavMesh);
                }
                
                entity.GeneralTeleport.CanTeleport = endOnNavmesh;
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
        private void Teleport(Filter entity)
        {
            // We calculate the current fill amount of the slider
            var currentFillAmount = entity.LRT_Comp.FillRect.fillAmount * entity.LRT_Comp.TimerBeforeTeleport;

            // If it's above the time provided in the LongRangeTeleportComponent, we set the TeleportState to Teleproting
            entity.GeneralTeleport.CurrentTeleportState = entity.GeneralTeleport.CanTeleport && currentFillAmount >= entity.LRT_Comp.TimerBeforeTeleport ?
                ETeleportState.Teleporting : ETeleportState.None;

            // We do the same for the Fading component if it exist. The TeleportUserSystem will handle the teleporting feature
            if (entity.SceneObjects.FadeComponent != null)
                entity.SceneObjects.FadeComponent.TeleportState = entity.GeneralTeleport.CurrentTeleportState;
        }


        /// <summary>
        /// If used, we deactivate the Teleport Slider and Text when the user release the button.
        /// </summary>
        private void DeactivateTeleportSlider(LongRangeTeleportComponent lrtComp)
        {
            if (lrtComp.UseLoadingSlider)
            {
                if (lrtComp.FillRect != null)
                {
                    lrtComp.FillRect.gameObject.SetActive(false);
                }

                if (lrtComp.TeleportText != null)
                {
                    lrtComp.TeleportText.gameObject.SetActive(false);
                }
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
                // Setting up teleport layer
                e.GeneralTeleport.TeleportLayer = LayerMask.NameToLayer("Teleport");

                if (e.GeneralTeleport.TeleportLayer == -1)
                {
                    Debug.LogError("VRSF : You won't be able to teleport on the floor, as you didn't set the Ground Layer");
                }

                SetupListenersResponses(e);
                DeactivateTeleportSlider(e.LRT_Comp);
            }
        }
        #endregion
    }
}