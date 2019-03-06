using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRSF.Core.Inputs;
using VRSF.Interactions;

namespace VRSF.Utils.Systems
{
    public class ScriptableVariableResetterSystem : ComponentSystem
    {
        protected override void OnCreateManager()
        {
            base.OnCreateManager();

            ResetInputContainer();
            ResetInteractionContainer();

            SceneManager.sceneLoaded += OnSceneLoaded;

            this.Enabled = false;
        }

        protected override void OnUpdate() { }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void ResetInputContainer()
        {
            var inputContainer = InputVariableContainer.Instance;

            inputContainer?.GazeIsCliking?.SetValue(false);
            inputContainer?.GazeIsTouching?.SetValue(false);

            inputContainer?.WheelIsClicking?.SetValue(false);

            inputContainer?.RightThumbPosition?.SetValue(new Vector2(0, 0));
            inputContainer?.LeftThumbPosition?.SetValue(new Vector2(0, 0));
            
            foreach (var entry in inputContainer.RightClickBoolean.Items)
            {
                entry.Value?.SetValue(false);
            }

            foreach (var entry in inputContainer.LeftClickBoolean.Items)
            {
                entry.Value?.SetValue(false);
            }

            foreach (var entry in inputContainer.RightTouchBoolean.Items)
            {
                entry.Value?.SetValue(false);
            }

            foreach (var entry in inputContainer.LeftTouchBoolean.Items)
            {
                entry.Value?.SetValue(false);
            }
        }

        private void ResetInteractionContainer()
        {
            var interactionContainer = InteractionVariableContainer.Instance;

            interactionContainer.HasClickSomethingGaze?.SetValue(false);
            interactionContainer.HasClickSomethingLeft?.SetValue(false);
            interactionContainer.HasClickSomethingRight?.SetValue(false);


            interactionContainer.IsOverSomethingGaze?.SetValue(false);
            interactionContainer.IsOverSomethingLeft?.SetValue(false);
            interactionContainer.IsOverSomethingRight?.SetValue(false);


            interactionContainer.RightRay?.SetValue(new Ray());
            interactionContainer.LeftRay?.SetValue(new Ray());
            interactionContainer.GazeRay?.SetValue(new Ray());


            interactionContainer.RightHit?.SetValue(new RaycastHit());
            interactionContainer.RightHit?.SetIsNull(true);

            interactionContainer.LeftHit?.SetValue(new RaycastHit());
            interactionContainer.LeftHit?.SetIsNull(true);

            interactionContainer.GazeHit?.SetValue(new RaycastHit());
            interactionContainer.GazeHit?.SetIsNull(true);

            interactionContainer.PreviousRightHit = null;
            interactionContainer.PreviousLeftHit = null;
            interactionContainer.PreviousGazeHit = null;
        }

        private void OnSceneLoaded(Scene newScene, LoadSceneMode loadMode)
        {
            if (loadMode == LoadSceneMode.Single)
            {
                ResetInputContainer();
                ResetInteractionContainer();
            }
        }
    }
}