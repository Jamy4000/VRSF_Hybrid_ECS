using ScriptableFramework.Variables;
using Unity.Entities;
using UnityEngine;
using VRSF.Controllers.Components;
using VRSF.Gaze;
using VRSF.Interactions;

namespace VRSF.Controllers.Systems
{
    public class PointerColorSystems : ComponentSystem
    {
        struct Filter
        {
            public ControllerPointerComponents ControllerPointerComp;
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
        }

        // Update is called once per frame
        protected override void OnUpdate()
        {
            foreach (var e in GetEntities<Filter>())
            {
                // As the vive send errors if the controller are not seen on the first frame, we need to put that in the update method
                if (e.ControllerPointerComp.IsSetup)
                {
                    // If we use the controllers, we check their PointerStates
                    if (_controllersParameters.UseControllers)
                    {
                        _controllersParameters.RightPointerState =
                            CheckPointerState(_interactionsContainer.IsOverSomethingRight, _controllersParameters.RightPointerState, e.ControllerPointerComp.RightHandPointer, EHand.RIGHT);

                        _controllersParameters.LeftPointerState =
                            CheckPointerState(_interactionsContainer.IsOverSomethingLeft, _controllersParameters.LeftPointerState, e.ControllerPointerComp.LeftHandPointer, EHand.LEFT);
                    }

                    // If we use the Gaze, we check its PointerState
                    if (_gazeParameters.UseGaze)
                    {
                        CheckGazeState(e.ControllerPointerComp);
                    }
                }
            }
        }
        #endregion ComponentSystem_Methods


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
        private EPointerState CheckPointerState(BoolVariable isOver, EPointerState pointerState, LineRenderer pointer, EHand hand)
        {
            Color on = Color.white;
            Color off = Color.white;
            Color selectable = Color.white;

            GetControllerColor(hand, ref on, ref off, ref selectable);

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
        private void GetControllerColor(EHand hand, ref Color on, ref Color off, ref Color selectable)
        {
            var controllersParam = ControllersParametersVariable.Instance;
            switch (hand)
            {
                case (EHand.RIGHT):
                    on = controllersParam.ColorMatOnRight;
                    off = controllersParam.ColorMatOffRight;
                    selectable = controllersParam.ColorMatSelectableRight;
                    break;

                case (EHand.LEFT):
                    on = controllersParam.ColorMatOnLeft;
                    off = controllersParam.ColorMatOffLeft;
                    selectable = controllersParam.ColorMatSelectableLeft;
                    break;

                default:
                    Debug.LogError("The hand wasn't specified, setting pointer color to white.");
                    break;
            }
        }

        /// <summary>
        /// Check the color of the gaze depending on the checkGazeStates bool
        /// </summary>
        private void CheckGazeState(ControllerPointerComponents comp)
        {
            // If we use different type of states
            if (comp.CheckGazeStates)
            {
                SetGazeColorState(comp);
            }
            else
            {
                if (comp.GazeBackground != null)
                    comp.GazeBackground.color = _gazeParameters.ReticleColor;
                if (comp.GazeBackground != null)
                    comp.GazeTarget.color = _gazeParameters.ReticleTargetColor;
            }
        }

        /// <summary>
        /// Set the color of the gaze depending on its state
        /// </summary>
        private void SetGazeColorState(ControllerPointerComponents comp)
        {
            // If the Gaze is supposed to be off
            if (_gazeParameters.GazePointerState == EPointerState.OFF)
            {
                if (comp.GazeBackground != null)
                    comp.GazeBackground.color = _gazeParameters.ColorOffReticleBackgroud;

                if (comp.GazeTarget != null)
                    comp.GazeTarget.color = _gazeParameters.ColorOffReticleTarget;
            }
            // If the Gaze is not over something and it's state is not On
            else if (!_interactionsContainer.IsOverSomethingGaze.Value && _gazeParameters.GazePointerState != EPointerState.ON)
            {
                if (comp.GazeBackground)
                    comp.GazeBackground.color = _gazeParameters.ColorOnReticleBackgroud;

                if (comp.GazeTarget != null)
                    comp.GazeTarget.color = _gazeParameters.ColorOnReticleTarget;

                _gazeParameters.GazePointerState = EPointerState.ON;
            }
            // If the Gaze is over something and it's state is not at Selectable
            else if (_interactionsContainer.IsOverSomethingGaze.Value && _gazeParameters.GazePointerState != EPointerState.SELECTABLE)
            {
                if (comp.GazeBackground != null)
                    comp.GazeBackground.color = _gazeParameters.ColorSelectableReticleBackgroud;

                if (comp.GazeTarget != null)
                    comp.GazeTarget.color = _gazeParameters.ColorSelectableReticleTarget;

                _gazeParameters.GazePointerState = EPointerState.SELECTABLE;
            }
        }
        #endregion PRIVATE_METHODS
    }
}