using UnityEngine;
using VRSF.Inputs;
using VRSF.MoveAround.Components;
using VRSF.Utils;
using VRSF.Utils.Components.ButtonActionChoser;
using VRSF.Utils.Systems.ButtonActionChoser;

namespace VRSF.MoveAround.Systems
{
    public class CameraRotationSetupSystem : BACUpdateSystem
    {

        struct Filter
        {
            public CameraRotationComponent RotationComp;
            public ButtonActionChoserComponents ButtonComponents;
        }


        #region PRIVATE_VARIABLES
        private Filter _currentSetupEntity;
        #endregion PRIVATE_VARIABLES


        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        protected override void OnStartRunning()
        {
            if (GetEntities<Filter>().Length == 0)
            {
                this.Enabled = false;
                return;
            }

            base.OnStartRunning();

            foreach (var e in GetEntities<Filter>())
            {
                _currentSetupEntity = e;
                SetupListenersResponses();
            }
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            foreach (var e in GetEntities<Filter>())
            {
                if (e.RotationComp.UseAccelerationEffect)
                {
                    HandleRotationWithAcceleration(e);
                }
                else if (e.RotationComp.IsRotating)
                {
                    HandleRotationWithoutAcceleration(e);
                }
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
        #endregion


        #region PUBLIC_METHODS

        #region Setup_Listeners_Responses
        public override void SetupListenersResponses()
        {
            if ((_currentSetupEntity.ButtonComponents.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                _currentSetupEntity.ButtonComponents.OnButtonStartClicking.AddListener(delegate { StartRotating(_currentSetupEntity.RotationComp); });
                _currentSetupEntity.ButtonComponents.OnButtonIsClicking.AddListener(delegate { StartRotating(_currentSetupEntity.RotationComp); });

                _currentSetupEntity.ButtonComponents.OnButtonStopClicking.AddListener(delegate { StopRotating(_currentSetupEntity.RotationComp); });
            }

            if ((_currentSetupEntity.ButtonComponents.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                _currentSetupEntity.ButtonComponents.OnButtonStartTouching.AddListener(delegate { StartRotating(_currentSetupEntity.RotationComp); });
                _currentSetupEntity.ButtonComponents.OnButtonIsTouching.AddListener(delegate { StartRotating(_currentSetupEntity.RotationComp); });

                _currentSetupEntity.ButtonComponents.OnButtonStopTouching.AddListener(delegate { StopRotating(_currentSetupEntity.RotationComp); });
            }
        }

        public override void RemoveListenersOnEndApp()
        {
            if ((_currentSetupEntity.ButtonComponents.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                _currentSetupEntity.ButtonComponents.OnButtonStartClicking.RemoveAllListeners();
                _currentSetupEntity.ButtonComponents.OnButtonIsClicking.RemoveAllListeners();
                _currentSetupEntity.ButtonComponents.OnButtonStopClicking.RemoveAllListeners();
            }

            if ((_currentSetupEntity.ButtonComponents.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                _currentSetupEntity.ButtonComponents.OnButtonStartTouching.RemoveAllListeners();
                _currentSetupEntity.ButtonComponents.OnButtonIsTouching.RemoveAllListeners();
                _currentSetupEntity.ButtonComponents.OnButtonStopTouching.RemoveAllListeners();
            }
        }
        #endregion Setup_Listeners_Responses

        #endregion PUBLIC_METHODS


        #region PRIVATE_METHODS
        /// <summary>
        /// Set the _rotating bool to true. Need to be called from IsTouching, IsClicking, StartTouching or StartClicking.
        /// </summary>
        private void StartRotating(CameraRotationComponent comp)
        {
            comp.IsRotating = true;
        }

        /// <summary>
        /// Set the _rotating bool to false. Need to be called from StopTouching or StopClicking.
        /// </summary>
        private void StopRotating(CameraRotationComponent comp)
        {
            comp.IsRotating = false;
        }

        private void HandleRotationWithoutAcceleration(Filter entity)
        {
            Vector3 eyesPosition = VRSF_Components.VRCamera.transform.parent.position;
            Vector3 rotationAxis = new Vector3(0, entity.ButtonComponents.ThumbPos.Value.x, 0);
            float rotationAngle = Time.deltaTime * entity.RotationComp.MaxSpeed * 20.0f;

            VRSF_Components.CameraRig.transform.RotateAround(eyesPosition, rotationAxis, rotationAngle);
        }


        private void HandleRotationWithAcceleration(Filter entity)
        {
            // isAccelerating : The user is Rotating (touching/clicking the thumbstick) and the currentSpeed is < (maxSpeed / 5)
            bool isAccelerating = entity.RotationComp.IsRotating && entity.RotationComp.CurrentSpeed < (entity.RotationComp.MaxSpeed / 5);

            // isDecelerating : The user is not Rotating (not touching/clicking the thumbstick) and the currentSpeed is > 0
            bool isDecelerating = !entity.RotationComp.IsRotating && entity.RotationComp.CurrentSpeed > 0.0f;

            // maxSpeedTimeDeltaTime : To calculate the current speed according to deltaTime and Max Speed
            float maxSpeedTimeDeltaTime = Time.deltaTime * (entity.RotationComp.MaxSpeed / 5);

            // LastThumbPos : The last thumbPos of the user when rotating (touching/clicking the thumbstick) only 
            entity.RotationComp.LastThumbPos = entity.RotationComp.IsRotating ? entity.ButtonComponents.ThumbPos.Value.x : entity.RotationComp.LastThumbPos;

            if (isAccelerating)
            {
                entity.RotationComp.CurrentSpeed += maxSpeedTimeDeltaTime;
            }
            else if (isDecelerating)
            {
                entity.RotationComp.CurrentSpeed -= maxSpeedTimeDeltaTime;
            }

            if (entity.RotationComp.CurrentSpeed > 0.0f)
            {
                Vector3 eyesPosition = VRSF_Components.VRCamera.transform.parent.position;
                Vector3 rotationAxis = new Vector3(0, entity.RotationComp.LastThumbPos, 0);

                VRSF_Components.CameraRig.transform.RotateAround(eyesPosition, rotationAxis, entity.RotationComp.CurrentSpeed);
            }
        }
        #endregion
    }
}