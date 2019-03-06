using Unity.Entities;
using UnityEngine;
using VRSF.Controllers;
using VRSF.Gaze;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Script attached to the OculusSDK Prefab.
    /// Set the GameEvent depending on the Oculus Inputs
    /// </summary>
    public class RiftGazeInputCaptureSystem : ComponentSystem
    {
        struct Filter
        {
            public RiftGazeInputCaptureComponent GazeInputCapture;
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
            Debug.LogError("TODO : Redo Gaze Input Capture for Oculus");
            // Checking Click event
            //if (!_inputContainer.GazeIsCliking.Value && OVRInput.Get(GazeInteractionRift.ClickDictionnary[new STuples<EControllersButton, EHand>(_gazeParameters.GazeButtonRift, EHand.GAZE)]))
            //{
            //    _inputContainer.GazeIsCliking.SetValue(true);
            //    new ButtonClickEvent(EHand.GAZE, _gazeParameters.GazeButtonRift);
            //}
            //else if (_inputContainer.GazeIsCliking.Value && !OVRInput.Get(GazeInteractionRift.ClickDictionnary[new STuples<EControllersButton, EHand>(_gazeParameters.GazeButtonRift, EHand.GAZE)]))
            //{
            //    _inputContainer.GazeIsCliking.SetValue(false);
            //    new ButtonUnclickEvent(EHand.GAZE, _gazeParameters.GazeButtonRift);
            //}

            //// Checking Touch event
            //if (!_inputContainer.GazeIsTouching.Value && OVRInput.Get(GazeInteractionRift.TouchDictionnary[new STuples<EControllersButton, EHand>(_gazeParameters.GazeButtonRift, EHand.GAZE)]))
            //{
            //    _inputContainer.GazeIsTouching.SetValue(true);
            //    new ButtonTouchEvent(EHand.GAZE, _gazeParameters.GazeButtonRift);
            //}
            //else if (_inputContainer.GazeIsTouching.Value && !OVRInput.Get(GazeInteractionRift.TouchDictionnary[new STuples<EControllersButton, EHand>(_gazeParameters.GazeButtonRift, EHand.GAZE)]))
            //{
            //    _inputContainer.GazeIsTouching.SetValue(false);
            //    new ButtonUntouchEvent(EHand.GAZE, _gazeParameters.GazeButtonRift);
            //}
        }


        /// <summary>
        /// Check if the specified button in the Gaze Parameters Window was set correctly 
        /// </summary>
        private void CheckGazeClickButton(RiftGazeInputCaptureComponent gazeInputCapture)
        {
            if ((_gazeParameters.GazeButtonRift == EControllersButton.NONE) || !_gazeParameters.UseGaze)
            {
                gazeInputCapture.CheckGazeInteractions = false;
            }
            else if (_gazeParameters.GazeButtonRift == EControllersButton.WHEEL_BUTTON)
            {
                gazeInputCapture.CheckGazeInteractions = false;
                Debug.LogError("VRSF : Cannot check the Gaze Click with the Wheel Button of the mouse for the Oculus.");
            }
        }
        #endregion
    }
}