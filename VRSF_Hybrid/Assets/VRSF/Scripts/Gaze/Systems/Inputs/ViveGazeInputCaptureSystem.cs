using Unity.Entities;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.Gaze;
using VRSF.Core.Inputs;
using VRSF.Utils;

namespace VRSF.Gaze.Inputs
{
    /// <summary>
    /// Script attached to the ViveSDK Prefab.
    /// Set the GameEvent depending on the Vive Inputs.
    /// </summary>
    public class ViveGazeInputCaptureSystem : ComponentSystem
    {
        struct ViveFilter
        {
            public ViveGazeInputCaptureComponent GazeInputCapture;
            public ViveControllersInputCaptureComponent ControllersInputCapture;
        }

        #region PRIVATE_VARIABLES
        // VRSF Parameters references
        private GazeParametersVariable _gazeParameters;
        private InputVariableContainer _inputContainer;
        #endregion PRIVATE_VARIABLES


        #region ComponentSystem_Methods
        protected override void OnCreateManager()
        {
            base.OnCreateManager();
            _gazeParameters = GazeParametersVariable.Instance;
            _inputContainer = InputVariableContainer.Instance;
        }

        protected override void OnUpdate()
        {
            // If we doesn't use the controllers, we don't check for the inputs.
            if (ControllersParametersVariable.Instance.UseControllers)
            {
                foreach (var entity in GetEntities<ViveFilter>())
                {
                    if (entity.GazeInputCapture.GazeReferencesSetup && entity.GazeInputCapture.CheckGazeInteractions)
                        CheckGazeInputs(entity.GazeInputCapture);
                }
            }
        }
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Handle the Gaze click button, in the simulator case, the mouse wheel
        /// </summary>
        private void CheckGazeInputs(ViveGazeInputCaptureComponent viveInputCapture)
        {
            Debug.LogError("TODO : Not sure it's working using EHand.GAZE, need to test");
            // Checking Click event
            if (!_inputContainer.GazeIsCliking.Value && Input.GetButton(GazeInteractionVive.ClickDictionnary[new STuples<EControllersButton, EHand>(_gazeParameters.GazeButtonVive, EHand.GAZE)]))
            {
                _inputContainer.GazeIsCliking.SetValue(true);
                _inputContainer.GazeIsTouching.SetValue(false);
                new ButtonClickEvent(EHand.GAZE, _gazeParameters.GazeButtonVive);
            }
            else if (_inputContainer.GazeIsCliking.Value && !Input.GetButton(GazeInteractionVive.ClickDictionnary[new STuples<EControllersButton, EHand>(_gazeParameters.GazeButtonVive, EHand.GAZE)]))
            {
                _inputContainer.GazeIsCliking.SetValue(false);
                new ButtonUnclickEvent(EHand.GAZE, _gazeParameters.GazeButtonVive);
            }

            // Checking Touch event
            if (!_inputContainer.GazeIsTouching.Value && Input.GetButton(GazeInteractionVive.TouchDictionnary[new STuples<EControllersButton, EHand>(_gazeParameters.GazeButtonVive, EHand.GAZE)]))
            {
                _inputContainer.GazeIsTouching.SetValue(true);
                new ButtonTouchEvent(EHand.GAZE, _gazeParameters.GazeButtonVive);
            }
            else if (_inputContainer.GazeIsTouching.Value && !Input.GetButton(GazeInteractionVive.TouchDictionnary[new STuples<EControllersButton, EHand>(_gazeParameters.GazeButtonVive, EHand.GAZE)]))
            {
                _inputContainer.GazeIsTouching.SetValue(false);
                new ButtonUntouchEvent(EHand.GAZE, _gazeParameters.GazeButtonVive);
            }
        }

        /// <summary>
        /// Set the references in the viveInputCaptureComponent script. 
        /// We need to do it in an update method as the two Vive controllers need to be seen by the sensors to be referenced. 
        /// </summary>
        /// <param name="gazeInputCapture">The ViveInputCaptureComponent on the CameraRig Entity</param>
        private void CheckGazeReferencesVive(ViveGazeInputCaptureComponent gazeInputCapture, ViveControllersInputCaptureComponent controllersInputCapture)
        {
            try
            {
                // We check the Gaze Click Button
                if (_gazeParameters.GazeButtonVive == EControllersButton.NONE)
                {
                    gazeInputCapture.CheckGazeInteractions = false;
                }
                else if (_gazeParameters.GazeButtonVive == EControllersButton.WHEEL_BUTTON)
                {
                    gazeInputCapture.CheckGazeInteractions = false;
                    Debug.LogError("<b>[VRSF] :</b> Cannot check the Gaze Click with the Wheel Button of the mouse for the Vive.");
                }
                else if (_gazeParameters.GazeButtonVive == EControllersButton.A_BUTTON || _gazeParameters.GazeButtonVive == EControllersButton.B_BUTTON ||
                         _gazeParameters.GazeButtonVive == EControllersButton.X_BUTTON || _gazeParameters.GazeButtonVive == EControllersButton.Y_BUTTON ||
                         _gazeParameters.GazeButtonVive == EControllersButton.THUMBREST)
                {
                    gazeInputCapture.CheckGazeInteractions = false;
                    Debug.LogError("<b>[VRSF] :</b> Cannot check the Gaze Click with the " + _gazeParameters.GazeButtonVive + " button for the Vive.");
                }

                gazeInputCapture.GazeReferencesSetup = true;
            }
            catch (System.Exception e)
            {
                Debug.Log("<b>[VRSF] :</b> VR Components not setup yet, waiting for next frame.\n" + e);
            }
        }
        #endregion PRIVATE_METHODS
    }
}
