using Unity.Entities;
using UnityEngine;
using VRSF.Core.FadingEffect;
using VRSF.Core.SetupVR;

namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// Used by The LongRangeTeleportSystem and the CurveTeleporter Systems to Teleport the user using a Fade effect if there's one
    /// </summary>
    public class TeleportUserSystem : ComponentSystem
    {
        private struct Filter
        {
            public SceneObjectsComponent SceneObjects;
            public TeleportGeneralComponent TeleportGeneral;
        }

        protected override void OnCreateManager()
        {
            OnTeleportUser.Listeners += TeleportUserCallback;
            OnFadingOutEndedEvent.Listeners += OnFadindOutEndedCallback;
            base.OnCreateManager();
            this.Enabled = false;
        }

        protected override void OnUpdate() { }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
            OnTeleportUser.Listeners -= TeleportUserCallback;
            OnFadingOutEndedEvent.Listeners -= OnFadindOutEndedCallback;
        }

        private void TeleportUserCallback(OnTeleportUser info)
        {
            // If we use a fade effect
            if (info.TeleportGeneral.IsUsingFadingEffect)
            {
                // the fade component is present And the fading effect is not in progress
                if (info.SceneObjects.FadeComponent != null && !info.SceneObjects.FadeComponent.FadingInProgress)
                {
                    SetTeleportState(ETeleportState.Teleporting, info.TeleportGeneral);
                    ChangeTeleportStatus(false);
                    new StartFadingOutEvent(true);
                }
                // If we use a fade effect and the fade component is NOT present
                else if (info.SceneObjects.FadeComponent == null)
                {
                    Debug.LogError("<b>[VRSF] :</b> You cannot use a fade effect if the CameraFadeComponent is not placed on the VRCamera (or as a child of the VRCamera) of your VR SDKs Prefabs.");
                    TeleportUser(info.TeleportGeneral);
                }
                else
                {
                    Debug.LogError("<b>[VRSF] :</b> Fading Effect already in progress, cannot start it for the Teleport System.");
                }
            }
            // If not, we teleport directly the user
            else
            {
                TeleportUser(info.TeleportGeneral);
            }
        }

        private void OnFadindOutEndedCallback(OnFadingOutEndedEvent info)
        {
            foreach (var e in GetEntities<Filter>())
            {
                if (e.TeleportGeneral.IsUsingFadingEffect && e.TeleportGeneral.CurrentTeleportState == ETeleportState.Teleporting)
                    TeleportUser(e.TeleportGeneral);
            }
        }

        private void TeleportUser(TeleportGeneralComponent teleportGeneral)
        {
            VRSF_Components.SetCameraRigPosition(TeleportGeneralComponent.PointToGoTo);
            SetTeleportState(ETeleportState.None, teleportGeneral);
            ChangeTeleportStatus(true);
        }
        
        public static void SetTeleportState(ETeleportState newState, TeleportGeneralComponent teleportGeneral)
        {
            // We set the teleporting state to the new state
            teleportGeneral.CurrentTeleportState = newState;
        }

        private void ChangeTeleportStatus(bool newStatus)
        {
            TeleportGeneralComponent.CanTeleport = newStatus;
        }
    }
}