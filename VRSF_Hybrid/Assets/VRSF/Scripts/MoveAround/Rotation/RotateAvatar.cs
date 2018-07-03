using UnityEngine;
using VRSF.Utils;

namespace VRSF.MoveAround
{
    public class RotateAvatar : ButtonActionChoser
    {
        #region PUBLIC_VARIABLES
        [Header("Rotate Avatar Speed")]
        public float MaxSpeed = 1.0f;
        public bool UseAccelerationEffect = true;
        #endregion


        #region PRIVATE_VARIABLES
        private GameObject _avatarObject;                //The CameraRig object
        private Transform _camera;

        private bool _isRotating;
        private float _currentSpeed = 0.0f;
        private float _lastThumbPos;
        #endregion


        #region MONOBEHAVIOUR_METHODS
        public override void Start()
        {
            base.Start();
            _avatarObject = VRSF_Components.CameraRig;
            _camera = VRSF_Components.VRCamera.transform;
        }

        public override void Update()
        {
            base.Update();

            if (UseAccelerationEffect)
            {
                HandleRotationWithAcceleration();
            }
            else if (_isRotating)
            {
                _avatarObject.transform.RotateAround(_camera.position, new Vector3(0, ThumbPos.Value.x, 0), Time.deltaTime * MaxSpeed * 20.0f);
            }
        }
        #endregion


        #region PUBLIC_METHODS
        /// <summary>
        /// Set the _rotating bool to true. Need to be called from IsTouching, IsClicking, StartTouching or StartClicking.
        /// </summary>
        public void StartRotating()
        {
            _isRotating = true;
        }

        /// <summary>
        /// Set the _rotating bool to false. Need to be called from StopTouching or StopClicking.
        /// </summary>
        public void StopRotating()
        {
            _isRotating = false;
        }
        #endregion

        
        #region PRIVATE_METHODS
        private void HandleRotationWithAcceleration()
        {
            // If the user is rotating and the current speed is not set to the maximum speed yet
            if (_isRotating && _currentSpeed < (MaxSpeed / 5))
            {
                _currentSpeed += (Time.deltaTime / (MaxSpeed * 5));
                _lastThumbPos = ThumbPos.Value.x;
            }
            // If the user is not rotating anymore and the current speed is superior to 0, with decelerate.
            else if (_currentSpeed > 0.0f && !_isRotating)
            {
                _currentSpeed -= (Time.deltaTime / (MaxSpeed * 5));
            }
            
            if (_currentSpeed > 0.0f)
            {
                _avatarObject.transform.Rotate(new Vector3(0, _lastThumbPos, 0), _currentSpeed);
            }
        }
        #endregion


        // EMPTY
        #region GETTERS_SETTERS

        #endregion
    }
}