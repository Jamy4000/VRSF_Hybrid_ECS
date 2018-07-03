using System;
using ScriptableFramework.Variables;
using UnityEngine;
using VRSF.Gaze;
using VRSF.Interactions;
using VRSF.Utils;

namespace VRSF.Controllers
{
    /// <summary>
    /// Script attached to the CameraRig of each VR SDK Prefab.
    /// Check if the PointerRayCast has hit something.
    /// </summary>
    public class CheckRayLength : MonoBehaviour
    {
        // EMPTY
        #region PUBLIC_VARIABLE
        #endregion PUBLIC_VARIABLE


        #region PRIVATE_VARIABLE
        // The Controllers and Gaze Parameters
        private ControllersParametersVariable _controllersParameter;
        private GazeParametersVariable _gazeParameter;

        // The Interaction variable and GameEvents Container
        private InteractionVariableContainer _interactionContainer;

        // OPTIONAL : The reticle for the Gaze
        private Gaze.Gaze _reticle;

        private bool _isSetup = false;
        #endregion PRIVATE_VARIABLE


        #region MONOBEHAVIOUR_METHODS
        private void Start()
        {
            _controllersParameter = ControllersParametersVariable.Instance;
            _gazeParameter = GazeParametersVariable.Instance;
            _interactionContainer = InteractionVariableContainer.Instance;

            // We check if the user is at least using the gaze or the controllers
            if (!_controllersParameter.UseControllers && !_gazeParameter.UseGaze)
                this.enabled = false;

            SetupVRComponents();
        }

        private void Update()
        {
            if (!_isSetup)
            {
                SetupVRComponents();
                return;
            }

            if (_controllersParameter.UseControllers)
            { 
                SetControllerRayLength(_interactionContainer.LeftHit, VRSF_Components.LeftController, EHand.LEFT);
                SetControllerRayLength(_interactionContainer.RightHit, VRSF_Components.RightController, EHand.RIGHT);
            }

            if (_gazeParameter.UseGaze && _reticle != null)
            {
                CheckGaze();
            }
        }
        #endregion MONOBEHAVIOUR_METHODS


        //EMPTY
        #region PUBLIC_METHODS
        #endregion PUBLIC_METHODS


        #region PRIVATE_METHODS
        /// <summary>
        /// Set the size of the line renderer depending on the hit from the RayCast.
        /// </summary>
        /// <param name="hit">The RaycastHitVariable containing the RaycastHit for the controller</param>
        /// <param name="controller">The controller GameObject from which the ray started</param>
        /// <param name="hand">The hand rom which we are checking the raycastHit</param>
        private void SetControllerRayLength(RaycastHitVariable hit, GameObject controller, EHand hand)
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
                    ? _controllersParameter.MaxDistancePointerLeft
                    : _controllersParameter.MaxDistancePointerRight);

                //put back lineRenderer to its normal length if nothing was hit
                controller.GetComponent<LineRenderer>().SetPositions(new Vector3[]
                {
                    new Vector3(0.0f, 0.0f, 0.03f),
                    new Vector3(0, 0, maxDistanceLr),
                });
            }
            catch (Exception e)
            {
                Debug.Log("VRSF : VR Components not setup yet, waiting for next frame.\n" + e);
            }
        }

        /// <summary>
        /// Check if the Gaze ray has hit something on the way
        /// </summary>
        private void CheckGaze()
        {
            if (!_interactionContainer.GazeHit.isNull)
            {
                //Reduce the reticle positon to the object that was hit
                _reticle.SetPosition(_interactionContainer.GazeHit.Value);
                return;
            }

            //put back the reticle positon to its normal distance if nothing was hit
            _reticle.SetPositionToNormal();
        }

        /// <summary>
        /// Setup the VR Components References at runtime
        /// </summary>
        private void SetupVRComponents()
        {
            try
            {
                if (_gazeParameter.UseGaze)
                {
                    _reticle = FindObjectOfType<Gaze.Gaze>();
                }
                _isSetup = true;
            }
            catch (System.Exception e)
            {
                Debug.Log("VRSF : VR Components not setup yet, waiting for next frame.\n" + e);
            }
        }
        #endregion PRIVATE_METHODS
    }
}