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

        #region PRIVATE_VARIABLE
        private GazeParametersVariable _gazeParameters;
        private ControllersParametersVariable _controllersParameters;
        private InteractionVariableContainer _interactionsContainer;
        #endregion PRIVATE_VARIABLE


        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            _controllersParameters = ControllersParametersVariable.Instance;
            _gazeParameters = GazeParametersVariable.Instance;
            _interactionsContainer = InteractionVariableContainer.Instance;

            foreach (var entity in GetEntities<Filter>())
            {
                // We check the raycast if we use the controllers or we use the gaze
                entity.RaycastComponents.CheckRaycast = _controllersParameters.UseControllers || _gazeParameters.UseGaze;
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


        #region PRIVATE_METHODS
        /// <summary>
        /// Check the Rays from the two controllers
        /// </summary>
        private void CheckVRRays(PointerRaycastComponents pointerRaycast)
        {
            // Handle raycasting for both controllers
            if (_controllersParameters.UseControllers)
            {
                try
                {
                    // If the rightPointer State is not off, we check its raycast
                    if (_controllersParameters.RightPointerState != EPointerState.OFF)
                    {
                        var startTransform = VRSF_Components.RightController.transform;
                        _interactionsContainer.RightRay.SetValue(new Ray(startTransform.position, startTransform.TransformDirection(Vector3.forward)));

                        RaycastHandler(_interactionsContainer.RightRay.Value, _controllersParameters.MaxDistancePointerRight,
                            _controllersParameters.GetExclusionsLayer(EHand.RIGHT), ref _interactionsContainer.RightHit);
                    }

                    // If the leftPointer State is not off, we check its raycast
                    if (_controllersParameters.LeftPointerState != EPointerState.OFF)
                    {
                        var startTransform = VRSF_Components.LeftController.transform;
                        _interactionsContainer.LeftRay.SetValue(new Ray(startTransform.position, startTransform.TransformDirection(Vector3.forward)));

                        RaycastHandler(_interactionsContainer.LeftRay.Value, _controllersParameters.MaxDistancePointerLeft,
                            _controllersParameters.GetExclusionsLayer(EHand.LEFT), ref _interactionsContainer.LeftHit);
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("VRSF : VR Components not setup yet, waiting for next frame.\n" + e);
                }
            }

            // Handle raycast for the Gaze
            if (_gazeParameters.UseGaze)
            {
                try
                {
                    var startTransform = VRSF_Components.VRCamera.transform;
                    _interactionsContainer.GazeRay.SetValue(new Ray(startTransform.position, startTransform.forward));

                    RaycastHandler(_interactionsContainer.GazeRay.Value, _gazeParameters.DefaultDistance,
                        _gazeParameters.GetGazeExclusionsLayer(), ref _interactionsContainer.GazeHit);
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
            if (_controllersParameters.UseControllers)
            {
                if (_controllersParameters.RightPointerState != EPointerState.OFF)
                {
                    _interactionsContainer.RightRay.SetValue(VRSF_Components.VRCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition));

                    RaycastHandler(_interactionsContainer.RightRay.Value, _controllersParameters.MaxDistancePointerRight,
                        _controllersParameters.GetExclusionsLayer(EHand.RIGHT), ref _interactionsContainer.RightHit);
                }

                if (_controllersParameters.LeftPointerState != EPointerState.OFF)
                {
                    _interactionsContainer.LeftRay.SetValue(VRSF_Components.VRCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition));

                    RaycastHandler(_interactionsContainer.LeftRay.Value, _controllersParameters.MaxDistancePointerLeft,
                        _controllersParameters.GetExclusionsLayer(EHand.LEFT), ref _interactionsContainer.LeftHit);
                }
            }

            if (_gazeParameters.UseGaze)
            {
                _interactionsContainer.GazeRay.SetValue(VRSF_Components.VRCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition));

                RaycastHandler(_interactionsContainer.GazeRay.Value, _gazeParameters.DefaultDistance,
                    _gazeParameters.GetGazeExclusionsLayer(), ref _interactionsContainer.GazeHit);
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