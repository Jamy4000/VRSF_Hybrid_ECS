using HPA_Boat.VR.Component;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRSF.MoveAround.Components;
using VRSF.Utils.Components.ButtonActionChoser;

namespace VRSF.MoveAround.Systems
{
    /// <summary>
    /// New Implementation of the fly effect to add an option where the user see his speed multiplied by 20 when clicking on the thumbstick
    /// </summary>
	public class RocketManSetupSystem : FlySetupSystem
    {
        public struct RocketManFilter
        {
            public RocketManComponent RocketManComp;
            public FlyParametersComponent FlyParameters;
            public FlyAccelerationComponent FlyAcceleration;
            public ButtonActionChoserComponents ButtonComponents;
        }

        private RocketManFilter _currentSetupEntity;


        protected override void OnStartRunning()
        {
            SceneManager.sceneUnloaded += OnSceneUnloaded;

            foreach (var e in GetEntities<RocketManFilter>())
            {
                _currentSetupEntity = e;
                SetupListenersResponses();

                this.Enabled = false;
            }
            
            foreach (var e in GetEntities<RocketManFilter>())
            {
                e.RocketManComp.BaseSpeedValue = e.FlyParameters.FlyingSpeed;
            }
        }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }


        #region PUBLIC_METHODS
        public override void SetupListenersResponses()
        {
            //Create new filter based on RocketMan Components
            Filter newFilter = new Filter
            {
                FlyComponent = _currentSetupEntity.FlyParameters,
                ButtonComponents = _currentSetupEntity.ButtonComponents
            };

            // Add listener for when user is clicking
            _currentSetupEntity.ButtonComponents.OnButtonIsClicking.AddListener(delegate { ButtonIsClicking(_currentSetupEntity, newFilter); });
            _currentSetupEntity.ButtonComponents.OnButtonStopClicking.AddListener(delegate { ButtonStopClicking(_currentSetupEntity); });

            // Add listener for when user is touching
            _currentSetupEntity.ButtonComponents.OnButtonIsTouching.AddListener(delegate { ButtonIsInteracting(newFilter); });
            _currentSetupEntity.ButtonComponents.OnButtonStopTouching.AddListener(delegate { ButtonStopInteracting(newFilter); });
        }

        public override void RemoveListenersOnEndApp()
        {
            _currentSetupEntity.ButtonComponents.OnButtonIsClicking.RemoveAllListeners();
            _currentSetupEntity.ButtonComponents.OnButtonStopClicking.RemoveAllListeners();

            _currentSetupEntity.ButtonComponents.OnButtonIsTouching.RemoveAllListeners();
            _currentSetupEntity.ButtonComponents.OnButtonStopTouching.RemoveAllListeners();
        }


        public void ButtonIsClicking(RocketManFilter rocketManEntity, Filter flyEntitiy)
        {
            if (!rocketManEntity.RocketManComp.SpeedHasBeenSet)
            {
                if (rocketManEntity.FlyParameters.FlyingSpeed < 50)
                {
                    rocketManEntity.FlyParameters.FlyingSpeed += (Time.deltaTime * rocketManEntity.FlyAcceleration.AccelerationEffectFactor * 10);
                }
                else
                {
                    rocketManEntity.FlyParameters.FlyingSpeed = 50;
                }
            }

            ButtonIsInteracting(flyEntitiy);
        }

        public void ButtonStopClicking(RocketManFilter entity)
        {
            entity.RocketManComp.RocketSlowingDown = true;
        }
        #endregion

        
        #region PRIVATE_METHODS
        /// <summary>
        /// Reactivate the System when switching to another Scene.
        /// </summary>
        /// <param name="oldScene">The previous scene before switching</param>
        private void OnSceneUnloaded(Scene oldScene)
        {
            this.Enabled = true;
        }
        #endregion
    }
}