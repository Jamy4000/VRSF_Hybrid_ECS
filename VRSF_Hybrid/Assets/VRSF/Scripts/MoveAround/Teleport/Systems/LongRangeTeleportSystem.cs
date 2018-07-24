using UnityEngine;
using VRSF.Inputs;
using VRSF.MoveAround.Teleport.Components;
using VRSF.MoveAround.Teleport.Interfaces;
using VRSF.Utils;
using VRSF.Utils.Components;
using VRSF.Utils.Components.ButtonActionChoser;
using VRSF.Utils.Systems.ButtonActionChoser;

namespace VRSF.MoveAround.Teleport.Systems
{
    public class LongRangeTeleportSystem : BACUpdateSystem, ITeleportSystem
    {
        struct Filter : ITeleportFilter
        {
            public LongRangeTeleportComponent LRT_Comp;
            public ButtonActionChoserComponents BAC_Comp;
            public ScriptableRaycastComponent RaycastComp;
            public TeleportBoundariesComponent TeleportBoundaries;
            public TeleportGeneralComponent GeneralTeleport;
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
                e.GeneralTeleport.TeleportLayer = LayerMask.NameToLayer("Teleport");

                if (e.GeneralTeleport.TeleportLayer == -1)
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
        public void CheckNewPosWithBoundaries(ITeleportFilter teleportFilter, ref Vector3 posToCheck)
        {
            Filter entity = (Filter)teleportFilter;

            Vector3 minPos = entity.TeleportBoundaries._MinUserPosition;
            Vector3 maxPos = entity.TeleportBoundaries._MaxUserPosition;

            posToCheck.x = Mathf.Clamp(posToCheck.x, minPos.x, maxPos.x);
            posToCheck.y = Mathf.Clamp(posToCheck.y, minPos.y, maxPos.y);
            posToCheck.z = Mathf.Clamp(posToCheck.z, minPos.z, maxPos.z);
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
                if (entity.LRT_Comp.FillRect != null && entity.LRT_Comp.FillRect.fillAmount < entity.LRT_Comp.TimerBeforeTeleport)
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
        }


        /// <summary>
        /// Handle the Teleport when the user is releasing the button.
        /// </summary>
        private void Teleport(Filter entity)
        {
            if (entity.GeneralTeleport.CanTeleport && entity.LRT_Comp.FillRect.fillAmount >= entity.LRT_Comp.TimerBeforeTeleport)
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
                if (entity.TeleportBoundaries._UseBoundaries)
                {
                    CheckNewPosWithBoundaries(entity, ref newPos);
                }

                VRSF_Components.CameraRig.transform.position = newPos;
            }
        }


        /// <summary>
        /// Check if the Teleport ray is on a Teleport Layer, and set the _canTeleport bool and the color of the Loading Slider accordingly.
        /// </summary>
        private void CheckTeleport(Filter entity)
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