using UnityEngine;
using UnityEngine.SceneManagement;
using VRSF.Inputs;
using VRSF.MoveAround.Components;
using VRSF.Utils.Components.ButtonActionChoser;
using VRSF.Utils.Systems.ButtonActionChoser;

namespace VRSF.MoveAround.Systems
{
    public class CameraRotationSetupSystem : BACUpdateSystem
    {

        struct Filter
        {
            public CameraRotationComponent RotationComp;
            public ButtonActionChoserComponents ButtonComponents;
        }


        #region PRIVATE_VARIABLES
        private Filter _currentSetupEntity;
        #endregion PRIVATE_VARIABLES


        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            SceneManager.sceneUnloaded += OnSceneUnloaded;

            foreach (var e in GetEntities<Filter>())
            {
                _currentSetupEntity = e;
                SetupListenersResponses();
            }
        }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();

            SceneManager.sceneUnloaded += OnSceneUnloaded;

            foreach (var e in GetEntities<Filter>())
            {
                _currentSetupEntity = e;
                RemoveListenersOnEndApp();
            }
        }
        #endregion


        #region PUBLIC_METHODS

        #region Setup_Listeners_Responses
        public override void SetupListenersResponses()
        {
            if ((_currentSetupEntity.ButtonComponents.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                _currentSetupEntity.ButtonComponents.OnButtonStartClicking.AddListener(delegate { StartRotating(_currentSetupEntity.RotationComp); });
                _currentSetupEntity.ButtonComponents.OnButtonIsClicking.AddListener(delegate { StartRotating(_currentSetupEntity.RotationComp); });

                _currentSetupEntity.ButtonComponents.OnButtonStopClicking.AddListener(delegate { StopRotating(_currentSetupEntity.RotationComp); });
            }

            if ((_currentSetupEntity.ButtonComponents.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                _currentSetupEntity.ButtonComponents.OnButtonStartTouching.AddListener(delegate { StartRotating(_currentSetupEntity.RotationComp); });
                _currentSetupEntity.ButtonComponents.OnButtonIsTouching.AddListener(delegate { StartRotating(_currentSetupEntity.RotationComp); });

                _currentSetupEntity.ButtonComponents.OnButtonStopTouching.AddListener(delegate { StopRotating(_currentSetupEntity.RotationComp); });
            }
        }

        public override void RemoveListenersOnEndApp()
        {
            if ((_currentSetupEntity.ButtonComponents.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                _currentSetupEntity.ButtonComponents.OnButtonStartClicking.RemoveAllListeners();
                _currentSetupEntity.ButtonComponents.OnButtonIsClicking.RemoveAllListeners();
                _currentSetupEntity.ButtonComponents.OnButtonStopClicking.RemoveAllListeners();
            }

            if ((_currentSetupEntity.ButtonComponents.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                _currentSetupEntity.ButtonComponents.OnButtonStartTouching.RemoveAllListeners();
                _currentSetupEntity.ButtonComponents.OnButtonIsTouching.RemoveAllListeners();
                _currentSetupEntity.ButtonComponents.OnButtonStopTouching.RemoveAllListeners();
            }
        }
        #endregion Setup_Listeners_Responses

        #endregion PUBLIC_METHODS


        #region PRIVATE_METHODS
        /// <summary>
        /// Set the _rotating bool to true. Need to be called from IsTouching, IsClicking, StartTouching or StartClicking.
        /// </summary>
        private void StartRotating(CameraRotationComponent comp)
        {
            comp._IsRotating = true;
        }

        /// <summary>
        /// Set the _rotating bool to false. Need to be called from StopTouching or StopClicking.
        /// </summary>
        private void StopRotating(CameraRotationComponent comp)
        {
            comp._IsRotating = false;
            comp._HasRotated = false;
        }


        /// <summary>
        /// Reactivate the System when switching to another Scene.
        /// </summary>
        /// <param name="oldScene">The previous scene before switching</param>
        private void OnSceneUnloaded(Scene oldScene)
        {
            foreach (var e in GetEntities<Filter>())
            {
                _currentSetupEntity = e;
                SetupListenersResponses();
            }
        }
        #endregion
    }
}