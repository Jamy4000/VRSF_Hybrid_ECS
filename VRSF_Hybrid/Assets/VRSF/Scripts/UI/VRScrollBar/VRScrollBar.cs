using ScriptableFramework.Events;
using ScriptableFramework.Util;
using ScriptableFramework.Variables;
using VRSF.Controllers;
using VRSF.Gaze;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRSF.Interactions;
using VRSF.Inputs;

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
        [Header("The GameObject containing the Game Event Listeners, set at runtime")]
        [HideInInspector] public GameObject GameEventListenersContainer;

        [Tooltip("If you want to set the collider yourself, set this value to false.")]
        [SerializeField] public bool SetColliderAuto = true;
        #endregion


        #region PRIVATE_VARIABLES
        // The Parameters Containers for the COntrollers and the Gaze
        // If UseController is set at false, this script will be disable as we need to click on a Input Field
        private ControllersParametersVariable _controllersParameter;
        private GazeParametersVariable _gazeParameter;

        // The Interaction and Inputs Variable and GameEvents Container
        private InteractionVariableContainer _interactionContainer;
        private InputVariableContainer _inputContainer;

        private BoolVariable _rightTriggerDown;
        private BoolVariable _leftTriggerDown;

        Transform _MinPosBar;
        Transform _MaxPosBar;

        EHand _HandHoldingHandle = EHand.NONE;

        GameObject _GameEventListenersContainer;

        Dictionary<string, GameEventListenerTransform> _ListenersDictionary;
        Dictionary<string, GameEventTransform> _EventsDictionary;
        Dictionary<string, RaycastHitVariable> _RaycastHitDictionary;

        IUISetupScrollable _ScrollableSetup;
        IUISetupClickOnly _ClickOnlySetup;
        VRUISetup _uiSetup;

        VRUISetup.CheckObjectDelegate _CheckObject;

        private bool _boxColliderSetup;
        #endregion


        #region MONOBEHAVIOUR_METHODS
        protected override void OnValidate()
        {
            base.OnValidate();
            MakeBasicSetup();
        }

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
                CheckClickDown();

                if (_HandHoldingHandle != EHand.NONE)
                {
                    value = _ScrollableSetup.MoveComponent(_HandHoldingHandle, _MinPosBar, _MaxPosBar, _RaycastHitDictionary);
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
                    _ListenersDictionary = _uiSetup.EndApp(_ListenersDictionary, _EventsDictionary);
                }
            }
            catch
            {
                // Listeners not set in the scene yet.
            }
        }

        private void OnApplicationQuit()
        {
            if (this.enabled)
            {
                _ListenersDictionary = _uiSetup.EndApp(_ListenersDictionary, _EventsDictionary);
            }
        }
        #endregion


        // EMPTY
        #region PUBLIC_METHODS
        #endregion


        #region PRIVATE_METHODS
        private void MakeBasicSetup()
        {
            // We initialize the _ListenersDictionary
            _ListenersDictionary = new Dictionary<string, GameEventListenerTransform>
            {
                { "Right", null },
                { "Left", null },
                { "Gaze", null },
            };

            // We create new object to setup the button references; listeners and GameEventListeners
            _CheckObject = CheckBarClick;
            _uiSetup = new VRUISetup(_CheckObject);
            _ClickOnlySetup = new VRScrollbarSetup();
            _ScrollableSetup = new VRUIScrollableSetup(UnityUIToVRSFUI.ScrollbarDirectionToUIDirection(direction));

            // Check if the Listeners GameObject is set correctly. If not, create the child
            if (!_ClickOnlySetup.CheckGameEventListenerChild(ref _GameEventListenersContainer, ref _ListenersDictionary, transform))
                _uiSetup.CreateGameEventListenerChild(ref _GameEventListenersContainer, transform);

            if (!_boxColliderSetup && gameObject.activeInHierarchy)
            {
                StartCoroutine(SetupBoxCollider());
            }

            GetHandleRectReference();
        }

        private void SetupUIElement()
        {
            _controllersParameter = ControllersParametersVariable.Instance;
            _gazeParameter = GazeParametersVariable.Instance;

            _interactionContainer = InteractionVariableContainer.Instance;
            _inputContainer = InputVariableContainer.Instance;

            _rightTriggerDown = _inputContainer.RightClickBoolean.Get("TriggerIsDown");
            _leftTriggerDown = _inputContainer.LeftClickBoolean.Get("TriggerIsDown");

            // If the controllers are not used, we cannot click on a Scroll Bar
            if (!_controllersParameter.UseControllers)
            {
                Debug.Log("VRSF : You won't be able to use the VR ScrollBar if you're not using the Controllers. To change that,\n" +
                    "Go into the Window/VRSF/VR Interaction Parameters and set the UseControllers bool to true.");
            }

            // We initialize the _EventsDictionary
            _EventsDictionary = new Dictionary<string, GameEventTransform>
            {
                { "Right", _interactionContainer.RightObjectWasClicked },
                { "Left", _interactionContainer.LeftObjectWasClicked },
                { "Gaze", _interactionContainer.GazeObjectWasClicked },
            };

            // We initialize the RaycastHitDictionary
            _RaycastHitDictionary = new Dictionary<string, RaycastHitVariable>
            {
                { "Right", _interactionContainer.RightHit },
                { "Left", _interactionContainer.LeftHit },
                { "Gaze", _interactionContainer.GazeHit },
            };
            // We setup the ListenersDictionary
            _ListenersDictionary = _uiSetup.CheckGameEventListenersPresence(_GameEventListenersContainer, _ListenersDictionary);
            _ListenersDictionary = _uiSetup.SetGameEventListeners(_ListenersDictionary, _EventsDictionary, _gazeParameter.UseGaze);


            // Check if the Min and Max object are already created, and set there references
            _ScrollableSetup.CheckMinMaxGameObjects(handleRect.parent, UnityUIToVRSFUI.ScrollbarDirectionToUIDirection(direction));
            _ScrollableSetup.SetMinMaxPos(ref _MinPosBar, ref _MaxPosBar, handleRect.parent);

            value = 1;
        }

        /// <summary>
        /// Event called when the user is clicking on something
        /// </summary>
        /// <param name="transformValue">The object that was clicked</param>
        void CheckBarClick(Transform transformValue)
        {
            if (interactable && transformValue == transform && _HandHoldingHandle == EHand.NONE)
            {
                CheckHandClickingScrollbar();
            }
        }

        /// <summary>
        /// Check which hand is holding clicking on the scrollbar
        /// </summary>
        private void CheckHandClickingScrollbar()
        {
            // Next if statements are to check which click was used on the slider
            if (_rightTriggerDown.Value && _ListenersDictionary["Right"].Value == transform)
            {
                _HandHoldingHandle = EHand.RIGHT;
            }
            else if (_leftTriggerDown.Value && _ListenersDictionary["Left"].Value == transform)
            {
                _HandHoldingHandle = EHand.LEFT;
            }
            else if (_gazeParameter.UseGaze && _inputContainer.GazeIsCliking.Value && _ListenersDictionary["Gaze"].Value == transform)
            {
                _HandHoldingHandle = EHand.GAZE;
            }
            else
            {
                Debug.LogError("Couldn't find reference to the Click Listener on the slider.");
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
                box = _uiSetup.CheckBoxColliderSize(box, GetComponent<RectTransform>());
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