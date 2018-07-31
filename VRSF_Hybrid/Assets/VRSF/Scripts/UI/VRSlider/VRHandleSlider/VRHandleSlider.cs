using System.Collections.Generic;
using ScriptableFramework.Events;
using ScriptableFramework.Util;
using ScriptableFramework.Variables;
using VRSF.Controllers;
using VRSF.Gaze;
using UnityEngine;
using UnityEngine.UI;
using VRSF.Interactions;
using VRSF.Inputs;

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
        [Header("The GameObject containing the Game Event Listeners, set at runtime")]
        [HideInInspector] public GameObject GameEventListenersContainer;

        [Tooltip("If you want to set the collider yourself, set this value to false.")]
        [SerializeField] public bool SetColliderAuto = true;
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

        Transform _MinPosBar;
        Transform _MaxPosBar;

        EHand _HandHoldingHandle = EHand.NONE;

        GameObject gameEventListenersContainer;

        Dictionary<string, GameEventListenerTransform> _ListenersDictionary;
        Dictionary<string, GameEventTransform> _EventsDictionary;
        Dictionary<string, RaycastHitVariable> _RaycastHitDictionary;

        IUISetupScrollable ScrollableSetup;
        IUISetupClickOnly ClickOnlySetup;
        VRUISetup _uiSetup;

        VRUISetup.CheckObjectDelegate CheckObject;

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

            if (Application.isPlaying && gameObject.activeInHierarchy)
            {
                CheckClickDown();

                if (_HandHoldingHandle != EHand.NONE)
                    value = ScrollableSetup.MoveComponent(_HandHoldingHandle, _MinPosBar, _MaxPosBar, _RaycastHitDictionary);
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
            _ListenersDictionary = new Dictionary<string, GameEventListenerTransform>
            {
                { "Right", null },
                { "Left", null },
                { "Gaze", null },
            };

            CheckObject = CheckSliderClick;
            _uiSetup = new VRUISetup(CheckObject);
            ClickOnlySetup = new VRHandleSliderSetup();
            ScrollableSetup = new VRUIScrollableSetup(UnityUIToVRSFUI.SliderDirectionToUIDirection(direction), minValue, maxValue, wholeNumbers);

            // Check if the Listeners GameObject is set correctly. If not, create the child
            if (!ClickOnlySetup.CheckGameEventListenerChild(ref gameEventListenersContainer, ref _ListenersDictionary, transform))
                _uiSetup.CreateGameEventListenerChild(ref gameEventListenersContainer, transform);

            if (!_boxColliderSetup && gameObject.activeInHierarchy)
            {
                StartCoroutine(SetupBoxCollider());
            }

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
        private void SetupUIElement()
        {
            _controllersParameter = ControllersParametersVariable.Instance;
            _gazeParameter = GazeParametersVariable.Instance;

            _interactionContainer = InteractionVariableContainer.Instance;
            _inputContainer = InputVariableContainer.Instance;

            _rightIsClicking = _inputContainer.RightClickBoolean.Get("TriggerIsDown");
            _leftIsClicking = _inputContainer.LeftClickBoolean.Get("TriggerIsDown");

            // If the controllers are not used, we cannot click on the slider, so we will fill the slider with the Over events
            if (!_controllersParameter.UseControllers)
            {
                Debug.Log("VRSF : You won't be able to use the VR Handle Slider if you're not using the Controllers. To change that,\n" +
                    "Go into the Window/VRSF/VR Interaction Parameters and set the UseControllers bool to true.");
            }

            _EventsDictionary = new Dictionary<string, GameEventTransform>
            {
                { "Right", _interactionContainer.RightObjectWasClicked},
                { "Left", _interactionContainer.LeftObjectWasClicked },
                { "Gaze", _interactionContainer.GazeObjectWasClicked },
            };

            _RaycastHitDictionary = new Dictionary<string, RaycastHitVariable>
                {
                    { "Right", _interactionContainer.RightHit },
                    { "Left", _interactionContainer.LeftHit },
                    { "Gaze", _interactionContainer.GazeHit },
                };

            ScrollableSetup.CheckMinMaxGameObjects(handleRect.parent, UnityUIToVRSFUI.SliderDirectionToUIDirection(direction));

            _ListenersDictionary = _uiSetup.CheckGameEventListenersPresence(gameEventListenersContainer, _ListenersDictionary);

            _ListenersDictionary = _uiSetup.SetGameEventListeners(_ListenersDictionary, _EventsDictionary, _gazeParameter.UseGaze);

            ScrollableSetup.SetMinMaxPos(ref _MinPosBar, ref _MaxPosBar, handleRect.parent);
        }


        /// <summary>
        /// Event called when the user is clicking on something
        /// </summary>
        /// <param name="transformValue">The object that was clicked</param>
        void CheckSliderClick(Transform transformValue)
        {
            if (interactable && transformValue == transform && _HandHoldingHandle == EHand.NONE)
            {
                CheckHandClickingSlider();
            }
        }


        /// <summary>
        /// Check which hand is holding clicking on the slider
        /// </summary>
        private void CheckHandClickingSlider()
        {
            // Next if statements are to check which click was used on the slider
            if (_rightIsClicking.Value && _ListenersDictionary["Right"].Value == transform)
            {
                _HandHoldingHandle = EHand.RIGHT;
            }
            else if (_leftIsClicking.Value && _ListenersDictionary["Left"].Value == transform)
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
                    ScrollableSetup.CheckClickStillDown(ref _HandHoldingHandle, _inputContainer.GazeIsCliking.Value);
                    break;
                case (EHand.LEFT):
                    ScrollableSetup.CheckClickStillDown(ref _HandHoldingHandle, _leftIsClicking.Value);
                    break;
                case (EHand.RIGHT):
                    ScrollableSetup.CheckClickStillDown(ref _HandHoldingHandle, _rightIsClicking.Value);
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
        #endregion


        // EMPTY
        #region GETTERS_SETTERS

        #endregion
    }
}