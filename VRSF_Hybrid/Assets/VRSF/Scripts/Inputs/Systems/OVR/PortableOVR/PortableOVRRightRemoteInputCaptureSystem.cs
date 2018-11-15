
using ScriptableFramework.Variables;
using System.Collections;
using Unity.Entities;
using UnityEngine;
using VRSF.Controllers;
using VRSF.Inputs.Components;
using VRSF.Inputs.Events;
using VRSF.Utils;
using VRSF.Utils.Components;

namespace VRSF.Inputs.Systems
{
    /// <summary>
    /// Script attached to the Portable OVR Prefab.
    /// Set the GameEvent depending on the Oculus Inputs
    /// </summary>
    public class PortableOVRRightRemoteInputCaptureSystem : ComponentSystem
    {
        struct Filter
        {
            public PortableOVRRemoteInputCaptureComponent VRInputCapture;
        }

        #region PRIVATE_VARIABLES
        // VRSF Parameters references
        private InputVariableContainer _inputContainer;
        private ControllersParametersVariable _controllersParameters;
        #endregion PRIVATE_VARIABLES


        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            GameObject.FindObjectOfType<SetupVRComponents>()?.StartCoroutine(Init());
        }

        protected override void OnUpdate()
        {
            // If we doesn't use the controllers, we don't check for the inputs.
            if (_controllersParameters != null && _controllersParameters.UseControllers)
            {
                VRSF_Components.RightController.SetActive(OVRInput.IsControllerConnected(OVRInput.Controller.RTrackedRemote));

                foreach (var entity in GetEntities<Filter>())
                {
                    CheckRemoteInput(entity.VRInputCapture);
                }
            }
        }
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Handle the Remote inputs when user is Right handed and put them in the Events
        /// </summary>
        void CheckRemoteInput(PortableOVRRemoteInputCaptureComponent vrInputCapture)
        {
            BoolVariable tempClick;
            BoolVariable tempTouch;

            #region TRIGGER
            tempClick = _inputContainer.RightClickBoolean.Get("TriggerIsDown");
            tempTouch = _inputContainer.RightTouchBoolean.Get("TriggerIsTouching");

            // Checking Click event 
            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                new ButtonClickEvent(EHand.RIGHT, EControllersButton.TRIGGER);
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
            {
                tempClick.SetValue(false);
                new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.TRIGGER);
            }
            // Checking Touch event if user is not clicking
            else if (!tempClick.Value && !tempTouch.Value && OVRInput.Get(OVRInput.Touch.PrimaryIndexTrigger))
            {
                tempTouch.SetValue(true);
                new ButtonTouchEvent(EHand.RIGHT, EControllersButton.TRIGGER);
            }
            else if (!tempClick.Value && tempTouch.Value && !OVRInput.Get(OVRInput.Touch.PrimaryIndexTrigger))
            {
                tempTouch.SetValue(false);
                new ButtonUntouchEvent(EHand.RIGHT, EControllersButton.TRIGGER);
            }
            #endregion TRIGGER

            #region THUMBSTICK
            _inputContainer.RightThumbPosition.SetValue(OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad));
            tempClick = _inputContainer.RightClickBoolean.Get("ThumbIsDown");
            tempTouch = _inputContainer.RightTouchBoolean.Get("ThumbIsTouching");

            // Checking Click event 
            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.PrimaryTouchpad))
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                new ButtonClickEvent(EHand.RIGHT, EControllersButton.THUMBSTICK);
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.PrimaryTouchpad))
            {
                tempClick.SetValue(false);
                new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.THUMBSTICK);
            }
            // Checking Touch event if user is not clicking
            else if (!tempClick.Value && !tempTouch.Value && (OVRInput.Get(OVRInput.Touch.PrimaryTouchpad) || _inputContainer.RightThumbPosition.Value != Vector2.zero))
            {
                tempTouch.SetValue(true);
                new ButtonTouchEvent(EHand.RIGHT, EControllersButton.THUMBSTICK);
            }
            else if (tempTouch.Value && (!OVRInput.Get(OVRInput.Touch.PrimaryTouchpad) && _inputContainer.RightThumbPosition.Value == Vector2.zero))
            {
                tempTouch.SetValue(false);
                new ButtonUntouchEvent(EHand.RIGHT, EControllersButton.THUMBSTICK);
            }
            #endregion THUMBSTICK

            #region BACK
            tempClick = _inputContainer.RightClickBoolean.Get("BackButtonIsDown");

            // Checking Click event 
            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.Back))
            {
                tempClick.SetValue(true);
                new ButtonClickEvent(EHand.RIGHT, EControllersButton.BACK_BUTTON);
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.Back))
            {
                tempClick.SetValue(false);
                new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.BACK_BUTTON);
            }
            // Touch Event not existing on BACK
            #endregion BACK
        }

        private IEnumerator Init()
        {
            // We wait until VRSF instantiate the prefab and load everything
            while (!VRSF_Components.SetupVRIsReady)
            {
                yield return new WaitForEndOfFrame();
            }

            // We wait until one of the LTrackedConnected and RTrackedConnected controller is on and the other is off
            while ((OVRInput.IsControllerConnected(OVRInput.Controller.RTrackedRemote) && OVRInput.IsControllerConnected(OVRInput.Controller.LTrackedRemote))
                || (!OVRInput.IsControllerConnected(OVRInput.Controller.RTrackedRemote) && !OVRInput.IsControllerConnected(OVRInput.Controller.LTrackedRemote)))
            {
                yield return new WaitForEndOfFrame();
            }

            // We setup everything
            foreach (var entity in GetEntities<Filter>())
            {
                if (!OVRInput.IsControllerConnected(OVRInput.Controller.RTrackedRemote))
                {
                    VRSF_Components.RightController.SetActive(false);
                    this.Enabled = false;
                }
                else
                {
                    entity.VRInputCapture.RemoteTracker.m_controller = OVRInput.Controller.RTrackedRemote;
                    entity.VRInputCapture.RemoteTracker.transform.SetParent(VRSF_Components.RightController.transform);
                }
            }

            _inputContainer = InputVariableContainer.Instance;
            _controllersParameters = ControllersParametersVariable.Instance;
        }
        #endregion
    }
}