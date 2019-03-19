using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.Inputs;
using VRSF.Core.SetupVR;
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

            OnSetupVRReady.Listeners -= Init;
            foreach (var e in GetEntities<Filter>())
            {
                RemoveListeners(e);
            }
        }

        public override void SetupListenersResponses(object entity)
        {
            var e = (Filter)entity;
            if ((e.BAC_Comp.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                e.BAC_Comp.OnButtonStartClicking.AddListener(delegate { OnStartInteractingCallback(e); });
                e.BAC_Comp.OnButtonIsClicking.AddListener(delegate { OnIsInteractingCallback(e); });
                e.BAC_Comp.OnButtonStopClicking.AddListener(delegate { OnStopInteractingCallback(e); });
            }

            if ((e.BAC_Comp.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                e.BAC_Comp.OnButtonStartTouching.AddListener(delegate { OnStartInteractingCallback(e); });
                e.BAC_Comp.OnButtonIsTouching.AddListener(delegate { OnIsInteractingCallback(e); });
                e.BAC_Comp.OnButtonStopTouching.AddListener(delegate { OnStopInteractingCallback(e); });
            }
        }

        public override void RemoveListeners(object entity)
        {
            var e = (Filter)entity;
            if ((e.BAC_Comp.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                e.BAC_Comp.OnButtonStartClicking.RemoveAllListeners();
                e.BAC_Comp.OnButtonIsClicking.RemoveAllListeners();
                e.BAC_Comp.OnButtonStopClicking.RemoveAllListeners();
            }

            if ((e.BAC_Comp.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                e.BAC_Comp.OnButtonStartTouching.RemoveAllListeners();
                e.BAC_Comp.OnButtonIsTouching.RemoveAllListeners();
                e.BAC_Comp.OnButtonStopTouching.RemoveAllListeners();
            }
        }

        #region BAC_Callbacks
        /// <summary>
        /// Deactivate the laser and Activate the Parabole when the user start to click
        /// </summary>
        /// <param name="e"></param>
        private void OnStartInteractingCallback(Filter e)
        {
            if (TeleportGeneralComponent.CanTeleport)
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
            if (TeleportGeneralComponent.CanTeleport)
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
            ParabolicRendererHelper.ToggleHandLaser(e, true);
            e.PointerObjects._selectionPadObject?.SetActive(false);
            e.PointerObjects._invalidPadObject?.SetActive(false);
        }
        #endregion


        #region PRIVATE_METHODS
        private void Init(OnSetupVRReady setupVRReady)
        {
            foreach (var e in GetEntities<Filter>())
            {
                SetupListenersResponses(e);
            }
        }
        #endregion PRIVATE_METHODS
    }
}