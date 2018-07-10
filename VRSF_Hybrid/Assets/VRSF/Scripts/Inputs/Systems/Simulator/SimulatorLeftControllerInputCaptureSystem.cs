﻿using ScriptableFramework.Events;
using ScriptableFramework.Variables;
using Unity.Entities;
using UnityEngine;
using VRSF.Controllers;
using VRSF.Inputs.Components;

namespace VRSF.Inputs
{
    /// <summary>
    /// Script attached to the SimulatorSDK Prefab.
    /// Set the GameEvent depending on the Keyboard and Mouse Inputs.
    /// You can find a layout of the current mapping in the Wiki of the Repository.
    /// </summary>
    public class SimulatorLeftControllerInputCaptureSystem : ComponentSystem
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
                    CheckLeftControllerInput(entity.ControllersInputCapture);
                }
            }
        }
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Handle the Left Controller input and put them in the Events
        /// </summary>
        void CheckLeftControllerInput(SimulatorControllersInputCaptureComponent controllerInputCapture)
        {
            BoolVariable temp;

            //Left Click
            #region TRIGGER
            temp = _inputContainer.LeftClickBoolean.Get("TriggerIsDown");

            if (!temp.Value && Input.GetMouseButtonDown(0))
            {
                temp.SetValue(true);
                controllerInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("TriggerDown");
                controllerInputCapture.TempEvent.Raise();
            }
            else if (temp.Value && Input.GetMouseButtonUp(0))
            {
                temp.SetValue(false);
                controllerInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("TriggerUp");
                controllerInputCapture.TempEvent.Raise();
            }
            #endregion TRIGGERa


            //W, A, S and D
            #region THUMB
            temp = _inputContainer.LeftClickBoolean.Get("ThumbIsDown");

            //GO UP
            if (!temp.Value && Input.GetKeyDown(KeyCode.W))
            {
                temp.SetValue(true);
                _inputContainer.LeftThumbPosition.SetValue(Vector2.up);

                controllerInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("ThumbDown");
                controllerInputCapture.TempEvent.Raise();

                // Touching event raise as well
                controllerInputCapture.TempEvent = (GameEvent)_inputContainer.LeftTouchEvents.Get("ThumbStartTouching");
                controllerInputCapture.TempEvent.Raise();
                _inputContainer.LeftTouchBoolean.Get("ThumbIsTouching").SetValue(true);
            }
            else if (temp.Value && _inputContainer.LeftThumbPosition.Value.Equals(Vector2.up) && Input.GetKeyUp(KeyCode.W))
            {
                temp.SetValue(false);
                _inputContainer.LeftThumbPosition.SetValue(Vector2.zero);
                controllerInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("ThumbUp");
                controllerInputCapture.TempEvent.Raise();

                // Touching event raise as well
                controllerInputCapture.TempEvent = (GameEvent)_inputContainer.LeftTouchEvents.Get("ThumbStopTouching");
                controllerInputCapture.TempEvent.Raise();
                _inputContainer.LeftTouchBoolean.Get("ThumbIsTouching").SetValue(false);
            }

            // GO DOWN
            if (!temp.Value && Input.GetKeyDown(KeyCode.S))
            {
                temp.SetValue(true);
                _inputContainer.LeftThumbPosition.SetValue(Vector2.down);

                controllerInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("ThumbDown");
                controllerInputCapture.TempEvent.Raise();

                // Touching event raise as well
                controllerInputCapture.TempEvent = (GameEvent)_inputContainer.LeftTouchEvents.Get("ThumbStartTouching");
                controllerInputCapture.TempEvent.Raise();
                _inputContainer.LeftTouchBoolean.Get("ThumbIsTouching").SetValue(true);
            }
            else if (temp.Value && _inputContainer.LeftThumbPosition.Value.Equals(Vector2.down) && Input.GetKeyUp(KeyCode.S))
            {
                temp.SetValue(false);
                _inputContainer.LeftThumbPosition.SetValue(Vector2.zero);
                controllerInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("ThumbUp");
                controllerInputCapture.TempEvent.Raise();

                // Touching event raise as well
                controllerInputCapture.TempEvent = (GameEvent)_inputContainer.LeftTouchEvents.Get("ThumbStopTouching");
                controllerInputCapture.TempEvent.Raise();
                _inputContainer.LeftTouchBoolean.Get("ThumbIsTouching").SetValue(false);
            }

            //GO RIGHT
            if (!temp.Value && Input.GetKeyDown(KeyCode.D))
            {
                temp.SetValue(true);
                _inputContainer.LeftThumbPosition.SetValue(Vector2.right);

                controllerInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("ThumbDown");
                controllerInputCapture.TempEvent.Raise();

                // Touching event raise as well
                controllerInputCapture.TempEvent = (GameEvent)_inputContainer.LeftTouchEvents.Get("ThumbStartTouching");
                controllerInputCapture.TempEvent.Raise();
                _inputContainer.LeftTouchBoolean.Get("ThumbIsTouching").SetValue(true);
            }
            else if (temp.Value && _inputContainer.LeftThumbPosition.Value.Equals(Vector2.right) && Input.GetKeyUp(KeyCode.D))
            {
                temp.SetValue(false);
                _inputContainer.LeftThumbPosition.SetValue(Vector2.zero);
                controllerInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("ThumbUp");
                controllerInputCapture.TempEvent.Raise();

                // Touching event raise as well
                controllerInputCapture.TempEvent = (GameEvent)_inputContainer.LeftTouchEvents.Get("ThumbStopTouching");
                controllerInputCapture.TempEvent.Raise();
                _inputContainer.LeftTouchBoolean.Get("ThumbIsTouching").SetValue(false);
            }

            //GO LEFT
            if (!temp.Value && Input.GetKeyDown(KeyCode.A))
            {
                temp.SetValue(true);
                _inputContainer.LeftThumbPosition.SetValue(Vector2.left);

                controllerInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("ThumbDown");
                controllerInputCapture.TempEvent.Raise();

                // Touching event raise as well
                controllerInputCapture.TempEvent = (GameEvent)_inputContainer.LeftTouchEvents.Get("ThumbStartTouching");
                controllerInputCapture.TempEvent.Raise();
                _inputContainer.LeftTouchBoolean.Get("ThumbIsTouching").SetValue(true);
            }
            else if (temp.Value && _inputContainer.LeftThumbPosition.Value.Equals(Vector2.left) && Input.GetKeyUp(KeyCode.A))
            {
                temp.SetValue(false);
                _inputContainer.LeftThumbPosition.SetValue(Vector2.zero);
                controllerInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("ThumbUp");
                controllerInputCapture.TempEvent.Raise();

                // Touching event raise as well
                controllerInputCapture.TempEvent = (GameEvent)_inputContainer.LeftTouchEvents.Get("ThumbStopTouching");
                controllerInputCapture.TempEvent.Raise();
                _inputContainer.LeftTouchBoolean.Get("ThumbIsTouching").SetValue(false);
            }
            #endregion THUMB
        

            //Left Shift
            #region GRIP
            temp = _inputContainer.LeftClickBoolean.Get("GripIsDown");

            if (!temp.Value && Input.GetKeyDown(KeyCode.LeftShift))
            {
                temp.SetValue(true);
                controllerInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("GripDown");
                controllerInputCapture.TempEvent.Raise();
            }
            else if (temp.Value && Input.GetKeyUp(KeyCode.LeftShift))
            {
                temp.SetValue(false);
                controllerInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("GripUp");
                controllerInputCapture.TempEvent.Raise();
            }
            #endregion GRIP


            //Left Control
            #region MENU
            temp = _inputContainer.LeftClickBoolean.Get("MenuIsDown");

            if (!temp.Value && Input.GetKeyDown(KeyCode.LeftControl))
            {
                temp.SetValue(true);
                controllerInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("MenuDown");
                controllerInputCapture.TempEvent.Raise();
            }
            else if (temp.Value && Input.GetKeyUp(KeyCode.LeftControl))
            {
                temp.SetValue(false);
                controllerInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("MenuUp");
                controllerInputCapture.TempEvent.Raise();
            }
            #endregion MENU


            #region OCULUS_PARTICULARITIES
            
            //F
            #region X BUTTON
            temp = _inputContainer.LeftClickBoolean.Get("XButtonIsDown");

            if (!temp.Value && Input.GetKeyDown(KeyCode.F))
            {
                temp.SetValue(true);
                controllerInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("XButtonDown");
                controllerInputCapture.TempEvent.Raise();
            }
            else if (temp.Value && Input.GetKeyUp(KeyCode.F))
            {
                temp.SetValue(false);
                controllerInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("XButtonUp");
                controllerInputCapture.TempEvent.Raise();
            }
            #endregion X BUTTON


            //R
            #region Y BUTTON
            temp = _inputContainer.LeftClickBoolean.Get("YButtonIsDown");

            if (!temp.Value && Input.GetKeyDown(KeyCode.R))
            {
                temp.SetValue(true);
                controllerInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("YButtonDown");
                controllerInputCapture.TempEvent.Raise();
            }
            else if (temp.Value && Input.GetKeyUp(KeyCode.R))
            {
                temp.SetValue(false);
                controllerInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("YButtonUp");
                controllerInputCapture.TempEvent.Raise();
            }
            #endregion Y BUTTON

            #endregion OCULUS_PARTICULARITIES
        }
        #endregion
    }
}