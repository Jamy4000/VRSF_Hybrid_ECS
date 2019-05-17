using Unity.Entities;
using VRSF.Core.Controllers;
using VRSF.Core.Events;
using VRSF.Core.Gaze;
using VRSF.Core.Inputs;
using VRSF.Core.Interactions;
using VRSF.Core.Raycast;
using VRSF.Interactions;

namespace VRSF.Gaze.Interactions
{
    public class OnColliderGazeClickSystem : ComponentSystem
    {
        struct Filter
        {
            public OnColliderClickComponent OnClickComp;
            public ScriptableRaycastComponent PointerRaycast;
        }

        private GazeParametersVariable _gazeParam;
        private InputVariableContainer _inputContainer;
        private InteractionVariableContainer _interactionContainer;

        #region ComponentSystem_Methods
        protected override void OnCreateManager()
        {
            base.OnCreateManager();
            _gazeParam = GazeParametersVariable.Instance;
            _inputContainer = InputVariableContainer.Instance;
            _interactionContainer = InteractionVariableContainer.Instance;
        }

        protected override void OnUpdate()
        {
            if (_gazeParam.UseGaze)
            {
                foreach (var entity in GetEntities<Filter>())
                {
                    if (entity.PointerRaycast.CheckRaycast)
                    {
                        CheckResetClick();

                        if (!_inputContainer.GazeIsCliking.Value && _interactionContainer.HasClickSomethingGaze.Value)
                        {
                            HandleClick();
                        }
                    }
                }
            }
        }
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Check if there's 
        /// </summary>
        void CheckResetClick()
        {
            if (!_inputContainer.GazeIsCliking.Value && _interactionContainer.HasClickSomethingGaze.Value)
                _interactionContainer.HasClickSomethingGaze.SetValue(false);
        }

        /// <summary>
        /// Handle the raycastHits to check if one object was clicked
        /// </summary>
        /// <param name="hits">The list of RaycastHits to check</param>
        /// <param name="hasClicked">the BoolVariable to set if something got clicked</param>
        /// <param name="objectClicked">The GameEvent to raise with the transform of the hit</param>
        private void HandleClick()
        {
            //If nothing is hit, we set the isOver value to false
            if (_interactionContainer.GazeHit.IsNull)
            {
                _interactionContainer.HasClickSomethingGaze.SetValue(false);
            }
            else
            {
                _interactionContainer.HasClickSomethingGaze.SetValue(true);

                var objectClicked = _interactionContainer.GazeHit.Value.collider.transform;
                new ObjectWasClickedEvent(EHand.GAZE, objectClicked);
            }
        }
        #endregion PRIVATE_METHODS
    }
}