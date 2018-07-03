using ScriptableFramework.Events;
using VRSF.Controllers;
using VRSF.Gaze;
using System.Collections.Generic;
using UnityEngine;
using VRSF.Interactions;

namespace VRSF.UI
{
    /// <summary>
    /// Create a new VRToggle based on the Toggle for Unity, but usable in VR with the Scriptable Framework
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class VRToggle : UnityEngine.UI.Toggle
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

        private bool _boxColliderSetup;
        #endregion PRIVATE_VARIABLES


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
                // We setup the BoxCollider size and center
                StartCoroutine(SetupBoxCollider());
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
            _CheckObject = CheckToggleClick;
            _UISetup = new VRUISetup(_CheckObject);
            _ClickOnlySetup = new VRToggleSetup();

            // Check if the Listeners GameObject is set correctly. If not, create the child
            if (!_ClickOnlySetup.CheckGameEventListenerChild(ref _GameEventListenersContainer, ref _ListenersDictionary, transform))
                _UISetup.CreateGameEventListenerChild(ref _GameEventListenersContainer, transform);

            if (!_boxColliderSetup && gameObject.activeInHierarchy)
            {
                // We setup the BoxCollider size and center
                StartCoroutine(SetupBoxCollider());
            }
        }

        private void SetupUIElement()
        {
            _controllersParameter = ControllersParametersVariable.Instance;
            _gazeParameter = GazeParametersVariable.Instance;

            _interactionContainer = InteractionVariableContainer.Instance;

            // If the controllers are not used, we cannot click on a button
            if (!_controllersParameter.UseControllers)
            {
                Debug.Log("VRSF : You won't be able to use the VR Toggle if you're not using the Controllers. To change that,\n" +
                    "Go into the Window/VRSF/VR Interaction Parameters and set the UseControllers bool to true.");
            }

            // We initialize the _EventsDictionary
            _EventsDictionary = new Dictionary<string, GameEventTransform>
            {
                { "Right", _interactionContainer.RightObjectWasClicked},
                { "Left", _interactionContainer.LeftObjectWasClicked },
                { "Gaze", _interactionContainer.GazeObjectWasClicked },
            };

            // We setup the ListenersDictionary
            _ListenersDictionary = _UISetup.CheckGameEventListenersPresence(_GameEventListenersContainer, _ListenersDictionary);
            _ListenersDictionary = _UISetup.SetGameEventListeners(_ListenersDictionary, _EventsDictionary, _gazeParameter.UseGaze);
        }

        /// <summary>
        /// Event called when the button is clicked
        /// </summary>
        /// <param name="value">The object that was clicked</param>
        void CheckToggleClick(Transform value)
        {
            if (interactable && value == transform)
            {
                isOn = !isOn;
            }
        }

        /// <summary>
        /// Setup the BoxCOllider size and center by colling the NotScrollableSetup method CheckBoxColliderSize.
        /// We use a coroutine and wait for the end of the first frame as the element cannot be correctly setup on the first frame
        /// </summary>
        /// <returns></returns>
        IEnumerator<WaitForEndOfFrame> SetupBoxCollider()
        {
            yield return new WaitForEndOfFrame();

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