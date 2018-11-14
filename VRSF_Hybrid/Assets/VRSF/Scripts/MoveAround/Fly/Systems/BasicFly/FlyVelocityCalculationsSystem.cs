using Unity.Entities;
using UnityEngine;
using VRSF.MoveAround.Components;

namespace VRSF.MoveAround.Systems
{
    /// <summary>
    /// System Allowing the user to fly with the thumbstick / touchpad. 
    /// </summary>
    public class FlyVelocityCalculationsSystem : ComponentSystem
    {
        struct Filter
        {
            public FlyParametersComponent ParametersComponent;
            public FlyVelocityComponent VelocityComponent;
            public FlyAccelerationComponent AccelerationComponent;
        }


        #region ComponentSystem_Methods
        protected override void OnUpdate()
        {
            foreach (var e in GetEntities<Filter>())
            {
                if (e.ParametersComponent._IsInteracting || e.VelocityComponent.CurrentFlightVelocity > 0.0f)
                {
                    CheckFlyingMode(e);
                }
            }
        }
        #endregion ComponentSystem_Methods


        #region PRIVATE_METHODS
        /// <summary>
        /// Add an acceleration sensation when the user is flying if specified by the user
        /// </summary>
        private void CheckFlyingMode(Filter entity)
        {
            //If the user is pressing/touching the flying button, we handle the acceleration
            if (entity.ParametersComponent._WantToFly)
            {
                if (entity.ParametersComponent._TimeSinceStartFlying >= 0 && entity.ParametersComponent._TimeSinceStartFlying < 1.0f)
                {
                    if (entity.AccelerationComponent.AccelerationDecelerationEffect)
                        entity.ParametersComponent._TimeSinceStartFlying += (Time.deltaTime / entity.AccelerationComponent.AccelerationEffectFactor);
                    else
                        entity.ParametersComponent._TimeSinceStartFlying = 1.0f;
                }

                if (entity.ParametersComponent._SlowDownTimer > 0.0f)
                {
                    entity.ParametersComponent._TimeSinceStartFlying = entity.ParametersComponent._SlowDownTimer;
                    entity.ParametersComponent._SlowDownTimer = 0.0f;
                }

                entity.VelocityComponent.CurrentFlightVelocity = GetSpeed(entity.ParametersComponent) * entity.ParametersComponent._TimeSinceStartFlying;
            }

            //If the user stop pressing/touching the flying button, we handle the deceleration
            else
            {
                if (entity.ParametersComponent._SlowDownTimer > 0.0f)
                {
                    if (entity.AccelerationComponent.AccelerationDecelerationEffect)
                        entity.ParametersComponent._SlowDownTimer -= (Time.deltaTime / entity.AccelerationComponent.DecelerationEffectFactor);
                    else
                        entity.ParametersComponent._SlowDownTimer = 0.0f;
                }

                //Sliding effect when touchpad is released
                entity.VelocityComponent.CurrentFlightVelocity = GetSpeed(entity.ParametersComponent) * entity.ParametersComponent._SlowDownTimer;

                if (entity.VelocityComponent.CurrentFlightVelocity <= 0.0f)
                {
                    entity.ParametersComponent._IsSlowingDown = false;
                }
            }
        }


        /// <summary>
        /// Get basic vertical speed (0.3) and multiply it by the flying speed factor
        /// </summary>
        /// <returns>The new vertical axis speed</returns>
        private float GetSpeed(FlyParametersComponent flyComp)
        {
            return 0.05f * flyComp.FlyingSpeed;
        }
        #endregion PRIVATE_METHODS
    }
}