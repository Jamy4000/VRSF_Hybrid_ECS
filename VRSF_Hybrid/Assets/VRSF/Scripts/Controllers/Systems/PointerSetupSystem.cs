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
            public ControllerPointerComponents ControllerPointerComp;
        }

        #region ComponentSystem_Methods
        // Use this for initialization
        protected override void OnStartRunning()
        {
            foreach (var e in GetEntities<Filter>())
            {
                // Init
                e.ControllerPointerComp.ControllersParameters = ControllersParametersVariable.Instance;
                e.ControllerPointerComp.GazeParameters = GazeParametersVariable.Instance;
                e.ControllerPointerComp.InteractionContainer = InteractionVariableContainer.Instance;

                if (e.ControllerPointerComp.GazeParameters.UseGaze && e.ControllerPointerComp.GazeParameters.UseDifferentStates)
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