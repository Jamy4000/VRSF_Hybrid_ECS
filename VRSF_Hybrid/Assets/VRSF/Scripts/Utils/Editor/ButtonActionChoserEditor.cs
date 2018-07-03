using UnityEditor;
using UnityEngine;
using VRSF.Controllers;
using VRSF.Gaze;
using VRSF.Inputs;

namespace VRSF.Utils.Editor
{
    /// <summary>
    /// Handle the Options in the Inspector for the class that extend ButtonActionChoser 
    /// </summary>
    [CustomEditor(typeof(ButtonActionChoser), true)]
    public class ButtonActionChoserEditor : UnityEditor.Editor
    {
        // EMPTY
        #region PUBLIC_VARIABLES

        #endregion


        #region PRIVATE_VARIABLES
        private GazeParametersVariable _gazeParameters;
        private ControllersParametersVariable _controllersParameters;

        // let us know if we are showing the left and right thumb position parameters
        private bool _leftThumbPosIsShown;
        private bool _rightThumbPosIsShown;

        // The reference to the target
        private ButtonActionChoser _buttonActionChoser;

        // The References for the UnityEvents
        private SerializedProperty _onButtonStartTouchingProperty;
        private SerializedProperty _onButtonStopTouchingProperty;
        private SerializedProperty _onButtonIsTouchingProperty;

        private SerializedProperty _onButtonStartClickingProperty;
        private SerializedProperty _onButtonStopClickingProperty;
        private SerializedProperty _onButtonIsClickingProperty;
        #endregion


        #region PUBLIC_METHODS
        public virtual void OnEnable()
        {
            // We set the buttonActionChoser reference
            _buttonActionChoser = (ButtonActionChoser)target;
            _gazeParameters = GazeParametersVariable.Instance;
            _controllersParameters = ControllersParametersVariable.Instance;

            _onButtonStartTouchingProperty = serializedObject.FindProperty("OnButtonStartTouching");
            _onButtonStopTouchingProperty = serializedObject.FindProperty("OnButtonStopTouching");
            _onButtonIsTouchingProperty = serializedObject.FindProperty("OnButtonIsTouching");

            _onButtonStartClickingProperty = serializedObject.FindProperty("OnButtonStartClicking");
            _onButtonStopClickingProperty = serializedObject.FindProperty("OnButtonStopClicking");
            _onButtonIsClickingProperty = serializedObject.FindProperty("OnButtonIsClicking");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            base.DrawDefaultInspector();

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Button Action Choser Parameters", EditorStyles.boldLabel);

            // We display the toggles for which sdk this script is for, and if none of them is selected, we don't display the rest of the parameters.
            if (!DisplaySDKsToggles()) return;

            EditorGUILayout.Space();

            // We check that the user has set a good value for the Raycast Origin. if not, we don't display the rest of the parameters.
            if (!DisplayRaycastOriginParameters()) return;

            EditorGUILayout.Space();

            // We check that the user has set a good value for the Interaction Type. if not, we don't display the rest of the parameters.
            if (!DisplayInteractionTypeParameters()) return;

            EditorGUILayout.Space();

            if (_gazeParameters.UseGaze && _controllersParameters.UseControllers)
            {
                EditorGUILayout.LabelField("If you want to use the Gaze Button. Specified in Window/VRSF/Gaze Parameters.", EditorStyles.miniBoldLabel);
                _buttonActionChoser.UseGazeButton = EditorGUILayout.Toggle("Use Gaze Button", _buttonActionChoser.UseGazeButton);
            }
            else
            {
                _buttonActionChoser.UseGazeButton = false;
            }

            if (_buttonActionChoser.UseGazeButton)
            {
                DisplayGazeInfo();
            }
            else
            {
                EditorGUILayout.Space();

                EditorGUILayout.LabelField("The button you wanna use for this feature", EditorStyles.miniBoldLabel);
                _buttonActionChoser.ActionButton = (EControllersInput)EditorGUILayout.EnumPopup("Button to use", _buttonActionChoser.ActionButton);

                EditorGUILayout.Space();

                switch (_buttonActionChoser.InteractionType)
                {
                    case EControllerInteractionType.TOUCH:
                        HandleTouchDisplay();
                        break;

                    case EControllerInteractionType.CLICK:
                        HandleClickDisplay();
                        break;

                    case EControllerInteractionType.ALL:
                        HandleTouchDisplay();
                        EditorGUILayout.Space();
                        HandleClickDisplay();
                        break;

                    default:
                        _buttonActionChoser.ParametersAreInvalid = true;
                        _buttonActionChoser.ActionButton = EControllersInput.NONE;
                        EditorGUILayout.HelpBox("Please chose a valid Interaction Type.", MessageType.Error);
                        break;
                }
            }

            CheckThumbPos();

            DisplayInteractionEvents();

            EditorUtility.SetDirty(_buttonActionChoser);

            serializedObject.ApplyModifiedProperties();
        }

        private void HandleTouchDisplay()
        {
            switch (_buttonActionChoser.ActionButton)
            {
                case EControllersInput.A_BUTTON:
                case EControllersInput.B_BUTTON:
                case EControllersInput.X_BUTTON:
                case EControllersInput.Y_BUTTON:
                case EControllersInput.LEFT_THUMBREST:
                case EControllersInput.RIGHT_THUMBREST:
                    DisplayOculusWarning();
                    _buttonActionChoser.ParametersAreInvalid = false;
                    _leftThumbPosIsShown = false;
                    _rightThumbPosIsShown = false;
                    break;

                case EControllersInput.LEFT_TRIGGER:
                case EControllersInput.RIGHT_TRIGGER:
                    _buttonActionChoser.ParametersAreInvalid = false;
                    _leftThumbPosIsShown = false;
                    _rightThumbPosIsShown = false;
                    break;

                case EControllersInput.RIGHT_THUMBSTICK:
                    DisplayThumbPosition(EControllerInteractionType.TOUCH, EControllersInput.RIGHT_THUMBSTICK);
                    break;

                case EControllersInput.LEFT_THUMBSTICK:
                    DisplayThumbPosition(EControllerInteractionType.TOUCH, EControllersInput.LEFT_THUMBSTICK);
                    break;

                case EControllersInput.NONE:
                    _buttonActionChoser.ParametersAreInvalid = true;
                    _leftThumbPosIsShown = false;
                    _rightThumbPosIsShown = false;
                    EditorGUILayout.HelpBox("Touch : Please chose a valid Action Button.", MessageType.Error);
                    break;

                default:
                    _buttonActionChoser.ParametersAreInvalid = true;
                    _leftThumbPosIsShown = false;
                    _rightThumbPosIsShown = false;
                    DisplayTouchError();
                    break;
            }
        }

        private void HandleClickDisplay()
        {
            switch (_buttonActionChoser.ActionButton)
            {
                case EControllersInput.A_BUTTON:
                case EControllersInput.B_BUTTON:
                case EControllersInput.X_BUTTON:
                case EControllersInput.Y_BUTTON:
                    DisplayOculusWarning();
                    _buttonActionChoser.ParametersAreInvalid = false;
                    _leftThumbPosIsShown = false;
                    _rightThumbPosIsShown = false;
                    break;

                case EControllersInput.RIGHT_MENU:
                    DisplayViveWarning();
                    _buttonActionChoser.ParametersAreInvalid = false;
                    _leftThumbPosIsShown = false;
                    _rightThumbPosIsShown = false;
                    break;

                case EControllersInput.WHEEL_BUTTON:
                    DisplaySimulatorWarning();
                    _buttonActionChoser.ParametersAreInvalid = false;
                    _leftThumbPosIsShown = false;
                    _rightThumbPosIsShown = false;
                    break;

                case EControllersInput.RIGHT_THUMBREST:
                case EControllersInput.LEFT_THUMBREST:
                    DisplayClickError();
                    _buttonActionChoser.ParametersAreInvalid = true;
                    _leftThumbPosIsShown = false;
                    _rightThumbPosIsShown = false;
                    break;

                case EControllersInput.RIGHT_THUMBSTICK:
                    DisplayThumbPosition(EControllerInteractionType.CLICK, EControllersInput.RIGHT_THUMBSTICK);
                    break;

                case EControllersInput.LEFT_THUMBSTICK:
                    DisplayThumbPosition(EControllerInteractionType.CLICK, EControllersInput.LEFT_THUMBSTICK);
                    break;

                case EControllersInput.NONE:
                    _buttonActionChoser.ParametersAreInvalid = true;
                    _leftThumbPosIsShown = false;
                    _rightThumbPosIsShown = false;
                    EditorGUILayout.HelpBox("Click : Please chose a valid Action Button.", MessageType.Error);
                    break;

                default:
                    _buttonActionChoser.ParametersAreInvalid = false;
                    _leftThumbPosIsShown = false;
                    _rightThumbPosIsShown = false;
                    break;
            }
        }

        private void DisplayOculusWarning()
        {
            EditorGUILayout.HelpBox("This feature will only be available for the Oculus Touch Controllers, " +
                "as the A, B, X, Y button and the Thumbrests doesn't exist on the Vive Controllers.", MessageType.Warning);
        }

        private void DisplayViveWarning()
        {
            EditorGUILayout.HelpBox("This feature will only be available for the Vive Controllers, " +
                "as the Right Menu button cannot be use with the Oculus Touch Controllers.", MessageType.Warning);
        }

        private void DisplaySimulatorWarning()
        {
            EditorGUILayout.HelpBox("The Wheel Button will only be available for the Simulator, " +
                "as the Wheel Button is on the Mouse.", MessageType.Warning);
        }

        private void DisplayTouchError()
        {
            EditorGUILayout.HelpBox("This Button cannot be use for the Touch Interaction, as it is not available " +
                "on the Vive Controller and Oculus Touch Controllers.", MessageType.Error);
        }

        private void DisplayClickError()
        {
            EditorGUILayout.HelpBox("This Button cannot be use for the Click Interaction, as it is not available " +
                "on the Vive Controller and Oculus Touch Controllers.", MessageType.Error);
        }


        private void DisplayThumbPosition(EControllerInteractionType interactionType, EControllersInput inputSide)
        {
            switch (interactionType)
            {
                case EControllerInteractionType.CLICK:
                    switch (inputSide)
                    {
                        case EControllersInput.LEFT_THUMBSTICK:
                            EditorGUILayout.LabelField("Left Thumb Position to use for this feature", EditorStyles.miniBoldLabel);
                            _buttonActionChoser.LeftClickThumbPosition = (EThumbPosition)EditorGUILayout.EnumFlagsField("Thumb Click Position", _buttonActionChoser.LeftClickThumbPosition);
                            _leftThumbPosIsShown = true;
                            break;

                        case EControllersInput.RIGHT_THUMBSTICK:
                            EditorGUILayout.LabelField("Right Thumb Position to use for this feature", EditorStyles.miniBoldLabel);
                            _buttonActionChoser.RightClickThumbPosition = (EThumbPosition)EditorGUILayout.EnumFlagsField("Thumb Click Position", _buttonActionChoser.RightClickThumbPosition);
                            _rightThumbPosIsShown = true;
                            break;
                    }
                    _buttonActionChoser.ClickThreshold = EditorGUILayout.Slider("Click Detection Threshold", _buttonActionChoser.ClickThreshold, 0.0f, 1.0f);
                    break;

                case EControllerInteractionType.TOUCH:
                    switch (inputSide)
                    {
                        case EControllersInput.LEFT_THUMBSTICK:
                            EditorGUILayout.LabelField("Left Thumb Position to use for this feature", EditorStyles.miniBoldLabel);
                            _buttonActionChoser.LeftTouchThumbPosition = (EThumbPosition)EditorGUILayout.EnumFlagsField("Thumb Touch Position", _buttonActionChoser.LeftTouchThumbPosition);
                            _leftThumbPosIsShown = true;
                            break;

                        case EControllersInput.RIGHT_THUMBSTICK:
                            EditorGUILayout.LabelField("Right Thumb Position to use for this feature", EditorStyles.miniBoldLabel);
                            _buttonActionChoser.RightTouchThumbPosition = (EThumbPosition)EditorGUILayout.EnumFlagsField("Thumb Touch Position", _buttonActionChoser.RightTouchThumbPosition);
                            _rightThumbPosIsShown = true;
                            break;

                    }
                    _buttonActionChoser.TouchThreshold = EditorGUILayout.Slider("Touch Detection Threshold", _buttonActionChoser.TouchThreshold, 0.0f, 1.0f);
                    break;
            }
        }

        private void DisplayGazeInfo()
        {
            _buttonActionChoser.ParametersAreInvalid = false;
            EditorGUILayout.HelpBox("You can set the button to use for the Gaze in the Gaze Parameters window (Window/VRSF/Gaze Parameters.", MessageType.Info);
        }


        private bool DisplaySDKsToggles()
        {
            GUILayoutOption[] options = { GUILayout.MaxWidth(100.0f), GUILayout.MinWidth(10.0f) };

            EditorGUILayout.LabelField("Chose which SDK is using this script", EditorStyles.miniBoldLabel);

            EditorGUILayout.BeginHorizontal();

            _buttonActionChoser.UseOpenVR = EditorGUILayout.ToggleLeft("OpenVR", _buttonActionChoser.UseOpenVR, options);
            _buttonActionChoser.UseOVR = EditorGUILayout.ToggleLeft("OVR", _buttonActionChoser.UseOVR, options);
            _buttonActionChoser.UseSimulator = EditorGUILayout.ToggleLeft("Simulator", _buttonActionChoser.UseSimulator, options);

            EditorGUILayout.EndHorizontal();

            if (!_buttonActionChoser.UseOpenVR && !_buttonActionChoser.UseOVR && !_buttonActionChoser.UseSimulator)
            {
                return false;
            }
            return true;
        }


        private bool DisplayRaycastOriginParameters()
        {
            EditorGUILayout.LabelField("From where the Raycast should start for this feature", EditorStyles.miniBoldLabel);
            _buttonActionChoser.RayOrigin = (EHand)EditorGUILayout.EnumPopup("Ray Origin", _buttonActionChoser.RayOrigin);

            if (_buttonActionChoser.RayOrigin == EHand.NONE)
            {
                _buttonActionChoser.ParametersAreInvalid = true;
                EditorGUILayout.HelpBox("The RayOrigin cannot be NONE.", MessageType.Error);
                return false;
            }
            return true;
        }


        private bool DisplayInteractionTypeParameters()
        {
            EditorGUILayout.LabelField("Type of Interaction with the Controller", EditorStyles.miniBoldLabel);
            _buttonActionChoser.InteractionType = (EControllerInteractionType)EditorGUILayout.EnumFlagsField("Interaction Type", _buttonActionChoser.InteractionType);

            if (_buttonActionChoser.InteractionType == EControllerInteractionType.NONE)
            {
                _buttonActionChoser.ParametersAreInvalid = true;
                EditorGUILayout.HelpBox("The Interaction type cannot be NONE.", MessageType.Error);
                return false;
            }
            return true;
        }


        private void CheckThumbPos()
        {
            if (_leftThumbPosIsShown)
            {
                if ((_buttonActionChoser.InteractionType == EControllerInteractionType.CLICK && _buttonActionChoser.LeftClickThumbPosition == EThumbPosition.NONE) ||
                    (_buttonActionChoser.InteractionType == EControllerInteractionType.TOUCH && _buttonActionChoser.LeftTouchThumbPosition == EThumbPosition.NONE) ||
                    (_buttonActionChoser.InteractionType == EControllerInteractionType.ALL &&
                        (_buttonActionChoser.LeftTouchThumbPosition == EThumbPosition.NONE || _buttonActionChoser.LeftClickThumbPosition == EThumbPosition.NONE)))
                {
                    _buttonActionChoser.ParametersAreInvalid = true;
                    EditorGUILayout.HelpBox("Please chose a valid Thumb Position.", MessageType.Error);
                }
                else
                {
                    _buttonActionChoser.ParametersAreInvalid = false;
                }
            }
            else if (_rightThumbPosIsShown)
            {
                if ((_buttonActionChoser.InteractionType == EControllerInteractionType.CLICK && _buttonActionChoser.RightClickThumbPosition == EThumbPosition.NONE) ||
                    (_buttonActionChoser.InteractionType == EControllerInteractionType.TOUCH && _buttonActionChoser.RightTouchThumbPosition == EThumbPosition.NONE) ||
                    (_buttonActionChoser.InteractionType == EControllerInteractionType.ALL &&
                        (_buttonActionChoser.RightTouchThumbPosition == EThumbPosition.NONE || _buttonActionChoser.RightClickThumbPosition == EThumbPosition.NONE)))
                {
                    _buttonActionChoser.ParametersAreInvalid = true;
                    EditorGUILayout.HelpBox("Please chose a valid Thumb Position.", MessageType.Error);
                }
                else
                {
                    _buttonActionChoser.ParametersAreInvalid = false;
                }
            }
        }


        private void DisplayInteractionEvents()
        {
            EditorGUILayout.Space();
            if ((_buttonActionChoser.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                EditorGUILayout.Space();
                DisplayTouchEvents();
            }
            if ((_buttonActionChoser.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                EditorGUILayout.Space();
                DisplayClickEvents();
            }
        }


        private void DisplayTouchEvents()
        {
            EditorGUILayout.PropertyField(_onButtonStartTouchingProperty);
            EditorGUILayout.PropertyField(_onButtonStopTouchingProperty);
            EditorGUILayout.PropertyField(_onButtonIsTouchingProperty);
        }


        private void DisplayClickEvents()
        {
            EditorGUILayout.PropertyField(_onButtonStartClickingProperty);
            EditorGUILayout.PropertyField(_onButtonStopClickingProperty);
            EditorGUILayout.PropertyField(_onButtonIsClickingProperty);
        }
        #endregion


        // EMPTY
        #region PRIVATE_METHODS

        #endregion


        // EMPTY
        #region GETTERS_SETTERS

        #endregion
    }
}