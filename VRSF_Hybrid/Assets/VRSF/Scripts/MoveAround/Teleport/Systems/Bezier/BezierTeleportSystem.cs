using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using VRSF.Controllers;
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
            public BezierTeleportComponent BezierComp;
        }


        #region ComponentSystem_Methods
        protected override void OnUpdate()
        {
            foreach (var e in GetEntities<Filter>())
            {
                if (e.BezierComp._IsSetup && e.BezierComp._DisplayActive)
                {
                    UpdatePath(e);
                }
            }
        }
        #endregion ComponentSystem_Methods


        #region PUBLIC_METHODS
        
        #region Teleport_Interface
        /// <summary>
        /// Method to call from StartClicking or StartTouching, teleport the user one meter away.
        /// </summary>
        public void TeleportUser(ITeleportFilter teleportFilter)
        {
            // HANDLE IN BezierSetupSystem
        }


        /// <summary>
        /// Check the newPos for theStep by Step feature depending on the Teleport Boundaries
        /// </summary>
        public Vector3 CheckNewPosWithBoundaries(ITeleportFilter teleportFilter, Vector3 posToCheck)
        {
            Filter entity = (Filter)teleportFilter;

            Vector3 minPos = entity.BezierComp._MinUserPosition;
            Vector3 maxPos = entity.BezierComp._MaxUserPosition;

            posToCheck.x = Mathf.Clamp(posToCheck.x, minPos.x, maxPos.x);
            posToCheck.y = Mathf.Clamp(posToCheck.y, minPos.y, maxPos.y);
            posToCheck.z = Mathf.Clamp(posToCheck.z, minPos.z, maxPos.z);

            return posToCheck;
        }
        #endregion

        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Display the TeleportPath and check if it touch the ground
        /// </summary>
        private void UpdatePath(Filter entity)
        {
            Transform cameraRig = VRSF_Components.CameraRig.transform;
            float strength = entity.BezierComp.BezierStrength;

            entity.BezierComp._ArcRenderer.startWidth = cameraRig.localScale.x / 10;
            entity.BezierComp._ArcRenderer.endWidth = cameraRig.localScale.x / 10;

            entity.BezierComp._ArcRenderer.textureMode = cameraRig.localScale.y > 1.0f ? LineTextureMode.DistributePerSegment : LineTextureMode.RepeatPerSegment;

            // Setting lineRenderer size according to cameraRig size
            if (cameraRig.localScale.y > 1.0f)
            {
                strength *= cameraRig.localScale.x / 10;
            }

            // Init
            entity.BezierComp._GroundDetected = false;
            entity.BezierComp._VertexList.Clear(); // delete all previouse vertices
            entity.BezierComp._LimitDetected = false;

            // Check the Velocity
            entity.BezierComp._Velocity = Quaternion.AngleAxis(-entity.BezierComp.BezierAngle, entity.BezierComp._CurveOrigin.right) * entity.BezierComp._CurveOrigin.forward * strength;

            DisplayArc(entity);
            CheckHitPointArc(entity.BezierComp);

            // Update Line Renderer
            entity.BezierComp._ArcRenderer.positionCount = entity.BezierComp._VertexList.Count;
            entity.BezierComp._ArcRenderer.SetPositions(entity.BezierComp._VertexList.ToArray());
        }


        /// <summary>
        /// Start while loop to check if the ground was hit. During that, register all new vertec position for Line Renderer.
        /// </summary>
        private void DisplayArc(Filter entity)
        {
            // Init
            RaycastHit hit = new RaycastHit();
            Vector3 pos = entity.BezierComp._CurveOrigin.position; // take off position
            entity.BezierComp._VertexList.Add(pos);

            // Start the loop to check the ground
            while (!entity.BezierComp._GroundDetected && entity.BezierComp._VertexList.Count < entity.BezierComp._MaxVertexcount)
            {
                Vector3 newPos = pos + entity.BezierComp._Velocity * entity.BezierComp._VertexDelta
                    + 0.5f * Physics.gravity * entity.BezierComp._VertexDelta * entity.BezierComp._VertexDelta;

                Vector3 boundedPos = CheckNewPosWithBoundaries(entity, newPos);

                entity.BezierComp._Velocity += Physics.gravity * entity.BezierComp._VertexDelta;

                // add new calculated vertex
                entity.BezierComp._VertexList.Add(boundedPos);

                // if linecast between last vertex and current vertex hit something
                if (Physics.Linecast(pos, boundedPos, out hit, entity.BezierComp._ExclusionLayer))
                {
                    entity.BezierComp._GroundDetected = true;
                    entity.BezierComp._GroundPos = hit.point;
                    entity.BezierComp._LastNormal = hit.normal;
                    entity.BezierComp._VertexList.RemoveAt(entity.BezierComp._VertexList.Count - 1);
                }
                // if the boundedPos is different from the theoritical newPos
                else if (boundedPos != newPos)
                {
                    entity.BezierComp._LimitDetected = true;
                    entity.BezierComp._GroundPos = boundedPos;
                    entity.BezierComp._LastNormal = hit.normal;
                    entity.BezierComp._VertexList.RemoveAt(entity.BezierComp._VertexList.Count - 1);
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