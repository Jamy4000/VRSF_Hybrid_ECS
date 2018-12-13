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
    public class CurveTeleporterUpdateSystem : BACListenersSetupSystem, ITeleportSystem
    {
        private struct Filter : ITeleportFilter
        {
            public BACGeneralComponent BACGeneral;
            public SceneObjectsComponent SceneObjects;
            public TeleportGeneralComponent TeleportGeneral;
            public ParabolCalculationsComponent PointerCalculations;
        }


        #region ComponentSystem_Methods
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
                RemoveListeners(e);
            }
        }
        #endregion ComponentSystem_Methods


        #region PUBLIC_METHODS

        #region Listeners_Setup
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

        public override void RemoveListeners(object entity)
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
        #endregion Listeners_Setup


        #region Teleport_Interface
        /// <summary>
        /// Method to call from StartClicking or StartTouching, teleport the user one meter away.
        /// </summary>
        public void TeleportUser(ITeleportFilter teleportFilter)
        {
            var e = (Filter)teleportFilter;

            // If the user has decided to teleport (ie lets go of touchpad) then remove all visual indicators
            // related to selecting things and actually teleport
            if (e.PointerCalculations.PointOnNavMesh)
                TeleportUserSystem.SetTeleportState(e.TeleportGeneral, e.SceneObjects, ETeleportState.Teleporting);
            else
                TeleportUserSystem.SetTeleportState(e.TeleportGeneral, e.SceneObjects, ETeleportState.None);
        }
        #endregion

        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// This is the callback for when the user start clicking on the button. We check if the timer is ok if we use one.
        /// </summary>
        /// <param name="e"></param>
        private void OnStartInteractingCallback(Filter e)
        {
            // We reset the current State to Selecting
            TeleportUserSystem.SetTeleportState(e.TeleportGeneral, e.SceneObjects, ETeleportState.Selecting);
        }

        private void OnStopInteractingCallback(Filter e)
        {
            TeleportUser(e);
        }
        #endregion
    }
}