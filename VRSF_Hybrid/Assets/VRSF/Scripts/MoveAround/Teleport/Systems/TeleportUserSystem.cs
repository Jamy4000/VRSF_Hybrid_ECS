using Unity.Entities;
using UnityEngine;
using VRSF.Utils;

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

        protected override void OnUpdate()
        {
            foreach (var e in GetEntities<Filter>())
            {
                if (e.TeleportGeneral.CurrentTeleportState == ETeleportState.Teleporting)
                {
                    if (TeleportGeneralComponent.CanTeleport)
                    {
                        // If we use a fade effect
                        if (e.TeleportGeneral.IsUsingFadingEffect)
                        {
                            // the fade component is present And the fading out is in progress
                            if (e.SceneObjects.FadeComponent != null && !TeleportGeneralComponent.FadingInProgress)
                                ChangeTeleportStatus(false);
                            // If we use a fade effect and the fade component is NOT present
                            else if (e.SceneObjects.FadeComponent == null)
                                Debug.LogError("VRSF : You cannot use a fade effect if the FadeComponent is not placed on the VRCamera of your VR SDKs Prefabs.");
                        }
                        // If not, we teleport directly the user
                        else
                        {
                            SetTeleportState(ETeleportState.None, e.TeleportGeneral);
                            VRSF_Components.SetCameraRigPosition(TeleportGeneralComponent.PointToGoTo);
                            ChangeTeleportStatus(true);
                        }
                    }
                    else if (TeleportGeneralComponent.FadingInProgress)
                    {
                        HandleTeleportingStateAndAlpha(e);
                    }
                }
            }
        }

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

        public static void SetTeleportState(ETeleportState newState, TeleportGeneralComponent teleportGeneral)
        {
            // We set the teleporting state to the new state
            teleportGeneral.CurrentTeleportState = newState;
        }

        private void ChangeTeleportStatus(bool newStatus)
        {
            TeleportGeneralComponent.CanTeleport = newStatus;
            TeleportGeneralComponent.FadingInProgress = !newStatus;
        }
    }
}