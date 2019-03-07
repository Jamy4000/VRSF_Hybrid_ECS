using VRSF.Core.Controllers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRSF.Core.Events;

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
        public VRKeyboard VRKeyboard;

        [Tooltip("If you want to set the collider yourself, set this value to false.")]
        [SerializeField] public bool SetColliderAuto = true;
        #endregion PUBLIC_VARIABLES


        #region MONOBEHAVIOUR_METHODS
        protected override void OnEnable()
        {
            base.OnEnable();

            if (Application.isPlaying)
            {
                SetupUIElement();

                // We setup the BoxCollider size and center
                if (SetColliderAuto)
                    StartCoroutine(SetupBoxCollider());
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ObjectWasClickedEvent.UnregisterListener(CheckInputFieldClick);
        }
        #endregion MONOBEHAVIOUR_METHODS


        // EMPTY
        #region PUBLIC_METHODS
        #endregion PUBLIC_METHODS


        #region PRIVATE_METHODS
        private void SetupUIElement()
        {
            // If the controllers are not used, we cannot click on a InputField
            if (!ControllersParametersVariable.Instance.UseControllers)
            {
                Debug.Log("VRSF : You won't be able to use the VR InputField if you're not using the Controllers. To change that,\n" +
                    "Go into the Window/VRSF/VR Interaction Parameters and set the UseControllers bool to true.");
            }

            ObjectWasClickedEvent.RegisterListener(CheckInputFieldClick);
            
            SetInputFieldReferences();
        }

        /// <summary>
        /// Method called when the user is clicking
        /// </summary>
        /// <param name="clickEvent">The event raised when an object is clicked</param>
        void CheckInputFieldClick(ObjectWasClickedEvent clickEvent)
        {
            if (interactable && clickEvent.ObjectClicked == transform)
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
            if (UseVRKeyboard && VRKeyboard != null)
            {
                VRKeyboard.InputField = this;
            }
            else if (UseVRKeyboard)
            {
                try
                {
                    VRKeyboard = FindObjectOfType<VRKeyboard>();
                    VRKeyboard.InputField = this;
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
        IEnumerator<WaitForEndOfFrame> SetupBoxCollider()
        {
            yield return new WaitForEndOfFrame();
            
            BoxCollider box = GetComponent<BoxCollider>();
            box = VRUIBoxColliderSetup.CheckBoxColliderSize(box, GetComponent<RectTransform>());
        }
        #endregion PRIVATE_METHODS
    }
}