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
        private FlyingParametersVariable _flyingParameters;

        private Filter _currentSetupEntity;
        #endregion PRIVATE_VARIABLES


        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            
            _flyingParameters = FlyingParametersVariable.Instance;

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
                    if (_flyingParameters.AccelerationDecelerationEffect)
                        entity.FlyComponents.TimeSinceStartFlying += (Time.deltaTime / _flyingParameters.AccelerationEffectFactor);
                    else
                        entity.FlyComponents.TimeSinceStartFlying = 1.0f;
                }

                if (entity.FlyComponents.SlowDownTimer > 0.0f)
                {
                    entity.FlyComponents.TimeSinceStartFlying = entity.FlyComponents.SlowDownTimer;
                    entity.FlyComponents.SlowDownTimer = 0.0f;
                }

                entity.FlyComponents.CurrentFlightVelocity = _flyingParameters.GetSpeed() * entity.FlyComponents.TimeSinceStartFlying;
            }

            //If the user stop pressing/touching the flying button, we handle the deceleration
            else
            {
                if (entity.FlyComponents.SlowDownTimer > 0.0f)
                {
                    if (_flyingParameters.AccelerationDecelerationEffect)
                        entity.FlyComponents.SlowDownTimer -= (Time.deltaTime / _flyingParameters.DecelerationEffectFactor);
                    else
                        entity.FlyComponents.SlowDownTimer = 0.0f;
                }

                if (entity.FlyComponents.CurrentFlightVelocity <= 0.0f)
                {
                    entity.FlyComponents.IsSlowingDown = false;
                    return;
                }

                //Sliding effect when touchpad is released
                entity.FlyComponents.CurrentFlightVelocity = _flyingParameters.GetSpeed() * entity.FlyComponents.SlowDownTimer;
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
            float minPosY = (_flyingParameters.UseBoundaries ? _flyingParameters.MinAvatarPosition.y : _flyingParameters.MinAvatarYPosition);
            float maxPosY = (_flyingParameters.UseBoundaries ? _flyingParameters.MaxAvatarPosition.y : _flyingParameters.MaxAvatarYPosition);

            // if we change the speed depending on the Height of the User
            if (_flyingParameters.ChangeSpeedDependingOnHeight)
            {
                entity.FlyComponents.CurrentFlightVelocity *= MapRangeClamp(cameraRigTransform.position.y, Mathf.Abs(minPosY), Mathf.Abs(maxPosY), 1.0f, maxPosY / 100);
            }

            // if we change the speed depending on the Scale of the User
            if (_flyingParameters.ChangeSpeedDependingOnScale)
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
            if (_flyingParameters.UseBoundaries)
            {
                // Clamp new values between min pos and max pos
                newPos.x = Mathf.Clamp(newPos.x, _flyingParameters.MinAvatarPosition.x, _flyingParameters.MaxAvatarPosition.x);
                newPos.y = Mathf.Clamp(newPos.y, _flyingParameters.MinAvatarPosition.y, _flyingParameters.MaxAvatarPosition.y);
                newPos.z = Mathf.Clamp(newPos.z, _flyingParameters.MinAvatarPosition.z, _flyingParameters.MaxAvatarPosition.z);
            }

            // Set avatar position
            VRSF_Components.CameraRig.transform.position = newPos;
        }
        #endregion Fly_Calculations

        #endregion PRIVATE_METHODS
    }
}