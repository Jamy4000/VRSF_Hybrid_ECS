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
    /// Create a new VRScrollRect based on the ScrollRect for Unity, but usable in VR with the Scriptable Framework
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class VRScrollRect : ScrollRect
    {
        #region PUBLIC_VARIABLES
        [Header("The Direction of this ScrollRect.")]
        [Tooltip("Will be override by the Scrollbar direction if there's at least one.")]
        [SerializeField] public EUIDirection Direction = EUIDirection.TopToBottom;

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
            ObjectWasClickedEvent.UnregisterListener(CheckRectClick);
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
                    if (vertical && verticalScrollbar)
                        verticalScrollbar.value = _ScrollableSetup.MoveComponent(_HandHoldingHandle, _MinPosBar, _MaxPosBar, _RaycastHitDictionary);

                    if (horizontal && horizontalScrollbar)
                        horizontalScrollbar.value = _ScrollableSetup.MoveComponent(_HandHoldingHandle, _MinPosBar, _MaxPosBar, _RaycastHitDictionary);
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

            // We setup the references to the ScrollRect elements
            SetScrollRectReferences();

            // We override the directio selected in the inspector by the scrollbar direction if we use one
            // The vertical direction will always have top priority on the horizontal direction
            if (vertical && verticalScrollbar.gameObject != null)
            {
                Direction = UnityUIToVRSFUI.ScrollbarDirectionToUIDirection(verticalScrollbar.direction);
            }
            else if (horizontal && horizontalScrollbar.gameObject != null)
            {
                Direction = UnityUIToVRSFUI.ScrollbarDirectionToUIDirection(horizontalScrollbar.direction);
            }

            _ScrollableSetup = new VRUIScrollableSetup(Direction);

            // If the controllers are not used, we cannot click on a Scroll Bar
            if (!ControllersParametersVariable.Instance.UseControllers)
            {
                Debug.Log("VRSF : You won't be able to use the VR ScrollRect if you're not using the Controllers. To change that,\n" +
                    "Go into the Window/VRSF/VR Interaction Parameters and set the UseControllers bool to true.");
            }

            ObjectWasClickedEvent.RegisterListener(CheckRectClick);
            
            // We initialize the _RaycastHitDictionary
            _RaycastHitDictionary = new Dictionary<string, RaycastHitVariable>
            {
                { "Right", InteractionVariableContainer.Instance.RightHit },
                { "Left", InteractionVariableContainer.Instance.LeftHit },
                { "Gaze", InteractionVariableContainer.Instance.GazeHit },
            };
            
            // We setup the Min and Max pos transform
            _ScrollableSetup.CheckMinMaxGameObjects(transform, Direction);
            _ScrollableSetup.SetMinMaxPos(ref _MinPosBar, ref _MaxPosBar, GetComponent<Transform>());
        }

        /// <summary>
        /// Event called when the user is clicking on something
        /// </summary>
        /// <param name="clickEvent">The event raised when something is clicked</param>
        void CheckRectClick(ObjectWasClickedEvent clickEvent)
        {
            if (clickEvent.ObjectClicked == transform && _HandHoldingHandle == EHand.NONE)
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
                box.center = Vector3.zero;

                if (vertical)
                {
                    var barCollider = verticalScrollbar.GetComponent<BoxCollider>();
                    float x = (box.size.x - barCollider.size.x);
                    box.size = new Vector3(x, box.size.y, box.size.z);
                    box.center = new Vector3(-barCollider.size.x / 2, box.center.y, box.center.z);
                }

                if (horizontal)
                {
                    var barCollider = horizontalScrollbar.GetComponent<BoxCollider>();
                    float y = (box.size.y - barCollider.size.y);
                    box.size = new Vector3(box.size.x, y, box.size.z);
                    box.center = new Vector3(box.center.x, barCollider.size.y / 2, box.center.z);
                }
            }

            _boxColliderSetup = true;
        }

        /// <summary>
        /// Set the scrollRect references (scrollbar, content and viewport) by looking in the DeepChildren
        /// </summary>
        void SetScrollRectReferences()
        {
            // get VRScrollBarVertical if we use the scrollrect as vertical
            try
            {
                if (vertical)
                    verticalScrollbar = transform.FindDeepChild("VRScrollbarVertical").GetComponent<VRScrollBar>();
            }
            catch { /* No vertical Scrollbar was found. */ }

            // get VRScrollBarHorizontal if we use the scrollrect as horizontal
            try
            {
                if (horizontal)
                    horizontalScrollbar = transform.FindDeepChild("VRScrollbarHorizontal").GetComponent<VRScrollBar>();
            }
            catch { /* No horizontal Scrollbar was found. */ }

            // get the viewport
            try { viewport = transform.FindDeepChild("Viewport").GetComponent<RectTransform>(); }
            catch { /* No Viewport was found.*/ }


            // get the content
            try { content = transform.FindDeepChild("Content").GetComponent<RectTransform>(); }
            catch { /* No Content was found.*/ }
        }
        #endregion


        // EMPTY
        #region GETTERS_SETTERS

        #endregion
    }
}