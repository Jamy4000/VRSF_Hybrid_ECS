using UnityEngine;
using UnityEngine.SceneManagement;
using VRSF.Inputs;
using VRSF.MoveAround.Components;
using VRSF.Utils;
using VRSF.Utils.Components;
using VRSF.Utils.Components.ButtonActionChoser;
using VRSF.Utils.Systems.ButtonActionChoser;

namespace VRSF.MoveAround.Systems
{
    public class CameraRotationWithoutAccelerationSystem : BACUpdateSystem
    {
        struct Filter
        {
            public CameraRotationComponent RotationComp;
            public ButtonActionChoserComponents ButtonComponents;
            public ScriptableRaycastComponent RaycastComp;
        }
        
        #region ComponentSystem_Methods
        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            SceneManager.sceneUnloaded += OnSceneUnloaded;

            foreach (var e in GetEntities<Filter>())
            {
                if (!e.RotationComp.UseAccelerationEffect)
                {
                    SetupListenersResponses(e);
                }
            }

            this.Enabled = false;
        }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();

            foreach (var e in GetEntities<Filter>())
            {
                RemoveListenersOnEndApp(e);
            }

            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }
        #endregion


        #region PUBLIC_METHODS
        public override void SetupListenersResponses(object entity)
        {
            var e = (Filter)entity;
            
            if ((e.ButtonComponents.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                e.ButtonComponents.OnButtonIsClicking.AddListener(delegate { HandleRotationWithoutAcceleration(e); });
            }

            if ((e.ButtonComponents.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                e.ButtonComponents.OnButtonIsTouching.AddListener(delegate { HandleRotationWithoutAcceleration(e); });
            }
        }

        public override void RemoveListenersOnEndApp(object entity)
        {
            var e = (Filter)entity;

            if ((e.ButtonComponents.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                e.ButtonComponents.OnButtonIsClicking.RemoveAllListeners();
            }

            if ((e.ButtonComponents.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                e.ButtonComponents.OnButtonIsTouching.RemoveAllListeners();
            }
        }
        #endregion PUBLIC_METHODS


        #region PRIVATE_METHODS
        private void HandleRotationWithoutAcceleration(Filter entity)
        {
            Debug.Log("HandleRotationWithoutAcceleration");
            if (!entity.RotationComp.HasRotated)
            {
                var cameraRigTransform = VRSF_Components.CameraRig.transform;

                Vector3 eyesPosition = VRSF_Components.VRCamera.transform.parent.position;
                Vector3 rotationAxis = new Vector3(0, entity.ButtonComponents.ThumbPos.Value.x, 0);

                cameraRigTransform.RotateAround(eyesPosition, rotationAxis, entity.RotationComp.DegreesToTurn);

                // We check if the rotation value is not above 180 or below -180. if so, we substract/add 360 degrees to it.
                var newRot = cameraRigTransform.rotation;

                newRot.y = (newRot.y > 180.0f) ? (newRot.y - 360.0f) : newRot.y;
                newRot.y = (newRot.y < -180.0f) ? (newRot.y + 360.0f) : newRot.y;

                cameraRigTransform.rotation = newRot;

                entity.RotationComp.HasRotated = true;
            }
        }

        private void OnSceneUnloaded(Scene oldScene)
        {
            this.Enabled = true;
        }
        #endregion
    }
}