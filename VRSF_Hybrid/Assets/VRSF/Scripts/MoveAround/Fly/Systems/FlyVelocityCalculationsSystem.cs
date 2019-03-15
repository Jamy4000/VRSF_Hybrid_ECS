using Unity.Entities;
using UnityEngine;

namespace VRSF.MoveAround.Fly
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
        private void CheckFlyingMode(Filter e)
        {
            //If the user is pressing/touching the flying button, we handle the acceleration
            if (e.ParametersComponent._WantToFly)
            {
                if (e.ParametersComponent._TimeSinceStartFlying >= 0 && e.ParametersComponent._TimeSinceStartFlying < 1.0f)
                    e.ParametersComponent._TimeSinceStartFlying = e.AccelerationComponent.AccelerationDecelerationEffect ? e.ParametersComponent._TimeSinceStartFlying + (Time.deltaTime / e.AccelerationComponent.AccelerationEffectFactor) : 1.0f;

                if (e.ParametersComponent._SlowDownTimer > 0.0f)
                {
                    e.ParametersComponent._TimeSinceStartFlying = e.ParametersComponent._SlowDownTimer;
                    e.ParametersComponent._SlowDownTimer = 0.0f;
                }

                e.VelocityComponent.CurrentFlightVelocity = GetSpeed(e.ParametersComponent) * e.ParametersComponent._TimeSinceStartFlying;
            }

            //If the user stop pressing/touching the flying button, we handle the deceleration
            else
            {
                if (e.ParametersComponent._SlowDownTimer > 0.0f)
                {
                    if (e.AccelerationComponent.AccelerationDecelerationEffect)
                        e.ParametersComponent._SlowDownTimer -= (Time.deltaTime / e.AccelerationComponent.DecelerationEffectFactor);
                    else
                        e.ParametersComponent._SlowDownTimer = 0.0f;
                }

                //Sliding effect when touchpad is released
                e.VelocityComponent.CurrentFlightVelocity = GetSpeed(e.ParametersComponent) * e.ParametersComponent._SlowDownTimer;

                if (e.VelocityComponent.CurrentFlightVelocity <= 0.0f)
                {
                    e.ParametersComponent._IsSlowingDown = false;
                }
            }

            /// <summary>
            /// Get basic vertical speed (0.05) and multiply it by the flying speed factor
            /// </summary>
            /// <returns>The new vertical axis speed</returns>
            float GetSpeed(FlyParametersComponent flyComp)
            {
                return 0.05f * flyComp.FlyingSpeed;
            }
        }
        #endregion PRIVATE_METHODS
    }
}