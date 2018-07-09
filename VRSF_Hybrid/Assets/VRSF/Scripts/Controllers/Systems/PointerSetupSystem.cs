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
        private GazeParametersVariable _gazeParameters;
        private ControllersParametersVariable _controllersParameters;
        #endregion PRIVATE_VARIABLES

        #region ComponentSystem_Methods
        // Use this for initialization
        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            _controllersParameters = ControllersParametersVariable.Instance;
            _gazeParameters = GazeParametersVariable.Instance;

            foreach (var e in GetEntities<Filter>())
            {
                if (_gazeParameters.UseGaze && _gazeParameters.UseDifferentStates)
                {
                    e.ControllerPointerComp.CheckGazeStates = true;
                }

                SetupVRComponents(e.ControllerPointerComp);
            }
        }

        // Update is called once per frame
        protected override void OnUpdate()
        {
            foreach (var e in GetEntities<Filter>())
            {
                // As the vive send errors if the controller are not seen on the first frame, we need to put that in the update method
                if (!e.ControllerPointerComp.IsSetup)
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
                comp.CameraRigTransform = VRSF_Components.CameraRig.transform;

                if (_controllersParameters.UseControllers)
                {
                    SetupPointers(comp);
                }

                if (_gazeParameters.UseGaze)
                {
                    SetupGazeImages(comp);
                }

                comp.IsSetup = true;
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
            comp.RightHandPointer = VRSF_Components.RightController.GetComponent<LineRenderer>();
            comp.RightHandPointer.enabled = _controllersParameters.UsePointerRight;
            comp.LeftHandPointer = VRSF_Components.LeftController.GetComponent<LineRenderer>();
            comp.LeftHandPointer.enabled = _controllersParameters.UsePointerLeft;
        }


        /// <summary>
        /// Try to get the background and target images of the Gaze based on the Gaze script.
        /// </summary>
        private void SetupGazeImages(ControllerPointerComponents comp)
        {
            try
            {
                    comp.GazeScript = GameObject.FindObjectOfType<Gaze.Gaze>();

                    if (!comp.GazeBackground)
                        comp.GazeBackground = comp.GazeScript.ReticleBackground;

                    if (!comp.GazeTarget)
                        comp.GazeTarget = comp.GazeScript.ReticleTarget;
            }
            catch (System.Exception e)
            {
                Debug.Log("Couldn't find the Gaze script, cannot get the images for the ColorPointer script.\n" + e.ToString());
            }
        }
        #endregion PRIVATE_METHODS
    }
}