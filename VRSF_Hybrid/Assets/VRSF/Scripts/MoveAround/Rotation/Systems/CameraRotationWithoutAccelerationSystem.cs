using UnityEngine;
using VRSF.Core.Inputs;
using VRSF.Core.SetupVR;
using VRSF.Core.Utils.ButtonActionChoser;
using VRSF.Core.Raycast;

namespace VRSF.MoveAround.Rotate
{
    public class CameraRotationWithoutAccelerationSystem : BACListenersSetupSystem
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
            OnSetupVRReady.Listeners += OnSetupVRIsReady;
            base.OnStartRunning();
        }
        
        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();

            OnSetupVRReady.Listeners -= OnSetupVRIsReady;

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
                e.BACGeneral.OnButtonStartClicking.AddListener(delegate { HandleRotationWithoutAcceleration(e); });
            }

            if ((e.BACGeneral.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                e.BACGeneral.OnButtonStartTouching.AddListener(delegate { HandleRotationWithoutAcceleration(e); });
            }
        }

        public override void RemoveListeners(object entity)
        {
            var e = (Filter)entity;

            if ((e.BACGeneral.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                e.BACGeneral.OnButtonStartClicking.RemoveAllListeners();
            }

            if ((e.BACGeneral.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                e.BACGeneral.OnButtonStartTouching.RemoveAllListeners();
            }
        }
        #endregion PUBLIC_METHODS


        #region PRIVATE_METHODS
        private void HandleRotationWithoutAcceleration(Filter entity)
        {
            var cameraRigTransform = VRSF_Components.CameraRig.transform;

            Vector3 eyesPosition = VRSF_Components.VRCamera.transform.position;
            Vector3 rotationAxis = new Vector3(0, entity.BACCalculations.ThumbPos.Value.x, 0);

            cameraRigTransform.RotateAround(eyesPosition, rotationAxis, entity.RotationComp.DegreesToTurn);
        }


        /// <summary>
        /// Callback for when SetupVR is setup. Setup the lsiteners.
        /// </summary>
        /// <param name="onSetupVR"></param>
        private void OnSetupVRIsReady(OnSetupVRReady onSetupVR)
        {
            foreach (var e in GetEntities<Filter>())
            {
                if (e.BACGeneral.ActionButton != EControllersButton.THUMBSTICK)
                {
                    Debug.LogError("<b>[VRSF] :</b> You need to assign Left Thumbstick or Right Thumbstick to use the Rotation script. Setting CanBeUsed at false.");
                    e.BACCalculations.CanBeUsed = false;
                    return;
                }

                SetupListenersResponses(e);
            }
        }
        #endregion
    }
}