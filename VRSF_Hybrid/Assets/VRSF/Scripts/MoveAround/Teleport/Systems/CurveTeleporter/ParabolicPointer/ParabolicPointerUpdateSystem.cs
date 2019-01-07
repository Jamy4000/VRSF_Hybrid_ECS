﻿using UnityEngine;
using VRSF.Controllers;
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
    public class ParabolicPointerUpdateSystem : BACListenersSetupSystem
    {
        private struct Filter
        {
            public BACGeneralComponent BAC_Comp;
            public ParabolObjectsComponent PointerObjects;
            public ParabolCalculationsComponent PointerCalculations;
            public SceneObjectsComponent SceneObjects;
            public TeleportGeneralComponent TeleportGeneral;
        }

        private ControllersParametersVariable _controllersParameters;

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            _controllersParameters = ControllersParametersVariable.Instance;
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
            ToggleNormalLaser(e, false);
            ForceUpdateCurrentAngle(e);
        }

        /// <summary>
        /// Update the Parabole mesh when user click on the button
        /// </summary>
        /// <param name="e"></param>
        private void OnIsInteractingCallback(Filter e)
        {
            // Deactivate laser if it's still active
            if (e.PointerObjects._ControllerPointer.enabled)
                ToggleNormalLaser(e, false);

            // 1. Calculate Parabola Points
            var velocity = ForceUpdateCurrentAngle(e);
            var normal = ParabolaPointsCalculations(e, velocity);

            // 2. Render the Parabole's pads, aka the targets at the end of the parabole
            RenderParabolePads(e, normal);

            // 3. Draw parabola (BEFORE the outside faces of the selection pad, to avoid depth issues)
            ParaboleCalculationsHelper.GenerateMesh(ref e.PointerObjects._parabolaMesh, e.PointerObjects.ParabolaPoints, velocity, Time.time % 1, e.PointerCalculations.GraphicThickness);
            Graphics.DrawMesh(e.PointerObjects._parabolaMesh, Matrix4x4.identity, e.PointerCalculations.GraphicMaterial, e.PointerObjects.gameObject.layer);
        }

        /// <summary>
        /// Reactivate the laser and Deactivate the pads when realising the button 
        /// </summary>
        /// <param name="e"></param>
        private void OnStopInteractingCallback(Filter e)
        {
            ToggleNormalLaser(e, true);

            if (e.PointerObjects._selectionPadObject != null)
                e.PointerObjects._selectionPadObject.SetActive(false);
            if (e.PointerObjects._invalidPadObject != null)
                e.PointerObjects._invalidPadObject.SetActive(false);
        }
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Calculate the points on the way of the Parabola
        /// </summary>
        /// <param name="e">The reference to the entity to check</param>
        /// <param name="velocity">The velocity of the Parabole</param>
        /// <returns>The normal of the Curve</returns>
        private Vector3 ParabolaPointsCalculations(Filter e, Vector3 velocity)
        {
            e.PointerCalculations.PointOnNavMesh = ParaboleCalculationsHelper.CalculateParabolicCurve
            (
                e.PointerCalculations.transform.position,
                velocity,
                e.PointerCalculations.Acceleration,
                e.PointerCalculations.PointSpacing,
                e.PointerCalculations.PointCount,
                e.SceneObjects._TeleportNavMesh,
                _controllersParameters.GetExclusionsLayer(e.BAC_Comp.ButtonHand),
                e.PointerObjects.ParabolaPoints,
                out Vector3 normal
            );

            e.TeleportGeneral.PointToGoTo = e.PointerObjects.ParabolaPoints[e.PointerObjects.ParabolaPoints.Count - 1];
            return normal;
        }

        /// <summary>
        /// Render the targets of the parabola at the end of the curve, to give a visual feedback to the user on whether he can or cannot teleport.
        /// </summary>
        /// <param name="e">Entity to check</param>
        /// <param name="normal">The normal of the curve</param>
        private void RenderParabolePads(Filter e, Vector3 normal)
        {
            // Display the valid pad if the user is on the navMesh
            if (e.PointerObjects._selectionPadObject != null)
            {
                e.PointerObjects._selectionPadObject.SetActive(e.PointerCalculations.PointOnNavMesh);
                e.PointerObjects._selectionPadObject.transform.position = e.TeleportGeneral.PointToGoTo + (Vector3.one * 0.005f);
                if (e.PointerCalculations.PointOnNavMesh)
                {
                    e.PointerObjects._selectionPadObject.transform.rotation = Quaternion.LookRotation(normal);
                    e.PointerObjects._selectionPadObject.transform.Rotate(90, 0, 0);
                }
            }

            // Display the invalid pad if the user is not on the navMesh
            if (e.PointerObjects._invalidPadObject != null)
            {
                e.PointerObjects._invalidPadObject.SetActive(!e.PointerCalculations.PointOnNavMesh);
                e.PointerObjects._invalidPadObject.transform.position = e.TeleportGeneral.PointToGoTo + (Vector3.one * 0.005f);
                if (!e.PointerCalculations.PointOnNavMesh)
                {
                    e.PointerObjects._invalidPadObject.transform.rotation = Quaternion.LookRotation(normal);
                    e.PointerObjects._invalidPadObject.transform.Rotate(90, 0, 0);
                }
            }
        }

        /// <summary>
        /// Active Teleporter Arc Path
        /// </summary>
        /// <param name="active"></param>
        private void ToggleNormalLaser(Filter entity, bool active)
        {
            if (Utils.VRSF_Components.DeviceLoaded != Utils.EDevice.SIMULATOR)
            {
                // Change pointer activation if the user is using it
                if ((entity.BAC_Comp.ButtonHand == EHand.LEFT && _controllersParameters.UsePointerLeft) ||
                    (entity.BAC_Comp.ButtonHand == EHand.RIGHT && _controllersParameters.UsePointerRight))
                {
                    entity.PointerObjects._ControllerPointer.enabled = active;
                    foreach(var particleSystem in entity.PointerObjects._ControllerPointer.GetComponentsInChildren<ParticleSystem>())
                    {
                        if (active)
                        {
                            particleSystem.Play();
                        }
                        else
                        {
                            particleSystem.Stop();
                            particleSystem.Clear();
                        }
                    }
                }
            }
        }


        /// <summary>
        ///  Used when you can't depend on Update() to automatically update CurrentParabolaAngle
        /// (for example, directly after enabling the component)
        /// </summary>
        private Vector3 ForceUpdateCurrentAngle(Filter e)
        {
            Vector3 velocity = e.PointerObjects.transform.TransformDirection(e.PointerCalculations.InitialVelocity);
            e.PointerCalculations.CurrentParabolaAngleY = ParaboleCalculationsHelper.ClampInitialVelocity(ref velocity, out Vector3 d, e.PointerCalculations.InitialVelocity);
            e.PointerCalculations.CurrentPointVector = d;
            return velocity;
        }
        #endregion PRIVATE_METHODS
    }
}