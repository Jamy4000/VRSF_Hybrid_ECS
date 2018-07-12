using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
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
            public BezierTeleportComponent BezierTeleport;
            public TeleportBoundariesComponent TeleportBoundaries;
        }


        #region ComponentSystem_Methods
        protected override void OnUpdate()
        {
            foreach (var e in GetEntities<Filter>())
            {
                if (e.BezierTeleport._IsSetup && e.BezierTeleport._DisplayActive)
                {
                    UpdatePath(e);
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
        /// Display the TeleportPath and check if it touch the ground
        /// </summary>
        private void UpdatePath(Filter entity)
        {
            Transform cameraRig = VRSF_Components.CameraRig.transform;
            float strength = entity.BezierTeleport.BezierStrength;

            entity.BezierTeleport._ArcRenderer.startWidth = cameraRig.localScale.x / 10;
            entity.BezierTeleport._ArcRenderer.endWidth = cameraRig.localScale.x / 10;

            entity.BezierTeleport._ArcRenderer.textureMode = cameraRig.localScale.y > 1.0f ? LineTextureMode.DistributePerSegment : LineTextureMode.RepeatPerSegment;

            // Setting lineRenderer size according to cameraRig size
            if (cameraRig.localScale.y > 1.0f)
            {
                strength *= cameraRig.localScale.x / 10;
            }

            // Init
            entity.BezierTeleport._GroundDetected = false;
            entity.BezierTeleport._VertexList.Clear(); // delete all previouse vertices
            entity.BezierTeleport._LimitDetected = false;

            // Check the Velocity
            entity.BezierTeleport._Velocity = Quaternion.AngleAxis(-entity.BezierTeleport.BezierAngle, entity.BezierTeleport._CurveOrigin.right) * entity.BezierTeleport._CurveOrigin.forward * strength;

            DisplayArc(entity);
            CheckHitPointArc(entity.BezierTeleport);

            // Update Line Renderer
            entity.BezierTeleport._ArcRenderer.positionCount = entity.BezierTeleport._VertexList.Count;
            entity.BezierTeleport._ArcRenderer.SetPositions(entity.BezierTeleport._VertexList.ToArray());
        }


        /// <summary>
        /// Start while loop to check if the ground was hit. During that, register all new vertec position for Line Renderer.
        /// </summary>
        private void DisplayArc(Filter entity)
        {
            // Init
            RaycastHit hit = new RaycastHit();
            Vector3 pos = entity.BezierTeleport._CurveOrigin.position; // take off position
            entity.BezierTeleport._VertexList.Add(pos);

            // Start the loop to check the ground
            while (!entity.BezierTeleport._GroundDetected && entity.BezierTeleport._VertexList.Count < entity.BezierTeleport._MaxVertexcount)
            {
                Vector3 newPos = pos + entity.BezierTeleport._Velocity * entity.BezierTeleport._VertexDelta
                    + 0.5f * Physics.gravity * entity.BezierTeleport._VertexDelta * entity.BezierTeleport._VertexDelta;

                Vector3 boundedPos = CheckNewPosWithBoundaries(entity, newPos);

                entity.BezierTeleport._Velocity += Physics.gravity * entity.BezierTeleport._VertexDelta;

                // add new calculated vertex
                entity.BezierTeleport._VertexList.Add(boundedPos);

                // if linecast between last vertex and current vertex hit something
                if (Physics.Linecast(pos, boundedPos, out hit, entity.BezierTeleport._ExclusionLayer))
                {
                    entity.BezierTeleport._GroundDetected = true;
                    entity.BezierTeleport._GroundPos = hit.point;
                    entity.BezierTeleport._LastNormal = hit.normal;
                    entity.BezierTeleport._VertexList.RemoveAt(entity.BezierTeleport._VertexList.Count - 1);
                }
                // if the boundedPos is different from the theoritical newPos
                else if (boundedPos != newPos)
                {
                    entity.BezierTeleport._LimitDetected = true;
                    entity.BezierTeleport._GroundPos = boundedPos;
                    entity.BezierTeleport._LastNormal = hit.normal;
                    entity.BezierTeleport._VertexList.RemoveAt(entity.BezierTeleport._VertexList.Count - 1);
                    break;
                }
                pos = boundedPos; // update current vertex as last vertex
            }
        }


        /// <summary>
        /// Check if the ground or the boundaries were hit, and set the Target position and color and the arc color accordingly
        /// </summary>
        private void CheckHitPointArc(BezierTeleportComponent bezierComp)
        {
            if (bezierComp._GroundDetected || bezierComp._LimitDetected)
            {
                bezierComp.TargetMarker.SetActive(true);

                bezierComp.TargetMarker.transform.position = bezierComp._GroundPos;
                bezierComp.TargetMarker.transform.rotation = Quaternion.FromToRotation(Vector3.forward, bezierComp._LastNormal);

                bezierComp._ArcRenderer.material.color = bezierComp.ColorBezierHitting;
                bezierComp.TargetMarker.GetComponent<Image>().color = bezierComp.ColorBezierHitting;
            }
            else
            {
                bezierComp.TargetMarker.SetActive(false);

                bezierComp._ArcRenderer.material.color = bezierComp.ColorBezierNotHitting;
                bezierComp.TargetMarker.GetComponent<Image>().color = bezierComp.ColorBezierNotHitting;
            }
        }
        #endregion PRIVATE_METHODS
    }
}