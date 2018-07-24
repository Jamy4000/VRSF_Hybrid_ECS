using UnityEngine;
using UnityEngine.SceneManagement;
using VRSF.Inputs;
using VRSF.MoveAround.Components;
using VRSF.Utils.Components.ButtonActionChoser;
using VRSF.Utils.Systems.ButtonActionChoser;

namespace VRSF.MoveAround.Systems
{
    /// <summary>
    /// System Allowing the user to fly with the thumbstick / touchpad. 
    /// </summary>
    public class FlySetupSystem : BACUpdateSystem
    {
        public struct Filter
        {
            public FlyParametersComponent FlyComponent;
            public ButtonActionChoserComponents ButtonComponents;
        }


        #region PRIVATE_VARIABLES
        private Filter _currentSetupEntity;     // Allow us to pass the entity in the overrided method from BACUpdateSystem
        #endregion PRIVATE_VARIABLES


        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            SceneManager.activeSceneChanged += OnSceneChanged;

            foreach (var e in GetEntities<Filter>())
            {
                if (e.ButtonComponents.ActionButton != EControllersInput.LEFT_THUMBSTICK && e.ButtonComponents.ActionButton != EControllersInput.RIGHT_THUMBSTICK)
                {
                    Debug.LogError("VRSF : You need to assign Left Thumbstick or Right Thumbstick to use the Fly script. Setting CanBeUsed at false.");
                    e.ButtonComponents.CanBeUsed = false;
                }

                _currentSetupEntity = e;
                SetupListenersResponses();
            }
        }

        protected override void OnDestroyManager()
        {
            base.OnStopRunning();

            foreach (var e in GetEntities<Filter>())
            {
                _currentSetupEntity = e;
                RemoveListenersOnEndApp();
            }
        }
        #endregion ComponentSystem_Methods


        #region PUBLIC_METHODS

        public override void SetupListenersResponses()
        {
            if ((_currentSetupEntity.ButtonComponents.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                _currentSetupEntity.ButtonComponents.OnButtonIsClicking.AddListener(delegate { ButtonIsInteracting(_currentSetupEntity); });
                _currentSetupEntity.ButtonComponents.OnButtonStopClicking.AddListener(delegate { ButtonStopInteracting(_currentSetupEntity); });
            }

            if ((_currentSetupEntity.ButtonComponents.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                _currentSetupEntity.ButtonComponents.OnButtonIsTouching.AddListener(delegate { ButtonIsInteracting(_currentSetupEntity); });
                _currentSetupEntity.ButtonComponents.OnButtonStopTouching.AddListener(delegate { ButtonStopInteracting(_currentSetupEntity); });
            }
        }

        public override void RemoveListenersOnEndApp()
        {
            if ((_currentSetupEntity.ButtonComponents.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                _currentSetupEntity.ButtonComponents.OnButtonIsClicking.RemoveAllListeners();
                _currentSetupEntity.ButtonComponents.OnButtonStopClicking.RemoveAllListeners();
            }

            if ((_currentSetupEntity.ButtonComponents.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                _currentSetupEntity.ButtonComponents.OnButtonIsTouching.RemoveAllListeners();
                _currentSetupEntity.ButtonComponents.OnButtonStopTouching.RemoveAllListeners();
            }
        }
        #endregion PUBLIC_METHODS


        #region PRIVATE_METHODS
        /// <summary>
        /// Called from OnButtonStopClicking or OnButtonStopTouching event
        /// </summary>
        private void ButtonStopInteracting(Filter entity)
        {
            entity.FlyComponent.SlowDownTimer = entity.FlyComponent.TimeSinceStartFlying;
            entity.FlyComponent.IsSlowingDown = true;
            entity.FlyComponent.WantToFly = false;
            entity.FlyComponent.IsInteracting = false;
        }

        /// <summary>
        /// Called from OnButtonIsTouching or OnButtonIsClickingevent
        /// </summary>
        private void ButtonIsInteracting(Filter entity)
        {
            entity.FlyComponent.IsInteracting = true;
        }


        /// <summary>
        /// Reactivate the System when switching to another Scene.
        /// </summary>
        /// <param name="oldScene">The previous scene before switching</param>
        /// <param name="newScene">The new scene after switching</param>
        private void OnSceneChanged(Scene oldScene, Scene newScene)
        {
            this.Enabled = true;
        }
        #endregion PRIVATE_METHODS
    }
}