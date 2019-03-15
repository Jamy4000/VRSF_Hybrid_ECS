using UnityEngine;
using VRSF.Core.SetupVR;
using VRSF.Utils.ButtonActionChoser;
using VRSF.Core.Raycast;
using VRSF.Core.Inputs;

namespace VRSF.MoveAround.Rotate
{
    /// <summary>
    /// Rotate the user based on the Speed parameter using a sliding effect.
    /// WARNING Can give motion sickness !
    /// </summary>
    public class CameraRotationWithAccelerationSystem : BACListenersSetupSystem
    {

        struct Filter
        {
            public CameraRotationComponent RotationComp;
            public BACGeneralComponent BACGeneral;
            public BACCalculationsComponent BACCalculations;
            public ScriptableRaycastComponent RaycastComp;
        }


        #region ComponentSystem_Methods
        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            foreach (var e in GetEntities<Filter>())
            {
                if (!e.RotationComp.UseAccelerationEffect)
                {
                    SetupListenersResponses(e);
                }
            }
        }

        protected override void OnUpdate() {}

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();

            foreach (var e in GetEntities<Filter>())
            {
                RemoveListeners(e);
            }
        }
        #endregion


        #region PUBLIC_METHODS
        public override void SetupListenersResponses(object entity)
        {
            var e = (Filter)entity;

            if ((e.BACGeneral.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                e.BACGeneral.OnButtonIsClicking.AddListener(delegate { HandleRotationWithAcceleration(e); });
            }

            if ((e.BACGeneral.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                e.BACGeneral.OnButtonIsTouching.AddListener(delegate { HandleRotationWithAcceleration(e); });
            }
        }

        public override void RemoveListeners(object entity)
        {
            var e = (Filter)entity;

            if ((e.BACGeneral.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                e.BACGeneral.OnButtonIsClicking.RemoveAllListeners();
            }

            if ((e.BACGeneral.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                e.BACGeneral.OnButtonIsTouching.RemoveAllListeners();
            }
        }
        #endregion PUBLIC_METHODS

        #region PRIVATE_METHODS
        private void HandleRotationWithAcceleration(Filter entity)
        {
            // If the user is not aiming to the UI
            if (!entity.RaycastComp.RaycastHitVar.RaycastHitIsOnUI())
            {
                // isAccelerating : The user is Rotating (touching/clicking the thumbstick) and the currentSpeed is < (maxSpeed / 5)
                bool isAccelerating = entity.RotationComp.IsRotating && entity.RotationComp.CurrentSpeed < (entity.RotationComp.MaxSpeed / 20);

                // isDecelerating : The user is not Rotating (not touching/clicking the thumbstick) and the currentSpeed is > 0
                bool isDecelerating = !entity.RotationComp.IsRotating && entity.RotationComp.CurrentSpeed > 0.0f;

                // maxSpeedTimeDeltaTime : To calculate the current speed according to deltaTime and Max Speed
                float maxSpeedTimeDeltaTime = Time.deltaTime * (entity.RotationComp.MaxSpeed / 50);

                // LastThumbPos : The last thumbPos of the user when rotating (touching/clicking the thumbstick) only 
                entity.RotationComp.LastThumbPos = entity.RotationComp.IsRotating ? entity.BACCalculations.ThumbPos.Value.x : entity.RotationComp.LastThumbPos;
                
                // Setting the current speed of the user
                entity.RotationComp.CurrentSpeed += isAccelerating ? maxSpeedTimeDeltaTime : -maxSpeedTimeDeltaTime;

                if (entity.RotationComp.CurrentSpeed > 0.0f)
                {
                    Vector3 eyesPosition = VRSF_Components.VRCamera.transform.position;
                    Vector3 rotationAxis = new Vector3(0, entity.RotationComp.LastThumbPos, 0);

                    VRSF_Components.CameraRig.transform.RotateAround(eyesPosition, rotationAxis, entity.RotationComp.CurrentSpeed);
                }
            }
        }
        #endregion
    }
}