using Unity.Entities;
using UnityEngine;
using VRSF.Core.Raycast;
using VRSF.Core.SetupVR;
using VRSF.MoveAround.Components;

namespace VRSF.MoveAround.Fly
{
    /// <summary>
    /// System Allowing the user to fly with the thumbstick / touchpad. 
    /// </summary>
    public class FlyAwaySystem : ComponentSystem
    {
        struct Filter
        {
            public FlyParametersComponent ParametersComponent;
            public FlyVelocityComponent VelocityComponent;
            public FlyDirectionComponent DirectionComponent;
            public FlyBoundariesComponent BoundariesComponent;
            public ScriptableRaycastComponent BAC_RayComp;
        }


        #region ComponentSystem_Methods
        protected override void OnUpdate()
        {
            foreach (var e in GetEntities<Filter>())
            {
                if (e.ParametersComponent._IsInteracting || e.VelocityComponent.CurrentFlightVelocity > 0.0f)
                {
                    FlyAway(e);
                }
            }
        }
        #endregion ComponentSystem_Methods


        #region PRIVATE_METHODS
        /// <summary>
        /// Actual script to make the user fly
        /// </summary>
        private void FlyAway(Filter entity)
        {
            // We get the new position of the user according to where he's pointing/looking
            Vector3 newPos = GetNewPosition(entity);

            // If we use boundaries for the flying mode
            if (entity.BoundariesComponent.UseHorizontalBoundaries)
            {
                // Clamp new values between min pos and max pos
                newPos.x = Mathf.Clamp(newPos.x, entity.BoundariesComponent.MinAvatarPosition.x, entity.BoundariesComponent.MaxAvatarPosition.x);
                newPos.y = Mathf.Clamp(newPos.y, entity.BoundariesComponent.MinAvatarPosition.y, entity.BoundariesComponent.MaxAvatarPosition.y);
                newPos.z = Mathf.Clamp(newPos.z, entity.BoundariesComponent.MinAvatarPosition.z, entity.BoundariesComponent.MaxAvatarPosition.z);
            }

            // Set avatar position
            VRSF_Components.CameraRig.transform.position = newPos;
        }

        /// <summary>
        /// Check if the user fly forward or backward
        /// </summary>
        /// <returns>The new position without the boundaries</returns>
        private Vector3 GetNewPosition(Filter entity)
        {
            Transform cameraRigTransform = VRSF_Components.CameraRig.transform;

            if (entity.ParametersComponent._WantToFly)
            {
                entity.DirectionComponent.FlightDirection = entity.ParametersComponent._FlyForward ? 1.0f : -1.0f;

                entity.DirectionComponent.NormalizedDir = Vector3.Normalize(entity.BAC_RayComp.RayVar.Value.direction);
            }

            // We get the min and max pos in Y depending if we're using boundaries or not.
            float minPosY = (entity.BoundariesComponent.UseHorizontalBoundaries ? entity.BoundariesComponent.MinAvatarPosition.y : entity.BoundariesComponent.MinAvatarYPosition);
            float maxPosY = (entity.BoundariesComponent.UseHorizontalBoundaries ? entity.BoundariesComponent.MaxAvatarPosition.y : entity.BoundariesComponent.MaxAvatarYPosition);

            // if we change the speed depending on the Height of the User
            if (entity.ParametersComponent.ChangeSpeedDependingOnHeight)
            {
                entity.VelocityComponent.CurrentFlightVelocity *= MapRangeClamp(cameraRigTransform.position.y, Mathf.Abs(minPosY), Mathf.Abs(maxPosY), 1.0f, maxPosY / 100);
            }

            // if we change the speed depending on the Scale of the User
            if (entity.ParametersComponent.ChangeSpeedDependingOnScale)
            {
                entity.VelocityComponent.CurrentFlightVelocity /= MapRangeClamp(cameraRigTransform.lossyScale.y, Mathf.Abs(minPosY), Mathf.Abs(maxPosY), 1.0f, maxPosY / 100);
            }

            entity.DirectionComponent.FinalDirection = entity.DirectionComponent.NormalizedDir * entity.VelocityComponent.CurrentFlightVelocity * entity.DirectionComponent.FlightDirection;

            return (cameraRigTransform.position + entity.DirectionComponent.FinalDirection);
        }

        /// <summary>
        /// Override of the Math.Clamp method for the current flight velocity calculations.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="srcMin"></param>
        /// <param name="srcMax"></param>
        /// <param name="dstMin"></param>
        /// <param name="dstMax"></param>
        /// <returns></returns>
        private float MapRangeClamp(float val, float srcMin, float srcMax, float dstMin, float dstMax)
        {
            if (val <= srcMin) return dstMin;
            else if (val >= srcMax) return dstMax;

            float denominator = (srcMax - srcMin) * (dstMax - dstMin);

            denominator = (denominator == 0.0f ? 0.000001f : denominator);

            return dstMin + (val - srcMin) / denominator;
        }
        #endregion PRIVATE_METHODS
    }
}