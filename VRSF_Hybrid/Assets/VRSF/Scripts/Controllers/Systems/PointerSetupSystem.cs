using Unity.Entities;
using UnityEngine;
using VRSF.Controllers.Components;
using VRSF.Gaze;
using VRSF.Interactions;
using VRSF.Utils;

namespace VRSF.Controllers.Systems
{
    public class PointerSetupSystem : ComponentSystem
    {
        struct Filter
        {
            public ControllerPointerComponents colorPointerComp;
        }

        #region ComponentSystem_Methods
        // Use this for initialization
        protected override void OnStartRunning()
        {
            foreach (var e in GetEntities<Filter>())
            {
                // Init
                e.colorPointerComp.ControllersParameters = ControllersParametersVariable.Instance;
                e.colorPointerComp.GazeParameters = GazeParametersVariable.Instance;
                e.colorPointerComp.InteractionContainer = InteractionVariableContainer.Instance;

                if (e.colorPointerComp.GazeParameters.UseGaze && e.colorPointerComp.GazeParameters.UseDifferentStates)
                {
                    e.colorPointerComp.CheckGazeStates = true;
                }

                SetupVRComponents(e.colorPointerComp);
            }
        }

        // Update is called once per frame
        protected override void OnUpdate()
        {
            foreach (var e in GetEntities<Filter>())
            {
                // As the vive send errors if the controller are not seen on the first frame, we need to put that in the update method
                if (!e.colorPointerComp.IsSetup)
                {
                    SetupVRComponents(e.colorPointerComp);
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

                if (comp.ControllersParameters.UseControllers)
                {
                    SetupPointers(comp);
                }

                if (comp.GazeParameters.UseGaze)
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
            comp.RightHandPointer.enabled = comp.ControllersParameters.UsePointerRight;
            comp.LeftHandPointer = VRSF_Components.LeftController.GetComponent<LineRenderer>();
            comp.LeftHandPointer.enabled = comp.ControllersParameters.UsePointerLeft;
        }


        /// <summary>
        /// Try to get the background and target images of the Gaze based on the Gaze script.
        /// </summary>
        private void SetupGazeImages(ControllerPointerComponents comp)
        {
            try
            {
                if (!comp.GazeBackground)
                    comp.GazeBackground = GameObject.FindObjectOfType<Gaze.Gaze>().ReticleBackground;

                if (!comp.GazeTarget)
                    comp.GazeTarget = GameObject.FindObjectOfType<Gaze.Gaze>().ReticleTarget;
            }
            catch (System.Exception e)
            {
                Debug.Log("Couldn't find the Gaze script, cannot get the images for the ColorPointer script.\n" + e.ToString());
            }
        }
        #endregion PRIVATE_METHODS
    }
}