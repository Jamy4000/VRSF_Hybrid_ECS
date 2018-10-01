
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
    public class RiftRightControllerInputCaptureSystem : ComponentSystem
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
                    CheckRightControllerInput(entity.VRInputCapture);
                }
            }
        }
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Handle the Right Controller input and put them in the Events
        /// </summary>
        void CheckRightControllerInput(RiftControllersInputCaptureComponent vrInputCapture)
        {
            BoolVariable tempClick;
            BoolVariable tempTouch;

            #region TRIGGER
            tempClick = _inputContainer.RightClickBoolean.Get("TriggerIsDown");
            tempTouch = _inputContainer.RightTouchBoolean.Get("TriggerIsTouching");

            // Checking Click event
            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                new ButtonInteractingEvent(EControllerInteractionType.CLICK, EHand.RIGHT, EControllersInput.TRIGGER, EFingerMovement.DOWN);
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
            {
                tempClick.SetValue(false);
                new ButtonInteractingEvent(EControllerInteractionType.CLICK, EHand.RIGHT, EControllersInput.TRIGGER, EFingerMovement.UP);
            }
            // Checking Touch event if user is not clicking
            else if (!tempClick.Value && !tempTouch.Value && OVRInput.Get(OVRInput.Touch.SecondaryIndexTrigger))
            {
                tempTouch.SetValue(true);
                new ButtonInteractingEvent(EControllerInteractionType.TOUCH, EHand.RIGHT, EControllersInput.TRIGGER, EFingerMovement.DOWN);
            }
            else if (!tempClick.Value && tempTouch.Value && !OVRInput.Get(OVRInput.Touch.SecondaryIndexTrigger))
            {
                tempTouch.SetValue(false);
                new ButtonInteractingEvent(EControllerInteractionType.TOUCH, EHand.RIGHT, EControllersInput.TRIGGER, EFingerMovement.UP);
            }
            #endregion TRIGGER

            #region THUMBSTICK
            _inputContainer.RightThumbPosition.SetValue(OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick));

            tempClick = _inputContainer.RightClickBoolean.Get("ThumbIsDown");
            tempTouch = _inputContainer.RightTouchBoolean.Get("ThumbIsTouching");

            // Checking Click event
            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.SecondaryThumbstick))
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                new ButtonInteractingEvent(EControllerInteractionType.CLICK, EHand.RIGHT, EControllersInput.THUMBSTICK, EFingerMovement.DOWN);
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.SecondaryThumbstick))
            {
                tempClick.SetValue(false);
                new ButtonInteractingEvent(EControllerInteractionType.CLICK, EHand.RIGHT, EControllersInput.THUMBSTICK, EFingerMovement.UP);
            }
            // Checking Touch event if user is not clicking
            else if (!tempClick.Value && !tempTouch.Value && (OVRInput.Get(OVRInput.Touch.SecondaryThumbstick) || _inputContainer.RightThumbPosition.Value != Vector2.zero))
            {
                tempTouch.SetValue(true);
                new ButtonInteractingEvent(EControllerInteractionType.TOUCH, EHand.RIGHT, EControllersInput.THUMBSTICK, EFingerMovement.DOWN);
            }
            else if (tempTouch.Value && (!OVRInput.Get(OVRInput.Touch.SecondaryThumbstick) && _inputContainer.RightThumbPosition.Value == Vector2.zero))
            {
                tempTouch.SetValue(false);
                new ButtonInteractingEvent(EControllerInteractionType.TOUCH, EHand.RIGHT, EControllersInput.THUMBSTICK, EFingerMovement.UP);
            }
            #endregion THUMBSTICK

            #region GRIP
            tempClick = _inputContainer.RightClickBoolean.Get("GripIsDown");

            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
            {
                tempClick.SetValue(true);
                new ButtonInteractingEvent(EControllerInteractionType.CLICK, EHand.RIGHT, EControllersInput.GRIP, EFingerMovement.DOWN);
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
            {
                tempClick.SetValue(false);
                new ButtonInteractingEvent(EControllerInteractionType.CLICK, EHand.RIGHT, EControllersInput.GRIP, EFingerMovement.UP);
            }
            // Touch Event not existing on Grip 
            #endregion GRIP

            //No Right menu button on the oculus

            #region Button A
            tempClick = _inputContainer.RightClickBoolean.Get("AButtonIsDown");
            tempTouch = _inputContainer.RightTouchBoolean.Get("AButtonIsTouching");

            // Checking Click event
            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.One))
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                new ButtonInteractingEvent(EControllerInteractionType.CLICK, EHand.RIGHT, EControllersInput.A_BUTTON, EFingerMovement.DOWN);
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.One))
            {
                tempClick.SetValue(false);
                new ButtonInteractingEvent(EControllerInteractionType.CLICK, EHand.RIGHT, EControllersInput.A_BUTTON, EFingerMovement.UP);
            }
            // Checking Touch event if user is not clicking
            else if (!tempClick.Value && !tempTouch.Value && OVRInput.Get(OVRInput.Touch.One))
            {
                tempTouch.SetValue(true);
                new ButtonInteractingEvent(EControllerInteractionType.TOUCH, EHand.RIGHT, EControllersInput.A_BUTTON, EFingerMovement.DOWN);
            }
            else if (tempTouch.Value && !OVRInput.Get(OVRInput.Touch.One))
            {
                tempTouch.SetValue(false);
                new ButtonInteractingEvent(EControllerInteractionType.TOUCH, EHand.RIGHT, EControllersInput.A_BUTTON, EFingerMovement.UP);
            }
            #endregion

            #region Button B
            tempClick = _inputContainer.RightClickBoolean.Get("BButtonIsDown");
            tempTouch = _inputContainer.RightTouchBoolean.Get("BButtonIsTouching");

            // Checking Click event
            if (!tempClick.Value && OVRInput.Get(OVRInput.Button.Two))
            {
                tempClick.SetValue(true);
                tempTouch.SetValue(false);
                new ButtonInteractingEvent(EControllerInteractionType.CLICK, EHand.RIGHT, EControllersInput.B_BUTTON, EFingerMovement.DOWN);
            }
            else if (tempClick.Value && !OVRInput.Get(OVRInput.Button.Two))
            {
                tempClick.SetValue(false);
                new ButtonInteractingEvent(EControllerInteractionType.CLICK, EHand.RIGHT, EControllersInput.B_BUTTON, EFingerMovement.UP);
            }
            // Checking Touch event if user is not clicking
            else if (!tempClick.Value && !tempTouch.Value && OVRInput.Get(OVRInput.Touch.Two))
            {
                tempTouch.SetValue(true);
                new ButtonInteractingEvent(EControllerInteractionType.TOUCH, EHand.RIGHT, EControllersInput.B_BUTTON, EFingerMovement.DOWN);
            }
            else if (!tempClick.Value && tempTouch.Value && !OVRInput.Get(OVRInput.Touch.Two))
            {
                tempTouch.SetValue(false);
                new ButtonInteractingEvent(EControllerInteractionType.TOUCH, EHand.RIGHT, EControllersInput.B_BUTTON, EFingerMovement.UP);
            }
            #endregion

            #region THUMBREST_TOUCH_ONLY
            tempTouch = _inputContainer.RightTouchBoolean.Get("ThumbrestIsTouching");

            // Checking Touch event
            if (!tempTouch.Value && OVRInput.Get(OVRInput.Touch.SecondaryThumbRest))
            {
                tempTouch.SetValue(true);
                new ButtonInteractingEvent(EControllerInteractionType.TOUCH, EHand.RIGHT, EControllersInput.THUMBREST, EFingerMovement.DOWN);
            }
            else if (tempTouch.Value && !OVRInput.Get(OVRInput.Touch.SecondaryThumbRest))
            {
                tempTouch.SetValue(false);
                new ButtonInteractingEvent(EControllerInteractionType.TOUCH, EHand.RIGHT, EControllersInput.THUMBREST, EFingerMovement.UP);
            }
            #endregion
        }
        #endregion
    }
}