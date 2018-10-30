using UnityEngine;
using VRSF.Inputs;
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
    public class CurveTeleporterUpdateSystem : BACUpdateSystem<CurveTeleporterUpdateSystem>
    {
        private new struct Filter
        {
            public BACGeneralComponent BACGeneral;
            public BACCalculationsComponent BACCalculations;

            public SceneObjectsComponent SceneObjects;
            public TeleportCalculationsComponent TeleportCalculations;
            public TeleportNavMeshComponent NavMeshComp;
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
                e.BACGeneral.OnButtonIsClicking.AddListener(delegate { OnIsInteractingCallback(e); });
                e.BACGeneral.OnButtonStopClicking.AddListener(delegate { OnStopInteractingCallback(e); });
            }

            if ((e.BACGeneral.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                e.BACGeneral.OnButtonStartTouching.AddListener(delegate { OnStartInteractingCallback(e); });
                e.BACGeneral.OnButtonIsTouching.AddListener(delegate { OnIsInteractingCallback(e); });
                e.BACGeneral.OnButtonStopTouching.AddListener(delegate { OnStopInteractingCallback(e); });
            }
        }

        public override void RemoveListenersOnEndApp(object entity)
        {
            var e = (Filter)entity;
            if ((e.BACGeneral.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                e.BACGeneral.OnButtonStartClicking.RemoveAllListeners();
                e.BACGeneral.OnButtonIsClicking.RemoveAllListeners();
                e.BACGeneral.OnButtonStopClicking.RemoveAllListeners();
            }

            if ((e.BACGeneral.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                e.BACGeneral.OnButtonStartTouching.RemoveAllListeners();
                e.BACGeneral.OnButtonIsTouching.RemoveAllListeners();
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
            // enable the parabolic pointer and visual indicators that the user can use to determine where they are able to teleport.
            e.SceneObjects.Pointer.enabled = true;

            e.TeleportCalculations.CurrentTeleportState = ETeleportState.Selecting;

            if (e.NavMeshComp._navmeshAnimator != null)
                e.NavMeshComp._navmeshAnimator.SetBool(e.NavMeshComp._enabledAnimatorID, true);

            e.SceneObjects.Pointer.ForceUpdateCurrentAngle();
            e.TeleportCalculations._lastClickAngle = e.SceneObjects.Pointer.CurrentPointVector;
            e.TeleportCalculations.IsClicking = e.SceneObjects.Pointer.PointOnNavMesh;
        }

        /// <summary>
        /// Calculate the Room Border Positions when user is clicking on the Teleport Button
        /// </summary>
        /// <param name="e"></param>
        private void OnIsInteractingCallback(Filter e)
        {
            // The user is still deciding where to teleport and has the touchpad held down.
            // Note: rendering of the parabolic pointer / marker is done in ParabolicPointer
            Vector3 offset = e.SceneObjects._headTransform.position - e.SceneObjects._originTransform.position;
            offset.y = 0;

            // Render representation of where the chaperone bounds will be after teleporting
            e.SceneObjects._roomBorder.enabled = e.SceneObjects.Pointer.PointOnNavMesh;
            e.SceneObjects._roomBorder.Transpose = Matrix4x4.TRS(e.SceneObjects.Pointer.SelectedPoint - offset, Quaternion.identity, Vector3.one);
        }

        private void OnStopInteractingCallback(Filter e)
        {
            // If the user has decided to teleport (ie lets go of touchpad) then remove all visual indicators
            // related to selecting things and actually teleport
            if (e.SceneObjects.Pointer.PointOnNavMesh)
            {
                // Begin teleport sequence
                e.TeleportCalculations.CurrentTeleportState = ETeleportState.Teleporting;
                e.TeleportCalculations._teleportTimeMarker = Time.time;
            }
            else
            {
                e.TeleportCalculations.CurrentTeleportState = ETeleportState.None;
            }

            // Reset active controller, disable pointer, disable visual indicators
            e.SceneObjects.Pointer.enabled = false;
            e.SceneObjects._roomBorder.enabled = false;
            //RoomBorder.Transpose = Matrix4x4.TRS(OriginTransform.position, Quaternion.identity, Vector3.one);
            if (e.NavMeshComp._navmeshAnimator != null)
                e.NavMeshComp._navmeshAnimator.SetBool(e.NavMeshComp._enabledAnimatorID, false);
        }
    }
}