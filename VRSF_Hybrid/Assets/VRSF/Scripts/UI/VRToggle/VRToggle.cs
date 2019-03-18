using VRSF.Core.Controllers;
using System.Collections.Generic;
using UnityEngine;
using VRSF.Core.Events;

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


        #region MONOBEHAVIOUR_METHODS
        protected override void OnEnable()
        {
            base.OnEnable();

            if (Application.isPlaying)
            {
                SetupListener();

                // We setup the BoxCollider size and center
                if (SetColliderAuto)
                    StartCoroutine(SetupBoxCollider());
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ObjectWasClickedEvent.UnregisterListener(CheckToggleClick);
        }
        #endregion MONOBEHAVIOUR_METHODS


        // EMPTY
        #region PUBLIC_METHODS
        #endregion PUBLIC_METHODS


        #region PRIVATE_METHODS
        private void SetupListener()
        {
            // If the controllers are not used, we cannot click on a button
            if (!ControllersParametersVariable.Instance.UseControllers)
            {
                Debug.Log("<b>[VRSF] :</b> You won't be able to use the VR Toggle if you're not using the Controllers. To change that,\n" +
                    "Go into the Window/VRSF/VR Interaction Parameters and set the UseControllers bool to true.");
            }

            ObjectWasClickedEvent.RegisterListener(CheckToggleClick);
        }

        /// <summary>
        /// Event called when the button is clicked
        /// </summary>
        /// <param name="clickEvent">The event raised when an object is clicked</param>
        void CheckToggleClick(ObjectWasClickedEvent clickEvent)
        {
            if (interactable && clickEvent.ObjectClicked == transform)
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
            
            BoxCollider box = GetComponent<BoxCollider>();
            box = VRUIBoxColliderSetup.CheckBoxColliderSize(box, GetComponent<RectTransform>());
        }
        #endregion PRIVATE_METHODS
    }
}