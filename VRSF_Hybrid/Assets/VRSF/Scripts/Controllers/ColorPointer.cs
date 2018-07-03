using ScriptableFramework.Variables;
using UnityEngine;
using VRSF.Gaze;
using UnityEngine.UI;
using VRSF.Utils;
using VRSF.Interactions;

namespace VRSF.Controllers
{
    /// <summary>
    /// Script attached to the CameraRig, check the color of the Linerenderer attached to the controllers
    /// </summary>
    public class ColorPointer : MonoBehaviour
    {
        // EMPTY
        #region PUBLIC_VARIABLES
        #endregion PUBLIC_VARIABLES


        #region PRIVATE_VARIABLES
        // VRSF Parameters and ScriptableSingletons
        private ControllersParametersVariable _controllersParameters;
        private GazeParametersVariable _gazeParameters;
        private InteractionVariableContainer _interactionContainer;

        // Wheter we need to change the gaze state or not
        private bool _checkGazeStates = false;

        // OPTIONAL : Gaze Reticle Sprites
        private Image _gazeBackground;
        private Image _gazeTarget;

        // LineRenderer attached to the right and left controllers
        private LineRenderer _rightHandPointer;
        private LineRenderer _leftHandPointer;

        private bool _isSetup = false;

        private Transform _cameraRig;
        #endregion PRIVATE_VARIABLES


        #region MONOBEHAVIOUR_METHODS
        private void Start()
        {
            // Init
            _controllersParameters = ControllersParametersVariable.Instance;
            _gazeParameters = GazeParametersVariable.Instance;
            _interactionContainer = InteractionVariableContainer.Instance;

            if (_gazeParameters.UseGaze && _gazeParameters.UseDifferentStates)
            {
                _checkGazeStates = true;
            }

            SetupVRComponents();
        }

        void Update()
        {
            // As the vive send errors if the controller are not seen on the first frame, we need to put that in the update method
            if (!_isSetup)
            {
                SetupVRComponents();
                return;
            }

            // If we use the controllers, we check their PointerStates
            if (_controllersParameters.UseControllers)
            {
                _controllersParameters.RightPointerState = CheckPointer(_interactionContainer.IsOverSomethingRight, _controllersParameters.RightPointerState, _rightHandPointer, EHand.RIGHT);
                _controllersParameters.LeftPointerState = CheckPointer(_interactionContainer.IsOverSomethingLeft, _controllersParameters.LeftPointerState, _leftHandPointer, EHand.LEFT);
                CheckPointersScale();
            }

            // If we use the Gaze, we check its PointerState
            if (_gazeParameters.UseGaze)
            {
                CheckGaze();
            }
        }
        #endregion MONOBEHAVIOUR_METHODS


        //EMPTY
        #region PUBLIC_METHODS
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Check if the pointer is touching the UI
        /// </summary>
        /// <param name="isOver">If the Raycast is over something</param>
        /// <param name="pointerState">The current state of the pointer</param>
        /// <param name="pointer">The linerenderer to which the material is attached</param>
        /// <returns>The new state of the pointer</returns>
        private EPointerState CheckPointer(BoolVariable isOver, EPointerState pointerState, LineRenderer pointer, EHand hand)
        {
            Color on = Color.white;
            Color off = Color.white;
            Color selectable = Color.white;

            GetColor(hand, ref on, ref off, ref selectable);

            // If the pointer is supposed to be off
            if (pointerState == EPointerState.OFF)
            {
                pointer.material.color = off;
                return EPointerState.OFF;
            }
            // If the pointer is not over something and it's state is not On
            else if (!isOver.Value && pointerState != EPointerState.ON)
            {
                pointer.material.color = on;
                return EPointerState.ON;
            }
            // If the pointer is over something and it's state is not at Selectable
            else if (isOver.Value && pointerState != EPointerState.SELECTABLE)
            {
                pointer.material.color = selectable;
                return EPointerState.SELECTABLE;
            }
            return pointerState;
        }

        /// <summary>
        /// Get the color of the Hand pointers by getting the Controllers Parameters
        /// </summary>
        /// <param name="hand">the Hand to check</param>
        /// <param name="on">The color for the On State</param>
        /// <param name="off">The color for the Off State</param>
        /// <param name="selectable">The color for the Selectable State</param>
        private void GetColor(EHand hand, ref Color on, ref Color off, ref Color selectable)
        {
            switch (hand)
            {
                case (EHand.RIGHT):
                    on = _controllersParameters.ColorMatOnRight;
                    off = _controllersParameters.ColorMatOffRight;
                    selectable = _controllersParameters.ColorMatSelectableRight;
                    break;

                case (EHand.LEFT):
                    on = _controllersParameters.ColorMatOnLeft;
                    off = _controllersParameters.ColorMatOffLeft;
                    selectable = _controllersParameters.ColorMatSelectableLeft;
                    break;

                default:
                    Debug.LogError("The hand wasn't specified, setting pointer color to white.");
                    break;
            }
        }

        /// <summary>
        /// Check the color of the gaze depending on the checkGazeStates bool
        /// </summary>
        private void CheckGaze()
        {
            // If we use different type of states
            if (_checkGazeStates)
            {
                SetToColorState();
            }
            else
            {
                if (_gazeBackground != null)
                    _gazeBackground.color = _gazeParameters.ReticleColor;
                if (_gazeBackground != null)
                    _gazeTarget.color = _gazeParameters.ReticleTargetColor;
            }
        }

        /// <summary>
        /// Set the color of the gaze depending on its state
        /// </summary>
        private void SetToColorState()
        {
            // If the Gaze is supposed to be off
            if (_gazeParameters.GazePointerState == EPointerState.OFF)
            {
                if (_gazeBackground != null)
                    _gazeBackground.color = _gazeParameters.ColorOffReticleBackgroud;

                if (_gazeTarget != null)
                    _gazeTarget.color = _gazeParameters.ColorOffReticleTarget;
            }
            // If the Gaze is not over something and it's state is not On
            else if (!_interactionContainer.IsOverSomethingGaze.Value && _gazeParameters.GazePointerState != EPointerState.ON)
            {
                if (_gazeBackground)
                    _gazeBackground.color = _gazeParameters.ColorOnReticleBackgroud;

                if (_gazeTarget != null)
                    _gazeTarget.color = _gazeParameters.ColorOnReticleTarget;

                _gazeParameters.GazePointerState = EPointerState.ON;
            }
            // If the Gaze is over something and it's state is not at Selectable
            else if (_interactionContainer.IsOverSomethingGaze.Value && _gazeParameters.GazePointerState != EPointerState.SELECTABLE)
            {
                if (_gazeBackground != null)
                    _gazeBackground.color = _gazeParameters.ColorSelectableReticleBackgroud;

                if (_gazeTarget != null)
                    _gazeTarget.color = _gazeParameters.ColorSelectableReticleTarget;

                _gazeParameters.GazePointerState = EPointerState.SELECTABLE;
            }
        }

        /// <summary>
        /// Check the scale of the pointer, if the user is going bigger for some reason
        /// transform here is the CameraRig object
        /// </summary>
        private void CheckPointersScale()
        {
            _rightHandPointer.startWidth = _cameraRig.lossyScale.x / 100;
            _rightHandPointer.endWidth = _cameraRig.lossyScale.x / 100;

            _leftHandPointer.startWidth = _cameraRig.lossyScale.x / 100;
            _leftHandPointer.endWidth = _cameraRig.lossyScale.x / 100;
        }


        /// <summary>
        /// Setup the VR Components thanks to the VRSF_Components static class
        /// </summary>
        private void SetupVRComponents()
        {
            try
            {
                _cameraRig = VRSF_Components.CameraRig.transform;

                if (_controllersParameters.UseControllers)
                {
                    SetupPointers();
                }

                if (_gazeParameters.UseGaze)
                {
                    SetupGazeImages();
                }

                _isSetup = true;
            }
            catch (System.Exception e)
            {
                Debug.Log("VRSF : VR Components not setup yet, waiting for next frame.\n" + e);
            }
        }

        /// <summary>
        /// Setup the pointer with the values of the VRSF Interaction Parameters
        /// </summary>
        private void SetupPointers()
        {
            _rightHandPointer = VRSF_Components.RightController.GetComponent<LineRenderer>();
            _rightHandPointer.enabled = _controllersParameters.UsePointerRight;
            _leftHandPointer = VRSF_Components.LeftController.GetComponent<LineRenderer>();
            _leftHandPointer.enabled = _controllersParameters.UsePointerLeft;
        }

        /// <summary>
        /// Try to get the background and target images of the Gaze based on the Gaze script.
        /// </summary>
        private void SetupGazeImages()
        {
            try
            {
                if (!_gazeBackground)
                    _gazeBackground = FindObjectOfType<Gaze.Gaze>().ReticleBackground;

                if (!_gazeTarget)
                    _gazeTarget = FindObjectOfType<Gaze.Gaze>().ReticleTarget;
            }
            catch (System.Exception e)
            {
                Debug.Log("Couldn't find the Gaze script, cannot get the images for the ColorPointer script.\n" + e.ToString());
            }
        }
        #endregion PRIVATE_METHODS
    }
}