using Unity.Entities;
using UnityEngine;
using VRSF.Utils;

namespace VRSF.MoveAround.Teleport
{
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
                        // We then Check if the user can teleport
                        HandleTeleportingState(e);
                    }
                    // If not, we teleport directly the user
                    else
                    {
                        e.TeleportGeneral.CurrentTeleportState = ETeleportState.None;
                        VRSF_Components.SetCameraRigPosition(e.TeleportGeneral.PointToGoTo + new Vector3(0.0f, -0.2f, 0.0f));
                    }
                }
            }
        }

        /// <summary>
        /// Called in Update when the user is teleporting.
        /// CHeck the Fading status and teleport the user when the Fading out is done.
        /// </summary>
        private void HandleTeleportingState(Filter e)
        {
            // If we are currently teleporting (ie handling the fade in/out transition)...
            // Wait until half of the teleport time has passed before the next event (note: both the switch from fade
            // out to fade in and the switch from fade in to stop the animation is half of the fade duration)
            if (Time.time - e.SceneObjects.FadeComponent._teleportTimeMarker >= e.SceneObjects.FadeComponent.TeleportFadeDuration / 2)
            {
                if (e.SceneObjects.FadeComponent._fadingIn)
                {
                    // We have finished fading in
                    e.TeleportGeneral.CurrentTeleportState = ETeleportState.None;
                    e.SceneObjects.FadeComponent.TeleportState = ETeleportState.None;
                }
                else
                {
                    // We have finished fading out - time to teleport!
                    VRSF_Components.SetCameraRigPosition(e.TeleportGeneral.PointToGoTo);
                }

                e.SceneObjects.FadeComponent._teleportTimeMarker = Time.time;
                e.SceneObjects.FadeComponent._fadingIn = !e.SceneObjects.FadeComponent._fadingIn;
            }
        }
    }
}