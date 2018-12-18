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
                    // If we use a fade effect
                    if (e.SceneObjects.FadeComponent != null)
                    {
                        HandleTeleportingStateAndAlpha(e);
                    }
                    // If not, we teleport directly the user
                    else
                    {
                        e.TeleportGeneral.CurrentTeleportState = ETeleportState.None;
                        VRSF_Components.SetCameraRigPosition(e.TeleportGeneral.PointToGoTo);
                    }
                }
            }
        }

        private void HandleTeleportingStateAndAlpha(Filter e)
        {
            var alpha = e.SceneObjects.FadeComponent._FadingImage.color.a;

            // If we are in a fadingOut state and the alpha is still not set at one
            if (!e.SceneObjects.FadeComponent._IsFadingIn && alpha < 1)
            {
                alpha += Time.deltaTime * e.SceneObjects.FadeComponent.TeleportFadeSpeed;
            }
            // If we are in a fadingOut state and the screen is already black, we teleport the user
            else if (!e.SceneObjects.FadeComponent._IsFadingIn && alpha >= 1)
            {
                e.SceneObjects.FadeComponent._IsFadingIn = true;
                // We have finished fading out - time to teleport!
                VRSF_Components.SetCameraRigPosition(e.TeleportGeneral.PointToGoTo);
            }
            // If we are in a fadingIn state and the screen is still black
            else if (e.SceneObjects.FadeComponent._IsFadingIn && alpha > 0)
            {
                alpha -= Time.deltaTime * e.SceneObjects.FadeComponent.TeleportFadeSpeed;
            }
            // If the fadingIn is finished
            else
            {
                e.TeleportGeneral.CurrentTeleportState = ETeleportState.None;
                e.SceneObjects.FadeComponent._IsFadingIn = false;
            }

            // We set the new alpha of the black image
            e.SceneObjects.FadeComponent._FadingImage.color = new Color(0, 0, 0, alpha);
        }

        public static void SetTeleportState(TeleportGeneralComponent teleportGeneral, SceneObjectsComponent sceneObjects, ETeleportState newState)
        {
            // We set the teleporting state to teleporting
            teleportGeneral.CurrentTeleportState = newState;
        }
    }
}