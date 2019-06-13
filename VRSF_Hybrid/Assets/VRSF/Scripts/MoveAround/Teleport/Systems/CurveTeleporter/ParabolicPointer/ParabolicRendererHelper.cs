using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.Interactions;

namespace VRSF.MoveAround.Teleport
{
    public static class ParabolicRendererHelper
    {
        public static ControllersParametersVariable ControllersParameters;

        /// <summary>
        /// Calculate the points on the way of the Parabola
        /// </summary>
        /// <param name="e">The reference to the entity to check</param>
        /// <param name="velocity">The velocity of the Parabole</param>
        /// <returns>The normal of the Curve</returns>
        public static Vector3 ParabolaPointsCalculations(ParabolicPointerUpdateSystem.Filter e, Vector3 velocity)
        {
            e.PointerCalculations.PointOnNavMesh = ParaboleCalculationsHelper.CalculateParabolicCurve
            (
                e.PointerCalculations.transform.position,
                velocity,
                e.PointerCalculations.Acceleration,
                e.PointerCalculations.PointSpacing,
                e.PointerCalculations.PointCount,
                e.SceneObjects._TeleportNavMesh,
                ControllersParameters.GetExclusionsLayer(e.BAC_Comp.ButtonHand),
                out e.PointerObjects.ParabolaPoints,
                out Vector3 normal
            );
            
            e.PointerCalculations.TempPointToGoTo = e.PointerObjects.ParabolaPoints[e.PointerObjects.ParabolaPoints.Count - 1];
            return normal;
        }

        /// <summary>
        /// Render the targets of the parabola at the end of the curve, to give a visual feedback to the user on whether he can or cannot teleport.
        /// </summary>
        /// <param name="e">Entity to check</param>
        /// <param name="normal">The normal of the curve</param>
        public static void RenderParabolePads(ParabolicPointerUpdateSystem.Filter e, Vector3 normal)
        {
            // Display the valid pad if the user is on the navMesh
            if (e.PointerObjects._selectionPadObject != null)
            {
                e.PointerObjects._selectionPadObject.SetActive(e.PointerCalculations.PointOnNavMesh);
                if (e.PointerCalculations.PointOnNavMesh)
                {
                    e.PointerObjects._selectionPadObject.transform.position = e.PointerCalculations.TempPointToGoTo + (Vector3.one * 0.005f);
                    e.PointerObjects._selectionPadObject.transform.rotation = Quaternion.LookRotation(normal);
                    e.PointerObjects._selectionPadObject.transform.Rotate(90, 0, 0);
                }
            }

            // Display the invalid pad if the user is not on the navMesh
            if (e.PointerObjects._invalidPadObject != null)
            {
                e.PointerObjects._invalidPadObject.SetActive(!e.PointerCalculations.PointOnNavMesh);
                if (!e.PointerCalculations.PointOnNavMesh)
                {
                    e.PointerObjects._invalidPadObject.transform.position = e.PointerCalculations.TempPointToGoTo + (Vector3.one * 0.005f);
                    e.PointerObjects._invalidPadObject.transform.rotation = Quaternion.LookRotation(normal);
                    e.PointerObjects._invalidPadObject.transform.Rotate(90, 0, 0);
                }
            }
        }

        /// <summary>
        /// Activate/Deactivate the pointer on the left hand
        /// </summary>
        /// <param name="active"></param>
        public static void ToggleHandLaser(ParabolicPointerUpdateSystem.Filter e, bool active)
        {
            // Change pointer activation if the user is using it
            if ((e.BAC_Comp.ButtonHand == EHand.LEFT && ControllersParameters.UsePointerLeft) ||
                (e.BAC_Comp.ButtonHand == EHand.RIGHT && ControllersParameters.UsePointerRight))
            {
                // We deactivate the fact that the user is able to click on stuffs as long as the curve teleport is on
                if (e.BAC_Comp.ButtonHand == EHand.LEFT)
                    PointerClickComponent.LeftTriggerCanClick = active;
                else
                    PointerClickComponent.RightTriggerCanClick = active;

                if (e.PointerObjects._ControllerPointer != null)
                {
                    // We change the status of the laser gameObject
                    e.PointerObjects._ControllerPointer.enabled = active;
                }
            }
        }


        /// <summary>
        ///  Used when you can't depend on Update() to automatically update CurrentParabolaAngle
        /// (for example, directly after enabling the component)
        /// </summary>
        public static Vector3 ForceUpdateCurrentAngle(ParabolicPointerUpdateSystem.Filter e)
        {
            Vector3 velocity = e.PointerObjects.transform.TransformDirection(e.PointerCalculations.InitialVelocity);
            e.PointerCalculations.CurrentParabolaAngleY = ParaboleCalculationsHelper.ClampInitialVelocity(ref velocity, out Vector3 d, e.PointerCalculations.InitialVelocity);
            e.PointerCalculations.CurrentPointVector = d;
            return velocity;
        }
    }
}