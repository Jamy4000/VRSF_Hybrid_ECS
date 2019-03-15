using UnityEngine;
using VRSF.Core.Inputs;
using VRSF.Core.SetupVR;
using VRSF.Utils.ButtonActionChoser;

namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// Used to display a Slider and Text on the User's controller when loading the LongRangeTeleport feature
    /// </summary>
    public class LongRangeUISystem : BACListenersSetupSystem
    {
        struct Filter
        {
            public LongRangeTeleportComponent LRT_Comp;
            public BACGeneralComponent BAC_Comp;
            public TeleportGeneralComponent TeleportGeneral;
        }

        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnStartRunning()
        {
            OnSetupVRReady.Listeners += Init;
            base.OnStartRunning();
        }
        
        protected override void OnDestroyManager()
        {
            OnSetupVRReady.Listeners -= Init;
            base.OnDestroyManager();

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
        
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Method to call from the StartTouching or StartClicking Method, set the Loading Slider values if used.
        /// </summary>
        private void OnStartInteractingCallback(Filter entity)
        {
            // If we use the loading slider, we set the fillRect value and the TeleportText value
            if (entity.LRT_Comp.FillRect != null)
            {
                entity.LRT_Comp.FillRect.gameObject.SetActive(true);
                entity.LRT_Comp.FillRect.fillAmount = 0.0f;
            }

            // If we use the teleport text, we set the text to Preparing
            if (entity.LRT_Comp.TeleportText != null)
            {
                entity.LRT_Comp.TeleportText.gameObject.SetActive(true);
                entity.LRT_Comp.TeleportText.text = "Preparing Teleport ...";
            }
        }

        /// <summary>
        /// To call from the IsClicking or IsTouching event
        /// </summary>
        private void OnIsInteractingCallback(Filter e)
        {
            Color32 fillRectColor = TeleportGeneralComponent.CanTeleport ? new Color32(100, 255, 100, 255) : new Color32(0, 180, 255, 255);

            // If we use a loading slider and the fillRect to give the user a visual feedback is not null
            if (e.LRT_Comp.UseLoadingTimer && e.LRT_Comp.FillRect != null)
            {
                e.LRT_Comp.FillRect.color = fillRectColor;

                // If the loading slider is still not full
                if (e.LRT_Comp.FillRect.fillAmount < 1.0f)
                    e.LRT_Comp.FillRect.fillAmount += Time.deltaTime / e.LRT_Comp.LoadingTime;
            }

            // If we use a loading slider and the Text to give the user a visual feedback is not null
            if (e.LRT_Comp.UseLoadingTimer && e.LRT_Comp.TeleportText != null)
            {
                // If we use a text to give a feedback to the user
                e.LRT_Comp.TeleportText.color = fillRectColor;
                e.LRT_Comp.TeleportText.text = TeleportGeneralComponent.CanTeleport ? "Release To Teleport !" : "Waiting for ground ...";
            }
        }


        /// <summary>
        /// If used it, we deactivate the Teleport Slider and Text when the user release the button.
        /// </summary>
        private void OnStopInteractingCallback(Filter e)
        {
            e.LRT_Comp.FillRect?.gameObject.SetActive(false);
            e.LRT_Comp.TeleportText?.gameObject.SetActive(false);
        }


        /// <summary>
        /// Reactivate the System when switching to another Scene.
        /// </summary>
        /// <param name="oldScene">The previous scene before switching</param>
        private void Init(OnSetupVRReady setupVRReady)
        {
            bool isUsingSystem = false;

            foreach (var e in GetEntities<Filter>())
            {
                if (e.LRT_Comp.UseLoadingTimer && (e.LRT_Comp.TeleportText != null || e.LRT_Comp.FillRect != null))
                {
                    isUsingSystem = true;
                    SetupListenersResponses(e);
                    OnStopInteractingCallback(e);
                }
            }

            this.Enabled = isUsingSystem;
        }
        #endregion
    }
}