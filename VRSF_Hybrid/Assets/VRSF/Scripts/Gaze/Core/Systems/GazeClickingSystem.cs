using Unity.Entities;
using VRSF.Core.Events;
using VRSF.Core.Inputs;
using VRSF.Core.Raycast;
using VRSF.Interactions;

namespace VRSF.Gaze.Interactions
{
    public class GazeClickingSystem : ComponentSystem
    {
        struct Filter
        {
            public GazeInputsComponent GazeInputs;
        }

        private InteractionVariableContainer _interactionsVariables;

        #region ComponentSystem_Methods
        protected override void OnCreateManager()
        {
            base.OnCreateManager();
            _interactionsVariables = InteractionVariableContainer.Instance;
        }

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            // Just checking if there's entities in the scene
            foreach (var e in GetEntities<Filter>())
            {
                ButtonClickEvent.Listeners += CheckObjectClicked;
                ButtonUnclickEvent.Listeners += ResetVariable;
                return;
            }
        }

        protected override void OnUpdate() { }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
            // Just checking if there's entities in the scene
            foreach (var e in GetEntities<Filter>())
            {
                ButtonClickEvent.Listeners -= CheckObjectClicked;
                ButtonUnclickEvent.Listeners -= ResetVariable;
                return;
            }
        }
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Reset the HasClickSomethingGaze bool if the user is not clicking anymore
        /// </summary>
        void CheckObjectClicked(ButtonClickEvent info)
        {
            foreach (var e in GetEntities<Filter>())
            {
                if (info.ButtonInteracting == e.GazeInputs.GazeButton && info.HandInteracting == e.GazeInputs.GazeButtonHand)
                {
                    //If nothing is hit, we set the isOver value to false
                    if (_interactionsVariables.GazeHit.IsNull)
                    {
                        _interactionsVariables.HasClickSomethingGaze.SetValue(false);
                    }
                    else
                    {
                        _interactionsVariables.HasClickSomethingGaze.SetValue(true);

                        var objectClicked = _interactionsVariables.GazeHit.Value.collider.transform;
                        new ObjectWasClickedEvent(ERayOrigin.CAMERA, objectClicked);
                    }
                }
            }
        }

        void ResetVariable(ButtonUnclickEvent info)
        {
            foreach (var e in GetEntities<Filter>())
            {
                if (info.ButtonInteracting == e.GazeInputs.GazeButton && info.HandInteracting == e.GazeInputs.GazeButtonHand)
                {
                    _interactionsVariables.HasClickSomethingGaze.SetValue(false);
                }
            }
        }
        #endregion PRIVATE_METHODS
    }
}