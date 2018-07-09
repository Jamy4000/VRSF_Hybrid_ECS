using Unity.Entities;
using UnityEngine;
using VRSF.Controllers.Components;
using VRSF.Gaze;
using VRSF.Utils;

namespace VRSF.Controllers.Systems
{
    public class PointerSetupSystem : ComponentSystem
    {
        struct Filter
        {
            public ControllerPointerComponents ControllerPointerComp;
        }

        #region PRIVATE_VARIABLES
        // VRSF Parameters references
        private ControllersParametersVariable _controllersParameters;
        #endregion PRIVATE_VARIABLES

        #region ComponentSystem_Methods
        // Use this for initialization
        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            _controllersParameters = ControllersParametersVariable.Instance;

            foreach (var e in GetEntities<Filter>())
            {
                SetupVRComponents(e.ControllerPointerComp);
            }
        }

        // Update is called once per frame
        protected override void OnUpdate()
        {
            foreach (var e in GetEntities<Filter>())
            {
                // As the vive send errors if the controller are not seen on the first frame, we need to put that in the update method
                if (!e.ControllerPointerComp._IsSetup)
                {
                    SetupVRComponents(e.ControllerPointerComp);
                }
            }
        }
        #endregion ComponentSystem_Methods


        #region PRIVATE_METHODS
        /// <summary>
        /// Setup the VR Components thanks to the VRSF_Components static class
        /// </summary>
        private void SetupVRComponents(ControllerPointerComponents comp)
        {
            try
            {
                comp._CameraRigTransform = VRSF_Components.CameraRig.transform;

                if (_controllersParameters.UseControllers)
                {
                    SetupPointers(comp);
                }

                comp._IsSetup = true;
            }
            catch (System.Exception e)
            {
                Debug.Log("VRSF : VR Components not setup yet, waiting for next frame.\n" + e);
            }
        }

        /// <summary>
        /// Setup the pointer with the values of the VRSF Interaction Parameters
        /// </summary>
        private void SetupPointers(ControllerPointerComponents comp)
        {
            comp._RightHandPointer = VRSF_Components.RightController.GetComponent<LineRenderer>();
            comp._RightHandPointer.enabled = _controllersParameters.UsePointerRight;
            comp._LeftHandPointer = VRSF_Components.LeftController.GetComponent<LineRenderer>();
            comp._LeftHandPointer.enabled = _controllersParameters.UsePointerLeft;
        }
        #endregion PRIVATE_METHODS
    }
}