using Unity.Entities;
using UnityEngine;
using VRSF.MoveAround.Teleport.Components;
using VRSF.MoveAround.Teleport.Interfaces;
using VRSF.Utils;

namespace VRSF.MoveAround.Teleport.Systems
{
    /// <summary>
    /// Handle the Jobs to display the Bezier Curves when user is clicking the assigned button.
    /// </summary>
    public class BezierTeleportSystem : ComponentSystem
    {
        struct Filter : ITeleportFilter
        {
            public BezierTeleportCalculationComponent BezierCalculations;
            public TeleportGeneralComponent GeneralTeleport;
            public TeleportBoundariesComponent TeleportBoundaries;
            public BezierTeleportParametersComponent BezierParameters;
        }


        #region ComponentSystem_Methods
        protected override void OnUpdate()
        {
            foreach (var e in GetEntities<Filter>())
            {
                if (e.BezierCalculations._IsSetup && e.BezierCalculations._DisplayActive)
                {
                    UpdatePath(e);
                }
            }
        }
        #endregion ComponentSystem_Methods


        // EMPTY
        #region PUBLIC_METHODS
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Display the TeleportPath and check if it touch the ground
        /// </summary>
        private void UpdatePath(Filter entity)
        {
            Transform cameraRig = VRSF_Components.CameraRig.transform;
            float strength = entity.BezierParameters.BezierStrength;

            entity.BezierCalculations._ArcRenderer.startWidth = cameraRig.localScale.x / 10;
            entity.BezierCalculations._ArcRenderer.endWidth = cameraRig.localScale.x / 10;

            entity.BezierCalculations._ArcRenderer.textureMode = cameraRig.localScale.y > 1.0f ? LineTextureMode.DistributePerSegment : LineTextureMode.RepeatPerSegment;

            // Setting lineRenderer size according to cameraRig size
            if (cameraRig.localScale.y > 1.0f)
            {
                strength *= cameraRig.localScale.x / 10;
            }

            // Init
            entity.BezierCalculations._GroundDetected = false;
            entity.BezierCalculations._VertexList.Clear(); // delete all previouse vertices
            entity.BezierCalculations._LimitDetected = false;

            // Check the Velocity
            entity.BezierCalculations._Velocity = Quaternion.AngleAxis(-entity.BezierParameters.BezierAngle, entity.BezierCalculations._CurveOrigin.right) * entity.BezierCalculations._CurveOrigin.forward * strength;
        }
        #endregion PRIVATE_METHODS
    }
}