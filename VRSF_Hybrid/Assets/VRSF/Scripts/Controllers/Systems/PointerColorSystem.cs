using ScriptableFramework.Variables;
using Unity.Entities;
using UnityEngine;
using VRSF.Controllers.Components;
using VRSF.Interactions;

namespace VRSF.Controllers.Systems
{
    /// <summary>
    /// Handle the color of the controllers pointer
    /// </summary>
    public class PointerColorSystem : ComponentSystem
    {
        struct Filter
        {
            public ControllerPointerComponents ControllerPointerComp;
        }


        #region PRIVATE_VARIABLE
        private ControllersParametersVariable _controllersParameters;
        private InteractionVariableContainer _interactionsContainer;
        #endregion PRIVATE_VARIABLE


        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            _controllersParameters = ControllersParametersVariable.Instance;
            _interactionsContainer = InteractionVariableContainer.Instance;
        }

        // Update is called once per frame
        protected override void OnUpdate()
        {
            // If we use the controllers, we check their PointerStates
            if (_controllersParameters.UseControllers)
            {
                foreach (var e in GetEntities<Filter>())
                {
                    // As the vive send errors if the controller are not seen on the first frame, we need to put that in the update method
                    if (e.ControllerPointerComp._IsSetup)
                    {
                        _controllersParameters.RightPointerState =
                            CheckPointerState(_interactionsContainer.IsOverSomethingRight, _controllersParameters.RightPointerState, e.ControllerPointerComp._RightHandPointer, EHand.RIGHT);

                        _controllersParameters.LeftPointerState =
                            CheckPointerState(_interactionsContainer.IsOverSomethingLeft, _controllersParameters.LeftPointerState, e.ControllerPointerComp._LeftHandPointer, EHand.LEFT);
                    }
                }
            }
        }
        #endregion ComponentSystem_Methods


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
                pointer.material.SetColor("_MainColor", off);
                return EPointerState.OFF;
            }
            // If the pointer is not over something and it's state is not On
            else if (!isOver.Value && pointerState != EPointerState.ON)
            {
                pointer.material.SetColor("_MainColor", on);
                return EPointerState.ON;
            }
            // If the pointer is over something and it's state is not at Selectable
            else if (isOver.Value && pointerState != EPointerState.SELECTABLE)
            {
                pointer.material.SetColor("_MainColor", selectable);
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
                    Debug.LogError("The hand wasn't specified, setting pointer color to RGB Colors.");
                    on = Color.blue;
                    off = Color.red;
                    selectable = Color.green;
                    break;
            }
        }
        #endregion PRIVATE_METHODS
    }
}