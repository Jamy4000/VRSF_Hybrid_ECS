using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using VRSF.MoveAround.Teleport.Components;
using VRSF.MoveAround.Teleport.Interfaces;

namespace VRSF.MoveAround.Teleport.Systems
{
    /// <summary>
    /// Handle the Jobs to check if the bezier curve is hitting the floor.
    /// </summary>
    public class BezierHitPointSystem : ComponentSystem
    {
        struct Filter : ITeleportFilter
        {
            public BezierTeleportCalculationComponent BezierTeleport;
            public BezierTeleportParametersComponent BezierParameters;
        }


        #region ComponentSystem_Methods
        protected override void OnUpdate()
        {
            foreach (var e in GetEntities<Filter>())
            {
                if (e.BezierTeleport._IsSetup && e.BezierTeleport._DisplayActive)
                {
                    CheckHitPointArc(e);
                }
            }
        }
        #endregion ComponentSystem_Methods


        // EMPTY
        #region PUBLIC_METHODS
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Check if the ground or the boundaries were hit, and set the Target position and color and the arc color accordingly
        /// </summary>
        private void CheckHitPointArc(Filter entity)
        {
            if (entity.BezierTeleport._GroundDetected || entity.BezierTeleport._LimitDetected)
            {
                entity.BezierParameters.TargetMarker.SetActive(true);

                entity.BezierParameters.TargetMarker.transform.position = entity.BezierTeleport._GroundPos;
                entity.BezierParameters.TargetMarker.transform.rotation = Quaternion.FromToRotation(Vector3.forward, entity.BezierTeleport._LastNormal);

                entity.BezierTeleport._ArcRenderer.material.color = entity.BezierParameters.ColorBezierHitting;
                entity.BezierParameters.TargetMarker.GetComponent<Image>().color = entity.BezierParameters.ColorBezierHitting;
            }
            else
            {
                entity.BezierParameters.TargetMarker.SetActive(false);

                entity.BezierTeleport._ArcRenderer.material.color = entity.BezierParameters.ColorBezierNotHitting;
                entity.BezierParameters.TargetMarker.GetComponent<Image>().color = entity.BezierParameters.ColorBezierNotHitting;
            }
        }
        #endregion PRIVATE_METHODS
    }
}