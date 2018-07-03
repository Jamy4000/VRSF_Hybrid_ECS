using ScriptableFramework.Events;
using ScriptableFramework.Variables;
using VRSF.Controllers;
using VRSF.Gaze;
using UnityEngine;
using VRSF.Inputs;

namespace VRSF.Interactions
{
    /// <summary>
    /// Script placed on the SetupVR Prefab.
    /// Handle all the click from the button linked that are made on colliders.
    /// </summary>
    public class OnColliderClickedHandler : MonoBehaviour
    {
        // EMPTY
        #region PUBLIC_VARIABLES
        #endregion PUBLIC_VARIABLES


        #region PRIVATE_VARIABLES
        //References to the Controllers and the Gaze Parameters
        private ControllersParametersVariable _controllersParameter;
        private GazeParametersVariable _gazeParameter;

        //References to the Interaction and the Input Variables and GameEvents
        private InputVariableContainer _inputContainer;
        private InteractionVariableContainer _interactionContainer;

        // Reference to the Left and Right Click Variables, meaning the Trigger
        private BoolVariable _leftClick;
        private BoolVariable _rightClick;
        #endregion PRIVATE_VARIABLES


        #region MONOBEHAVIOUR_METHODS
        void Start()
        {
            _controllersParameter = ControllersParametersVariable.Instance;
            _gazeParameter = GazeParametersVariable.Instance;
            _inputContainer = InputVariableContainer.Instance;
            _interactionContainer = InteractionVariableContainer.Instance;

            _leftClick = _inputContainer.LeftClickBoolean.Get("TriggerIsDown");
            _rightClick = _inputContainer.RightClickBoolean.Get("TriggerIsDown");

            // Set to true to avoid error on the first frame.
            _interactionContainer.RightHit.isNull = true;
            _interactionContainer.LeftHit.isNull = true;
            _interactionContainer.GazeHit.isNull = true;

            // As we cannot click without controllers, we disable this script if we don't use them
            if (!_controllersParameter.UseControllers)
                this.enabled = false;
        }

        // Update is called once per frame
        void Update()
        {
            CheckResetClick();
            CheckClick();
        }
        #endregion MONOBEHAVIOUR_METHODS


        //EMPTY
        #region PUBLIC_METHODS
        #endregion PUBLIC_METHODS


        #region PRIVATE_METHODS
        /// <summary>
        /// Check if there's 
        /// </summary>
        void CheckResetClick()
        {
            if (!_rightClick.Value && _interactionContainer.HasClickSomethingRight.Value)
                _interactionContainer.HasClickSomethingRight.SetValue(false);

            if (!_leftClick.Value && _interactionContainer.HasClickSomethingLeft.Value)
                _interactionContainer.HasClickSomethingLeft.SetValue(false);

            if (_gazeParameter.UseGaze && !_inputContainer.GazeIsCliking.Value && _interactionContainer.HasClickSomethingGaze.Value)
                _interactionContainer.HasClickSomethingGaze.SetValue(false);
        }

        /// <summary>
        /// If the click button was pressed for the right or left controller, or the gaze, set the Scriptable Object that match
        /// </summary>
        void CheckClick()
        {
            if (_rightClick.Value && !_interactionContainer.HasClickSomethingRight.Value)
                HandleClick(_interactionContainer.RightHit, _interactionContainer.HasClickSomethingRight, _interactionContainer.RightObjectWasClicked);

            if (_leftClick.Value && !_interactionContainer.HasClickSomethingLeft.Value)
                HandleClick(_interactionContainer.LeftHit, _interactionContainer.HasClickSomethingLeft, _interactionContainer.LeftObjectWasClicked);

            if (_gazeParameter.UseGaze && _inputContainer.GazeIsCliking.Value && !_interactionContainer.HasClickSomethingGaze.Value)
                HandleClick(_interactionContainer.GazeHit, _interactionContainer.HasClickSomethingGaze, _interactionContainer.GazeObjectWasClicked);
        }

        /// <summary>
        /// Handle the raycastHits to check if one object was clicked
        /// </summary>
        /// <param name="hits">The list of RaycastHits to check</param>
        /// <param name="hasClicked">the BoolVariable to set if something got clicked</param>
        /// <param name="objectClicked">The GameEvent to raise with the transform of the hit</param>
        private void HandleClick(RaycastHitVariable hit, BoolVariable hasClicked, GameEventTransform objectClickedEvent)
        {
            //If nothing is hit, we set the isOver value to false
            if (hit.isNull)
            {
                hasClicked.SetValue(false);
            }
            else
            {
                if (hit.Value.collider != null)
                {
                    hasClicked.SetValue(true);
                    objectClickedEvent.Raise(hit.Value.collider.transform);
                }
            }
        }
        #endregion PRIVATE_METHODS
    }
}