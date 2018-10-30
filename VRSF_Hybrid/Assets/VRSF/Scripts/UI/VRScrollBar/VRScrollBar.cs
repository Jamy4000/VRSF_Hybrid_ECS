using ScriptableFramework.Util;
using ScriptableFramework.Variables;
using VRSF.Controllers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRSF.Interactions;
using VRSF.Inputs;
using VRSF.Utils.Events;

namespace VRSF.UI
{
    /// <summary>
    /// Create a new ScrollBar based on the Unity scrollbar, but adapted for VR with the Scriptable Framework
    /// </summary>
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Image))]
    public class VRScrollBar : Scrollbar
    {
        #region PUBLIC_VARIABLES
        [Tooltip("If you want to set the collider yourself, set this value to false.")]
        [SerializeField] public bool SetColliderAuto = true;
        #endregion


        #region PRIVATE_VARIABLES
        private InputVariableContainer _inputContainer;

        private BoolVariable _rightTriggerDown;
        private BoolVariable _leftTriggerDown;

        Transform _MinPosBar;
        Transform _MaxPosBar;

        EHand _HandHoldingHandle = EHand.NONE;
        
        Dictionary<string, RaycastHitVariable> _RaycastHitDictionary;

        IUISetupScrollable _ScrollableSetup;

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
            ObjectWasClickedEvent.UnregisterListener(CheckBarClick);
        }

        private void Update()
        {
            if (Application.isPlaying)
            {
                if (!_boxColliderSetup)
                {
                    return;
                }

                CheckClickDown();

                if (_HandHoldingHandle != EHand.NONE)
                {
                    value = _ScrollableSetup.MoveComponent(_HandHoldingHandle, _MinPosBar, _MaxPosBar, _RaycastHitDictionary);
                }
            }
        }
        #endregion


        // EMPTY
        #region PUBLIC_METHODS
        #endregion


        #region PRIVATE_METHODS
        private void SetupUIElement()
        {
            _inputContainer = InputVariableContainer.Instance;

            _rightTriggerDown = _inputContainer.RightClickBoolean.Get("TriggerIsDown");
            _leftTriggerDown = _inputContainer.LeftClickBoolean.Get("TriggerIsDown");

            _ScrollableSetup = new VRUIScrollableSetup(UnityUIToVRSFUI.ScrollbarDirectionToUIDirection(direction));

            GetHandleRectReference();

            // If the controllers are not used, we cannot click on a Scroll Bar
            if (!ControllersParametersVariable.Instance.UseControllers)
            {
                Debug.Log("VRSF : You won't be able to use the VR ScrollBar if you're not using the Controllers. To change that,\n" +
                    "Go into the Window/VRSF/VR Interaction Parameters and set the UseControllers bool to true.");
            }

            // We initialize the RaycastHitDictionary
            _RaycastHitDictionary = new Dictionary<string, RaycastHitVariable>
            {
                { "Right", InteractionVariableContainer.Instance.RightHit },
                { "Left", InteractionVariableContainer.Instance.LeftHit },
                { "Gaze", InteractionVariableContainer.Instance.GazeHit },
            };

            // We register the Listener
            ObjectWasClickedEvent.RegisterListener(CheckBarClick);

            // Check if the Min and Max object are already created, and set there references
            _ScrollableSetup.CheckMinMaxGameObjects(handleRect.parent, UnityUIToVRSFUI.ScrollbarDirectionToUIDirection(direction));
            _ScrollableSetup.SetMinMaxPos(ref _MinPosBar, ref _MaxPosBar, handleRect.parent);

            value = 1;
        }

        /// <summary>
        /// Event called when the user is clicking on something
        /// </summary>
        /// <param name="clickEvent">The event raised when an object is clicked</param>
        void CheckBarClick(ObjectWasClickedEvent clickEvent)
        {
            if (interactable && clickEvent.ObjectClicked == transform && _HandHoldingHandle == EHand.NONE)
            {
                _HandHoldingHandle = clickEvent.HandClicking;
            }
        }

        /// <summary>
        /// Depending on the hand holding the trigger, call the CheckClickStillDown with the right boolean
        /// </summary>
        void CheckClickDown()
        {
            switch (_HandHoldingHandle)
            {
                case (EHand.GAZE):
                    _ScrollableSetup.CheckClickStillDown(ref _HandHoldingHandle, _inputContainer.GazeIsCliking.Value);
                    break;
                case (EHand.LEFT):
                    _ScrollableSetup.CheckClickStillDown(ref _HandHoldingHandle, _leftTriggerDown.Value);
                    break;
                case (EHand.RIGHT):
                    _ScrollableSetup.CheckClickStillDown(ref _HandHoldingHandle, _rightTriggerDown.Value);
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
                box = VRUIBoxColliderSetup.CheckBoxColliderSize(box, GetComponent<RectTransform>());
            }
            _boxColliderSetup = true;
        }

        /// <summary>
        /// Try to get the handleRect rectTransform reference by finding the Handle deepChild of this GameObject
        /// </summary>
        void GetHandleRectReference()
        {
            try
            {
                handleRect = transform.FindDeepChild("Handle").GetComponent<RectTransform>();
            }
            catch
            {
                Debug.LogError("Please add a Handle and a Fill with RectTransform as children of this VR Handle Slider.");
            }
        }
        #endregion


        // EMPTY
        #region GETTERS_SETTERS

        #endregion
    }
}