using ScriptableFramework.Events;
using ScriptableFramework.Variables;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using VRSF.Controllers;
using VRSF.Gaze;
using VRSF.Inputs;
using VRSF.Interactions;

namespace VRSF.Utils
{
    [Serializable]
    public class ButtonActionChoser : MonoBehaviour
    {
        #region PUBLIC_VARIABLES
        /// <summary>
        /// Delegate to pass the method to call when the ActionButton is down or touched
        /// </summary>
        /// <param name="t">The transform that was hit</param>
        public delegate void OnButtonDelegate();

        [Header("SDKs using this script")]
        [HideInInspector] public bool UseOVR = true;
        [HideInInspector] public bool UseOpenVR = true;
        [HideInInspector] public bool UseSimulator = true;

        [Header("The Raycast Origin for this script")]
        [HideInInspector] public EHand RayOrigin = EHand.NONE;

        [Header("The type of Interaction you want to use")]
        [HideInInspector] public EControllerInteractionType InteractionType = EControllerInteractionType.NONE;

        [Header("Wheter you want to use the Gaze Click for the Action")]
        [HideInInspector] public bool UseGazeButton = false;

        [Header("The button you wanna use for the Action")]
        [HideInInspector] public EControllersInput ActionButton = EControllersInput.NONE;

        [Header("The position of the Thumb to start this feature")]
        [HideInInspector] public EThumbPosition LeftTouchThumbPosition = EThumbPosition.NONE;
        [HideInInspector] public EThumbPosition RightTouchThumbPosition = EThumbPosition.NONE;
        [HideInInspector] public EThumbPosition LeftClickThumbPosition = EThumbPosition.NONE;
        [HideInInspector] public EThumbPosition RightClickThumbPosition = EThumbPosition.NONE;

        [Header("The Thresholds for the Thumb on the Controller")]
        [HideInInspector] public float TouchThreshold = 0.5f;
        [HideInInspector] public float ClickThreshold = 0.5f;


        [Header("The UnityEvents called when the user is Touching")]
        [HideInInspector] [SerializeField] public UnityEvent OnButtonStartTouching;
        [HideInInspector] [SerializeField] public UnityEvent OnButtonStopTouching;
        [HideInInspector] [SerializeField] public UnityEvent OnButtonIsTouching;

        [Header("The UnityEvents called when the user is Clicking")]
        [HideInInspector] [SerializeField] public UnityEvent OnButtonStartClicking;
        [HideInInspector] [SerializeField] public UnityEvent OnButtonStopClicking;
        [HideInInspector] [SerializeField] public UnityEvent OnButtonIsClicking;


        [HideInInspector] public bool ParametersAreInvalid = false;

        [HideInInspector] public bool CanBeUsed = true;
        #endregion


        #region PRIVATE_VARIABLES
        // VRSF Parameters references
        private GazeParametersVariable _gazeParameters;
        private ControllersParametersVariable _controllersParameters;
        private InputVariableContainer _inputContainer;
        private InteractionVariableContainer _interactionContainer;

        // The RaycastHitVariable and Ray to check for this feature
        private RaycastHitVariable _hit;
        private RayVariable _rayVar;

        // The hand on which the button to use is situated
        private EHand _buttonHand = EHand.NONE;

        // To keep track of the event that were raised, used for the features that use the Thumbstick
        private bool _clickActionBeyondThreshold;
        private bool _touchActionBeyondThreshold;
        private bool _untouchedEventWasRaised;
        private bool _unclickEventWasRaised;

        // For SDKs Specific ActionButton 
        private bool _isUsingOculusButton;
        private bool _isUsingViveButton;
        private bool _isUsingWheelButton;

        // Thumb Parameters
        private Vector2Variable _thumbPos = null;

        // All GameEvents
        private GameEvent _geDown = null;
        private GameEvent _geUp = null;
        private GameEvent _geTouched = null;
        private GameEvent _geUntouched = null;

        // All GameEventListeners
        private GameEventListener _gelDown = null;
        private GameEventListener _gelUp = null;
        private GameEventListener _gelTouched = null;
        private GameEventListener _gelUntouched = null;

        // BoolVariable to check
        private BoolVariable _isTouching = null;
        private BoolVariable _isClicking = null;

        // GameObject containing the GameEventListeners
        private GameObject gameEventsContainer = null;
        #endregion


        #region MONOBEHAVIOUR_METHODS
        // Use this for initialization
        public virtual void Start()
        {
            // We check if the current loaded sdk is used for this feature
            if (!CheckUseSDKToggles())
            {
                this.enabled = false;
                return;
            }

            // We init the Scriptable Objects References
            InitSOs();

            // We check which hit to use for this feature with the RayOrigin
            CheckRayAndHit();

            // We check on which hand is set the Action Button selected
            CheckButtonHand();

            // We check that all the parameters are set correctly
            if (ParametersAreInvalid || !CheckParameters())
            {
                Debug.LogError("The Button Action Choser parameters for the " + this.GetType().Name + " script are invalid.\n" +
                    "Please specify valid values as displayed in the Help Boxes under your script. Disabling the script.");
                this.enabled = false;
                return;
            }

            // We init the Scriptable Object references and how they work
            if (!InitSOsReferences())
            {
                Debug.LogError("VRSF : An error has occured while initializing the Scriptable Objects reference in the " + this.GetType().Name + " script.\n" +
                    "If the error persist after reloading the Editor, please open an issue on Github. Disabling the Script.");
                this.enabled = false;
                return;
            }
        }

        public virtual void Update()
        {
            if (CanBeUsed)
            {
                // If we use the touch event and the user is touching on the button
                if (_isTouching != null && _isTouching.Value && !_untouchedEventWasRaised)
                {
                    StartActionIsTouching();
                }
                // If we use the click event and the user is clicking on the button
                if (_isClicking != null && _isClicking.Value && !_unclickEventWasRaised)
                {
                    StartActionIsClicking();
                }
            }
        }
        #endregion


        // EMPTY
        #region PUBLIC_METHODS
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Check if at least one of the three toggles for the SDK to Use is set at true, and if the current loaded Device is listed in those bool
        /// </summary>
        /// <returns>true if the current loaded SDK is selected in the inspector</returns>
        private bool CheckUseSDKToggles()
        {
            if (!UseOpenVR && !UseOVR && !UseSimulator)
            {
                Debug.LogError("VRSF : You need to chose at least one SDK to use the " + GetType().Name + " script. Disabling the script.");
                this.enabled = false;
                return false;
            }

            switch (VRSF_Components.DeviceLoaded)
            {
                case EDevice.OPENVR:
                    return UseOpenVR;

                case EDevice.OVR:
                    return UseOVR;

                case EDevice.SIMULATOR:
                    return UseSimulator;
            }

            return true;
        }


        /// <summary>
        /// Check which RaycastHitVariable is used depending on the RayOrigin specified
        /// </summary>
        private void CheckRayAndHit()
        {
            switch (RayOrigin)
            {
                case (EHand.LEFT):
                    _hit = _interactionContainer.LeftHit;
                    _rayVar = _interactionContainer.LeftRay;
                    break;

                case (EHand.RIGHT):
                    _hit = _interactionContainer.RightHit;
                    _rayVar = _interactionContainer.RightRay;
                    break;

                case (EHand.GAZE):
                    _hit = _interactionContainer.GazeHit;
                    _rayVar = _interactionContainer.GazeRay;
                    break;

                default:
                    Debug.LogError("VRSF : You need to specify the RayOrigin for the " + GetType().Name + " script. Disabling the script.");
                    this.enabled = false;
                    break;
            }
        }


        /// <summary>
        /// Instantiate and set the GameEventListeners and BoolVariable
        /// </summary>
        private bool InitSOsReferences()
        {
            // We set the GameEvents and BoolVariables depending on the InteractionType and the Hand of the ActionButton
            if (!SetupSOReferences())
            {
                return false;
            }

            //We create the container for the GameEventListeners
            gameEventsContainer = CreateGEContainer();

            // We add the GameEventListeners to the GameEventContainer, set the events and response for the listeners, 
            // and register the listeners in the GameEvent
            // Placed in coroutine as sometimes we need to wait for another ButtonActionChoser script to create the GELContainer 
            StartCoroutine(CreateGELInContainer());

            return true;
        }


        /// <summary>
        /// Set the GameEvents and GameEventListeners to null
        /// </summary>
        private void InitSOs()
        {
            _controllersParameters = ControllersParametersVariable.Instance;
            _gazeParameters = GazeParametersVariable.Instance;
            _inputContainer = InputVariableContainer.Instance;
            _interactionContainer = InteractionVariableContainer.Instance;

            _geDown = null;
            _geUp = null;
            _geTouched = null;
            _geUntouched = null;

            _gelDown = null;
            _gelUp = null;
            _gelTouched = null;
            _gelUntouched = null;
        }


        /// <summary>
        /// We check which hand correspond to the Action Button that was choosen
        /// </summary>
        private void CheckButtonHand()
        {
            EControllersInput gazeClick = GetGazeClick();

            // If we use the Gaze Button but the Controllers are inactive
            if (UseGazeButton && !_controllersParameters.UseControllers)
            {
                this.enabled = false;
                throw new Exception("The Button Action Choser parameters for the " + this.GetType().Name + " script are invalid.\n" +
                    "If you want to use the Gaze Click, please activate the Controllers by setting the UseControllers bool in the Window VRSF/Controllers Parameters to true.\n" +
                    "Disabling the script.");
            }
            // If we use the Gaze Button but the chosen gaze button is None
            else if (UseGazeButton && gazeClick == EControllersInput.NONE)
            {
                this.enabled = false;
                throw new Exception("The Button Action Choser parameters for the " + this.GetType().Name + " script are invalid.\n" +
                    "Please specify a GazeButton in the Gaze Parameters Window to use the Gaze Click feature. Disabling the script.");
            }

            // if the Action Button is set to the Wheel Button (SIMULATOR SPECIFIC)
            else if (ActionButton == EControllersInput.WHEEL_BUTTON)
            {
                _isUsingWheelButton = true;
            }

            // if the Action Button is set to the A, B or Right Thumbrest option (OCULUS SPECIFIC)
            else if (ActionButton == EControllersInput.A_BUTTON || ActionButton == EControllersInput.B_BUTTON || ActionButton == EControllersInput.RIGHT_THUMBREST)
            {
                _isUsingOculusButton = true;
                _buttonHand = EHand.RIGHT;
            }
            // if the Action Button is set to the X, Y or Left Thumbrest option (OCULUS SPECIFIC)
            else if (ActionButton == EControllersInput.X_BUTTON || ActionButton == EControllersInput.Y_BUTTON || ActionButton == EControllersInput.LEFT_THUMBREST)
            {
                _isUsingOculusButton = true;
                _buttonHand = EHand.LEFT;
            }

            // if the Action Button is set to the Right Menu option (VIVE AND SIMULATOR SPECIFIC)
            else if (ActionButton == EControllersInput.RIGHT_MENU)
            {
                _isUsingViveButton = true;
                _buttonHand = EHand.RIGHT;
            }

            // If non of the previous solution was chosen, we just check if the button is on the right or left controller
            else if (ActionButton.ToString().Contains("RIGHT"))
            {
                _buttonHand = EHand.RIGHT;
            }
            else if (ActionButton.ToString().Contains("LEFT"))
            {
                _buttonHand = EHand.LEFT;
            }
        }


        /// <summary>
        /// Check which button to use for the Gaze depending on the SDK Loaded
        /// </summary>
        /// <returns>The EControllersInput (button) to use for the Gaze Click</returns>
        private EControllersInput GetGazeClick()
        {
            switch (VRSF_Components.DeviceLoaded)
            {
                case EDevice.OPENVR:
                    return _gazeParameters.GazeButtonOpenVR;
                case EDevice.OVR:
                    return _gazeParameters.GazeButtonOVR;
                default:
                    return _gazeParameters.GazeButtonSimulator;
            }
        }


        /// <summary>
        /// Depending on the Button used for the feature and the Interaction Type, setup the BoolVariable and GameEvents accordingly
        /// </summary>
        /// <returns>true if everything was setup correctly</returns>
        private bool SetupSOReferences()
        {
            // If we use the Gaze Button specified in the Gaze Parameters Window
            if (UseGazeButton)
            {
                return SetupGazeInteraction();
            }
            // If we use the Mouse Wheel Button
            else if (_isUsingWheelButton)
            {
                if (InteractionType == EControllerInteractionType.NONE || InteractionType == EControllerInteractionType.TOUCH)
                {
                    return false;
                }
                _geDown = _inputContainer.WheelClickDown;
                _geUp = _inputContainer.WheelClickUp;
                _isClicking = _inputContainer.WheelIsClicking;
                return true;
            }
            else
            {
                return SetupNormalButton();
            }
        }


        /// <summary>
        /// Check the Interaction Type specified and set it to corresponds to the Gaze BoolVariable
        /// </summary>
        /// <returns>true if everything was setup correctly</returns>
        private bool SetupGazeInteraction()
        {
            if (InteractionType == EControllerInteractionType.NONE)
            {
                return false;
            }
            if ((InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                _geDown = _inputContainer.GazeClickDown;
                _geUp = _inputContainer.GazeClickUp;
                _isClicking = _inputContainer.GazeIsCliking;
            }
            if ((InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                _geTouched = _inputContainer.GazeStartTouching;
                _geUntouched = _inputContainer.GazeStopTouching;
                _isTouching = _inputContainer.GazeIsTouching;
            }
            return true;
        }


        /// <summary>
        /// Setup the _isClicking and _isTouching BoolVariable depending on the InteractionType and the _buttonHand variable.
        /// </summary>
        /// <returns>true if everything was setup correctly</returns>
        private bool SetupNormalButton()
        {
            // If the Interaction Type is set at NONE
            if (InteractionType == EControllerInteractionType.NONE)
            {
                Debug.LogError("Please chose a valid Interaction type in the Inspector. Disabling " + this.name + " script.");
                this.enabled = false;
                return false;
            }

            // If the Interaction Type contains at least CLICK
            if ((InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                switch (_buttonHand)
                {
                    case EHand.RIGHT:
                        _geDown = _inputContainer.RightClickEvents.Items[ControllerInputToSO.GetDownGameEventFor(ActionButton)] as GameEvent;
                        _geUp = _inputContainer.RightClickEvents.Items[ControllerInputToSO.GetUpGameEventFor(ActionButton)] as GameEvent;
                        _isClicking = _inputContainer.RightClickBoolean.Items[ControllerInputToSO.GetClickVariableFor(ActionButton)];
                        break;
                    case EHand.LEFT:
                        _geDown = _inputContainer.LeftClickEvents.Items[ControllerInputToSO.GetDownGameEventFor(ActionButton)] as GameEvent;
                        _geUp = _inputContainer.LeftClickEvents.Items[ControllerInputToSO.GetUpGameEventFor(ActionButton)] as GameEvent;
                        _isClicking = _inputContainer.LeftClickBoolean.Items[ControllerInputToSO.GetClickVariableFor(ActionButton)];
                        break;
                    default:
                        return false;
                }
            }

            // If the Interaction Type contains at least TOUCH
            if ((InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                // Handle Touch events
                switch (_buttonHand)
                {
                    case EHand.RIGHT:
                        _geTouched = _inputContainer.RightTouchEvents.Items[ControllerInputToSO.GetTouchGameEventFor(ActionButton)] as GameEvent;
                        _geUntouched = _inputContainer.RightTouchEvents.Items[ControllerInputToSO.GetReleasedGameEventFor(ActionButton)] as GameEvent;
                        _isTouching = _inputContainer.RightTouchBoolean.Items[ControllerInputToSO.GetTouchVariableFor(ActionButton)];
                        break;
                    case EHand.LEFT:
                        _geTouched = _inputContainer.LeftTouchEvents.Items[ControllerInputToSO.GetTouchGameEventFor(ActionButton)] as GameEvent;
                        _geUntouched = _inputContainer.LeftTouchEvents.Items[ControllerInputToSO.GetReleasedGameEventFor(ActionButton)] as GameEvent;
                        _isTouching = _inputContainer.LeftTouchBoolean.Items[ControllerInputToSO.GetTouchVariableFor(ActionButton)];
                        break;
                    default:
                        return false;
                }
            }

            return true;
        }


        /// <summary>
        /// Create and Setup the GameEventListeners for the Click and the Touch Events
        /// </summary>
        private IEnumerator CreateGELInContainer()
        {
            // We wait until the GEL were created, if necessary
            yield return new WaitForEndOfFrame();

            // CLICK
            if (_geDown != null)
            {
                _gelDown = gameEventsContainer.AddComponent<GameEventListener>();
                SetupGEListener(_gelDown, _geDown, StartActionDown);

                _gelUp = gameEventsContainer.AddComponent<GameEventListener>();
                SetupGEListener(_gelUp, _geUp, StartActionUp);
            }

            // TOUCH
            if (_geTouched != null)
            {
                _gelTouched = gameEventsContainer.AddComponent<GameEventListener>();
                SetupGEListener(_gelTouched, _geTouched, StartActionTouched);

                _gelUntouched = gameEventsContainer.AddComponent<GameEventListener>();
                SetupGEListener(_gelUntouched, _geUntouched, StartActionUntouched);
            }
        }


        /// <summary>
        /// Setup the GameEventListeners for the corresponding GameEvent
        /// </summary>
        /// <param name="gel">The gameEventListener to set</param>
        /// <param name="ge">The GameEvent corresponding</param>
        /// <param name="actionDelegate">The down delegate if relevant. If not, write null</param>
        /// <param name="upDelegate">The up delegate, optionnal</param>
        private void SetupGEListener(GameEventListener gel, GameEvent ge, OnButtonDelegate actionDelegate)
        {
            gel.Event = ge;
            gel.Response = new UnityEvent();
            ge.RegisterListener(gel);
            gel.Response.AddListener(delegate { actionDelegate(); });
        }


        /// <summary>
        /// Create the GameEventListener Container as a child of this transform
        /// </summary>
        /// <returns>The GameObject created</returns>
        private GameObject CreateGEContainer()
        {
            GameObject toReturn = null;
            var bac = GetComponents<ButtonActionChoser>();

            // If there's at least one another ButtonActionChoser on this gameObject and that his script is not the first of the least
            if (bac.Length > 1 && Array.IndexOf(bac, this) != 0)
            {
                StartCoroutine(GetGEL());
            }
            else
            {
                toReturn = new GameObject();
                toReturn.transform.SetParent(transform);
                toReturn.transform.name = this.name + "_GameEvent_Listeners";
            }
            return toReturn;
        }


        private IEnumerator GetGEL()
        {
            yield return new WaitForEndOfFrame();

            foreach (Transform t in GetComponentsInChildren<Transform>())
            {
                if (t.name.Contains("_GameEvent_Listeners"))
                {
                    gameEventsContainer = t.gameObject;
                    break;
                }
            }
            if (gameEventsContainer == null)
            {
                throw new Exception("VRSF : The gameEventsContainer couldn't be found");
            }
        }


        /// <summary>
        /// Check that all the parameters are set correctly in the Inspector.
        /// </summary>
        /// <returns>false if the parameters are incorrect</returns>
        private bool CheckParameters()
        {
            //Check if the Thumbstick are used, and if they are set correctly in that case.
            if (!CheckGivenThumbParameter())
            {
                return false;
            }

            //Check if the Action Button specified is set correctly
            if (!CheckActionButton())
            {
                return false;
            }

            if (UseGazeButton)
            {
                return (RayOrigin != EHand.NONE && InteractionType != EControllerInteractionType.NONE);
            }
            else
            {
                return (RayOrigin != EHand.NONE && InteractionType != EControllerInteractionType.NONE && ActionButton != EControllersInput.NONE);
            }
        }


        /// <summary>
        /// Called if the User is using his Thumb for this feature. Check if the Position to use on the thumbstick are set correctly in the Inspector.
        /// </summary>
        /// <returns>true if everything is set correctly</returns>
        private bool CheckGivenThumbParameter()
        {
            if (ActionButton == EControllersInput.LEFT_THUMBSTICK)
            {
                if (LeftClickThumbPosition == EThumbPosition.NONE &&
                    LeftTouchThumbPosition == EThumbPosition.NONE)
                {
                    Debug.LogError("VRSF : You need to assign a Thumb Position for the Left Thumbstick in this script : " + this.name);
                    return false;
                }

                _thumbPos = _inputContainer.LeftThumbPosition;
            }
            else if (ActionButton == EControllersInput.RIGHT_THUMBSTICK)
            {
                if (RightClickThumbPosition == EThumbPosition.NONE &&
                    RightTouchThumbPosition == EThumbPosition.NONE)
                {
                    Debug.LogError("VRSF : You need to assign a Thumb Position for the Right Thumbstick in this script : " + this.name);
                    return false;
                }

                _thumbPos = _inputContainer.RightThumbPosition;
            }

            return true;
        }


        /// <summary>
        /// Check that the ActionButton chosed by the user is corresponding to the SDK that was loaded.
        /// </summary>
        /// <returns>true if the ActionButton is correctly set</returns>
        private bool CheckActionButton()
        {
            // If we are using an Oculus Touch Specific Button but the device loaded is not the Oculus
            if (_isUsingOculusButton && (VRSF_Components.DeviceLoaded != EDevice.OVR && VRSF_Components.DeviceLoaded != EDevice.SIMULATOR))
            {
                Debug.LogError("The Button Action Choser parameters for the " + this.GetType().Name + " script are invalid.\n" +
                    "Please specify a button that is available for the current device (" + VRSF_Components.DeviceLoaded + ") and not only for the Oculus. Disabling the script.");
                return false;
            }
            // If we are using an OpenVR Specific Button but the device loaded is not the OpenVR
            else if (_isUsingViveButton && (VRSF_Components.DeviceLoaded != EDevice.OPENVR && VRSF_Components.DeviceLoaded != EDevice.SIMULATOR))
            {
                Debug.LogError("The Button Action Choser parameters for the " + this.GetType().Name + " script are invalid.\n" +
                    "Please specify a button that is available for the current device (" + VRSF_Components.DeviceLoaded + ") and not only for the Vive. Disabling the script.");
                return false;
            }
            // If we are using a Simulator Specific Button but the device loaded is not the Simulator
            else if (_isUsingWheelButton && VRSF_Components.DeviceLoaded != EDevice.SIMULATOR)
            {
                Debug.LogError("The Button Action Choser parameters for the " + this.GetType().Name + " script are invalid.\n" +
                    "Please specify a button that is available for the current device (" + VRSF_Components.DeviceLoaded + ") and not only for the Simulator. Disabling the script.");
                return false;
            }
            else
            {
                return true;
            }
        }


        /// <summary>
        /// Method called when user click the specified button
        /// </summary>
        private void StartActionDown()
        {
            if (CanBeUsed)
            {
                // if we use the Thumb, we need to check its position on the Thumbstick/Touchpad
                if (_thumbPos != null && ClickThreshold > 0.0f)
                {
                    _unclickEventWasRaised = false;
                    HandleClickMethodWithThumb(OnButtonStartClicking);
                }
                else
                {
                    OnButtonStartClicking.Invoke();
                }
            }
        }


        /// <summary>
        /// Method called when user release the specified button
        /// </summary>
        private void StartActionUp()
        {
            if (CanBeUsed)
            {
                // If we don't use the Thumb
                if (_thumbPos == null)
                    OnButtonStopClicking.Invoke();

                // If we use the Thumb and the click action is beyond the threshold
                else if (_thumbPos != null && _clickActionBeyondThreshold)
                    OnButtonStopClicking.Invoke();

                // If we use the Thumb and the ClickThreshold is equal to 0
                else if (_thumbPos != null && ClickThreshold == 0.0f)
                    OnButtonStopClicking.Invoke();
            }
        }


        /// <summary>
        /// Method called when user start touching the specified button
        /// </summary>
        private void StartActionTouched()
        {
            if (CanBeUsed)
            {
                // if we use the Thumb, we need to check its position on the Thumbstick/Touchpad
                if (_thumbPos != null && TouchThreshold > 0.0f)
                {
                    _untouchedEventWasRaised = false;
                    HandleTouchMethodWithThumb(OnButtonStartTouching);
                }
                else
                {
                    OnButtonStartTouching.Invoke();
                }
            }
        }


        /// <summary>
        /// Method called when user stop touching the specified button
        /// </summary>
        private void StartActionUntouched()
        {
            if (CanBeUsed)
            {
                // If we don't use the Thumb
                if (_thumbPos == null)
                    OnButtonStopTouching.Invoke();

                // If we use the Thumb and the click action is beyond the threshold
                else if (_thumbPos != null && _touchActionBeyondThreshold)
                    OnButtonStopTouching.Invoke();

                // If we use the Thumb and the ClickThreshold is equal to 0
                else if (_thumbPos != null && TouchThreshold == 0.0f)
                    OnButtonStopTouching.Invoke();
            }
        }


        /// <summary>
        /// Method called when user stop touching the specified button
        /// </summary>
        private void StartActionIsClicking()
        {
            _unclickEventWasRaised = false;

            // if we use the Thumb, we need to check its position on the Thumbstick/Touchpad
            if (_thumbPos != null)
            {
                bool oldState = _clickActionBeyondThreshold;
                _clickActionBeyondThreshold = HandleClickMethodWithThumb(OnButtonIsClicking);

                if (oldState && !_clickActionBeyondThreshold)
                {
                    OnButtonStopClicking.Invoke();
                    _unclickEventWasRaised = true;
                }
            }
            else
            {
                OnButtonIsClicking.Invoke();
            }
        }


        /// <summary>
        /// Method called when the user stop touching the specified button
        /// </summary>
        private void StartActionIsTouching()
        {
            _untouchedEventWasRaised = false;

            // if we use the Thumb, we need to check its position on the Thumbstick/Touchpad
            if (_thumbPos != null)
            {
                bool oldState = _touchActionBeyondThreshold;
                _touchActionBeyondThreshold = HandleTouchMethodWithThumb(OnButtonIsTouching);

                // If the user was above the threshold, but moved his finger, we invoke the StopTouching Event
                if (oldState && !_touchActionBeyondThreshold)
                {
                    OnButtonStopTouching.Invoke();
                    _untouchedEventWasRaised = true;
                }
            }
            else
            {
                OnButtonIsTouching.Invoke();
            }
        }


        /// <summary>
        /// Call the CheckThumbPosition method according to the button (_buttonHand) used for this feature for the Click Events
        /// </summary>
        /// <param name="buttonEvent">The delegate to call if the thumbPos is on the good position</param>
        /// <returns>true if the delegate was called</returns>
        private bool HandleClickMethodWithThumb(UnityEvent buttonEvent)
        {
            switch (_buttonHand)
            {
                case EHand.RIGHT:
                    return CheckThumbPosition(RightClickThumbPosition, buttonEvent, ClickThreshold);
                case EHand.LEFT:
                    return CheckThumbPosition(LeftClickThumbPosition, buttonEvent, ClickThreshold);
                default:
                    return false;
            }
        }


        /// <summary>
        /// Call the CheckThumbPosition method according to the button (_buttonHand) used for this feature for the Touch Events
        /// </summary>
        /// <param name="buttonEvent">The delegate to call if the thumbPos is on the good position</param>
        /// <param name="threshold">The minimum value on the controller thumbstick (between -1 and 1) to call the delegate method</param>
        /// <returns>true if the delegate was called</returns>
        private bool HandleTouchMethodWithThumb(UnityEvent buttonEvent)
        {
            switch (_buttonHand)
            {
                case EHand.RIGHT:
                    return CheckThumbPosition(RightTouchThumbPosition, buttonEvent, TouchThreshold);
                case EHand.LEFT:
                    return CheckThumbPosition(LeftTouchThumbPosition, buttonEvent, TouchThreshold);
                default:
                    return false;
            }
        }


        /// <summary>
        /// Check if the position of the finger on the controller correspond to the one specified in the Editor
        /// </summary>
        /// <param name="posToCheck">The thumb position required to call the delegate method</param>
        /// <param name="buttonEvent">The delegate method to call if the thumbPos is on the good position</param>
        /// <param name="threshold">The minimum value on the controller thumbstick (between -1 and 1) to call the delegate method</param>
        /// <returns>true if the delegate was called</returns>
        private bool CheckThumbPosition(EThumbPosition posToCheck, UnityEvent buttonEvent, float threshold)
        {
            // If the position to check is ANY, and at least one of the four position is more than the threshold in the both axis
            if (posToCheck == EThumbPosition.ANY && (_thumbPos.Value.x <= -threshold || _thumbPos.Value.x >= threshold || _thumbPos.Value.y >= threshold || _thumbPos.Value.y <= -threshold))
            {
                buttonEvent.Invoke();
                return true;
            }

            // If the position to check contains at least LEFT, we check if if the pos value is < to the threshold in the x axis
            if ((posToCheck & EThumbPosition.LEFT) == EThumbPosition.LEFT && _thumbPos.Value.x <= -threshold)
            {
                buttonEvent.Invoke();
                return true;
            }

            // If the position to check contains at least RIGHT, we check if if the pos value is > to the threshold in the x axis
            if ((posToCheck & EThumbPosition.RIGHT) == EThumbPosition.RIGHT && _thumbPos.Value.x >= threshold)
            {
                buttonEvent.Invoke();
                return true;
            }

            // If the position to check contains at least UP, we check if if the pos value is > to the threshold in the y axis
            if ((posToCheck & EThumbPosition.UP) == EThumbPosition.UP && _thumbPos.Value.y >= threshold)
            {
                buttonEvent.Invoke();
                return true;
            }

            // If the position to check contains at least DOWN, we check if if the pos value is < to the threshold in the y axis
            if ((posToCheck & EThumbPosition.DOWN) == EThumbPosition.DOWN && _thumbPos.Value.y <= -threshold)
            {
                buttonEvent.Invoke();
                return true;
            }

            return false;
        }
        #endregion


        #region GETTERS_SETTERS
        public RaycastHitVariable HitVar
        {
            get
            {
                return _hit;
            }
        }

        public GazeParametersVariable GazeParameters
        {
            get
            {
                return _gazeParameters;
            }
        }

        public ControllersParametersVariable ControllersParameters
        {
            get
            {
                return _controllersParameters;
            }
        }

        public InputVariableContainer InputContainer
        {
            get
            {
                return _inputContainer;
            }
        }

        public InteractionVariableContainer InteractionContainer
        {
            get
            {
                return _interactionContainer;
            }
        }

        public EHand ButtonHand
        {
            get
            {
                return _buttonHand;
            }
        }

        public Vector2Variable ThumbPos
        {
            get
            {
                return _thumbPos;
            }
        }

        public RayVariable RayVar
        {
            get
            {
                return _rayVar;
            }
        }
        #endregion
    }
}