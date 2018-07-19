using Unity.Entities;
using UnityEngine;
using VRSF.MoveAround.Teleport.Components;
using VRSF.MoveAround.Teleport.Interfaces;

namespace VRSF.MoveAround.Teleport.Systems
{
    /// <summary>
    /// Handle the Jobs to display the arc of the Bezier Curve
    /// </summary>
    public class BezierArcDisplayerSystem : ComponentSystem
    {
        struct Filter : ITeleportFilter
        {
            public BezierTeleportCalculationComponent BezierCalculations;
            public TeleportGeneralComponent GeneralTeleport;
            public TeleportBoundariesComponent TeleportBoundaries;
        }


        #region ComponentSystem_Methods
        protected override void OnUpdate()
        {
            foreach (var e in GetEntities<Filter>())
            {
                if (e.BezierCalculations._IsSetup && e.BezierCalculations._DisplayActive)
                {
                    DisplayArc(e);
                }
            }
        }
        #endregion ComponentSystem_Methods


        #region PUBLIC_METHODS
        /// <summary>
        /// Check the newPos for theStep by Step feature depending on the Teleport Boundaries
        /// </summary>
        public Vector3 CheckNewPosWithBoundaries(ITeleportFilter teleportFilter, Vector3 posToCheck)
        {
            Filter entity = (Filter)teleportFilter;

            if (entity.TeleportBoundaries._UseBoundaries)
            {
                Vector3 minPos = entity.TeleportBoundaries._MinUserPosition;
                Vector3 maxPos = entity.TeleportBoundaries._MaxUserPosition;

                posToCheck.x = Mathf.Clamp(posToCheck.x, minPos.x, maxPos.x);
                posToCheck.y = Mathf.Clamp(posToCheck.y, minPos.y, maxPos.y);
                posToCheck.z = Mathf.Clamp(posToCheck.z, minPos.z, maxPos.z);
            }

            return posToCheck;
        }
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Start while loop to check if the ground was hit. During that, register all new vertec position for Line Renderer.
        /// </summary>
        private void DisplayArc(Filter entity)
        {
            // Init
            RaycastHit hit = new RaycastHit();
            Vector3 pos = entity.BezierCalculations._CurveOrigin.position; // take off position
            entity.BezierCalculations._VertexList.Add(pos);

            // Start the loop to check the ground
            while (!entity.BezierCalculations._GroundDetected && entity.BezierCalculations._VertexList.Count < entity.BezierCalculations._MaxVertexcount)
            {
                Vector3 newPos = pos + entity.BezierCalculations._Velocity * entity.BezierCalculations._VertexDelta
                    + 0.5f * Physics.gravity * entity.BezierCalculations._VertexDelta * entity.BezierCalculations._VertexDelta;

                Vector3 boundedPos = CheckNewPosWithBoundaries(entity, newPos);

                entity.BezierCalculations._Velocity += Physics.gravity * entity.BezierCalculations._VertexDelta;

                // add new calculated vertex
                entity.BezierCalculations._VertexList.Add(boundedPos);

                // if linecast between last vertex and current vertex hit something
                if (Physics.Linecast(pos, boundedPos, out hit, entity.GeneralTeleport.ExclusionLayer))
                {
                    entity.BezierCalculations._GroundDetected = true;
                    entity.BezierCalculations._GroundPos = hit.point;
                    entity.BezierCalculations._LastNormal = hit.normal;
                    entity.BezierCalculations._VertexList.RemoveAt(entity.BezierCalculations._VertexList.Count - 1);
                }
                // if the boundedPos is different from the theoritical newPos
                else if (boundedPos != newPos)
                {
                    entity.BezierCalculations._LimitDetected = true;
                    entity.BezierCalculations._GroundPos = boundedPos;
                    entity.BezierCalculations._LastNormal = hit.normal;
                    entity.BezierCalculations._VertexList.RemoveAt(entity.BezierCalculations._VertexList.Count - 1);
                    break;
                }
                pos = boundedPos; // update current vertex as last vertex
            }

            // Update Line Renderer
            entity.BezierCalculations._ArcRenderer.positionCount = entity.BezierCalculations._VertexList.Count;
            entity.BezierCalculations._ArcRenderer.SetPositions(entity.BezierCalculations._VertexList.ToArray());
        }
        #endregion PRIVATE_METHODS
    }
}