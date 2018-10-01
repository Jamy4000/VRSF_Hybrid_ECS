﻿using ScriptableFramework.Variables;
using Unity.Entities;
using UnityEngine;
using VRSF.Controllers;
using VRSF.Inputs.Components;
using VRSF.Inputs.Events;

namespace VRSF.Inputs
{
    /// <summary>
    /// Script attached to the SimulatorSDK Prefab.
    /// Set the GameEvent depending on the Keyboard and Mouse Inputs.
    /// You can find a layout of the current mapping in the Wiki of the Repository.
    /// </summary>
    public class SimulatorControllersInputCaptureSystem : ComponentSystem
    {
        /// <summary>
        /// The filter for the entity component.
        /// </summary>
        struct Filter
        {
            public SimulatorControllersInputCaptureComponent ControllersInputCapture;
        }

        #region PRIVATE_VARIABLES
        // VRSF Parameters references
        private InputVariableContainer _inputContainer;
        #endregion PRIVATE_VARIABLES


        #region ComponentSystem_Methods
        /// <summary>
        /// Called after the scene was loaded, setup the entities variables
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            
            _inputContainer = InputVariableContainer.Instance;
        }

        protected override void OnUpdate()
        {
            // If we doesn't use the controllers, we don't check for the inputs.
            if (ControllersParametersVariable.Instance.UseControllers)
            {
                foreach (var entity in GetEntities<Filter>())
                {
                    CheckRightControllerInput(entity.ControllersInputCapture);
                }
            }
        }
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Handle the Right Controller input and put them in the Events
        /// </summary>
        void CheckRightControllerInput(SimulatorControllersInputCaptureComponent vrInputCapture)
        {
            BoolVariable temp;

            //Right Click
            #region TRIGGER
            temp = _inputContainer.RightClickBoolean.Get("TriggerIsDown");

            if (!temp.Value && Input.GetMouseButtonDown(1))
            {
                temp.SetValue(true);
                new ButtonInteractingEvent(EControllerInteractionType.CLICK, EHand.RIGHT, EControllersInput.TRIGGER, EFingerMovement.DOWN);
            }
            else if (temp.Value && Input.GetMouseButtonUp(1))
            {
                temp.SetValue(false);
                new ButtonInteractingEvent(EControllerInteractionType.CLICK, EHand.RIGHT, EControllersInput.TRIGGER, EFingerMovement.UP);
            }
            #endregion TRIGGER
        

            //Up, Down, Left and Right Arrows
            #region THUMB
            temp = _inputContainer.RightClickBoolean.Get("ThumbIsDown");

            //GO UP
            if (!temp.Value && Input.GetKeyDown(KeyCode.UpArrow))
            {
                _inputContainer.RightThumbPosition.SetValue(Vector2.up);
                temp.SetValue(true);

                new ButtonInteractingEvent(EControllerInteractionType.CLICK, EHand.RIGHT, EControllersInput.THUMBSTICK, EFingerMovement.DOWN);

                // Touching event raise as well
                new ButtonInteractingEvent(EControllerInteractionType.TOUCH, EHand.RIGHT, EControllersInput.THUMBSTICK, EFingerMovement.DOWN);
                _inputContainer.RightTouchBoolean.Get("ThumbIsTouching").SetValue(true);
            }
            else if (temp.Value && _inputContainer.RightThumbPosition.Value.Equals(Vector2.up) && Input.GetKeyUp(KeyCode.UpArrow))
            {
                _inputContainer.RightThumbPosition.SetValue(Vector2.zero);
                temp.SetValue(false);

                new ButtonInteractingEvent(EControllerInteractionType.CLICK, EHand.RIGHT, EControllersInput.THUMBSTICK, EFingerMovement.UP);

                // Touching event raise as well
                new ButtonInteractingEvent(EControllerInteractionType.TOUCH, EHand.RIGHT, EControllersInput.THUMBSTICK, EFingerMovement.UP);
                _inputContainer.RightTouchBoolean.Get("ThumbIsTouching").SetValue(false);
            }

            //GO DOWN
            if (!temp.Value && Input.GetKeyDown(KeyCode.DownArrow))
            {
                _inputContainer.RightThumbPosition.SetValue(Vector2.down);
                temp.SetValue(true);

                new ButtonInteractingEvent(EControllerInteractionType.CLICK, EHand.RIGHT, EControllersInput.THUMBSTICK, EFingerMovement.DOWN);

                // Touching event raise as well
                new ButtonInteractingEvent(EControllerInteractionType.TOUCH, EHand.RIGHT, EControllersInput.THUMBSTICK, EFingerMovement.DOWN);
                _inputContainer.RightTouchBoolean.Get("ThumbIsTouching").SetValue(true);
            }
            else if (temp.Value && _inputContainer.RightThumbPosition.Value.Equals(Vector2.down) && Input.GetKeyUp(KeyCode.DownArrow))
            {
                _inputContainer.RightThumbPosition.SetValue(Vector2.zero);
                temp.SetValue(false);

                new ButtonInteractingEvent(EControllerInteractionType.CLICK, EHand.RIGHT, EControllersInput.THUMBSTICK, EFingerMovement.UP);

                // Touching event raise as well
                new ButtonInteractingEvent(EControllerInteractionType.TOUCH, EHand.RIGHT, EControllersInput.THUMBSTICK, EFingerMovement.UP);
                _inputContainer.RightTouchBoolean.Get("ThumbIsTouching").SetValue(false);
            }

            //GO RIGHT
            if (!temp.Value && Input.GetKeyDown(KeyCode.RightArrow))
            {
                _inputContainer.RightThumbPosition.SetValue(Vector2.right);
                temp.SetValue(true);

                new ButtonInteractingEvent(EControllerInteractionType.CLICK, EHand.RIGHT, EControllersInput.THUMBSTICK, EFingerMovement.DOWN);

                // Touching event raise as well
                new ButtonInteractingEvent(EControllerInteractionType.TOUCH, EHand.RIGHT, EControllersInput.THUMBSTICK, EFingerMovement.DOWN);
                _inputContainer.RightTouchBoolean.Get("ThumbIsTouching").SetValue(true);
            }
            else if (temp.Value && _inputContainer.RightThumbPosition.Value.Equals(Vector2.right) && Input.GetKeyUp(KeyCode.RightArrow))
            {
                _inputContainer.RightThumbPosition.SetValue(Vector2.zero);
                temp.SetValue(false);

                new ButtonInteractingEvent(EControllerInteractionType.CLICK, EHand.RIGHT, EControllersInput.THUMBSTICK, EFingerMovement.UP);

                // Touching event raise as well
                new ButtonInteractingEvent(EControllerInteractionType.TOUCH, EHand.RIGHT, EControllersInput.THUMBSTICK, EFingerMovement.UP);
                _inputContainer.RightTouchBoolean.Get("ThumbIsTouching").SetValue(false);
            }

            //GO LEFT
            if (!temp.Value && Input.GetKeyDown(KeyCode.LeftArrow))
            {
                _inputContainer.RightThumbPosition.SetValue(Vector2.left);
                temp.SetValue(true);

                new ButtonInteractingEvent(EControllerInteractionType.CLICK, EHand.RIGHT, EControllersInput.THUMBSTICK, EFingerMovement.DOWN);

                // Touching event raise as well
                new ButtonInteractingEvent(EControllerInteractionType.TOUCH, EHand.RIGHT, EControllersInput.THUMBSTICK, EFingerMovement.DOWN);
                _inputContainer.RightTouchBoolean.Get("ThumbIsTouching").SetValue(true);
            }
            else if (temp.Value && _inputContainer.RightThumbPosition.Value.Equals(Vector2.left) && Input.GetKeyUp(KeyCode.LeftArrow))
            {
                _inputContainer.RightThumbPosition.SetValue(Vector2.zero);
                temp.SetValue(false);

                new ButtonInteractingEvent(EControllerInteractionType.CLICK, EHand.RIGHT, EControllersInput.THUMBSTICK, EFingerMovement.UP);

                // Touching event raise as well
                new ButtonInteractingEvent(EControllerInteractionType.TOUCH, EHand.RIGHT, EControllersInput.THUMBSTICK, EFingerMovement.UP);
                _inputContainer.RightTouchBoolean.Get("ThumbIsTouching").SetValue(false);
            }
            #endregion THUMB


            //Right Shift
            #region GRIP
            temp = _inputContainer.RightClickBoolean.Get("GripIsDown");

            if (!temp.Value && Input.GetKeyDown(KeyCode.RightShift))
            {
                temp.SetValue(true);
                new ButtonInteractingEvent(EControllerInteractionType.CLICK, EHand.RIGHT, EControllersInput.GRIP, EFingerMovement.DOWN);
            }
            else if (temp.Value && Input.GetKeyUp(KeyCode.RightShift))
            {
                temp.SetValue(false);
                new ButtonInteractingEvent(EControllerInteractionType.CLICK, EHand.RIGHT, EControllersInput.GRIP, EFingerMovement.UP);
            }
            #endregion GRIP


            #region VIVE_PARTICULARITY

            //Right Control
            #region MENU
            temp = _inputContainer.RightClickBoolean.Get("MenuIsDown");

            if (!temp.Value && Input.GetKeyDown(KeyCode.RightControl))
            {
                temp.SetValue(true);
                new ButtonInteractingEvent(EControllerInteractionType.CLICK, EHand.RIGHT, EControllersInput.MENU, EFingerMovement.DOWN);
            }
            else if (temp.Value && Input.GetKeyUp(KeyCode.RightControl))
            {
                temp.SetValue(false);
                new ButtonInteractingEvent(EControllerInteractionType.CLICK, EHand.RIGHT, EControllersInput.MENU, EFingerMovement.UP);
            }
            #endregion MENU

            #endregion VIVE_PARTICULARITY


            #region OCULUS_PARTICULARITIES

            //L
            #region A BUTTON
            temp = _inputContainer.RightClickBoolean.Get("AButtonIsDown");

            if (!temp.Value && Input.GetKeyDown(KeyCode.L))
            {
                temp.SetValue(true);
                new ButtonInteractingEvent(EControllerInteractionType.CLICK, EHand.RIGHT, EControllersInput.A_BUTTON, EFingerMovement.DOWN);
            }
            else if (temp.Value && Input.GetKeyUp(KeyCode.L))
            {
                temp.SetValue(false);
                new ButtonInteractingEvent(EControllerInteractionType.CLICK, EHand.RIGHT, EControllersInput.A_BUTTON, EFingerMovement.UP);
            }
            #endregion A BUTTON

            //O
            #region B BUTTON
            temp = _inputContainer.RightClickBoolean.Get("BButtonIsDown");

            if (!temp.Value && Input.GetKeyDown(KeyCode.O))
            {
                temp.SetValue(true);
                new ButtonInteractingEvent(EControllerInteractionType.CLICK, EHand.RIGHT, EControllersInput.B_BUTTON, EFingerMovement.DOWN);
            }
            else if (temp.Value && Input.GetKeyUp(KeyCode.O))
            {
                temp.SetValue(false);
                new ButtonInteractingEvent(EControllerInteractionType.CLICK, EHand.RIGHT, EControllersInput.B_BUTTON, EFingerMovement.UP);
            }
            #endregion B BUTTON

            #endregion OCULUS_PARTICULARITIES
        }
        #endregion
    }
}