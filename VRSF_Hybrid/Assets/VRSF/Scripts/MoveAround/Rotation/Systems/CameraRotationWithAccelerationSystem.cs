using ScriptableFramework.Util;
using Unity.Entities;
using UnityEngine;
using VRSF.MoveAround.Components;
using VRSF.Utils;
using VRSF.Utils.Components.ButtonActionChoser;

namespace VRSF.MoveAround.Systems
{
    public class CameraRotationWithAccelerationSystem : ComponentSystem
    {

        struct Filter
        {
            public CameraRotationComponent RotationComp;
            public ButtonActionChoserComponents ButtonComponents;
        }


        #region ComponentSystem_Methods
        protected override void OnUpdate()
        {
            foreach (var e in GetEntities<Filter>())
            {
                if (e.RotationComp.UseAccelerationEffect)
                {
                    HandleRotationWithAcceleration(e);
                }
            }
        }
        #endregion


        #region PRIVATE_METHODS
        private void HandleRotationWithAcceleration(Filter entity)
        {
            // isAccelerating : The user is Rotating (touching/clicking the thumbstick) and the currentSpeed is < (maxSpeed / 5)
            bool isAccelerating = entity.RotationComp.IsRotating && entity.RotationComp.CurrentSpeed < (entity.RotationComp.MaxSpeed / 20);

            // isDecelerating : The user is not Rotating (not touching/clicking the thumbstick) and the currentSpeed is > 0
            bool isDecelerating = !entity.RotationComp.IsRotating && entity.RotationComp.CurrentSpeed > 0.0f;

            // maxSpeedTimeDeltaTime : To calculate the current speed according to deltaTime and Max Speed
            float maxSpeedTimeDeltaTime = Time.deltaTime * (entity.RotationComp.MaxSpeed / 50);

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