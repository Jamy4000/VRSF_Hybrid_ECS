using ScriptableFramework.Variables;
using System;
using System.Linq;
using Unity.Entities;
using UnityEngine;
using VRSF.Controllers;
using VRSF.Gaze;
using VRSF.Interactions;
using VRSF.Utils.Components;

namespace VRSF.Utils.Systems
{
    /// <summary>
    /// Check the Raycast of the two controllers and the Gaze and reference them in RaycastHit and Ray ScriptableVariable
    /// </summary>
    public class PointerRaycastSystems : ComponentSystem
    {
        struct Filter
        {
            public PointerRaycastComponents RaycastComponents;
        }

        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnStartRunning()
        {
            foreach (var entity in GetEntities<Filter>())
            {
                entity.RaycastComponents.ControllersParameters = ControllersParametersVariable.Instance;
                entity.RaycastComponents.GazeParameters = GazeParametersVariable.Instance;
                entity.RaycastComponents.InteractionsContainer = InteractionVariableContainer.Instance;
                
                // We check the raycast if we use the controllers or we use the gaze
                entity.RaycastComponents.CheckRaycast = entity.RaycastComponents.ControllersParameters.UseControllers || entity.RaycastComponents.GazeParameters.UseGaze;
            }
        }

        protected override void OnUpdate()
        {
            foreach (var entity in GetEntities<Filter>())
            {
                if (entity.RaycastComponents.CheckRaycast)
                {
                    if (VRSF_Components.DeviceLoaded == EDevice.SIMULATOR)
                        CheckMouseRays(entity.RaycastComponents);
                    else if (VRSF_Components.DeviceLoaded != EDevice.NULL)
                        CheckVRRays(entity.RaycastComponents);
                }
            }
        }
        #endregion


        //EMPTY
        #region PUBLIC_METHODS
        #endregion PUBLIC_METHODS


        #region PRIVATE_METHODS
        /// <summary>
        /// Check the Rays from the two controllers
        /// </summary>
        private void CheckVRRays(PointerRaycastComponents pointerRaycast)
        {
            // assigning pointerRaycast variable for a better readability
            var interactionContainer = pointerRaycast.InteractionsContainer;
            var controllerParam = pointerRaycast.ControllersParameters;
            var gazeParam = pointerRaycast.GazeParameters;
            
            // Handle raycasting for both controllers
            if (controllerParam.UseControllers)
            {
                try
                {
                    // If the rightPointer State is not off, we check its raycast
                    if (controllerParam.RightPointerState != EPointerState.OFF)
                    {
                        var startTransform = VRSF_Components.RightController.transform;
                        interactionContainer.RightRay.SetValue(new Ray(startTransform.position, startTransform.TransformDirection(Vector3.forward)));

                        RaycastHandler(interactionContainer.RightRay.Value, controllerParam.MaxDistancePointerRight,
                            controllerParam.GetExclusionsLayer(EHand.RIGHT), ref interactionContainer.RightHit);
                    }

                    // If the leftPointer State is not off, we check its raycast
                    if (controllerParam.LeftPointerState != EPointerState.OFF)
                    {
                        var startTransform = VRSF_Components.LeftController.transform;
                        interactionContainer.LeftRay.SetValue(new Ray(startTransform.position, startTransform.TransformDirection(Vector3.forward)));

                        RaycastHandler(interactionContainer.LeftRay.Value, controllerParam.MaxDistancePointerLeft,
                            controllerParam.GetExclusionsLayer(EHand.LEFT), ref interactionContainer.LeftHit);
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("VRSF : VR Components not setup yet, waiting for next frame.\n" + e);
                }
            }

            // Handle raycast for the Gaze
            if (gazeParam.UseGaze)
            {
                try
                {
                    var startTransform = VRSF_Components.VRCamera.transform;
                    interactionContainer.GazeRay.SetValue(new Ray(startTransform.position, startTransform.forward));

                    RaycastHandler(interactionContainer.GazeRay.Value, gazeParam.DefaultDistance,
                        gazeParam.GetGazeExclusionsLayer(), ref interactionContainer.GazeHit);
                }
                catch (Exception e)
                {
                    Debug.Log("VRSF : VR Components not setup yet, waiting for next frame.\n" + e);
                }
            }
        }


        /// <summary>
        /// Check the Ray from the Mouse. We use the normal camera for the three raycast hit, meaning that they are all equals.
        /// </summary>
        private void CheckMouseRays(PointerRaycastComponents pointerRaycast)
        {
            // assigning pointerRaycast variable for a better readability
            var interactionContainer = pointerRaycast.InteractionsContainer;
            var controllerParam = pointerRaycast.ControllersParameters;
            var gazeParam = pointerRaycast.GazeParameters;

            if (controllerParam.UseControllers)
            {
                if (controllerParam.RightPointerState != EPointerState.OFF)
                {
                    interactionContainer.RightRay.SetValue(VRSF_Components.VRCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition));

                    RaycastHandler(interactionContainer.RightRay.Value, controllerParam.MaxDistancePointerRight,
                        controllerParam.GetExclusionsLayer(EHand.RIGHT), ref interactionContainer.RightHit);
                }

                if (controllerParam.LeftPointerState != EPointerState.OFF)
                {
                    interactionContainer.LeftRay.SetValue(VRSF_Components.VRCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition));

                    RaycastHandler(interactionContainer.LeftRay.Value, controllerParam.MaxDistancePointerLeft,
                        controllerParam.GetExclusionsLayer(EHand.LEFT), ref interactionContainer.LeftHit);
                }
            }

            if (gazeParam.UseGaze)
            {
                interactionContainer.GazeRay.SetValue(VRSF_Components.VRCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition));

                RaycastHandler(interactionContainer.GazeRay.Value, gazeParam.DefaultDistance,
                    gazeParam.GetGazeExclusionsLayer(), ref interactionContainer.GazeHit);
            }
        }


        /// <summary>
        /// Check if the Ray from a controller is hitting something
        /// </summary>
        /// <param name="ray">The ray to check</param>
        /// <param name="distance">The maximum distance to which we raycast</param>
        /// <param name="layerToIgnore">The layer(s) to ignore from raycasting</param>
        /// <param name="hitVariable">The RaycastHitVariable in which we store the hit value</param>
        private void RaycastHandler(Ray ray, float distance, int layerToIgnore, ref RaycastHitVariable hitVariable)
        {
            var hits = Physics.RaycastAll(ray, distance, layerToIgnore);

            if (hits.Length > 0)
            {
                var first3DHit = hits.OrderBy(x => x.distance).First();
                hitVariable.SetValue(first3DHit);
                hitVariable.isNull = false;
            }
            else
            {
                hitVariable.isNull = true;
            }
        }
        #endregion PRIVATE_METHODS
    }
}