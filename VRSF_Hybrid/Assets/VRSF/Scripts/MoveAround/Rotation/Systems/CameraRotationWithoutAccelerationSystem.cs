using Unity.Entities;
using UnityEngine;
using VRSF.MoveAround.Components;
using VRSF.Utils;
using VRSF.Utils.Components.ButtonActionChoser;

namespace VRSF.MoveAround.Systems
{
    public class CameraRotationWithoutAccelerationSystem : ComponentSystem
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
                if (!e.RotationComp.UseAccelerationEffect && !e.RotationComp._HasRotated && e.RotationComp._IsRotating)
                {
                    HandleRotationWithoutAcceleration(e);
                    e.RotationComp._HasRotated = true;
                }
            }
        }
        #endregion


        #region PRIVATE_METHODS
        private void HandleRotationWithoutAcceleration(Filter entity)
        {
            var cameraRigTransform = VRSF_Components.VRCamera.transform;
            Vector3 eyesPosition = cameraRigTransform.parent.position;
            Vector3 rotationAxis = new Vector3(0, entity.ButtonComponents.ThumbPos.Value.x, 0);

            cameraRigTransform.RotateAround(eyesPosition, rotationAxis, entity.RotationComp.DegreesToTurn);

            // We check if the rotation value is not above 180 or below -180. if so, we substract/add 360 degrees to it.
            var newRot = cameraRigTransform.rotation;

            newRot.y = (newRot.y > 180.0f) ? (newRot.y - 360.0f) : newRot.y;
            newRot.y = (newRot.y < -180.0f) ? (newRot.y + 360.0f) : newRot.y;

            cameraRigTransform.rotation = newRot;
        }
        #endregion
    }
}