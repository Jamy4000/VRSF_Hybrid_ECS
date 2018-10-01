
using ScriptableFramework.Variables;
using Unity.Entities;
using UnityEngine;
using VRSF.Controllers;
using VRSF.Inputs.Components;
using VRSF.Inputs.Events;

namespace VRSF.Inputs.Systems
{
    /// <summary>
    /// Script attached to the OculusSDK Prefab.
    /// Set the GameEvent depending on the Oculus Inputs
    /// </summary>
    public class RiftLeftControllerInputCaptureSystem : ComponentSystem
    {
        struct Filter
        {
            public RiftControllersInputCaptureComponent VRInputCapture;
        }

        #region PRIVATE_VARIABLES
        // VRSF Parameters references
        private InputVariableContainer _inputContainer;
        private ControllersParametersVariable _controllersParameters;
        #endregion PRIVATE_VARIABLES


        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            _inputContainer = InputVariableContainer.Instance;
            _controllersParameters = ControllersParametersVariable.Instance;
        }

        protected override void OnUpdate()
        {
            // If we doesn't use the controllers, we don't check for the inputs.
            if (_controllersParameters.UseControllers)
            {
                foreach (var entity in GetEntities<Filter>())
                {
                    CheckLeftControllerInput(entity.VRInputCapture);
                }
            }
        }
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Handle the Left Controller input and put them in the Events
        /// </summary>
        void CheckLeftControllerInput(RiftControllersInputCaptureComponent vrInputCapture)
        {
            BoolVariable tempClick;
            BoolVariable tempTouch;

            #region TRIGGER
            tempClick = _inputContainer.LeftClickBoolean.Get("TriggerIsDown");
            tempTouch = _inputContainer.LeftTouchBoolean.Get("TriggerIsTouching");

            // Checking Click event 
            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                new ButtonInteractingEvent(EControllerInteractionType.CLICK, EHand.LEFT, EControllersInput.TRIGGER, EFingerMovement.DOWN);
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
            {
                tempClick.SetValue(false);
                new ButtonInteractingEvent(EControllerInteractionType.CLICK, EHand.LEFT, EControllersInput.TRIGGER, EFingerMovement.UP);
            }
            // Checking Touch event if user is not clicking
            else if (!tempClick.Value && !tempTouch.Value && OVRInput.Get(OVRInput.Touch.PrimaryIndexTrigger))
            {
                tempTouch.SetValue(true);
                new ButtonInteractingEvent(EControllerInteractionType.TOUCH, EHand.LEFT, EControllersInput.TRIGGER, EFingerMovement.DOWN);
            }
            else if (!tempClick.Value && tempTouch.Value && !OVRInput.Get(OVRInput.Touch.PrimaryIndexTrigger))
            {
                tempTouch.SetValue(false);
                new ButtonInteractingEvent(EControllerInteractionType.TOUCH, EHand.LEFT, EControllersInput.TRIGGER, EFingerMovement.UP);
            }
            #endregion TRIGGER

            #region THUMBSTICK
            _inputContainer.LeftThumbPosition.SetValue(OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick));
            tempClick = _inputContainer.LeftClickBoolean.Get("ThumbIsDown");
            tempTouch = _inputContainer.LeftTouchBoolean.Get("ThumbIsTouching");

            // Checking Click event 
            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.PrimaryThumbstick))
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                new ButtonInteractingEvent(EControllerInteractionType.CLICK, EHand.LEFT, EControllersInput.THUMBSTICK, EFingerMovement.DOWN);
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.PrimaryThumbstick))
            {
                tempClick.SetValue(false);
                new ButtonInteractingEvent(EControllerInteractionType.CLICK, EHand.LEFT, EControllersInput.THUMBSTICK, EFingerMovement.UP);
            }
            // Checking Touch event if user is not clicking
            else if (!tempClick.Value && !tempTouch.Value && (OVRInput.Get(OVRInput.Touch.PrimaryThumbstick) || _inputContainer.LeftThumbPosition.Value != Vector2.zero))
            {
                tempTouch.SetValue(true);
                new ButtonInteractingEvent(EControllerInteractionType.TOUCH, EHand.LEFT, EControllersInput.THUMBSTICK, EFingerMovement.DOWN);
            }
            else if (tempTouch.Value && (!OVRInput.Get(OVRInput.Touch.PrimaryThumbstick) && _inputContainer.LeftThumbPosition.Value == Vector2.zero))
            {
                tempTouch.SetValue(false);
                new ButtonInteractingEvent(EControllerInteractionType.TOUCH, EHand.LEFT, EControllersInput.THUMBSTICK, EFingerMovement.UP);
            }
            #endregion THUMBSTICK

            #region GRIP
            tempClick = _inputContainer.LeftClickBoolean.Get("GripIsDown");

            // Checking Click event 
            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.PrimaryHandTrigger))
            {
                tempClick.SetValue(true);
                new ButtonInteractingEvent(EControllerInteractionType.CLICK, EHand.LEFT, EControllersInput.GRIP, EFingerMovement.DOWN);
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.PrimaryHandTrigger))
            {
                tempClick.SetValue(false);
                new ButtonInteractingEvent(EControllerInteractionType.CLICK, EHand.LEFT, EControllersInput.GRIP, EFingerMovement.UP);
            }
            // Touch Event not existing on Grip
            #endregion GRIP

            #region MENU
            tempClick = _inputContainer.LeftClickBoolean.Get("MenuIsDown");

            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.Start))
            {
                tempClick.SetValue(true);
                new ButtonInteractingEvent(EControllerInteractionType.CLICK, EHand.LEFT, EControllersInput.MENU, EFingerMovement.DOWN);
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.Start))
            {
                tempClick.SetValue(false);
                new ButtonInteractingEvent(EControllerInteractionType.CLICK, EHand.LEFT, EControllersInput.MENU, EFingerMovement.UP);
            }
            // Touch Event not existing on Start 
            #endregion MENU

            #region Button X
            tempClick = _inputContainer.LeftClickBoolean.Get("XButtonIsDown");
            tempTouch = _inputContainer.LeftTouchBoolean.Get("XButtonIsTouching");

            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.Three))
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                new ButtonInteractingEvent(EControllerInteractionType.CLICK, EHand.LEFT, EControllersInput.X_BUTTON, EFingerMovement.DOWN);
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.Three))
            {
                tempClick.SetValue(false);
                new ButtonInteractingEvent(EControllerInteractionType.CLICK, EHand.LEFT, EControllersInput.X_BUTTON, EFingerMovement.UP);
            }
            // Checking Touch event if user is not clicking
            else if (!tempClick.Value && !tempTouch.Value && OVRInput.Get(OVRInput.Touch.Three))
            {
                tempTouch.SetValue(true);
                new ButtonInteractingEvent(EControllerInteractionType.TOUCH, EHand.LEFT, EControllersInput.X_BUTTON, EFingerMovement.DOWN);
            }
            else if (!tempClick.Value && tempTouch.Value && !OVRInput.Get(OVRInput.Touch.Three))
            {
                tempTouch.SetValue(false);
                new ButtonInteractingEvent(EControllerInteractionType.TOUCH, EHand.LEFT, EControllersInput.X_BUTTON, EFingerMovement.UP);
            }
            #endregion Button X

            #region Button Y
            tempClick = _inputContainer.LeftClickBoolean.Get("YButtonIsDown");
            tempTouch = _inputContainer.LeftTouchBoolean.Get("YButtonIsTouching");

            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.Four))
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                new ButtonInteractingEvent(EControllerInteractionType.CLICK, EHand.LEFT, EControllersInput.Y_BUTTON, EFingerMovement.DOWN);
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.Four))
            {
                tempClick.SetValue(false);
                new ButtonInteractingEvent(EControllerInteractionType.CLICK, EHand.LEFT, EControllersInput.Y_BUTTON, EFingerMovement.UP);
            }
            // Checking Touch event if user is not clicking
            else if (!tempClick.Value && !tempTouch.Value && OVRInput.Get(OVRInput.Touch.Four))
            {
                tempTouch.SetValue(true);
                new ButtonInteractingEvent(EControllerInteractionType.TOUCH, EHand.LEFT, EControllersInput.Y_BUTTON, EFingerMovement.DOWN);
            }
            else if (!tempClick.Value && tempTouch.Value && !OVRInput.Get(OVRInput.Touch.Four))
            {
                tempTouch.SetValue(false);
                new ButtonInteractingEvent(EControllerInteractionType.TOUCH, EHand.LEFT, EControllersInput.Y_BUTTON, EFingerMovement.UP);
            }
            #endregion Button Y

            #region THUMBREST_TOUCH_ONLY
            tempTouch = _inputContainer.LeftTouchBoolean.Get("ThumbrestIsTouching");

            // Checking Touch event
            if (!tempTouch.Value && OVRInput.Get(OVRInput.Touch.PrimaryThumbRest))
            {
                tempTouch.SetValue(true);
                new ButtonInteractingEvent(EControllerInteractionType.TOUCH, EHand.LEFT, EControllersInput.THUMBREST, EFingerMovement.DOWN);
            }
            else if (tempTouch.Value && !OVRInput.Get(OVRInput.Touch.PrimaryThumbRest))
            {
                tempTouch.SetValue(false);
                new ButtonInteractingEvent(EControllerInteractionType.TOUCH, EHand.LEFT, EControllersInput.THUMBREST, EFingerMovement.UP);
            }
            #endregion
        }
        #endregion
    }
}