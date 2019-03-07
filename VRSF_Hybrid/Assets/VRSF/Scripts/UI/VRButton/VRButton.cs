using UnityEngine;
using System.Collections.Generic;
using VRSF.Core.Events;
using VRSF.Core.Controllers;

namespace VRSF.UI
{
    /// <summary>
    /// Create a new VRButton based on the Button for Unity, but usable in VR with the Scriptable Framework
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class VRButton : UnityEngine.UI.Button
    {
        #region PUBLIC_VARIABLES
        [Tooltip("If you want to set the collider yourself, set this value to false.")]
        [SerializeField] public bool SetColliderAuto = true;
        #endregion PUBLIC_VARIABLES


        #region MONOBEHAVIOUR_METHODS
        protected override void Awake()
        {
            base.Awake();

            if (Application.isPlaying)
            {
                SetupListener();

                // We setup the BoxCollider size and center
                if (SetColliderAuto)
                    StartCoroutine(SetupBoxCollider());
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ObjectWasClickedEvent.UnregisterListener(CheckObjectClicked);
        }
        #endregion MONOBEHAVIOUR_METHODS


        #region PRIVATE_METHODS
        private void SetupListener()
        {
            // If the controllers are not used, we cannot click on a button
            if (!ControllersParametersVariable.Instance.UseControllers)
            {
                Debug.LogWarning("VRSF : You won't be able to use the VR Button if you're not using the Controllers. To change that,\n" +
                    "Go into the Window/VRSF/VR Interaction Parameters and set the UseControllers bool to true.");
            }

            ObjectWasClickedEvent.RegisterListener(CheckObjectClicked);
        }

        /// <summary>
        /// Event called when the button is clicked
        /// </summary>
        /// <param name="value">The object that was clicked</param>
        void CheckObjectClicked(ObjectWasClickedEvent objectClickEvent)
        {
            if (interactable && objectClickEvent.ObjectClicked == transform)
            {
                onClick.Invoke();
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
            yield return new WaitForEndOfFrame();

            BoxCollider box = GetComponent<BoxCollider>();
            box = VRUIBoxColliderSetup.CheckBoxColliderSize(box, GetComponent<RectTransform>());
        }
        #endregion PRIVATE_METHODS
    }
}