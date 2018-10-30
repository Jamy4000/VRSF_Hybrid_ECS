using Unity.Entities;
using UnityEngine;

namespace VRSF.MoveAround.Teleport
{
    public class TeleportUserSystem : ComponentSystem
    {
        private struct Filter
        {
            public TeleportFadeComponent FadeComp;
            public TeleportCalculationsComponent TeleportCalculations;
            public SceneObjectsComponent SceneObjects;
        }
        
        protected override void OnUpdate()
        {
            foreach (var e in GetEntities<Filter>())
            {
                if (e.TeleportCalculations.CurrentTeleportState == ETeleportState.Teleporting)
                {
                    HandleTeleportingState(e);
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
            if (Time.time - e.TeleportCalculations._teleportTimeMarker >= e.FadeComp.TeleportFadeDuration / 2)
            {
                if (e.FadeComp._fadingIn)
                {
                    // We have finished fading in
                    e.TeleportCalculations.CurrentTeleportState = ETeleportState.None;
                }
                else
                {
                    // We have finished fading out - time to teleport!
                    Vector3 offset = e.SceneObjects._originTransform.position - e.SceneObjects._headTransform.position;
                    offset.y = 0;
                    e.SceneObjects._originTransform.position = e.SceneObjects.Pointer.SelectedPoint + offset;
                }

                e.TeleportCalculations._teleportTimeMarker = Time.time;
                e.FadeComp._fadingIn = !e.FadeComp._fadingIn;
            }
        }
    }
}