using Unity.Entities;
using UnityEngine;
using VRSF.Core.SetupVR;
using Valve.VR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Script attached to the ViveSDK Prefab.
    /// Set the GameEvent depending on the Vive Inputs.
    /// </summary>
    public class ViveControllersInputCaptureSystem : ComponentSystem
    {
        struct ViveFilter
        {
            public ViveControllersInputCaptureComponent ViveInputCapture;
        }

        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            OnSetupVRReady.RegisterListener(SetupControllersParameters);
        }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
            OnSetupVRReady.UnregisterListener(SetupControllersParameters);
            UnregisterLeftListeners();
            UnregisterRightListeners();
        }

        protected override void OnUpdate() {}
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Setup the two controllers parameters to use in the CheckControllersInput method.
        /// </summary>
        /// <param name="viveInputCapture">The ViveInputCaptureComponent on the CameraRig Entity</param>
        private void SetupControllersParameters(OnSetupVRReady setupVREnded)
        {
            foreach (var e in GetEntities<ViveFilter>())
            {
                // We give the references to the Scriptable variable containers in the Left Parameters variable
                e.ViveInputCapture.LeftParameters = new ViveInputParameters
                {
                    ClickBools = InputVariableContainer.Instance.LeftClickBoolean,
                    TouchBools = InputVariableContainer.Instance.LeftTouchBoolean,
                    ThumbPosition = InputVariableContainer.Instance.LeftThumbPosition
                };

                // We give the references to the Scriptable variable containers in the Right Parameters variable
                e.ViveInputCapture.RightParameters = new ViveInputParameters
                {
                    ClickBools = InputVariableContainer.Instance.RightClickBoolean,
                    TouchBools = InputVariableContainer.Instance.RightTouchBoolean,
                    ThumbPosition = InputVariableContainer.Instance.RightThumbPosition
                };

                RegisterLeftListeners(e.ViveInputCapture.LeftParameters);
                RegisterRightListeners(e.ViveInputCapture.RightParameters);

                e.ViveInputCapture.ControllersParametersSetup = true;
            }

            void RegisterLeftListeners(ViveInputParameters viveInputParam)
            {
                ViveLeftHandInputListeners.LeftInputParam = viveInputParam;

                #region TRIGGER
                SteamVR_Actions.vRSF_Vive_LeftTriggerClick[SteamVR_Input_Sources.Any].onStateDown += ViveLeftHandInputListeners.OnLeftTriggerDown;
                SteamVR_Actions.vRSF_Vive_LeftTriggerClick[SteamVR_Input_Sources.Any].onStateUp += ViveLeftHandInputListeners.OnLeftTriggerUp;
                SteamVR_Actions.vRSF_Vive_LeftTriggerPull[SteamVR_Input_Sources.Any].onAxis += ViveLeftHandInputListeners.OnLeftTriggerTouch;
                #endregion TRIGGER

                #region GRIP
                SteamVR_Actions.vRSF_Vive_LeftGrip[SteamVR_Input_Sources.Any].onStateDown += ViveLeftHandInputListeners.OnLeftGripDown;
                SteamVR_Actions.vRSF_Vive_LeftGrip[SteamVR_Input_Sources.Any].onStateUp += ViveLeftHandInputListeners.OnLeftGripUp;
                #endregion GRIP

                #region TOUCHPAD
                SteamVR_Actions.vRSF_Vive_LeftTouchpadClick[SteamVR_Input_Sources.Any].onStateDown += ViveLeftHandInputListeners.OnLeftTouchpadDown;
                SteamVR_Actions.vRSF_Vive_LeftTouchpadClick[SteamVR_Input_Sources.Any].onStateUp += ViveLeftHandInputListeners.OnLeftTouchpadUp;

                SteamVR_Actions.vRSF_Vive_LeftTouchpadTouch[SteamVR_Input_Sources.Any].onStateDown += ViveLeftHandInputListeners.OnLeftTouchpadTouch;
                SteamVR_Actions.vRSF_Vive_LeftTouchpadTouch[SteamVR_Input_Sources.Any].onStateUp += ViveLeftHandInputListeners.OnLeftTouchpadUntouch;

                SteamVR_Actions.vRSF_Vive_LeftTouchpadPosition[SteamVR_Input_Sources.Any].onAxis += ViveLeftHandInputListeners.OnLeftTouchpadAxisChanged;
                #endregion TOUCHPAD

                #region MENU
                SteamVR_Actions.vRSF_Vive_LeftMenu[SteamVR_Input_Sources.Any].onStateDown += ViveLeftHandInputListeners.OnLeftMenuDown;
                SteamVR_Actions.vRSF_Vive_LeftMenu[SteamVR_Input_Sources.Any].onStateUp += ViveLeftHandInputListeners.OnLeftMenuUp;
                #endregion MENU
            }

            void RegisterRightListeners(ViveInputParameters viveInputParam)
            {
                ViveRightHandInputListeners.RightInputParam = viveInputParam;

                #region TRIGGER
                SteamVR_Actions.vRSF_Vive_RightTriggerClick[SteamVR_Input_Sources.Any].onStateDown += ViveRightHandInputListeners.OnRightTriggerDown;
                SteamVR_Actions.vRSF_Vive_RightTriggerClick[SteamVR_Input_Sources.Any].onStateUp += ViveRightHandInputListeners.OnRightTriggerUp;
                SteamVR_Actions.vRSF_Vive_RightTriggerPull[SteamVR_Input_Sources.Any].onAxis += ViveRightHandInputListeners.OnRightTriggerTouch;
                #endregion TRIGGER

                #region GRIP
                SteamVR_Actions.vRSF_Vive_RightGrip[SteamVR_Input_Sources.Any].onStateDown += ViveRightHandInputListeners.OnRightGripDown;
                SteamVR_Actions.vRSF_Vive_RightGrip[SteamVR_Input_Sources.Any].onStateUp += ViveRightHandInputListeners.OnRightGripUp;
                #endregion GRIP

                #region TOUCHPAD
                SteamVR_Actions.vRSF_Vive_RightTouchpadClick[SteamVR_Input_Sources.Any].onStateDown += ViveRightHandInputListeners.OnRightTouchpadDown;
                SteamVR_Actions.vRSF_Vive_RightTouchpadClick[SteamVR_Input_Sources.Any].onStateUp += ViveRightHandInputListeners.OnRightTouchpadUp;

                SteamVR_Actions.vRSF_Vive_RightTouchpadTouch[SteamVR_Input_Sources.Any].onStateDown += ViveRightHandInputListeners.OnRightTouchpadTouch;
                SteamVR_Actions.vRSF_Vive_RightTouchpadTouch[SteamVR_Input_Sources.Any].onStateUp += ViveRightHandInputListeners.OnRightTouchpadUntouch;

                SteamVR_Actions.vRSF_Vive_RightTouchpadPosition[SteamVR_Input_Sources.Any].onAxis += ViveRightHandInputListeners.OnRightTouchpadAxisChanged;
                #endregion TOUCHPAD

                #region MENU
                SteamVR_Actions.vRSF_Vive_RightMenu[SteamVR_Input_Sources.Any].onStateDown += ViveRightHandInputListeners.OnRightMenuDown;
                SteamVR_Actions.vRSF_Vive_RightMenu[SteamVR_Input_Sources.Any].onStateUp += ViveRightHandInputListeners.OnRightMenuUp;
                #endregion MENU
            }
        }


        void UnregisterLeftListeners()
        {
            #region TRIGGER
            SteamVR_Actions.vRSF_Vive_LeftTriggerClick[SteamVR_Input_Sources.Any].onStateDown -= ViveLeftHandInputListeners.OnLeftTriggerDown;
            SteamVR_Actions.vRSF_Vive_LeftTriggerClick[SteamVR_Input_Sources.Any].onStateUp -= ViveLeftHandInputListeners.OnLeftTriggerUp;
            SteamVR_Actions.vRSF_Vive_LeftTriggerPull[SteamVR_Input_Sources.Any].onAxis -= ViveLeftHandInputListeners.OnLeftTriggerTouch;
            #endregion TRIGGER

            #region GRIP
            SteamVR_Actions.vRSF_Vive_LeftGrip[SteamVR_Input_Sources.Any].onStateDown -= ViveLeftHandInputListeners.OnLeftGripDown;
            SteamVR_Actions.vRSF_Vive_LeftGrip[SteamVR_Input_Sources.Any].onStateUp -= ViveLeftHandInputListeners.OnLeftGripUp;
            #endregion GRIP

            #region TOUCHPAD
            SteamVR_Actions.vRSF_Vive_LeftTouchpadClick[SteamVR_Input_Sources.Any].onStateDown -= ViveLeftHandInputListeners.OnLeftTouchpadDown;
            SteamVR_Actions.vRSF_Vive_LeftTouchpadClick[SteamVR_Input_Sources.Any].onStateUp -= ViveLeftHandInputListeners.OnLeftTouchpadUp;

            SteamVR_Actions.vRSF_Vive_LeftTouchpadTouch[SteamVR_Input_Sources.Any].onStateDown -= ViveLeftHandInputListeners.OnLeftTouchpadTouch;
            SteamVR_Actions.vRSF_Vive_LeftTouchpadTouch[SteamVR_Input_Sources.Any].onStateUp -= ViveLeftHandInputListeners.OnLeftTouchpadUntouch;

            SteamVR_Actions.vRSF_Vive_LeftTouchpadPosition[SteamVR_Input_Sources.Any].onAxis -= ViveLeftHandInputListeners.OnLeftTouchpadAxisChanged;
            #endregion TOUCHPAD

            #region MENU
            SteamVR_Actions.vRSF_Vive_LeftMenu[SteamVR_Input_Sources.Any].onStateDown -= ViveLeftHandInputListeners.OnLeftMenuDown;
            SteamVR_Actions.vRSF_Vive_LeftMenu[SteamVR_Input_Sources.Any].onStateUp -= ViveLeftHandInputListeners.OnLeftMenuUp;
            #endregion MENU
        }

        void UnregisterRightListeners()
        {
            #region TRIGGER
            SteamVR_Actions.vRSF_Vive_RightTriggerClick[SteamVR_Input_Sources.Any].onStateDown -= ViveRightHandInputListeners.OnRightTriggerDown;
            SteamVR_Actions.vRSF_Vive_RightTriggerClick[SteamVR_Input_Sources.Any].onStateUp -= ViveRightHandInputListeners.OnRightTriggerUp;
            SteamVR_Actions.vRSF_Vive_RightTriggerPull[SteamVR_Input_Sources.Any].onAxis -= ViveRightHandInputListeners.OnRightTriggerTouch;
            #endregion TRIGGER

            #region GRIP
            SteamVR_Actions.vRSF_Vive_RightGrip[SteamVR_Input_Sources.Any].onStateDown -= ViveRightHandInputListeners.OnRightGripDown;
            SteamVR_Actions.vRSF_Vive_RightGrip[SteamVR_Input_Sources.Any].onStateUp -= ViveRightHandInputListeners.OnRightGripUp;
            #endregion GRIP

            #region TOUCHPAD
            SteamVR_Actions.vRSF_Vive_RightTouchpadClick[SteamVR_Input_Sources.Any].onStateDown -= ViveRightHandInputListeners.OnRightTouchpadDown;
            SteamVR_Actions.vRSF_Vive_RightTouchpadClick[SteamVR_Input_Sources.Any].onStateUp -= ViveRightHandInputListeners.OnRightTouchpadUp;

            SteamVR_Actions.vRSF_Vive_RightTouchpadTouch[SteamVR_Input_Sources.Any].onStateDown -= ViveRightHandInputListeners.OnRightTouchpadTouch;
            SteamVR_Actions.vRSF_Vive_RightTouchpadTouch[SteamVR_Input_Sources.Any].onStateUp -= ViveRightHandInputListeners.OnRightTouchpadUntouch;

            SteamVR_Actions.vRSF_Vive_RightTouchpadPosition[SteamVR_Input_Sources.Any].onAxis -= ViveRightHandInputListeners.OnRightTouchpadAxisChanged;
            #endregion TOUCHPAD

            #region MENU
            SteamVR_Actions.vRSF_Vive_RightMenu[SteamVR_Input_Sources.Any].onStateDown -= ViveRightHandInputListeners.OnRightMenuDown;
            SteamVR_Actions.vRSF_Vive_RightMenu[SteamVR_Input_Sources.Any].onStateUp -= ViveRightHandInputListeners.OnRightMenuUp;
            #endregion MENU
        }
        #endregion PRIVATE_METHODS
    }
}
