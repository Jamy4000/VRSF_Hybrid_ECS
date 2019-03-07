using Unity.Entities;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.Gaze;
using VRSF.Core.Inputs;

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
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnStartRunning()
        {
            base.OnStartRunning();
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
                    if (entity.GazeInputCapture.GazeReferencesSetup)
                    {
                        // If we need to, we check the Gaze Interactions
                        if (entity.GazeInputCapture.CheckGazeInteractions)
                            CheckGazeInputs(entity.GazeInputCapture);
                    }
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
            // Checking Click event
            Debug.LogError("TODO : Redo Chaperone SteamVR");
            //if (!_inputContainer.GazeIsCliking.Value && viveInputCapture.GazeController.GetPressDown(Gaze.GazeInteractionOpenVR.Dictionarry[_gazeParameters.GazeButtonOpenVR]))
            //{
            //    _inputContainer.GazeIsCliking.SetValue(true);
            //    new ButtonClickEvent(EHand.GAZE, _gazeParameters.GazeButtonOpenVR);
            //}
            //else if (_inputContainer.GazeIsCliking.Value && viveInputCapture.GazeController.GetPressUp(Gaze.GazeInteractionOpenVR.Dictionarry[_gazeParameters.GazeButtonOpenVR]))
            //{
            //    _inputContainer.GazeIsCliking.SetValue(false);
            //    new ButtonUnclickEvent(EHand.GAZE, _gazeParameters.GazeButtonOpenVR);
            //}

            //// Checking Touch event
            //if (!_inputContainer.GazeIsTouching.Value && viveInputCapture.GazeController.GetTouchDown(Gaze.GazeInteractionOpenVR.Dictionarry[_gazeParameters.GazeButtonOpenVR]))
            //{
            //    _inputContainer.GazeIsTouching.SetValue(true);
            //    new ButtonTouchEvent(EHand.GAZE, _gazeParameters.GazeButtonOpenVR);
            //}
            //else if (_inputContainer.GazeIsTouching.Value && viveInputCapture.GazeController.GetTouchUp(Gaze.GazeInteractionOpenVR.Dictionarry[_gazeParameters.GazeButtonOpenVR]))
            //{
            //    _inputContainer.GazeIsTouching.SetValue(false);
            //    new ButtonUntouchEvent(EHand.GAZE, _gazeParameters.GazeButtonOpenVR);
            //}
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
                if (_gazeParameters.GazeButtonOpenVR == EControllersButton.NONE)
                {
                    gazeInputCapture.CheckGazeInteractions = false;
                }
                else if (_gazeParameters.GazeButtonOpenVR == EControllersButton.WHEEL_BUTTON)
                {
                    gazeInputCapture.CheckGazeInteractions = false;
                    Debug.LogError("VRSF : Cannot check the Gaze Click with the Wheel Button of the mouse for the Vive.");
                }
                else if (_gazeParameters.GazeButtonOpenVR == EControllersButton.A_BUTTON || _gazeParameters.GazeButtonOpenVR == EControllersButton.B_BUTTON ||
                         _gazeParameters.GazeButtonOpenVR == EControllersButton.X_BUTTON || _gazeParameters.GazeButtonOpenVR == EControllersButton.Y_BUTTON ||
                         _gazeParameters.GazeButtonOpenVR == EControllersButton.THUMBREST)
                {
                    gazeInputCapture.CheckGazeInteractions = false;
                    Debug.LogError("VRSF : Cannot check the Gaze Click with the " + _gazeParameters.GazeButtonOpenVR + " button for the Vive.");
                }
                Debug.LogError("TODO : Redo Gaze Input Capture SteamVR");
                //else if (_gazeParameters.GazeButtonOpenVR.ToString().Contains("RIGHT"))
                //{
                //    gazeInputCapture.GazeController = controllersInputCapture.RightController;
                //}
                //else if (_gazeParameters.GazeButtonOpenVR.ToString().Contains("LEFT"))
                //{
                //    gazeInputCapture.GazeController = controllersInputCapture.LeftController;
                //}

                gazeInputCapture.GazeReferencesSetup = true;
            }
            catch (System.Exception e)
            {
                Debug.Log("VRSF : VR Components not setup yet, waiting for next frame.\n" + e);
            }
        }
        #endregion PRIVATE_METHODS
    }
}
