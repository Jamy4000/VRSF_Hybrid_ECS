using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSF.Inputs;
using VRSF.MoveAround.Teleport.Components;
using VRSF.MoveAround.Teleport.Interfaces;
using VRSF.Utils;
using VRSF.Utils.Components;
using VRSF.Utils.Systems.ButtonActionChoser;

namespace VRSF.MoveAround.Teleport.Systems
{
    public class LongRangeTeleportSystem : BACUpdateSystem, ITeleportSystem
    {
        struct Filter : ITeleportFilter
        {
            public ButtonActionChoserComponents BAC_Comp;
            public LongRangeTeleportComponent LRT_Comp;
        }


        #region PRIVATE_VARIABLES
        private Filter _currentSetupEntity;
        #endregion PRIVATE_VARIABLES


        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            foreach (var e in GetEntities<Filter>())
            {
                // Setting up teleport layer
                e.LRT_Comp._teleportLayer = LayerMask.NameToLayer("Teleport");
                if (e.LRT_Comp._teleportLayer == -1)
                {
                    Debug.LogError("VRSF : You won't be able to teleport on the floor, as you didn't set the Ground Layer");
                }

                if (e.LRT_Comp.UseLoadingSlider)
                {
                    if (e.LRT_Comp.FillRect != null)
                        e.LRT_Comp.FillRect.gameObject.SetActive(false);

                    if (e.LRT_Comp.TeleportText != null)
                        e.LRT_Comp.TeleportText.gameObject.SetActive(false);
                }

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
                _currentSetupEntity.BAC_Comp.OnButtonStartClicking.AddListener(delegate { OnStartInteracting(_currentSetupEntity.LRT_Comp); });
                _currentSetupEntity.BAC_Comp.OnButtonIsClicking.AddListener(delegate { OnIsInteracting(_currentSetupEntity); });
                _currentSetupEntity.BAC_Comp.OnButtonStopClicking.AddListener(delegate { TeleportUser(_currentSetupEntity); });
            }

            if ((_currentSetupEntity.BAC_Comp.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                _currentSetupEntity.BAC_Comp.OnButtonStartTouching.AddListener(delegate { OnStartInteracting(_currentSetupEntity.LRT_Comp); });
                _currentSetupEntity.BAC_Comp.OnButtonIsTouching.AddListener(delegate { OnIsInteracting(_currentSetupEntity); });
                _currentSetupEntity.BAC_Comp.OnButtonStopTouching.AddListener(delegate { TeleportUser(_currentSetupEntity); });
            }
        }

        public override void RemoveListenersOnEndApp()
        {
            if ((_currentSetupEntity.BAC_Comp.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                _currentSetupEntity.BAC_Comp.OnButtonStartClicking.RemoveAllListeners();
                _currentSetupEntity.BAC_Comp.OnButtonIsClicking.RemoveAllListeners();
                _currentSetupEntity.BAC_Comp.OnButtonStopClicking.RemoveAllListeners();
            }

            if ((_currentSetupEntity.BAC_Comp.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                _currentSetupEntity.BAC_Comp.OnButtonStartTouching.RemoveAllListeners();
                _currentSetupEntity.BAC_Comp.OnButtonIsTouching.RemoveAllListeners();
                _currentSetupEntity.BAC_Comp.OnButtonStopTouching.RemoveAllListeners();
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
        }


        /// <summary>
        /// Check the newPos for theStep by Step feature depending on the Teleport Boundaries
        /// </summary>
        /// <returns>The new position of the user after checking the boundaries</returns>
        public Vector3 CheckNewPosWithBoundaries(ITeleportFilter teleportFilter, Vector3 posToCheck)
        {
            Filter entity = (Filter)teleportFilter;

            Vector3 minPos = entity.LRT_Comp._MinUserPosition;
            Vector3 maxPos = entity.LRT_Comp._MaxUserPosition;

            posToCheck.x = Mathf.Clamp(posToCheck.x, minPos.x, maxPos.x);
            posToCheck.y = Mathf.Clamp(posToCheck.y, minPos.y, maxPos.y);
            posToCheck.z = Mathf.Clamp(posToCheck.z, minPos.z, maxPos.z);
            
            return posToCheck;
        }
        #endregion

        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Method to call from the StartTouching or StartClicking Method, set the Loading Slider values if used.
        /// </summary>
        private void OnStartInteracting(LongRangeTeleportComponent lrtComp)
        {
            if (lrtComp.UseLoadingSlider)
            {
                if (lrtComp.FillRect != null)
                {
                    lrtComp.FillRect.gameObject.SetActive(true);
                    lrtComp.FillRect.fillAmount = 0.0f;
                }

                if (lrtComp.TeleportText != null)
                {
                    lrtComp.TeleportText.gameObject.SetActive(true);
                    lrtComp.TeleportText.text = "Preparing Teleport ...";
                }
            }
        }


        /// <summary>
        /// To call from the IsClicking or IsTouching event
        /// </summary>
        private void OnIsInteracting(Filter entity)
        {
            CheckTeleport(entity);

            if (entity.LRT_Comp.UseLoadingSlider)
            {
                if (entity.LRT_Comp.FillRect != null && entity.LRT_Comp.FillRect.fillAmount < 1.0f)
                {
                    entity.LRT_Comp.FillRect.fillAmount += Time.deltaTime / entity.LRT_Comp.TimerBeforeTeleport;
                }
                else if (entity.LRT_Comp.TeleportText != null && !entity.LRT_Comp.CanTeleport)
                {
                    entity.LRT_Comp.TeleportText.text = "Waiting for ground ...";
                }
                else if (entity.LRT_Comp.TeleportText != null)
                {
                    entity.LRT_Comp.TeleportText.text = "Release To Teleport !";
                }
            }
        }


        /// <summary>
        /// Check if the Teleport ray is on a Teleport Layer, and set the _canTeleport bool and the color of the Loading Slider accordingly.
        /// </summary>
        private void CheckTeleport(Filter entity)
        {
            Color32 fillRectColor;

            if (!entity.BAC_Comp.HitVar.isNull && entity.BAC_Comp.HitVar.Value.collider.gameObject.layer == entity.LRT_Comp._teleportLayer)
            {
                entity.LRT_Comp.CanTeleport = true;
                fillRectColor = new Color32(100, 255, 100, 255);
            }
            else
            {
                entity.LRT_Comp.CanTeleport = false;
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


        /// <summary>
        /// Handle the Teleport when the user is releasing the button.
        /// </summary>
        private void Teleport(Filter entity)
        {
            if (entity.LRT_Comp.CanTeleport)
            {
                Vector3 newPos = entity.BAC_Comp.HitVar.Value.point;

                if (entity.LRT_Comp.AdjustHeight)
                {
                    newPos.y += entity.LRT_Comp.HeightAboveGround + VRSF_Components.CameraRig.transform.localScale.y;
                }
                else
                {
                    newPos.y = VRSF_Components.CameraRig.transform.position.y;
                }

                // If we use the boundaires, we check the newPos, if not, we set the position of the user directly
                VRSF_Components.CameraRig.transform.position = entity.LRT_Comp._UseBoundaries ? CheckNewPosWithBoundaries(entity, newPos) : newPos;
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
        #endregion
    }
}