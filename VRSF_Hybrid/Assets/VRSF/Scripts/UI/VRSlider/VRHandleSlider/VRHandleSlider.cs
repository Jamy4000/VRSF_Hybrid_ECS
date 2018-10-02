using System.Collections.Generic;
using ScriptableFramework.Util;
using ScriptableFramework.Variables;
using VRSF.Controllers;
using UnityEngine;
using UnityEngine.UI;
using VRSF.Interactions;
using VRSF.Inputs;
using VRSF.Utils.Events;

namespace VRSF.UI
{
    /// <summary>
    /// This type of slider let the user click on a slider handler and move it through the slider bar.
    /// It work like a normal slider, and can be use for parameters or other GameObject that needs the SLider fill value.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class VRHandleSlider : Slider
    {
        #region PUBLIC_VARIABLES
        [Tooltip("If you want to set the collider yourself, set this value to false.")]
        [SerializeField] public bool SetColliderAuto = true;
        #endregion


        #region PRIVATE_VARIABLES
        private InputVariableContainer _inputContainer;

        private BoolVariable _leftIsClicking;
        private BoolVariable _rightIsClicking;

        Transform _MinPosBar;
        Transform _MaxPosBar;

        EHand _HandHoldingHandle = EHand.NONE;

        Dictionary<string, RaycastHitVariable> _RaycastHitDictionary;

        IUISetupScrollable _scrollableSetup;

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

                CheckClickDown();

                if (_HandHoldingHandle != EHand.NONE)
                    value = _scrollableSetup.MoveComponent(_HandHoldingHandle, _MinPosBar, _MaxPosBar, _RaycastHitDictionary);
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

            _rightIsClicking = _inputContainer.RightClickBoolean.Get("TriggerIsDown");
            _leftIsClicking = _inputContainer.LeftClickBoolean.Get("TriggerIsDown");

            _scrollableSetup = new VRUIScrollableSetup(UnityUIToVRSFUI.SliderDirectionToUIDirection(direction), minValue, maxValue, wholeNumbers);

            CheckSliderReferences();

            // If the controllers are not used, we cannot click on the slider, so we will fill the slider with the Over events
            if (!ControllersParametersVariable.Instance.UseControllers)
            {
                Debug.Log("VRSF : You won't be able to use the VR Handle Slider if you're not using the Controllers. To change that,\n" +
                    "Go into the Window/VRSF/VR Interaction Parameters and set the UseControllers bool to true.");
            }

            ObjectWasClickedEvent.RegisterListener(CheckSliderClick);

            _RaycastHitDictionary = new Dictionary<string, RaycastHitVariable>
                {
                    { "Right", InteractionVariableContainer.Instance.RightHit },
                    { "Left", InteractionVariableContainer.Instance.LeftHit },
                    { "Gaze", InteractionVariableContainer.Instance.GazeHit },
                };

            _scrollableSetup.CheckMinMaxGameObjects(handleRect.parent, UnityUIToVRSFUI.SliderDirectionToUIDirection(direction));
            
            _scrollableSetup.SetMinMaxPos(ref _MinPosBar, ref _MaxPosBar, handleRect.parent);
        }


        /// <summary>
        /// Event called when the user is clicking on something
        /// </summary>
        /// <param name="clickEvent">The event raised when an object is clicked</param>
        void CheckSliderClick(ObjectWasClickedEvent clickEvent)
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
                    _scrollableSetup.CheckClickStillDown(ref _HandHoldingHandle, _inputContainer.GazeIsCliking.Value);
                    break;
                case (EHand.LEFT):
                    _scrollableSetup.CheckClickStillDown(ref _HandHoldingHandle, _leftIsClicking.Value);
                    break;
                case (EHand.RIGHT):
                    _scrollableSetup.CheckClickStillDown(ref _HandHoldingHandle, _rightIsClicking.Value);
                    break;
            }
        }


        private void CheckSliderReferences()
        {
            try
            {
                handleRect = transform.FindDeepChild("Handle").GetComponent<RectTransform>();
                fillRect = transform.FindDeepChild("Fill").GetComponent<RectTransform>();
            }
            catch
            {
                Debug.LogError("Please add a Handle and a Fill with RectTransform as children of this VR Handle Slider.");
            }
        }


        /// <summary>
        /// Set the BoxCollider size if SetColliderAuto is at true
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
        #endregion


        // EMPTY
        #region GETTERS_SETTERS

        #endregion
    }
}