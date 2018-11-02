using UnityEngine;
using VRSF.Inputs;
using VRSF.MoveAround.Teleport.Interfaces;
using VRSF.Utils.Components.ButtonActionChoser;
using VRSF.Utils.Systems.ButtonActionChoser;

namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// A generic component that renders a border using the given polylines.  
    /// The borders are double sided and are oriented upwards (ie normals are parallel to the XZ plane)
    /// 
    /// Disclaimer : This script is based on the Flafla2 Vive-Teleporter Repository. You can check it out here :
    /// https://github.com/Flafla2/Vive-Teleporter
    /// </summary>
    public class CurveTeleporterUpdateSystem : BACUpdateSystem<ParabolCalculationsComponent>, ITeleportSystem
    {
        private new struct Filter : ITeleportFilter
        {
            public BACGeneralComponent BACGeneral;
            public SceneObjectsComponent SceneObjects;
            public TeleportGeneralComponent TeleportGeneral;
            public ParaboleAnimatorComponent NavMeshAnim;
            public ParabolCalculationsComponent PointerCalculations;
        }

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            foreach (var e in GetEntities<Filter>())
            {
                SetupListenersResponses(e);
            }
        }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
            foreach (var e in GetEntities<Filter>())
            {
                RemoveListenersOnEndApp(e);
            }
        }

        public override void SetupListenersResponses(object entity)
        {
            var e = (Filter)entity;
            if ((e.BACGeneral.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                e.BACGeneral.OnButtonStartClicking.AddListener(delegate { OnStartInteractingCallback(e); });
                e.BACGeneral.OnButtonStopClicking.AddListener(delegate { OnStopInteractingCallback(e); });
            }

            if ((e.BACGeneral.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                e.BACGeneral.OnButtonStartTouching.AddListener(delegate { OnStartInteractingCallback(e); });
                e.BACGeneral.OnButtonStopTouching.AddListener(delegate { OnStopInteractingCallback(e); });
            }
        }

        public override void RemoveListenersOnEndApp(object entity)
        {
            var e = (Filter)entity;
            if ((e.BACGeneral.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                e.BACGeneral.OnButtonStartClicking.RemoveAllListeners();
                e.BACGeneral.OnButtonStopClicking.RemoveAllListeners();
            }

            if ((e.BACGeneral.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                e.BACGeneral.OnButtonStartTouching.RemoveAllListeners();
                e.BACGeneral.OnButtonStopTouching.RemoveAllListeners();
            }
        }

        /// <summary>
        /// At this point the user is not holding down on the touchpad at all and hasn't
        /// let go of the touchpad.  So we wait for the user to press the touchpad and enable visual indicators
        /// if necessary.
        /// </summary>
        /// <param name="e"></param>
        private void OnStartInteractingCallback(Filter e)
        {
            if (e.SceneObjects.FadeComponent == null || e.TeleportGeneral.CurrentTeleportState != ETeleportState.Teleporting)
            {
                e.TeleportGeneral.CurrentTeleportState = ETeleportState.Selecting;

                if (e.SceneObjects.FadeComponent != null)
                {
                    e.SceneObjects.FadeComponent.TeleportState = ETeleportState.Selecting;
                }

                if (e.NavMeshAnim._NavmeshAnimator != null)
                    e.NavMeshAnim._NavmeshAnimator.SetBool(e.NavMeshAnim._EnabledAnimatorID, true);
            }
        }

        private void OnStopInteractingCallback(Filter e)
        {
            if (e.SceneObjects.FadeComponent == null || e.TeleportGeneral.CurrentTeleportState != ETeleportState.Teleporting)
            {
                // If the user has decided to teleport (ie lets go of touchpad) then remove all visual indicators
                // related to selecting things and actually teleport
                if (e.PointerCalculations.PointOnNavMesh)
                {
                    TeleportUser(e);
                }
                else
                {
                    e.TeleportGeneral.CurrentTeleportState = ETeleportState.None;

                    // If we use a fade effect, we set the info necessary to use this effect.
                    if (e.SceneObjects.FadeComponent != null)
                    {
                        e.SceneObjects.FadeComponent.TeleportState = ETeleportState.None;
                    }
                }

                if (e.NavMeshAnim._NavmeshAnimator != null)
                    e.NavMeshAnim._NavmeshAnimator.SetBool(e.NavMeshAnim._EnabledAnimatorID, false);
            }
        }

        public void TeleportUser(ITeleportFilter teleportFilter)
        {
            var e = (Filter)teleportFilter;

            // Begin teleport sequence
            e.TeleportGeneral.CurrentTeleportState = ETeleportState.Teleporting;

            // If we use a fade effect, we set the info necessary to use this effect.
            if (e.SceneObjects.FadeComponent != null)
            {
                e.SceneObjects.FadeComponent.TeleportState = ETeleportState.Teleporting;
                e.SceneObjects.FadeComponent._teleportTimeMarker = Time.time;
            }
        }
    }
}