using ScriptableFramework.Variables;
using Unity.Entities;
using UnityEngine;
using VRSF.Controllers.Components;
using VRSF.Utils;

namespace VRSF.Controllers.Systems
{
    public class PointerLengthSystems : ComponentSystem
    {
        struct Filter
        {
            public ControllerPointerComponents ControllerPointerComp;
        }

        #region ComponentSystem_Methods
        // Update is called once per frame
        protected override void OnUpdate()
        {
            foreach (var e in GetEntities<Filter>())
            {
                // As the vive send errors if the controller are not seen on the first frame, we need to put that in the update method
                if (e.ControllerPointerComp.IsSetup)
                {
                    if (e.ControllerPointerComp.ControllersParameters.UseControllers)
                    {
                        SetControllerRayLength(e.ControllerPointerComp.InteractionContainer.LeftHit, VRSF_Components.LeftController, EHand.LEFT, e.ControllerPointerComp);
                        SetControllerRayLength(e.ControllerPointerComp.InteractionContainer.RightHit, VRSF_Components.RightController, EHand.RIGHT, e.ControllerPointerComp);
                    }

                    if (e.ControllerPointerComp.GazeParameters.UseGaze && e.ControllerPointerComp.GazeScript != null)
                    {
                        CheckGazePosition(e.ControllerPointerComp);
                    }
                }
            }
        }
        #endregion ComponentSystem_Methods

        #region PRIVATE_METHODS
        /// <summary>
        /// Set the size of the line renderer depending on the hit from the RayCast.
        /// </summary>
        /// <param name="hit">The RaycastHitVariable containing the RaycastHit for the controller</param>
        /// <param name="controller">The controller GameObject from which the ray started</param>
        /// <param name="hand">The hand rom which we are checking the raycastHit</param>
        private void SetControllerRayLength(RaycastHitVariable hit, GameObject controller, EHand hand, ControllerPointerComponents comp)
        {
            try
            {
                if (!hit.isNull)
                {
                    //Reduce lineRenderer from the controllers position to the object that was hit
                    controller.GetComponent<LineRenderer>().SetPositions(new Vector3[]
                    {
                            new Vector3(0.0f, 0.0f, 0.03f),
                            controller.transform.InverseTransformPoint(hit.Value.point),
                    });
                    return;
                }

                // Checking max distance of Line renderer depending on the Hand Variable
                var maxDistanceLr = (hand == EHand.LEFT
                    ? comp.ControllersParameters.MaxDistancePointerLeft
                    : comp.ControllersParameters.MaxDistancePointerRight);

                //put back lineRenderer to its normal length if nothing was hit
                controller.GetComponent<LineRenderer>().SetPositions(new Vector3[]
                {
                        new Vector3(0.0f, 0.0f, 0.03f),
                        new Vector3(0, 0, maxDistanceLr),
                });
            }
            catch (System.Exception e)
            {
                Debug.Log("VRSF : VR Components not setup yet, waiting for next frame.\n" + e);
            }
        }

        /// <summary>
        /// Check if the Gaze ray has hit something on the way
        /// </summary>
        private void CheckGazePosition(ControllerPointerComponents comp)
        {
            if (!comp.InteractionContainer.GazeHit.isNull)
            {
                //Reduce the reticle positon to the object that was hit
                comp.GazeScript.SetPosition(comp.InteractionContainer.GazeHit.Value);
                return;
            }

            //put back the reticle positon to its normal distance if nothing was hit
            comp.GazeScript.SetPositionToNormal();
        }
        #endregion PRIVATE_METHODS
    }
}