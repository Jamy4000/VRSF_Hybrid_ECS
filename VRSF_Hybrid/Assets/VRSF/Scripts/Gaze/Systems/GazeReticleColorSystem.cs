using Unity.Entities;
using VRSF.Controllers;
using VRSF.Gaze.Components;
using VRSF.Interactions;

namespace VRSF.Gaze.Systems
{
    /// <summary>
    /// System to handle the color of the gaze if we use different states, like for the controllers (ON, OFF and SELECTABLE)
    /// </summary>
    public class GazeReticleColorSystem : ComponentSystem
    {
        struct Filter
        {
            public GazeComponent GazeComp;
        }


        #region PRIVATE_VARIABLES
        private GazeParametersVariable _gazeParameters;
        private InteractionVariableContainer _interactionsContainer;
        #endregion PRIVATE_VARIABLES


        #region ComponentSystem_Methods
        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            _gazeParameters = GazeParametersVariable.Instance;
            _interactionsContainer = InteractionVariableContainer.Instance;
        }

        protected override void OnUpdate()
        {
            if (_gazeParameters.UseGaze && _gazeParameters.UseDifferentStates)
            {
                foreach (var e in GetEntities<Filter>())
                {
                    if (e.GazeComp._IsSetup)
                    {
                        SetGazeColorState(e.GazeComp);
                    }
                }
            }
        }
        #endregion ComponentSystem_Methods


        #region PUBLIC_METHODS
        /// <summary>
        /// Set the color of the gaze depending on its state
        /// </summary>
        private void SetGazeColorState(GazeComponent comp)
        {
            // If the Gaze is supposed to be off
            if (_gazeParameters.GazePointerState == EPointerState.OFF)
            {
                if (comp.ReticleBackground != null)
                    comp.ReticleBackground.color = _gazeParameters.ColorOffReticleBackgroud;

                if (comp.ReticleTarget != null)
                    comp.ReticleTarget.color = _gazeParameters.ColorOffReticleTarget;
            }
            // If the Gaze is not over something and it's state is not On
            else if (!_interactionsContainer.IsOverSomethingGaze.Value && _gazeParameters.GazePointerState != EPointerState.ON)
            {
                if (comp.ReticleBackground)
                    comp.ReticleBackground.color = _gazeParameters.ColorOnReticleBackgroud;

                if (comp.ReticleTarget != null)
                    comp.ReticleTarget.color = _gazeParameters.ColorOnReticleTarget;

                _gazeParameters.GazePointerState = EPointerState.ON;
            }
            // If the Gaze is over something and it's state is not at Selectable
            else if (_interactionsContainer.IsOverSomethingGaze.Value && _gazeParameters.GazePointerState != EPointerState.SELECTABLE)
            {
                if (comp.ReticleBackground != null)
                    comp.ReticleBackground.color = _gazeParameters.ColorSelectableReticleBackgroud;

                if (comp.ReticleTarget != null)
                    comp.ReticleTarget.color = _gazeParameters.ColorSelectableReticleTarget;

                _gazeParameters.GazePointerState = EPointerState.SELECTABLE;
            }
        }
        #endregion PUBLIC_METHODS
    }
}