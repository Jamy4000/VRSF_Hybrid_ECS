using Valve.VR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Script attached to the ViveSDK Prefab.
    /// Set the GameEvent depending on the Binding Inputs.
    /// </summary>
    public static class ViveControllersInputCaptureSystem
    {
        #region PRIVATE_METHODS
        /// <summary>
        /// Setup the two controllers parameters to use in the CheckControllersInput method.
        /// </summary>
        /// <param name="viveInputCapture">The ViveInputCaptureComponent on the CameraRig Entity</param>
        public static void SetupControllersParameters(ViveControllersInputCaptureComponent InputCaptureComp)
        {
            // We give the references to the Scriptable variable containers in the Left Parameters variable
            InputCaptureComp.LeftParameters = new ViveInputParameters
            {
                ClickBools = InputVariableContainer.Instance.LeftClickBoolean,
                TouchBools = InputVariableContainer.Instance.LeftTouchBoolean,
                ThumbPosition = InputVariableContainer.Instance.LeftThumbPosition
            };

            // We give the references to the Scriptable variable containers in the Right Parameters variable
            InputCaptureComp.RightParameters = new ViveInputParameters
            {
                ClickBools = InputVariableContainer.Instance.RightClickBoolean,
                TouchBools = InputVariableContainer.Instance.RightTouchBoolean,
                ThumbPosition = InputVariableContainer.Instance.RightThumbPosition
            };

            RegisterLeftListeners(InputCaptureComp.LeftParameters);
            RegisterRightListeners(InputCaptureComp.RightParameters);

            InputCaptureComp.ControllersParametersSetup = true;

            void RegisterLeftListeners(ViveInputParameters viveInputParam)
            {
                ViveLeftHandInputListeners.LeftInputParam = viveInputParam;

                #region TRIGGER
                SteamVR_Actions.vRSF_Binding_LeftTriggerClick[SteamVR_Input_Sources.Any].onStateDown += ViveLeftHandInputListeners.OnLeftTriggerDown;
                SteamVR_Actions.vRSF_Binding_LeftTriggerClick[SteamVR_Input_Sources.Any].onStateUp += ViveLeftHandInputListeners.OnLeftTriggerUp;
                #endregion TRIGGER

                #region GRIP
                SteamVR_Actions.vRSF_Binding_LeftGrip[SteamVR_Input_Sources.Any].onStateDown += ViveLeftHandInputListeners.OnLeftGripDown;
                SteamVR_Actions.vRSF_Binding_LeftGrip[SteamVR_Input_Sources.Any].onStateUp += ViveLeftHandInputListeners.OnLeftGripUp;
                #endregion GRIP

                #region TOUCHPAD
                SteamVR_Actions.vRSF_Binding_LeftTouchpadClick[SteamVR_Input_Sources.Any].onStateDown += ViveLeftHandInputListeners.OnLeftTouchpadDown;
                SteamVR_Actions.vRSF_Binding_LeftTouchpadClick[SteamVR_Input_Sources.Any].onStateUp += ViveLeftHandInputListeners.OnLeftTouchpadUp;

                SteamVR_Actions.vRSF_Binding_LeftTouchpadTouch[SteamVR_Input_Sources.Any].onStateDown += ViveLeftHandInputListeners.OnLeftTouchpadTouch;
                SteamVR_Actions.vRSF_Binding_LeftTouchpadTouch[SteamVR_Input_Sources.Any].onStateUp += ViveLeftHandInputListeners.OnLeftTouchpadUntouch;

                SteamVR_Actions.vRSF_Binding_LeftTouchpadPosition[SteamVR_Input_Sources.Any].onAxis += ViveLeftHandInputListeners.OnLeftTouchpadAxisChanged;
                #endregion TOUCHPAD

                #region MENU
                SteamVR_Actions.vRSF_Binding_LeftMenu[SteamVR_Input_Sources.Any].onStateDown += ViveLeftHandInputListeners.OnLeftMenuDown;
                SteamVR_Actions.vRSF_Binding_LeftMenu[SteamVR_Input_Sources.Any].onStateUp += ViveLeftHandInputListeners.OnLeftMenuUp;
                #endregion MENU
            }

            void RegisterRightListeners(ViveInputParameters viveInputParam)
            {
                ViveRightHandInputListeners.RightInputParam = viveInputParam;

                #region TRIGGER
                SteamVR_Actions.vRSF_Binding_RightTriggerClick[SteamVR_Input_Sources.Any].onStateDown += ViveRightHandInputListeners.OnRightTriggerDown;
                SteamVR_Actions.vRSF_Binding_RightTriggerClick[SteamVR_Input_Sources.Any].onStateUp += ViveRightHandInputListeners.OnRightTriggerUp;
                #endregion TRIGGER

                #region GRIP
                SteamVR_Actions.vRSF_Binding_RightGrip[SteamVR_Input_Sources.Any].onStateDown += ViveRightHandInputListeners.OnRightGripDown;
                SteamVR_Actions.vRSF_Binding_RightGrip[SteamVR_Input_Sources.Any].onStateUp += ViveRightHandInputListeners.OnRightGripUp;
                #endregion GRIP

                #region TOUCHPAD
                SteamVR_Actions.vRSF_Binding_RightTouchpadClick[SteamVR_Input_Sources.Any].onStateDown += ViveRightHandInputListeners.OnRightTouchpadDown;
                SteamVR_Actions.vRSF_Binding_RightTouchpadClick[SteamVR_Input_Sources.Any].onStateUp += ViveRightHandInputListeners.OnRightTouchpadUp;

                SteamVR_Actions.vRSF_Binding_RightTouchpadTouch[SteamVR_Input_Sources.Any].onStateDown += ViveRightHandInputListeners.OnRightTouchpadTouch;
                SteamVR_Actions.vRSF_Binding_RightTouchpadTouch[SteamVR_Input_Sources.Any].onStateUp += ViveRightHandInputListeners.OnRightTouchpadUntouch;

                SteamVR_Actions.vRSF_Binding_RightTouchpadPosition[SteamVR_Input_Sources.Any].onAxis += ViveRightHandInputListeners.OnRightTouchpadAxisChanged;
                #endregion TOUCHPAD

                #region MENU
                SteamVR_Actions.vRSF_Binding_RightMenu[SteamVR_Input_Sources.Any].onStateDown += ViveRightHandInputListeners.OnRightMenuDown;
                SteamVR_Actions.vRSF_Binding_RightMenu[SteamVR_Input_Sources.Any].onStateUp += ViveRightHandInputListeners.OnRightMenuUp;
                #endregion MENU
            }
        }


        public static void UnregisterLeftListeners()
        {
            #region TRIGGER
            SteamVR_Actions.vRSF_Binding_LeftTriggerClick[SteamVR_Input_Sources.Any].onStateDown -= ViveLeftHandInputListeners.OnLeftTriggerDown;
            SteamVR_Actions.vRSF_Binding_LeftTriggerClick[SteamVR_Input_Sources.Any].onStateUp -= ViveLeftHandInputListeners.OnLeftTriggerUp;
            #endregion TRIGGER

            #region GRIP
            SteamVR_Actions.vRSF_Binding_LeftGrip[SteamVR_Input_Sources.Any].onStateDown -= ViveLeftHandInputListeners.OnLeftGripDown;
            SteamVR_Actions.vRSF_Binding_LeftGrip[SteamVR_Input_Sources.Any].onStateUp -= ViveLeftHandInputListeners.OnLeftGripUp;
            #endregion GRIP

            #region TOUCHPAD
            SteamVR_Actions.vRSF_Binding_LeftTouchpadClick[SteamVR_Input_Sources.Any].onStateDown -= ViveLeftHandInputListeners.OnLeftTouchpadDown;
            SteamVR_Actions.vRSF_Binding_LeftTouchpadClick[SteamVR_Input_Sources.Any].onStateUp -= ViveLeftHandInputListeners.OnLeftTouchpadUp;

            SteamVR_Actions.vRSF_Binding_LeftTouchpadTouch[SteamVR_Input_Sources.Any].onStateDown -= ViveLeftHandInputListeners.OnLeftTouchpadTouch;
            SteamVR_Actions.vRSF_Binding_LeftTouchpadTouch[SteamVR_Input_Sources.Any].onStateUp -= ViveLeftHandInputListeners.OnLeftTouchpadUntouch;

            SteamVR_Actions.vRSF_Binding_LeftTouchpadPosition[SteamVR_Input_Sources.Any].onAxis -= ViveLeftHandInputListeners.OnLeftTouchpadAxisChanged;
            #endregion TOUCHPAD

            #region MENU
            SteamVR_Actions.vRSF_Binding_LeftMenu[SteamVR_Input_Sources.Any].onStateDown -= ViveLeftHandInputListeners.OnLeftMenuDown;
            SteamVR_Actions.vRSF_Binding_LeftMenu[SteamVR_Input_Sources.Any].onStateUp -= ViveLeftHandInputListeners.OnLeftMenuUp;
            #endregion MENU
        }

        public static void UnregisterRightListeners()
        {
            #region TRIGGER
            SteamVR_Actions.vRSF_Binding_RightTriggerClick[SteamVR_Input_Sources.Any].onStateDown -= ViveRightHandInputListeners.OnRightTriggerDown;
            SteamVR_Actions.vRSF_Binding_RightTriggerClick[SteamVR_Input_Sources.Any].onStateUp -= ViveRightHandInputListeners.OnRightTriggerUp;
            #endregion TRIGGER

            #region GRIP
            SteamVR_Actions.vRSF_Binding_RightGrip[SteamVR_Input_Sources.Any].onStateDown -= ViveRightHandInputListeners.OnRightGripDown;
            SteamVR_Actions.vRSF_Binding_RightGrip[SteamVR_Input_Sources.Any].onStateUp -= ViveRightHandInputListeners.OnRightGripUp;
            #endregion GRIP

            #region TOUCHPAD
            SteamVR_Actions.vRSF_Binding_RightTouchpadClick[SteamVR_Input_Sources.Any].onStateDown -= ViveRightHandInputListeners.OnRightTouchpadDown;
            SteamVR_Actions.vRSF_Binding_RightTouchpadClick[SteamVR_Input_Sources.Any].onStateUp -= ViveRightHandInputListeners.OnRightTouchpadUp;

            SteamVR_Actions.vRSF_Binding_RightTouchpadTouch[SteamVR_Input_Sources.Any].onStateDown -= ViveRightHandInputListeners.OnRightTouchpadTouch;
            SteamVR_Actions.vRSF_Binding_RightTouchpadTouch[SteamVR_Input_Sources.Any].onStateUp -= ViveRightHandInputListeners.OnRightTouchpadUntouch;

            SteamVR_Actions.vRSF_Binding_RightTouchpadPosition[SteamVR_Input_Sources.Any].onAxis -= ViveRightHandInputListeners.OnRightTouchpadAxisChanged;
            #endregion TOUCHPAD

            #region MENU
            SteamVR_Actions.vRSF_Binding_RightMenu[SteamVR_Input_Sources.Any].onStateDown -= ViveRightHandInputListeners.OnRightMenuDown;
            SteamVR_Actions.vRSF_Binding_RightMenu[SteamVR_Input_Sources.Any].onStateUp -= ViveRightHandInputListeners.OnRightMenuUp;
            #endregion MENU
        }
        #endregion PRIVATE_METHODS
    }
}
