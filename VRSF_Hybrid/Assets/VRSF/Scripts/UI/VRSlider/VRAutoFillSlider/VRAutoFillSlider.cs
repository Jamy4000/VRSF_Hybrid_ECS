using ScriptableFramework.Util;
using ScriptableFramework.Variables;
using VRSF.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VRSF.Interactions;
using VRSF.Inputs;
using VRSF.Utils.Events;

namespace VRSF.UI
{
    /// <summary>
    /// Handle the references and setup for the GameEvents, GameEventListeners and boxCollider of the VRAutoFillSlider
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class VRAutoFillSlider : Slider
    {
        #region PUBLIC_VARIABLES
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
        private InputVariableContainer _inputContainer;
        private InteractionVariableContainer _interactionContainer;

        private BoolVariable _leftIsClicking;
        private BoolVariable _rightIsClicking;

        private bool _barFilled;                                           // Whether the bar is currently filled.
        private float _timer;                                              // Used to determine how much of the bar should be filled.
        private Coroutine _fillBarRoutine;                                 // Reference to the coroutine that controls the bar filling up, used to stop it if required.

        private EHand _handFilling = EHand.NONE;                              // Reference to the type of Hand that is filling the slider
        
        private bool _boxColliderSetup;
        #endregion


        #region MONOBEHAVIOUR_METHODS
        protected override void OnEnable()
        {
            base.OnEnable();

            if (Application.isPlaying)
            {
                SetupUIElement();

                // We setup the BoxCollider size and center
                StartCoroutine(SetupBoxCollider());
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ObjectWasClickedEvent.UnregisterListener(CheckSliderClick);
            ObjectWasHoveredEvent.UnregisterListener(CheckSliderHovered);
        }

        private void Update()
        {
            if (Application.isPlaying)
            {
                if (!_boxColliderSetup)
                {
                    SetupBoxCollider();
                    return;
                }

                // if the bar is being filled
                if (_fillBarRoutine != null)
                {
                    CheckHandStillOver();
                }
            }
        }
        #endregion


        #region PUBLIC_METHODS
        /// <summary>
        /// Event called when the user is clicking on something
        /// </summary>
        /// <param name="clickEvent">The event raised when an object is clicked</param>
        public void CheckSliderClick(ObjectWasClickedEvent clickEvent)
        {
            if (IsInteractable() && FillWithClick)
            {
                // if the object clicked correspond to this transform and the coroutine to fill the bar didn't started yet
                if (clickEvent.ObjectClicked == transform && _fillBarRoutine == null)
                {
                    HandleHandInteracting(clickEvent.HandClicking);
                }
                // If the user was clicking the bar but stopped
                else if (_fillBarRoutine != null)
                {
                    HandleUp();
                }
            }
        }

        /// <summary>
        /// Event called when the user is looking at the Slider
        /// </summary>
        /// <param name="hoverEvent">The event raised when an object is hovered</param>
        public void CheckSliderHovered(ObjectWasHoveredEvent hoverEvent)
        {
            if (IsInteractable() && !FillWithClick)
            {
                // if the object hovered correspond to this transform and the coroutine to fill the bar didn't started yet
                if (hoverEvent.ObjectHovered == transform && _fillBarRoutine == null)
                {
                    HandleHandInteracting(hoverEvent.HandHovering);
                }
                // If the user was hovering the bar but stopped
                else if (hoverEvent.ObjectHovered != transform && _fillBarRoutine != null)
                {
                    HandleUp();
                }
            }
        }
        #endregion PUBLIC_METHODS


        #region PRIVATE_METHODS
        private void SetupUIElement()
        {
            _interactionContainer = InteractionVariableContainer.Instance;
            _inputContainer = InputVariableContainer.Instance;

            _rightIsClicking = _inputContainer.RightClickBoolean.Get("TriggerIsDown");
            _leftIsClicking = _inputContainer.LeftClickBoolean.Get("TriggerIsDown");

            GetFillRectReference();
            
            ObjectWasClickedEvent.RegisterListener(CheckSliderClick);
            ObjectWasHoveredEvent.RegisterListener(CheckSliderHovered);

            // If the controllers are not used, we cannot click on the slider, so we will fill the slider with the Over events
            if (!ControllersParametersVariable.Instance.UseControllers && FillWithClick)
            {
                FillWithClick = false;
                Debug.LogError("VRSF : UseController is set at false. The auto fill slider won't use the controller to fill but the gaze.");
            }
        }

        /// <summary>
        /// Check which hand is pointing toward the slider
        /// </summary>
        private void HandleHandInteracting(EHand handPointing)
        {
            _handFilling = handPointing; 

            if (_handFilling != EHand.NONE && _fillBarRoutine == null)
            {
                _fillBarRoutine = StartCoroutine(FillBar());
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
                if (_handFilling == EHand.LEFT || _handFilling == EHand.RIGHT || _handFilling == EHand.GAZE)
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
                    if ((FillWithClick && !_leftIsClicking.Value) || !_interactionContainer.IsOverSomethingLeft.Value)
                        HandleUp();
                    break;

                case (EHand.RIGHT):
                    if ((FillWithClick && !_rightIsClicking.Value) || !_interactionContainer.IsOverSomethingRight.Value)
                        HandleUp();
                    break;

                case (EHand.GAZE):
                    if ((FillWithClick && !_inputContainer.GazeIsCliking.Value) || !_interactionContainer.IsOverSomethingGaze.Value)
                        HandleUp();
                    break;
            }
        }


        /// <summary>
        /// Set the BoxCollider if the SetColliderAuto is at true
        /// </summary>
        /// <returns></returns>
        IEnumerator<WaitForEndOfFrame> SetupBoxCollider()
        {
            yield return new WaitForEndOfFrame();

            if (SetColliderAuto)
            {
                BoxCollider box = GetComponent<BoxCollider>();
                box = VRUIBoxColliderSetup.CheckBoxColliderSize(box, GetComponent<RectTransform>());
            }

            _boxColliderSetup = true;
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