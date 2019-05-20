using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.Inputs;
using VRSF.Core.SetupVR;
using VRSF.Core.Utils;
using VRSF.Core.Utils.ButtonActionChoser;

namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// A generic component that renders a border using the given polylines.  
    /// The borders are double sided and are oriented upwards (ie normals are parallel to the XZ plane)
    /// 
    /// Disclaimer : This script is based on the Flafla2 Vive-Teleporter Repository. You can check it out here :
    /// https://github.com/Flafla2/Vive-Teleporter
    /// </summary>
    public class ParabolicPointerUpdateSystem : BACListenersSetupSystem
    {
        public struct Filter
        {
            public BACGeneralComponent BAC_Comp;
            public ParabolObjectsComponent PointerObjects;
            public ParabolCalculationsComponent PointerCalculations;
            public SceneObjectsComponent SceneObjects;
        }

        protected override void OnCreateManager()
        {
            base.OnCreateManager();
            ParabolicRendererHelper.ControllersParameters = ControllersParametersVariable.Instance;
            OnSetupVRReady.Listeners += Init;
        }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
            foreach (var e in GetEntities<Filter>())
            {
                RemoveListeners(e);
            }
            OnSetupVRReady.Listeners -= Init;
        }

        public override void SetupListenersResponses(object entity)
        {
            var e = (Filter)entity;

            if (e.PointerObjects.StartInteractingAction == null && e.PointerObjects.StopInteractingAction == null && e.PointerObjects.IsInteractingAction == null)
            {
                e.PointerObjects.StartInteractingAction = delegate { OnStartInteractingCallback(e); };
                e.PointerObjects.StopInteractingAction = delegate { OnIsInteractingCallback(e); };
                e.PointerObjects.IsInteractingAction = delegate { OnStopInteractingCallback(e); };

                if ((e.BAC_Comp.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
                {
                    e.BAC_Comp.OnButtonStartClicking.AddListenerExtend(e.PointerObjects.StartInteractingAction);
                    e.BAC_Comp.OnButtonIsClicking.AddListenerExtend(e.PointerObjects.StopInteractingAction);
                    e.BAC_Comp.OnButtonStopClicking.AddListenerExtend(e.PointerObjects.IsInteractingAction);
                }

                if ((e.BAC_Comp.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
                {
                    e.BAC_Comp.OnButtonStartTouching.AddListenerExtend(e.PointerObjects.StartInteractingAction);
                    e.BAC_Comp.OnButtonIsTouching.AddListenerExtend(e.PointerObjects.StopInteractingAction);
                    e.BAC_Comp.OnButtonStopTouching.AddListenerExtend(e.PointerObjects.IsInteractingAction);
                }
            }
        }

        public override void RemoveListeners(object entity)
        {
            var e = (Filter)entity;
            if ((e.BAC_Comp.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                e.BAC_Comp.OnButtonStartClicking.RemoveListenerExtend(e.PointerObjects.StartInteractingAction);
                e.BAC_Comp.OnButtonIsClicking.RemoveListenerExtend(e.PointerObjects.StopInteractingAction);
                e.BAC_Comp.OnButtonStopClicking.RemoveListenerExtend(e.PointerObjects.IsInteractingAction);
            }

            if ((e.BAC_Comp.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                e.BAC_Comp.OnButtonStartTouching.RemoveListenerExtend(e.PointerObjects.StartInteractingAction);
                e.BAC_Comp.OnButtonIsTouching.RemoveListenerExtend(e.PointerObjects.StopInteractingAction);
                e.BAC_Comp.OnButtonStopTouching.RemoveListenerExtend(e.PointerObjects.IsInteractingAction);
            }
        }

        #region BAC_Callbacks
        /// <summary>
        /// Deactivate the laser and Activate the Parabole when the user start to click
        /// </summary>
        /// <param name="e"></param>
        private void OnStartInteractingCallback(Filter e)
        {
            if (e.BAC_Comp != null && TeleportGeneralComponent.CanTeleport)
            {
                ParabolicRendererHelper.ToggleHandLaser(e, false);
                ParabolicRendererHelper.ForceUpdateCurrentAngle(e);
            }
        }

        /// <summary>
        /// Update the Parabole mesh when user click on the button
        /// </summary>
        /// <param name="e"></param>
        private void OnIsInteractingCallback(Filter e)
        {
            if (e.BAC_Comp != null && TeleportGeneralComponent.CanTeleport)
            {
                // Deactivate laser if it's still active
                if (e.PointerObjects._ControllerPointer.enabled)
                    ParabolicRendererHelper.ToggleHandLaser(e, false);

                // 1. Calculate Parabola Points
                var velocity = ParabolicRendererHelper.ForceUpdateCurrentAngle(e);
                var normal = ParabolicRendererHelper.ParabolaPointsCalculations(e, velocity);

                // 2. Render the Parabole's pads, aka the targets at the end of the parabole
                ParabolicRendererHelper.RenderParabolePads(e, normal);

                // 3. Draw parabola (BEFORE the outside faces of the selection pad, to avoid depth issues)
                ParaboleCalculationsHelper.GenerateMesh(ref e.PointerObjects._parabolaMesh, e.PointerObjects.ParabolaPoints, velocity, Time.time % 1, e.PointerCalculations.GraphicThickness);
                Graphics.DrawMesh(e.PointerObjects._parabolaMesh, Matrix4x4.identity, e.PointerCalculations.GraphicMaterial, e.PointerObjects.gameObject.layer);
            }
        }

        /// <summary>
        /// Reactivate the laser and Deactivate the pads when realising the button 
        /// </summary>
        /// <param name="e"></param>
        private void OnStopInteractingCallback(Filter e)
        {
            if (e.BAC_Comp != null)
            {
                ParabolicRendererHelper.ToggleHandLaser(e, true);
                e.PointerObjects._selectionPadObject?.SetActive(false);
                e.PointerObjects._invalidPadObject?.SetActive(false);
            }
        }
        #endregion


        #region PRIVATE_METHODS
        private void Init(OnSetupVRReady info)
        {
            foreach (var e in GetEntities<Filter>())
            {
                SetupListenersResponses(e);
            }
        }
        #endregion PRIVATE_METHODS
    }
}