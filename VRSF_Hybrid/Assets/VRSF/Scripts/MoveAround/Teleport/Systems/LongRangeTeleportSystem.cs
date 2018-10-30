using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRSF.Controllers;
using VRSF.Inputs;
using VRSF.MoveAround.Teleport.Components;
using VRSF.MoveAround.Teleport.Interfaces;
using VRSF.Utils;
using VRSF.Utils.Components;
using VRSF.Utils.Components.ButtonActionChoser;
using VRSF.Utils.Systems.ButtonActionChoser;

namespace VRSF.MoveAround.Teleport.Systems
{
    public class LongRangeTeleportSystem : BACUpdateSystem<LongRangeTeleportComponent>, ITeleportSystem
    {
        new struct Filter : ITeleportFilter
        {
            public LongRangeTeleportComponent LRT_Comp;
            public BACGeneralComponent BAC_Comp;
            public ScriptableRaycastComponent RaycastComp;
            public TeleportBoundariesComponent TeleportBoundaries;
            public TeleportGeneralComponent GeneralTeleport;
        }

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


        /// <summary>
        /// Check the newPos for theStep by Step feature depending on the Teleport Boundaries
        /// </summary>
        /// <returns>The new position of the user after checking the boundaries</returns>
        public void CheckNewPosWithBoundaries(ITeleportFilter teleportFilter, ref Vector3 posToCheck)
        {
            Filter entity = (Filter)teleportFilter;

            bool _isInBoundaries = false;
            List<Vector3> closestDists = new List<Vector3>();

            foreach (Bounds bound in entity.TeleportBoundaries.Boundaries())
            {
                if (bound.Contains(posToCheck))
                {
                    _isInBoundaries = true;
                    break;
                }
                else
                {
                    closestDists.Add(bound.ClosestPoint(posToCheck));
                }
            }

            // if the posToCheck is not in the boundaries, we check what's the closest point from it
            if (!_isInBoundaries)
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
        /// Method to call from the StartTouching or StartClicking Method, set the Loading Slider values if used.
        /// </summary>
        private void OnStartInteracting(Filter entity)
        {
            // If the user is aiming to the UI, we don't activate the system
            if (!entity.RaycastComp.RaycastHitVar.isNull && entity.RaycastComp.RaycastHitVar.Value.collider.gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                return;
            }
            else
            {
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
        }


        /// <summary>
        /// To call from the IsClicking or IsTouching event
        /// </summary>
        private void OnIsInteracting(Filter entity)
        {
            CheckTeleport();

            if (entity.LRT_Comp.UseLoadingSlider)
            {
                var currentFillAmount = entity.LRT_Comp.FillRect.fillAmount * entity.LRT_Comp.TimerBeforeTeleport;
                if (entity.LRT_Comp.FillRect != null && currentFillAmount < entity.LRT_Comp.TimerBeforeTeleport)
                {
                    entity.LRT_Comp.FillRect.fillAmount += Time.deltaTime / entity.LRT_Comp.TimerBeforeTeleport;
                }
                else if (entity.LRT_Comp.TeleportText != null && !entity.GeneralTeleport.CanTeleport)
                {
                    entity.LRT_Comp.TeleportText.text = "Waiting for ground ...";
                }
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

                if (!entity.RaycastComp.RaycastHitVar.isNull && entity.RaycastComp.RaycastHitVar.Value.collider.gameObject.layer == entity.GeneralTeleport.TeleportLayer)
                {
                    entity.GeneralTeleport.CanTeleport = true;
                    fillRectColor = new Color32(100, 255, 100, 255);
                }
                else
                {
                    entity.GeneralTeleport.CanTeleport = false;
                    fillRectColor = new Color32(0, 180, 255, 255);
                }

                if (entity.LRT_Comp.UseLoadingSlider && entity.LRT_Comp.FillRect != null)
                {
                    entity.LRT_Comp.FillRect.color = fillRectColor;
                }

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
            var currentFillAmount = entity.LRT_Comp.FillRect.fillAmount * entity.LRT_Comp.TimerBeforeTeleport;
            if (entity.GeneralTeleport.CanTeleport && currentFillAmount >= entity.LRT_Comp.TimerBeforeTeleport)
            {
                Vector3 newPos = entity.RaycastComp.RaycastHitVar.Value.point;

                if (entity.LRT_Comp.AdjustHeight)
                {
                    newPos.y += entity.LRT_Comp.HeightAboveGround + VRSF_Components.CameraRig.transform.localScale.y;
                }
                else
                {
                    newPos.y = VRSF_Components.CameraRig.transform.position.y;
                }

                // If we use the boundaries, we check the newPos, if not, we set the position of the user directly
                if (entity.TeleportBoundaries.UseBoundaries())
                {
                    CheckNewPosWithBoundaries(entity, ref newPos);
                }

                VRSF_Components.CameraRig.transform.position = newPos;
            }
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