using ScriptableFramework.Events;
using ScriptableFramework.Util;
using VRSF.Controllers;
using VRSF.Gaze;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRSF.Interactions;

namespace VRSF.UI
{
    /// <summary>
    /// Create a new VRDropdown element based on the Dropdown from Unity, but usable in VR with the Scriptable Framework
    /// The Template of the VRDropdown Prefab is modified in a way that the options, when displayed, don't use Toggle but VRToggle
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class VRDropdown : Dropdown
    {
        #region PUBLIC_VARIABLES
        [Tooltip("If you want to set the collider yourself, set this value to false.")]
        [SerializeField] public bool SetColliderAuto = true;
        #endregion PUBLIC_VARIABLES


        #region PRIVATE_VARIABLES
        // The Controllers and Gaze Parameters
        private ControllersParametersVariable _controllersParameter;
        private GazeParametersVariable _gazeParameter;

        // The Interaction Variable and GameEvents Container
        private InteractionVariableContainer _interactionContainer;

        GameObject _GameEventListenersContainer;

        Dictionary<string, GameEventListenerTransform> _ListenersDictionary;
        Dictionary<string, GameEventTransform> _EventsDictionary;

        IUISetupClickOnly _ClickOnlySetup;
        VRUISetup _UISetup;

        VRUISetup.CheckObjectDelegate _CheckObject;

        GameObject _Template;
        bool _IsShown = false;

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
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            onValueChanged.RemoveListener(delegate { SetDropDownNewState(); });

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
            onValueChanged.RemoveListener(delegate { SetDropDownNewState(); });

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
            _CheckObject = CheckObjectClicked;
            _UISetup = new VRUISetup(_CheckObject);
            _ClickOnlySetup = new VRDropdownSetup();

            // Check if the Listeners GameObject is set correctly. If not, create the child
            if (!_ClickOnlySetup.CheckGameEventListenerChild(ref _GameEventListenersContainer, ref _ListenersDictionary, transform))
                _UISetup.CreateGameEventListenerChild(ref _GameEventListenersContainer, transform);
        }


        private void SetupUIElement()
        {
            _controllersParameter = ControllersParametersVariable.Instance;
            _gazeParameter = GazeParametersVariable.Instance;
            _interactionContainer = InteractionVariableContainer.Instance;

            // If the controllers are not used, we cannot click on a Dropdown
            if (!_controllersParameter.UseControllers)
            {
                Debug.Log("VRSF : You won't be able to use the VR DropDown if you're not using the Controllers. To change that,\n" +
                    "Go into the Window/VRSF/VR Interaction Parameters and set the UseControllers bool to true.");
            }

            onValueChanged.AddListener(delegate { SetDropDownNewState(); });

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
            _ListenersDictionary = _UISetup.CheckGameEventListenersPresence(_GameEventListenersContainer, _ListenersDictionary);
            _ListenersDictionary = _UISetup.SetGameEventListeners(_ListenersDictionary, _EventsDictionary, _gazeParameter.UseGaze);

            // We setup the Template and Options to fit the VRFramework
            _Template = transform.Find("Template").gameObject;
            SetToggleReferences();
            ChangeTemplate();
        }

        /// <summary>
        /// Event called when the DropDown or its children is clicked
        /// </summary>
        /// <param name="value">The object that was clicked</param>
        public void CheckObjectClicked(Transform value)
        {
            if (interactable && value == transform)
            {
                SetDropDownNewState();
            }
        }

        /// <summary>
        /// Called when dropdown is click, setup the new state of the element
        /// </summary>
        void SetDropDownNewState()
        {
            if (!_IsShown)
            {
                Show();
                _IsShown = true;
            }
            else
            {
                Hide();
                _IsShown = false;
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

        /// <summary>
        /// Set the Dropdown references to the Toggle
        /// </summary>
        void SetToggleReferences()
        {
            template = _Template.GetComponent<RectTransform>();
            captionText = transform.Find("Label").GetComponent<Text>();
            itemText = transform.FindDeepChild("Item Label").GetComponent<Text>();
        }

        /// <summary>
        /// Change the Template to add the VRToggle instead of the one from Unity
        /// </summary>
        void ChangeTemplate()
        {
            _Template.SetActive(true);

            Transform item = _Template.transform.Find("Viewport/Content/Item");

            if (item.GetComponent<VRToggle>() == null)
            {
                DestroyImmediate(item.GetComponent<Toggle>());

                VRToggle newToggle = item.gameObject.AddComponent<VRToggle>();
                newToggle.targetGraphic = item.Find("Item Background").GetComponent<Image>();
                newToggle.isOn = true;
                newToggle.toggleTransition = Toggle.ToggleTransition.Fade;
                newToggle.graphic = item.Find("Item Checkmark").GetComponent<Image>();
            }

            _Template.SetActive(false);
        }
        #endregion PRIVATE_METHODS
    }
}