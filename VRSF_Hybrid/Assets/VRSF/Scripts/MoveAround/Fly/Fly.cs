using UnityEngine;
using ScriptableFramework.Variables;
using VRSF.Controllers;
using VRSF.Utils;
using VRSF.Inputs;

namespace VRSF.MoveAround
{
    /// <summary>
    /// Allow the user to fly with the thumbstick / touchpad. 
    /// </summary>
    public class Fly : ButtonActionChoser
    {
        // EMPTY
        #region PUBLIC_VARIABLES
        #endregion


        #region PRIVATE_VARIABLES
        // VRSF Parameters
        private FlyingParametersVariable _flyingParameters;

        private Vector2Variable _thumbPosition;          //Correspond to the Vector2Variable that is linked to the thumb the user is using to fly

        private GameObject _avatarObject;                //The CameraRig object

        private bool _flyForward = true;
        private float _flightDirection;
        private float _currentFlightVelocity = 0.0f;
        private Vector3 _normalizedDir;
        private Vector3 _finalDirection;
        private bool _wantToFly = false;

        private float _timeSinceStartFlying = 0.0f;
        private float _slowDownTimer = 0.0f;
        private bool _isSlowingDown = false;
        #endregion


        #region MONOBEHAVIOUR_METHODS
        // Use this for initialization
        public override void Start()
        {
            if (ActionButton != EControllersInput.LEFT_THUMBSTICK && ActionButton != EControllersInput.RIGHT_THUMBSTICK)
            {
                Debug.LogError("VRSF : You need to assign Left Thumbstick or Right Thumbstick to use the Fly script. Disabling the Script.");
                enabled = false;
                return;
            }

            base.Start();

            _avatarObject = VRSF_Components.CameraRig;
            _flyingParameters = FlyingParametersVariable.Instance;

            CheckHand();
        }

        public override void Update()
        {
            base.Update();

            if (_isSlowingDown)
            {
                CheckFlyingMode();
                FlyAway();
            }
        }
        #endregion


        #region PUBLIC_METHODS
        /// <summary>
        /// To be called from OnButtonStopClicking or OnButtonStopTouching event in editor
        /// </summary>
        public void ButtonStopInteracting()
        {
            if (_wantToFly)
            {
                StopMoving();
            }
        }

        /// <summary>
        /// To be called from OnButtonIsTouching or OnButtonIsClickingevent in editor
        /// </summary>
        public void ButtonIsInteracting()
        {
            CalculateFlyForward();
            CheckFlyingMode();
            FlyAway();
        }
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Add an acceleration sensation when the user is flying
        /// </summary>
        private void CheckFlyingMode()
        {
            //If the user is pressing/touching the flying button, we handle the acceleration
            if (_wantToFly)
            {
                if (_timeSinceStartFlying >= 0 && _timeSinceStartFlying < 1.0f)
                {
                    if (_flyingParameters.AccelerationDecelerationEffect)
                        _timeSinceStartFlying += (Time.deltaTime / _flyingParameters.AccelerationEffectFactor);
                    else
                        _timeSinceStartFlying = 1.0f;
                }

                if (_slowDownTimer > 0.0f)
                {
                    _timeSinceStartFlying = _slowDownTimer;
                    _slowDownTimer = 0.0f;
                }

                _currentFlightVelocity = _flyingParameters.GetSpeed() * _timeSinceStartFlying;
            }

            //If the user stop pressing/touching the flying button, we handle the deceleration
            else
            {
                if (_slowDownTimer > 0.0f)
                {
                    if (_flyingParameters.AccelerationDecelerationEffect)
                        _slowDownTimer -= (Time.deltaTime / _flyingParameters.DecelerationEffectFactor);
                    else
                        _slowDownTimer = 0.0f;
                }

                if (_currentFlightVelocity <= 0.0f)
                {
                    _isSlowingDown = false;
                    return;
                }

                //Sliding effect when touchpad is released
                _currentFlightVelocity = _flyingParameters.GetSpeed() * _slowDownTimer;
            }
        }

        /// <summary>
        /// Called when user release thumstick/touchpad
        /// </summary>
        private void StopMoving()
        {
            _slowDownTimer = _timeSinceStartFlying;
            _isSlowingDown = true;
            _wantToFly = false;
        }

        /// <summary>
        /// Calculate if the user is flying forward or backward and init some values.
        /// </summary>
        private void CalculateFlyForward()
        {
            _flyForward = (_thumbPosition.Value.y >= 0.0f) ? true : false;

            // If user just started to press/touch the thumbstick
            if (!_wantToFly)
            {
                _timeSinceStartFlying = 0.0f;
                _wantToFly = true;
            }
        }

        /// <summary>
        /// Check if the user fly forward or backward
        /// </summary>
        /// <returns>The new position without the boundaries</returns>
        private Vector3 GetNewPosition()
        {
            if (_wantToFly)
            {
                _flightDirection = _flyForward ? 1.0f : -1.0f;

                _normalizedDir = Vector3.Normalize(RayVar.Value.direction);
            }

            // We get the min and max pos in Y depending if we're using boundaries or not.
            float minPosY = (_flyingParameters.UseBoundaries ? _flyingParameters.MinAvatarPosition.y : _flyingParameters.MinAvatarYPosition);
            float maxPosY = (_flyingParameters.UseBoundaries ? _flyingParameters.MaxAvatarPosition.y : _flyingParameters.MaxAvatarYPosition);

            // if we change the speed depending on the Height of the User
            if (_flyingParameters.ChangeSpeedDependingOnHeight)
            {
                _currentFlightVelocity *= MapRangeClamp(_avatarObject.transform.position.y, Mathf.Abs(minPosY), Mathf.Abs(maxPosY), 1.0f, maxPosY / 100);
            }

            // if we change the speed depending on the Scale of the User
            if (_flyingParameters.ChangeSpeedDependingOnScale)
            {
                _currentFlightVelocity /= MapRangeClamp(_avatarObject.transform.lossyScale.y, Mathf.Abs(minPosY), Mathf.Abs(maxPosY), 1.0f, maxPosY / 100);
            }

            _finalDirection = _normalizedDir * _currentFlightVelocity * _flightDirection;

            return (_avatarObject.transform.position + _finalDirection);
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
        private void FlyAway()
        {
            // We get the new position of the user according to where he's pointing/looking
            Vector3 newPos = GetNewPosition();

            // If we use boundaries for the flying mode
            if (_flyingParameters.UseBoundaries)
            {
                // Clamp new values between min pos and max pos
                newPos.x = Mathf.Clamp(newPos.x, _flyingParameters.MinAvatarPosition.x, _flyingParameters.MaxAvatarPosition.x);
                newPos.y = Mathf.Clamp(newPos.y, _flyingParameters.MinAvatarPosition.y, _flyingParameters.MaxAvatarPosition.y);
                newPos.z = Mathf.Clamp(newPos.z, _flyingParameters.MinAvatarPosition.z, _flyingParameters.MaxAvatarPosition.z);
            }

            // Set avatar position
            _avatarObject.transform.position = newPos;
        }

        /// <summary>
        /// Assign either the Left or Right variable depending on the Hand that is selected
        /// </summary>
        void CheckHand()
        {
            switch (ButtonHand)
            {
                case EHand.LEFT:
                    _thumbPosition = InputContainer.LeftThumbPosition;
                    break;
                case EHand.RIGHT:
                    _thumbPosition = InputContainer.RightThumbPosition;
                    break;
                default:
                    Debug.LogError("VRSF : An error has occured in the Fly script. Disabling the Script.");
                    this.enabled = false;
                    break;
            }
        }
        #endregion


        // EMPTY
        #region GETTERS_SETTERS
        #endregion
    }
}