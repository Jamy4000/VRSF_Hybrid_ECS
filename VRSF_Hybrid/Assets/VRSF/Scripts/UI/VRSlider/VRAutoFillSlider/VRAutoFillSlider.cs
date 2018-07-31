using ScriptableFramework.Events;
using ScriptableFramework.Util;
using ScriptableFramework.Variables;
using VRSF.Controllers;
using VRSF.Gaze;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using VRSF.Interactions;
using VRSF.Inputs;

namespace VRSF.UI
{
    /// <summary>
    /// Handle the references and setup for the GameEvents, GameEventListeners and boxCollider of the VRAutoFillSlider
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class VRAutoFillSlider : Slider
    {
        #region PUBLIC_VARIABLES

        [Header("The GameObject containing the Game Event Listeners, set at runtime")]
        [HideInInspector] public GameObject GameEventListenersContainer;

        [Tooltip("If you want to set the collider yourself, set this value to false.")]
        [SerializeField] public bool SetColliderAuto = true;
        
        [Tooltip("If UseController is at false, will automatically be set at false.\n" +
            "If true, slider will fill only when the user is clicking on it.\n" +
            "If false, slider will fill only when the user is pointing at it.")]
        [SerializeField] public bool FillWithClick;

        [Tooltip("The time it takes to fill the slider.")]
        [SerializeField] public float FillTime = 3f;

        [Header("Unity Events for bar filled and released.")]
        [SerializeField] public UnityEvent OnBarFilled;
        [Tooltip("The OnBarReleased will only be called if the bar was filled before the user release it.")]
        [SerializeField] public UnityEvent OnBarReleased;

        #endregion


        #region PRIVATE_VARIABLES
        // The Controllers and Gaze Parameters
        private ControllersParametersVariable _controllersParameter;
        private GazeParametersVariable _gazeParameter;

        // The Interaction Variable and GameEvents Container
        private InteractionVariableContainer _interactionContainer;
        private InputVariableContainer _inputContainer;

        private BoolVariable _leftIsClicking;
        private BoolVariable _rightIsClicking;

        private bool _barFilled;                                           // Whether the bar is currently filled.
        private float _timer;                                              // Used to determine how much of the bar should be filled.
        private Coroutine _fillBarRoutine;                                 // Reference to the coroutine that controls the bar filling up, used to stop it if required.

        private EHand _handFilling = EHand.NONE;                              // Reference to the type of Hand that is filling the slider

        private GameObject _gameEventListenersContainer;

        private Dictionary<string, GameEventListenerTransform> _clickListenersDictionary;
        private Dictionary<string, GameEventListenerTransform> _overListenersDictionary;
        private Dictionary<string, GameEventTransform> _clickEventsDictionary;
        private Dictionary<string, GameEventTransform> _overEventsDictionary;

        private IUISetupClickAndOver _clickAndOverSetup;
        private VRUISetup _uiSetup;

        private VRUISetup.CheckObjectDelegate _checkObjectClicked;
        private VRUISetup.CheckObjectDelegate _checkObjectOver;

        private bool _boxColliderSetup;
        #endregion


        #region MONOBEHAVIOUR_METHODS
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            MakeBasicSetup();
        }
#endif

        protected override void Start()
        {
            base.Start();
            if (Application.isPlaying)
            {
                MakeBasicSetup();
                SetupUIElement();
            }
        }

        private void Update()
        {
            if (!_boxColliderSetup && gameObject.activeInHierarchy)
            {
                StartCoroutine(SetupBoxCollider());
                return;
            }

            if (Application.isPlaying)
            {
                // if the bar is being filled
                if (_fillBarRoutine != null)
                {
                    CheckHandStillOver();
                }
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            try
            {
                if (this.enabled)
                {
                    _clickListenersDictionary = _uiSetup.EndApp(_clickListenersDictionary, _clickEventsDictionary);
                    _overListenersDictionary = _uiSetup.EndApp(_overListenersDictionary, _overEventsDictionary);
                }
            }
            catch //(Exception e) 
            {
                // Listeners not set in the scene yet.
                //Debug.Log("VRSF : The listeners for the VR Auto Fill Slider weren't set properly.\n" + e.ToString());
            }
        }

        private void OnApplicationQuit()
        {
            try
            {
                if (this.enabled)
                {
                    _clickListenersDictionary = _uiSetup.EndApp(_clickListenersDictionary, _clickEventsDictionary);
                    _overListenersDictionary = _uiSetup.EndApp(_overListenersDictionary, _overEventsDictionary);
                }
            }
            catch (Exception e)
            {
                // Listeners not set in the scene yet.
                Debug.Log("VRSF : The listeners for the VR Auto Fill Slider weren't set properly.\n" + e.ToString());
            }
        }
        #endregion


        #region PUBLIC_METHODS

        /// <summary>
        /// Event called when the user is clicking on something
        /// </summary>
        /// <param name="value">The object that was clicked</param>
        public void CheckSliderClick(Transform value)
        {
            // if the slider is Interactable, the object clicked correspond to this transform, we use the click event to interact
            // and the coroutine to fill the bar didn't started yet
            if (IsInteractable() && value == transform && FillWithClick && _fillBarRoutine == null)
            {
                if (!FillWithClick)
                    CheckHandPointing();
                else
                    CheckHandClicking();
            }
        }

        /// <summary>
        /// Event called when the user is looking at the Slider
        /// </summary>
        /// <param name="value">The object that was looked at</param>
        public void CheckSliderHovered(Transform value)
        {
            // if the slider is Interactable, the object clicked correspond to this transform, we use the over event to interact
            // and the coroutine to fill the bar didn't started yet
            if (IsInteractable() && value == transform && !FillWithClick && _fillBarRoutine == null)
            {
                CheckHandPointing();
            }
        }

        #endregion PUBLIC_METHODS


        #region PRIVATE_METHODS
        private void MakeBasicSetup()
        {
            // We initialize the Listeners Dictionaries
            InitializeListenerDictionaries();

            // We create new object to setup the button references; listeners and GameEventListeners, and add the delegate method to it
            _checkObjectClicked = CheckSliderClick;
            _checkObjectOver = CheckSliderHovered;

            _uiSetup = new VRUISetup(_checkObjectClicked, _checkObjectOver);
            _clickAndOverSetup = new VRAutoFillSliderSetup();

            // Check if the Listeners GameObject are set correctly. If not, create the children
            bool clickListernersPresent = _clickAndOverSetup.CheckGameEventListenerChild(ref _gameEventListenersContainer, ref _clickListenersDictionary, transform, EUIInputType.CLICK);
            bool overListernersPresent = _clickAndOverSetup.CheckGameEventListenerChild(ref _gameEventListenersContainer, ref _overListenersDictionary, transform, EUIInputType.OVER);

            if (!clickListernersPresent && !overListernersPresent)
                _uiSetup.CreateGameEventListenerChild(ref _gameEventListenersContainer, transform);

            if (Application.isPlaying && gameObject.activeInHierarchy)
            {
                // We setup the BoxCollider size and center
                StartCoroutine(SetupBoxCollider());
            }

            GetFillRectReference();
        }

        private void SetupUIElement()
        {
            _controllersParameter = ControllersParametersVariable.Instance;
            _gazeParameter = GazeParametersVariable.Instance;

            _interactionContainer = InteractionVariableContainer.Instance;
            _inputContainer = InputVariableContainer.Instance;

            _rightIsClicking = _inputContainer.RightClickBoolean.Get("TriggerIsDown");
            _leftIsClicking = _inputContainer.LeftClickBoolean.Get("TriggerIsDown");

            // If the controllers are not used, we cannot click on the slider, so we will fill the slider with the Over events
            if (!_controllersParameter.UseControllers && FillWithClick)
            {
                FillWithClick = false;
                Debug.Log("VRSF : UseController is set at false. The auto fill slider won't use the controller to fill but the gaze.");
            }

            // We initialize the _EventsDictionary
            InitializeEventsDictionaries();

            // We setup the ListenersDictionary
            _clickListenersDictionary = _uiSetup.CheckGameEventListenersPresence(_gameEventListenersContainer, _clickListenersDictionary, 6);
            _overListenersDictionary = _uiSetup.CheckGameEventListenersPresence(_gameEventListenersContainer, _overListenersDictionary, 6);

            _clickListenersDictionary = _uiSetup.SetGameEventListeners(_clickListenersDictionary, _clickEventsDictionary, _gazeParameter.UseGaze);
            _overListenersDictionary = _uiSetup.SetGameEventListeners(_overListenersDictionary, _overEventsDictionary, _gazeParameter.UseGaze);
        }

        /// <summary>
        /// Check which hand is pointing toward the slider
        /// </summary>
        private void CheckHandPointing()
        {
            if (_controllersParameter.UseControllers && _overListenersDictionary["Right"].Value == transform)
            {
                _handFilling = EHand.RIGHT;
            }
            else if (_controllersParameter.UseControllers && (_overListenersDictionary["Left"].Value == transform))
            {
                _handFilling = EHand.LEFT;
            }
            else if (_gazeParameter.UseGaze && _overListenersDictionary["Gaze"].Value == transform)
            {
                _handFilling = EHand.GAZE;
            }
            else
            {
                Debug.LogError("VRSF : Couldn't find reference to the Click Listener on the slider.");
            }

            if (_handFilling != EHand.NONE && _fillBarRoutine == null)
            {
                _fillBarRoutine = StartCoroutine(FillBar());
            }
        }

        /// <summary>
        /// Check which hand is clicking on the Slider
        /// </summary>
        private void CheckHandClicking()
        {
            // Next if statements are to check which click was used on the slider
            if (_rightIsClicking.Value && _overListenersDictionary["Right"].Value == transform)
            {
                _handFilling = EHand.RIGHT;
                _fillBarRoutine = StartCoroutine(FillBar());
            }
            else if (_leftIsClicking.Value && _overListenersDictionary["Left"].Value == transform)
            {
                _handFilling = EHand.LEFT;
                _fillBarRoutine = StartCoroutine(FillBar());
            }
            else if (_gazeParameter.UseGaze && _inputContainer.GazeIsCliking.Value && _overListenersDictionary["Gaze"].Value == transform)
            {
                _handFilling = EHand.GAZE;
                _fillBarRoutine = StartCoroutine(FillBar());
            }
            else
            {
                Debug.LogError("VRSF : Couldn't find reference to the Click Listener on the slider.");
            }
        }

        /// <summary>
        /// Coroutine called to fill the bar. Stop only if the user release it.
        /// </summary>
        /// <returns>a new IEnumerator</returns>
        private IEnumerator FillBar()
        {
            // When the bar starts to fill, reset the timer.
            _timer = 0f;

            // Until the timer is greater than the fill time...
            while (_timer < FillTime)
            {
                // ... add to the timer the difference between frames.
                _timer += Time.deltaTime;

                // Set the value of the slider or the UV based on the normalised time.
                value = (_timer / FillTime);

                onValueChanged.Invoke(value);

                // Wait until next frame.
                yield return new WaitForEndOfFrame();

                // If the user is still looking at the bar, go on to the next iteration of the loop.
                if (_handFilling == EHand.GAZE)
                    continue;
                else if ((_handFilling == EHand.LEFT || _handFilling == EHand.RIGHT))
                    continue;

                // If the user is no longer looking at the bar, reset the timer and bar and leave the function.

                value = 0f;
                yield break;
            }

            // If the loop has finished the bar is now full.
            _barFilled = true;
            OnBarFilled.Invoke();
        }

        /// <summary>
        /// Method called when the user release the slider bar
        /// </summary>
        private void HandleUp()
        {
            // If the bar was filled and the user is releasing it, we invoke the OnBarReleased event
            if (_barFilled)
            {
                OnBarReleased.Invoke();
                _barFilled = false;
            }

            // If the coroutine has been started (and thus we have a reference to it) stop it.
            if (_fillBarRoutine != null)
            {
                StopCoroutine(_fillBarRoutine);
                _fillBarRoutine = null;
            }

            // Reset the timer and bar values.
            _timer = 0f;
            value = 0.0f;

            // Set the Hand filling at null
            _handFilling = EHand.NONE;
        }

        /// <summary>
        /// Check if the Controller or the Gaze filling the bar is still over the Slider or, if we use the click, if the user is still clicking
        /// </summary>
        private void CheckHandStillOver()
        {
            switch (_handFilling)
            {
                // if we fill with click and the user is not clicking anymore
                // OR, if the user is not on the slider anymore

                case (EHand.LEFT):
                    if ((FillWithClick && !_leftIsClicking.Value) || !_interactionContainer.IsOverSomethingLeft.Value || _overListenersDictionary["Left"].Value != transform)
                        HandleUp();
                    break;

                case (EHand.RIGHT):
                    if ((FillWithClick && !_rightIsClicking.Value) || !_interactionContainer.IsOverSomethingRight.Value || _overListenersDictionary["Right"].Value != transform)
                        HandleUp();
                    break;

                case (EHand.GAZE):
                    if ((FillWithClick && !_inputContainer.GazeIsCliking.Value) || !_interactionContainer.IsOverSomethingGaze.Value || _overListenersDictionary["Gaze"].Value != transform)
                        HandleUp();
                    break;
            }
        }


        /// <summary>
        /// Start a coroutine that wait for the second frame to set the BoxCollider
        /// </summary>
        /// <returns></returns>
        IEnumerator<WaitForEndOfFrame> SetupBoxCollider()
        {
            yield return new WaitForEndOfFrame();

            if (SetColliderAuto)
            {
                BoxCollider box = GetComponent<BoxCollider>();
                box = _uiSetup.CheckBoxColliderSize(box, GetComponent<RectTransform>());
            }

            _boxColliderSetup = true;
        }

        /// <summary>
        /// Initialize the CLick and Over Listener dictionaries with new GameEventListenerTransform
        /// </summary>
        void InitializeListenerDictionaries()
        {
            _clickListenersDictionary = new Dictionary<string, GameEventListenerTransform>
            {
                { "Right", null },
                { "Left", null },
                { "Gaze", null },
            };

            _overListenersDictionary = new Dictionary<string, GameEventListenerTransform>
            {
                { "Right", null },
                { "Left", null },
                { "Gaze", null },
            };
        }

        /// <summary>
        /// Initialize the CLick and Over Events dictionaries with the references to the corresponding GameEvents
        /// </summary>
        void InitializeEventsDictionaries()
        {
            _clickEventsDictionary = new Dictionary<string, GameEventTransform>
            {
                { "Right", _interactionContainer.RightObjectWasClicked},
                { "Left", _interactionContainer.LeftObjectWasClicked },
                { "Gaze", _interactionContainer.GazeObjectWasClicked },
            };

            _overEventsDictionary = new Dictionary<string, GameEventTransform>
            {
                { "Right", _interactionContainer.RightOverObject },
                { "Left", _interactionContainer.LeftOverObject },
                { "Gaze", _interactionContainer.GazeOverObject },
            };
        }

        /// <summary>
        /// Try to get and set the fillRect reference by looking for a Fill object in the deepChildren
        /// </summary>
        void GetFillRectReference()
        {
            try
            {
                fillRect = transform.FindDeepChild("Fill").GetComponent<RectTransform>();
            }
            catch
            {
                Debug.LogError("VRSF : Please add a Fill GameObject with RectTransform as a child or DeepChild of this VR Auto Fill Slider.");
            }
        }
        #endregion


        // EMPTY
        #region GETTERS_SETTERS

        #endregion
    }
}