using ScriptableFramework.Variables;
using Unity.Entities;
using UnityEngine;
using VRSF.Controllers.Components;
using VRSF.Interactions;

namespace VRSF.Controllers.Systems
{
    /// <summary>
    /// Make the Pointer appear only when it's not on Exluded Layer
    /// </summary>
    public class PointerStatusSystem : ComponentSystem
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
            _controllersParameters.RightPointerState = _controllersParameters.UsePointerRight ? EPointerState.ON : EPointerState.OFF;
            _controllersParameters.LeftPointerState = _controllersParameters.UsePointerLeft ? EPointerState.ON : EPointerState.OFF;
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
                        if (_controllersParameters.RightPointerState != EPointerState.OFF)
                            CheckPointerState(ref _controllersParameters.RightPointerState, e.ControllerPointerComp._RightHandPointer, e.ControllerPointerComp._LeftParticles, _interactionsContainer.RightHit);

                        if (_controllersParameters.LeftPointerState != EPointerState.OFF)
                            CheckPointerState(ref _controllersParameters.LeftPointerState, e.ControllerPointerComp._LeftHandPointer, e.ControllerPointerComp._LeftParticles, _interactionsContainer.LeftHit);
                    }
                }
            }
        }
        #endregion ComponentSystem_Methods


        #region PRIVATE_METHODS
        /// <summary>
        /// Check if the pointer is on a non Excluded Layer
        /// </summary>
        /// <param name="isOver">If the Raycast is over something</param>
        /// <param name="pointerState">The current state of the pointer</param>
        /// <param name="pointer">The linerenderer to which the material is attached</param>
        /// <returns>The new state of the pointer</returns>
        private void CheckPointerState(ref EPointerState pointerState, LineRenderer pointer, ParticleSystem[] particleSystem, RaycastHitVariable hitVariable)
        {
            // If the pointer is over something and it's state is not at Selectable
            if (pointerState != EPointerState.SELECTABLE && hitVariable.RaycastHitIsOnUI())
            {
                pointer.enabled = true;
                if (particleSystem != null)
                {
                    foreach (var ps in particleSystem)
                    {
                        ps.Play();
                    }
                }
                pointerState = EPointerState.SELECTABLE;
            }
            else if (pointerState != EPointerState.ON)
            {
                pointer.enabled = false;
                if (particleSystem != null)
                {
                    foreach (var ps in particleSystem)
                    {
                        ps.Stop();
                        ps.Clear();
                    }
                }
                pointerState = EPointerState.ON;
            }
        }
        #endregion PRIVATE_METHODS
    }
}