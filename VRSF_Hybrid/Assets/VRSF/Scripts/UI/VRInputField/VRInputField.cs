using ScriptableFramework.Events;
using VRSF.Controllers;
using VRSF.Gaze;
using VRSF.Interactions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VRSF.UI
{
    /// <summary>
    /// Create a new VRInputField element based on the InputField from Unity, but usable in VR with the Scriptable Framework
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class VRInputField : InputField
    {
        #region PUBLIC_VARIABLES
        [Header("The VRKeyboard parameters and references")]
        public bool UseVRKeyboard = true;
        public VRKeyboard vrKeyboard;

        [Tooltip("If you want to set the collider yourself, set this value to false.")]
        [SerializeField] public bool SetColliderAuto = true;
        #endregion PUBLIC_VARIABLES


        #region PRIVATE_VARIABLES
        // The Parameters Containers for the COntrollers and the Gaze
        // If UseController is set at false, this script will be disable as we need to click on a Input Field
        private ControllersParametersVariable _controllersParameter;
        private GazeParametersVariable _gazeParameter;

        // The Interaction Variable and GameEvents Container
        private InteractionVariableContainer _interactionContainer;

        GameObject gameEventListenersContainer;

        Dictionary<string, GameEventListenerTransform> _ListenersDictionary;
        Dictionary<string, GameEventTransform> _EventsDictionary;

        IUISetupClickOnly ClickOnlySetup;
        VRUISetup _UISetup;

        VRUISetup.CheckObjectDelegate CheckObject;

        private bool _boxColliderSetup;
        #endregion PRIVATE_VARIABLES


        #region MONOBEHAVIOUR_METHODS
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            BasicSetup();
        }
#endif

        protected override void Start()
        {
            base.Start();

            if (Application.isPlaying)
            {
                BasicSetup();
                SetupUIElement();
            }
        }

        private void Update()
        {
            if (!_boxColliderSetup && Application.isPlaying)
            {
                SetupBoxCollider();
                return;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            try
            {
                if (this.enabled)
                {
                    _ListenersDictionary = _UISetup.EndApp(_ListenersDictionary, _EventsDictionary);
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
                _ListenersDictionary = _UISetup.EndApp(_ListenersDictionary, _EventsDictionary);
            }
        }
        #endregion MONOBEHAVIOUR_METHODS


        // EMPTY
        #region PUBLIC_METHODS
        #endregion PUBLIC_METHODS


        #region PRIVATE_METHODS
        private void BasicSetup()
        {
            // We initialize the _ListenersDictionary
            _ListenersDictionary = new Dictionary<string, GameEventListenerTransform>
            {
                { "Right", null },
                { "Left", null },
                { "Gaze", null },
            };

            // We create new object to setup the button references; listeners and GameEventListeners
            CheckObject = CheckInputFieldClick;
            _UISetup = new VRUISetup(CheckObject);
            ClickOnlySetup = new VRInputFieldSetup();

            // Check if the Listeners GameObject is set correctly. If not, create the child
            if (!ClickOnlySetup.CheckGameEventListenerChild(ref gameEventListenersContainer, ref _ListenersDictionary, transform))
                _UISetup.CreateGameEventListenerChild(ref gameEventListenersContainer, transform);
        }

        private void SetupUIElement()
        {
            _controllersParameter = ControllersParametersVariable.Instance;
            _gazeParameter = GazeParametersVariable.Instance;
            _interactionContainer = InteractionVariableContainer.Instance;

            // If the controllers are not used, we cannot click on a InputField
            if (!_controllersParameter.UseControllers)
            {
                Debug.Log("VRSF : You won't be able to use the VR InputField if you're not using the Controllers. To change that,\n" +
                    "Go into the Window/VRSF/VR Interaction Parameters and set the UseControllers bool to true.");
            }

            // We initialize the _EventsDictionary
            _EventsDictionary = new Dictionary<string, GameEventTransform>
            {
                { "Right", _interactionContainer.RightObjectWasClicked },
                { "Left", _interactionContainer.LeftObjectWasClicked },
                { "Gaze", _interactionContainer.GazeObjectWasClicked },
            };

            // We setup the BoxCollider size and center
            if (!_boxColliderSetup && Application.isPlaying)
            {
                SetupBoxCollider();
            }

            // We setup the ListenersDictionary
            _ListenersDictionary = _UISetup.CheckGameEventListenersPresence(gameEventListenersContainer, _ListenersDictionary);
            _ListenersDictionary = _UISetup.SetGameEventListeners(_ListenersDictionary, _EventsDictionary, _gazeParameter.UseGaze);

            SetInputFieldReferences();
        }

        /// <summary>
        /// Method called when the user is clicking
        /// </summary>
        /// <param name="value">The object that was clicked</param>
        void CheckInputFieldClick(Transform value)
        {
            if (interactable && value == transform)
            {
                placeholder.GetComponent<Text>().text = "";
                ActivateInputField();
                CheckForVRKeyboard();
            }
        }

        /// <summary>
        /// Check if a VRKeyboard is used and present in the scene
        /// </summary>
        void CheckForVRKeyboard()
        {
            if (UseVRKeyboard && vrKeyboard != null)
            {
                vrKeyboard.InputField = this;
            }
            else if (UseVRKeyboard)
            {
                try
                {
                    vrKeyboard = FindObjectOfType<VRKeyboard>();
                    vrKeyboard.InputField = this;
                }
                catch
                {
                    Debug.LogError("The VRKeyboard is not present in the scene." +
                        "Please uncheck the Use VR Keyboard toggle or place a VRKeyboard in the Scene.");
                }
            }
        }

        /// <summary>
        /// Set the input field reference for the textComponent and the placeHolder
        /// </summary>
        void SetInputFieldReferences()
        {
            try
            {
                textComponent = transform.Find("Text").GetComponent<Text>();
                placeholder = transform.Find("Placeholder").GetComponent<Text>();
            }
            catch
            {
                Debug.LogError("Couldn't find the Text and the PlaceHolder in the VRInputField children.");
            }
        }

        /// <summary>
        /// Setup the BoxCOllider size and center by colling the NotScrollableSetup method CheckBoxColliderSize.
        /// We use a coroutine and wait for the end of the first frame as the element cannot be correctly setup on the first frame
        /// </summary>
        /// <returns></returns>
        void SetupBoxCollider()
        {
            if (SetColliderAuto)
            {
                BoxCollider box = GetComponent<BoxCollider>();
                box = _UISetup.CheckBoxColliderSize(box, GetComponent<RectTransform>());
            }
            _boxColliderSetup = true;
        }
        #endregion PRIVATE_METHODS
    }
}