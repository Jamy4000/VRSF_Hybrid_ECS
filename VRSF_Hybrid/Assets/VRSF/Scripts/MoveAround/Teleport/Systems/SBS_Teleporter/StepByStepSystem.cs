using UnityEngine;
using VRSF.Core.Inputs;
using VRSF.Core.SetupVR;
using VRSF.Core.Utils.ButtonActionChoser;
using VRSF.Core.Utils;

namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// Using the ButtonActionChoser, this System allow the user to move Step by Step, ie in the direction of its laser to which this feature is linked.
    /// </summary>
    public class StepByStepSystem : BACListenersSetupSystem, ITeleportSystem
    {
        public struct Filter : ITeleportFilter
        {
            public StepByStepComponent SBS_Comp;
            public BACGeneralComponent BAC_Comp;
            public SceneObjectsComponent SceneObjects;
            public TeleportGeneralComponent TeleportGeneral;
        }

        #region ComponentSystem_Methods

        protected override void OnCreateManager()
        {
            base.OnCreateManager();
            OnSetupVRReady.Listeners += Init;
        }

        protected override void OnUpdate() { }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
            foreach (var e in GetEntities<Filter>())
            {
                RemoveListeners(e);
            }
            OnSetupVRReady.Listeners -= Init;
        }
        #endregion ComponentSystem_Methods


        #region PUBLIC_METHODS

        #region Listeners_Setup
        public override void SetupListenersResponses(object entity)
        {
            var e = (Filter)entity;
            if (e.TeleportGeneral.StopInteractingAction == null)
            {
                e.TeleportGeneral.StopInteractingAction = delegate { OnStopInteractingCallback(e); };

                if ((e.BAC_Comp.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
                {
                    e.BAC_Comp.OnButtonStopClicking.AddListenerExtend(e.TeleportGeneral.StopInteractingAction);
                }

                if ((e.BAC_Comp.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
                {
                    e.BAC_Comp.OnButtonStopTouching.AddListenerExtend(e.TeleportGeneral.StopInteractingAction);
                }
            }
        }

        public override void RemoveListeners(object entity)
        {
            var e = (Filter)entity;
            if ((e.BAC_Comp.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                e.BAC_Comp.OnButtonStopClicking.RemoveListenerExtend(e.TeleportGeneral.StopInteractingAction);
            }

            if ((e.BAC_Comp.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                e.BAC_Comp.OnButtonStopTouching.RemoveListenerExtend(e.TeleportGeneral.StopInteractingAction);
            }
        }
        #endregion Listeners_Setup


        #region Teleport_Interface
        /// <summary>
        /// Method to call from StopTouching, teleport the user in the direction of its controller.
        /// </summary>
        public void TeleportUser(ITeleportFilter teleportFilter)
        {
            Filter e = (Filter)teleportFilter;
            
            if (SBSCalculationsHelper.UserIsOnNavMesh(e, out Vector3 newUsersPos, e.TeleportGeneral.ExcludedLayers))
                VRSF_Components.SetCameraRigPosition(newUsersPos, false);

            e.TeleportGeneral.CurrentTeleportState = ETeleportState.None;
        }
        #endregion

        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Callback for when the user stop to interact with the button link to this feature.
        /// Take account of the BAC Timer if the component is attached to this Entity.
        /// </summary>
        /// <param name="e"></param>
        private void OnStopInteractingCallback(Filter e)
        {
            if (e.BAC_Comp != null && !e.TeleportGeneral.RaycastHitVar.RaycastHitIsOnUI())
                TeleportUser(e);
        }

        /// <summary>
        /// Reactivate the System when we instantiate a new CameraRig
        /// </summary>
        private void Init(OnSetupVRReady info)
        {
            foreach (var e in GetEntities<Filter>())
            {
                SetupListenersResponses(e);
            }
        }
        #endregion
    }
}