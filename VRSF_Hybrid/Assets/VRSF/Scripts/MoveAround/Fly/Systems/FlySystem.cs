using UnityEngine;
using VRSF.Inputs;
using VRSF.MoveAround.Components;
using VRSF.Utils;
using VRSF.Utils.Components;
using VRSF.Utils.Systems.ButtonActionChoser;

namespace VRSF.MoveAround.Systems
{
    /// <summary>
    /// System Allowing the user to fly with the thumbstick / touchpad. 
    /// </summary>
    public class FlySystem : BACUpdateSystem
    {
        struct Filter
        {
            public FlyComponent FlyComponents;
            public ButtonActionChoserComponents ButtonComponents;
        }


        #region PRIVATE_VARIABLES
        private Filter _currentSetupEntity;
        #endregion PRIVATE_VARIABLES


        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            
            foreach (var e in GetEntities<Filter>())
            {
                if (e.ButtonComponents.ActionButton != EControllersInput.LEFT_THUMBSTICK && e.ButtonComponents.ActionButton != EControllersInput.RIGHT_THUMBSTICK)
                {
                    Debug.LogError("VRSF : You need to assign Left Thumbstick or Right Thumbstick to use the Fly script. Setting CanBeUsed at false.");
                    e.ButtonComponents.CanBeUsed = false;
                }

                _currentSetupEntity = e;

                SetupListenersResponses();

                e.FlyComponents.IsSetup = true;
            }
        }

        protected override void OnStopRunning()
        {
            base.OnStopRunning();

            foreach (var e in GetEntities<Filter>())
            {
                _currentSetupEntity = e;
                RemoveListenersOnEndApp();
            }
        }
        #endregion ComponentSystem_Methods
        

        #region PRIVATE_METHODS

        #region Setup_Fly
        public override void SetupListenersResponses()
        {
            if ((_currentSetupEntity.ButtonComponents.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                _currentSetupEntity.ButtonComponents.OnButtonIsClicking.AddListener(delegate { ButtonIsInteracting(_currentSetupEntity); });
                _currentSetupEntity.ButtonComponents.OnButtonStopClicking.AddListener(delegate { ButtonStopInteracting(_currentSetupEntity); });
            }

            if ((_currentSetupEntity.ButtonComponents.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                _currentSetupEntity.ButtonComponents.OnButtonIsTouching.AddListener(delegate { ButtonIsInteracting(_currentSetupEntity); });
                _currentSetupEntity.ButtonComponents.OnButtonStopTouching.AddListener(delegate { ButtonStopInteracting(_currentSetupEntity); });
            }
        }

        public override void RemoveListenersOnEndApp()
        {
            if ((_currentSetupEntity.ButtonComponents.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                _currentSetupEntity.ButtonComponents.OnButtonIsClicking.RemoveAllListeners();
                _currentSetupEntity.ButtonComponents.OnButtonStopClicking.RemoveAllListeners();
            }

            if ((_currentSetupEntity.ButtonComponents.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                _currentSetupEntity.ButtonComponents.OnButtonIsTouching.RemoveAllListeners();
                _currentSetupEntity.ButtonComponents.OnButtonStopTouching.RemoveAllListeners();
            }
        }
        #endregion Setup_Fly


        #region Listeners_Responses
        /// <summary>
        /// Called from OnButtonStopClicking or OnButtonStopTouching event
        /// </summary>
        private void ButtonStopInteracting(Filter entity)
        {
            if (entity.FlyComponents.WantToFly)
            {
                StopMoving(entity);
            }
        }

        /// <summary>
        /// Called from OnButtonIsTouching or OnButtonIsClickingevent
        /// </summary>
        private void ButtonIsInteracting(Filter entity)
        {
            CalculateFlyForward(entity);
            CheckFlyingMode(entity);
            FlyAway(entity);
        }
        #endregion Listeners_Responses


        #region Fly_Calculations
        /// <summary>
        /// Add an acceleration sensation when the user is flying
        /// </summary>
        private void CheckFlyingMode(Filter entity)
        {
            //If the user is pressing/touching the flying button, we handle the acceleration
            if (entity.FlyComponents.WantToFly)
            {
                if (entity.FlyComponents.TimeSinceStartFlying >= 0 && entity.FlyComponents.TimeSinceStartFlying < 1.0f)
                {
                    if (entity.FlyComponents.AccelerationDecelerationEffect)
                        entity.FlyComponents.TimeSinceStartFlying += (Time.deltaTime / entity.FlyComponents.AccelerationEffectFactor);
                    else
                        entity.FlyComponents.TimeSinceStartFlying = 1.0f;
                }

                if (entity.FlyComponents.SlowDownTimer > 0.0f)
                {
                    entity.FlyComponents.TimeSinceStartFlying = entity.FlyComponents.SlowDownTimer;
                    entity.FlyComponents.SlowDownTimer = 0.0f;
                }

                entity.FlyComponents.CurrentFlightVelocity = GetSpeed(entity.FlyComponents) * entity.FlyComponents.TimeSinceStartFlying;
            }

            //If the user stop pressing/touching the flying button, we handle the deceleration
            else
            {
                if (entity.FlyComponents.SlowDownTimer > 0.0f)
                {
                    if (entity.FlyComponents.AccelerationDecelerationEffect)
                        entity.FlyComponents.SlowDownTimer -= (Time.deltaTime / entity.FlyComponents.DecelerationEffectFactor);
                    else
                        entity.FlyComponents.SlowDownTimer = 0.0f;
                }

                if (entity.FlyComponents.CurrentFlightVelocity <= 0.0f)
                {
                    entity.FlyComponents.IsSlowingDown = false;
                    return;
                }

                //Sliding effect when touchpad is released
                entity.FlyComponents.CurrentFlightVelocity = GetSpeed(entity.FlyComponents) * entity.FlyComponents.SlowDownTimer;
            }
        }

        /// <summary>
        /// Called when user release thumstick/touchpad
        /// </summary>
        private void StopMoving(Filter entity)
        {
            entity.FlyComponents.SlowDownTimer = entity.FlyComponents.TimeSinceStartFlying;
            entity.FlyComponents.IsSlowingDown = true;
            entity.FlyComponents.WantToFly = false;
        }

        /// <summary>
        /// Calculate if the user is flying forward or backward and init some values.
        /// </summary>
        private void CalculateFlyForward(Filter entity)
        {
            entity.FlyComponents.FlyForward = (entity.ButtonComponents.ThumbPos.Value.y >= 0.0f) ? true : false;

            // If user just started to press/touch the thumbstick
            if (!entity.FlyComponents.WantToFly)
            {
                entity.FlyComponents.TimeSinceStartFlying = 0.0f;
                entity.FlyComponents.WantToFly = true;
            }
        }

        /// <summary>
        /// Check if the user fly forward or backward
        /// </summary>
        /// <returns>The new position without the boundaries</returns>
        private Vector3 GetNewPosition(Filter entity)
        {
            Transform cameraRigTransform = VRSF_Components.CameraRig.transform;

            if (entity.FlyComponents.WantToFly)
            {
                entity.FlyComponents.FlightDirection = entity.FlyComponents.FlyForward ? 1.0f : -1.0f;
                
                entity.FlyComponents.NormalizedDir = Vector3.Normalize(entity.ButtonComponents.RayVar.Value.direction);
            }
            
            // We get the min and max pos in Y depending if we're using boundaries or not.
            float minPosY = (entity.FlyComponents.UseHorizontalBoundaries ? entity.FlyComponents.MinAvatarPosition.y : entity.FlyComponents.MinAvatarYPosition);
            float maxPosY = (entity.FlyComponents.UseHorizontalBoundaries ? entity.FlyComponents.MaxAvatarPosition.y : entity.FlyComponents.MaxAvatarYPosition);

            // if we change the speed depending on the Height of the User
            if (entity.FlyComponents.ChangeSpeedDependingOnHeight)
            {
                entity.FlyComponents.CurrentFlightVelocity *= MapRangeClamp(cameraRigTransform.position.y, Mathf.Abs(minPosY), Mathf.Abs(maxPosY), 1.0f, maxPosY / 100);
            }

            // if we change the speed depending on the Scale of the User
            if (entity.FlyComponents.ChangeSpeedDependingOnScale)
            {
                entity.FlyComponents.CurrentFlightVelocity /= MapRangeClamp(cameraRigTransform.lossyScale.y, Mathf.Abs(minPosY), Mathf.Abs(maxPosY), 1.0f, maxPosY / 100);
            }

            entity.FlyComponents.FinalDirection = entity.FlyComponents.NormalizedDir * entity.FlyComponents.CurrentFlightVelocity * entity.FlyComponents.FlightDirection;

            return (cameraRigTransform.position + entity.FlyComponents.FinalDirection);
        }


        private float MapRangeClamp(float val, float srcMin, float srcMax, float dstMin, float dstMax)
        {
            if (val <= srcMin) return dstMin;
            else if (val >= srcMax) return dstMax;

            float denominator = (srcMax - srcMin) * (dstMax - dstMin);

            denominator = (denominator == 0.0f ? 0.000001f : denominator);

            return dstMin + (val - srcMin) / denominator;
        }

        /// <summary>
        /// Actual script to make the user fly
        /// </summary>
        private void FlyAway(Filter entity)
        {
            // We get the new position of the user according to where he's pointing/looking
            Vector3 newPos = GetNewPosition(entity);

            // If we use boundaries for the flying mode
            if (entity.FlyComponents.UseHorizontalBoundaries)
            {
                // Clamp new values between min pos and max pos
                newPos.x = Mathf.Clamp(newPos.x, entity.FlyComponents.MinAvatarPosition.x, entity.FlyComponents.MaxAvatarPosition.x);
                newPos.y = Mathf.Clamp(newPos.y, entity.FlyComponents.MinAvatarPosition.y, entity.FlyComponents.MaxAvatarPosition.y);
                newPos.z = Mathf.Clamp(newPos.z, entity.FlyComponents.MinAvatarPosition.z, entity.FlyComponents.MaxAvatarPosition.z);
            }

            // Set avatar position
            VRSF_Components.CameraRig.transform.position = newPos;
        }


        /// <summary>
        /// Get basic vertical speed (0.3) and multiply it by the flying speed factor
        /// </summary>
        /// <returns>The new vertical axis speed</returns>
        private float GetSpeed(FlyComponent flyComp)
        {
            return 0.3f * flyComp.FlyingSpeed;
        }
        #endregion Fly_Calculations

        #endregion PRIVATE_METHODS
    }
}