using VRSF.Controllers;
using VRSF.Gaze;
using UnityEngine;

namespace VRSF.Inputs
{
    /// <summary>
    /// Contains the references for the InputCapture script of the different Scriptable Variables and gameEvents
    /// </summary>
	public class VRInputCapture : MonoBehaviour
    {
        #region PUBLIC_VARIABLES
        public bool CheckGazeInteractions = true;
        #endregion


        #region PRIVATE_VARIABLES
        private ControllersParametersVariable _controllersParameters;
        private GazeParametersVariable _gazeParameters;
        private InputVariableContainer _inputContainer;
        #endregion


        #region MONOBEHAVIOUR_METHODS
        public virtual void Start()
        {
            _controllersParameters = ControllersParametersVariable.Instance;
            _gazeParameters = GazeParametersVariable.Instance;
            _inputContainer = InputVariableContainer.Instance;

            // We disable the input script if the controllers are not used
            if (!_controllersParameters.UseControllers)
                this.enabled = false;
            else
                this.enabled = true;
        }
        #endregion


        // EMPTY
        #region PUBLIC_METHODS

        #endregion


        // EMPTY
        #region PRIVATE_METHODS

        #endregion

            
        #region GETTERS_SETTERS
        public ControllersParametersVariable ControllersParameters
        {
            get
            {
                return _controllersParameters;
            }
        }

        public GazeParametersVariable GazeParameters
        {
            get
            {
                return _gazeParameters;
            }
        }

        public InputVariableContainer InputContainer
        {
            get
            {
                return _inputContainer;
            }
        }
        #endregion
    }
}