using ScriptableFramework.Events;
using ScriptableFramework.Variables;
using System;
using UnityEngine;
using UnityEngine.Events;
using VRSF.Controllers;
using VRSF.Gaze;
using VRSF.Inputs;
using VRSF.Interactions;

namespace VRSF.Utils.Components
{
    [Serializable]
    public class ButtonActionChoserComponents : MonoBehaviour
    {
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

        // VRSF Parameters references
        [HideInInspector] public GazeParametersVariable GazeParameters;
        [HideInInspector] public ControllersParametersVariable ControllersParameters;
        [HideInInspector] public InputVariableContainer InputsContainer;
        [HideInInspector] public InteractionVariableContainer InteractionsContainer;

        // The RaycastHitVariable and Ray to check for this feature
        [HideInInspector] public RaycastHitVariable HitVar;
        [HideInInspector] public RayVariable RayVar;

        // The hand on which the button to use is situated
        [HideInInspector] public EHand _buttonHand = EHand.NONE;

        // To keep track of the event that were raised, used for the features that use the Thumbstick
        [HideInInspector] public bool _clickActionBeyondThreshold;
        [HideInInspector] public bool _touchActionBeyondThreshold;
        [HideInInspector] public bool _untouchedEventWasRaised;
        [HideInInspector] public bool _unclickEventWasRaised;

        // For SDKs Specific ActionButton 
        [HideInInspector] public bool _isUsingOculusButton;
        [HideInInspector] public bool _isUsingViveButton;
        [HideInInspector] public bool _isUsingWheelButton;

        // Thumb Parameters
        [HideInInspector] public Vector2Variable _thumbPos = null;

        // All GameEvents
        [HideInInspector] public GameEvent _geDown = null;
        [HideInInspector] public GameEvent _geUp = null;
        [HideInInspector] public GameEvent _geTouched = null;
        [HideInInspector] public GameEvent _geUntouched = null;

        // All GameEventListeners
        [HideInInspector] public GameEventListener _gelDown = null;
        [HideInInspector] public GameEventListener _gelUp = null;
        [HideInInspector] public GameEventListener _gelTouched = null;
        [HideInInspector] public GameEventListener _gelUntouched = null;

        // BoolVariable to check
        [HideInInspector] public BoolVariable _isTouching = null;
        [HideInInspector] public BoolVariable _isClicking = null;

        // GameObject containing the GameEventListeners
        [HideInInspector] public GameObject gameEventsContainer = null;
    }
}