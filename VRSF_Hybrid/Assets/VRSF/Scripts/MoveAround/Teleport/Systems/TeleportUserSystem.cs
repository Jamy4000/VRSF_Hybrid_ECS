using System;
using Unity.Entities;
using UnityEngine;
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
            base.OnCreateManager();
            OnTeleportUser.Listeners += TeleportUserCallback;
        }

        protected override void OnUpdate()
        {
            foreach (var e in GetEntities<Filter>())
            {
                if (e.TeleportGeneral.CurrentTeleportState == ETeleportState.Teleporting && TeleportGeneralComponent.FadingInProgress)
                {
                    HandleTeleportingStateAndAlpha(e);
                }
            }
        }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
            OnTeleportUser.Listeners -= TeleportUserCallback;
        }

        /// <summary>
        /// Change the alpha of the fading canvas and set the current teleporting state if the fade in/out is done
        /// </summary>
        private void HandleTeleportingStateAndAlpha(Filter e)
        {
            var alpha = e.SceneObjects.FadeComponent._FadingImage.color.a;
            // If we are in a fadingOut state
            if (!e.SceneObjects.FadeComponent._IsFadingIn)
            {
                // if the alpha is still not set at one
                if (alpha < 1)
                {
                    alpha += Time.deltaTime * e.SceneObjects.FadeComponent.TeleportFadeSpeed;
                }
                // If we are in a fadingOut state and the screen is already black, we teleport the user
                else
                {
                    e.SceneObjects.FadeComponent._IsFadingIn = true;
                    // We have finished fading out - time to teleport!
                    VRSF_Components.SetCameraRigPosition(TeleportGeneralComponent.PointToGoTo);
                }
            }
            // If we are in a fadingIn state and the screen is still black
            else if (e.SceneObjects.FadeComponent._IsFadingIn && alpha > 0)
            {
                alpha -= Time.deltaTime * e.SceneObjects.FadeComponent.TeleportFadeSpeed;
            }
            // If the fadingIn is finished
            else
            {
                SetTeleportState(ETeleportState.None, e.TeleportGeneral);
                e.SceneObjects.FadeComponent._IsFadingIn = false;
                ChangeTeleportStatus(true);
            }

            // We set the new alpha of the black image
            e.SceneObjects.FadeComponent._FadingImage.color = new Color(0, 0, 0, alpha);
        }

        private void ChangeTeleportStatus(bool newStatus)
        {
            TeleportGeneralComponent.CanTeleport = newStatus;
            TeleportGeneralComponent.FadingInProgress = !newStatus;
        }


        private void TeleportUserCallback(OnTeleportUser teleportUser)
        {
            // If we use a fade effect
            if (teleportUser.TeleportGeneral.IsUsingFadingEffect)
            {
                // the fade component is present And the fading out is in progress
                if (teleportUser.SceneObjects.FadeComponent != null && !TeleportGeneralComponent.FadingInProgress)
                {
                    SetTeleportState(ETeleportState.Teleporting, teleportUser.TeleportGeneral);
                    ChangeTeleportStatus(false);
                }
                // If we use a fade effect and the fade component is NOT present
                else if (teleportUser.SceneObjects.FadeComponent == null)
                {
                    Debug.LogError("<b>[VRSF] :</b> You cannot use a fade effect if the FadeComponent is not placed on the VRCamera of your VR SDKs Prefabs.");
                }
            }
            // If not, we teleport directly the user
            else
            {
                SetTeleportState(ETeleportState.None, teleportUser.TeleportGeneral);
                VRSF_Components.SetCameraRigPosition(TeleportGeneralComponent.PointToGoTo);
                ChangeTeleportStatus(true);
            }
        }


        public static void SetTeleportState(ETeleportState newState, TeleportGeneralComponent teleportGeneral)
        {
            // We set the teleporting state to the new state
            teleportGeneral.CurrentTeleportState = newState;
        }
    }
}