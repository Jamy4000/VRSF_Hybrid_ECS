using ScriptableFramework.Events;
using ScriptableFramework.Variables;
using System;
using UnityEngine;
using UnityEngine.Events;
using VRSF.Controllers;
using VRSF.Inputs;

namespace VRSF.Utils.Components
{
    [Serializable]
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity))]
    public class ButtonActionChoserComponents : MonoBehaviour
    {
        /// <summary>
        /// Delegate to pass the method to call when the ActionButton is down or touched
        /// </summary>
        /// <param name="t">The transform that was hit</param>
        public delegate void OnButtonDelegate(ButtonActionChoserComponents comp);

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
        
        [HideInInspector] public bool ActionButtonIsReady = false;
        [HideInInspector] public bool ParametersAreInvalid = false;
        [HideInInspector] public bool CanBeUsed = true;

        // The RaycastHitVariable and Ray to check for this feature
        [HideInInspector] public RaycastHitVariable HitVar;
        [HideInInspector] public RayVariable RayVar;

        // The hand on which the button to use is situated
        [HideInInspector] public EHand ButtonHand = EHand.NONE;

        // To keep track of the event that were raised, used for the features that use the Thumbstick
        [HideInInspector] public bool ClickActionBeyondThreshold;
        [HideInInspector] public bool TouchActionBeyondThreshold;
        [HideInInspector] public bool UntouchedEventWasRaised;
        [HideInInspector] public bool UnclickEventWasRaised;

        // For SDKs Specific ActionButton 
        [HideInInspector] public bool IsUsingOculusButton;
        [HideInInspector] public bool IsUsingViveButton;
        [HideInInspector] public bool IsUsingWheelButton;

        // Thumb Parameters
        [HideInInspector] public Vector2Variable ThumbPos = null;

        // All GameEvents
        [HideInInspector] public GameEvent GeDown = null;
        [HideInInspector] public GameEvent GeUp = null;
        [HideInInspector] public GameEvent GeTouched = null;
        [HideInInspector] public GameEvent GeUntouched = null;

        // All GameEventListeners
        [HideInInspector] public GameEventListener GelDown = null;
        [HideInInspector] public GameEventListener GelUp = null;
        [HideInInspector] public GameEventListener GelTouched = null;
        [HideInInspector] public GameEventListener GelUntouched = null;

        // BoolVariable to check
        [HideInInspector] public BoolVariable IsTouching = null;
        [HideInInspector] public BoolVariable IsClicking = null;

        // GameObject containing the GameEventListeners
        [HideInInspector] public GameObject GameEventsContainer = null;
    }
}