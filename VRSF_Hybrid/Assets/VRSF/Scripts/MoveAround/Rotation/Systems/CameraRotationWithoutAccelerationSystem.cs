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
                if (!e.RotationComp.UseAccelerationEffect && e.RotationComp.IsRotating)
                {
                    HandleRotationWithoutAcceleration(e);
                }
            }
        }
        #endregion


        #region PRIVATE_METHODS
        private void HandleRotationWithoutAcceleration(Filter entity)
        {
            Vector3 eyesPosition = VRSF_Components.VRCamera.transform.parent.position;
            Vector3 rotationAxis = new Vector3(0, entity.ButtonComponents.ThumbPos.Value.x, 0);
            float rotationAngle = Time.deltaTime * entity.RotationComp.MaxSpeed;

            VRSF_Components.CameraRig.transform.RotateAround(eyesPosition, rotationAxis, rotationAngle);
        }
        #endregion
    }
}