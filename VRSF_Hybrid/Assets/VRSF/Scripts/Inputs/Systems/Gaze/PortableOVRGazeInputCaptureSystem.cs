using Unity.Entities;
using UnityEngine;
using VRSF.Controllers;
using VRSF.Gaze;
using VRSF.Inputs.Events;
using VRSF.Inputs.Gaze;
using VRSF.Utils;

namespace VRSF.Inputs.Systems
{
    /// <summary>
    /// Script attached to the Portable OVR Prefab.
    /// Set the GameEvent depending on the Portable OVR Inputs
    /// </summary>
    public class PortableOVRGazeInputCaptureSystem : ComponentSystem
    {
        struct Filter
        {
            public PortableOVRGazeInputCaptureComponent GazeInputCapture;
        }

        #region PRIVATE_VARIABLES
        // VRSF Parameters references
        private GazeParametersVariable _gazeParameters;
        private InputVariableContainer _inputContainer;
        #endregion PRIVATE_VARIABLES


        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            _gazeParameters = GazeParametersVariable.Instance;
            _inputContainer = InputVariableContainer.Instance;
            
            foreach (var entity in GetEntities<Filter>())
            {
                CheckGazeClickButton(entity.GazeInputCapture);
            }
        }

        protected override void OnUpdate()
        {
            // If we doesn't use the controllers, we don't check for the inputs.
            if (ControllersParametersVariable.Instance.UseControllers)
            {
                foreach (var entity in GetEntities<Filter>())
                {
                    if (entity.GazeInputCapture.CheckGazeInteractions)
                        CheckGazeInputs();
                }
            }
        }
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Handle the Gaze click button, in the simulator case, the mouse wheel
        /// </summary>
        private void CheckGazeInputs()
        {
            // Checking Click event
            if (!_inputContainer.GazeIsCliking.Value && OVRInput.Get(GazeInteractionPortableOVR.ClickDictionnary[new STuples<EControllersButton, EHand>(_gazeParameters.GazeButtonPortableOVR, EHand.GAZE)]))
            {
                _inputContainer.GazeIsCliking.SetValue(true);
                new ButtonClickEvent(EHand.GAZE, _gazeParameters.GazeButtonPortableOVR);
            }
            else if (_inputContainer.GazeIsCliking.Value && !OVRInput.Get(GazeInteractionPortableOVR.ClickDictionnary[new STuples<EControllersButton, EHand>(_gazeParameters.GazeButtonPortableOVR, EHand.GAZE)]))
            {
                _inputContainer.GazeIsCliking.SetValue(false);
                new ButtonUnclickEvent(EHand.GAZE, _gazeParameters.GazeButtonPortableOVR);
            }

            // Checking Touch event
            if (!_inputContainer.GazeIsTouching.Value && OVRInput.Get(GazeInteractionPortableOVR.TouchDictionnary[new STuples<EControllersButton, EHand>(_gazeParameters.GazeButtonPortableOVR, EHand.GAZE)]))
            {
                _inputContainer.GazeIsTouching.SetValue(true);
                new ButtonTouchEvent(EHand.GAZE, _gazeParameters.GazeButtonPortableOVR);
            }
            else if (_inputContainer.GazeIsTouching.Value && !OVRInput.Get(GazeInteractionPortableOVR.TouchDictionnary[new STuples<EControllersButton, EHand>(_gazeParameters.GazeButtonPortableOVR, EHand.GAZE)]))
            {
                _inputContainer.GazeIsTouching.SetValue(false);
                new ButtonUntouchEvent(EHand.GAZE, _gazeParameters.GazeButtonPortableOVR);
            }
        }


        /// <summary>
        /// Check if the specified button in the Gaze Parameters Window was set correctly 
        /// </summary>
        private void CheckGazeClickButton(PortableOVRGazeInputCaptureComponent gazeInputCapture)
        {
            switch (_gazeParameters.GazeButtonPortableOVR)
            {
                case EControllersButton.TRIGGER:
                case EControllersButton.THUMBSTICK:
                case EControllersButton.BACK_BUTTON:
                    // Everything's good, the button chosen was ok
                    if (!_gazeParameters.UseGaze)
                    {
                        gazeInputCapture.CheckGazeInteractions = false;
                    }
                    break;
                default:
                    gazeInputCapture.CheckGazeInteractions = false;
                    Debug.LogError("VRSF : Cannot check the Gaze Click with the Button you have choosen.\n" +
                        _gazeParameters.GazeButtonPortableOVR + " is not a button present on the Tracked Remote.");
                    break;
            }
        }
        #endregion
    }
}