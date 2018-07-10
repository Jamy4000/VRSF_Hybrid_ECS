using ScriptableFramework.Events;
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
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.RightClickEvents.Get("TriggerDown");
                vrInputCapture.TempEvent.Raise();
            }
            else if (temp.Value && Input.GetMouseButtonUp(1))
            {
                temp.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.RightClickEvents.Get("TriggerUp");
                vrInputCapture.TempEvent.Raise();
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

                vrInputCapture.TempEvent = (GameEvent)_inputContainer.RightClickEvents.Get("ThumbDown");
                vrInputCapture.TempEvent.Raise();

                // Touching event raise as well
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.RightTouchEvents.Get("ThumbStartTouching");
                vrInputCapture.TempEvent.Raise();
                _inputContainer.RightTouchBoolean.Get("ThumbIsTouching").SetValue(true);
            }
            else if (temp.Value && _inputContainer.RightThumbPosition.Value.Equals(Vector2.up) && Input.GetKeyUp(KeyCode.UpArrow))
            {
                _inputContainer.RightThumbPosition.SetValue(Vector2.zero);
                temp.SetValue(false);

                vrInputCapture.TempEvent = (GameEvent)_inputContainer.RightClickEvents.Get("ThumbUp");
                vrInputCapture.TempEvent.Raise();

                // Touching event raise as well
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.RightTouchEvents.Get("ThumbStopTouching");
                vrInputCapture.TempEvent.Raise();
                _inputContainer.RightTouchBoolean.Get("ThumbIsTouching").SetValue(false);
            }

            //GO DOWN
            if (!temp.Value && Input.GetKeyDown(KeyCode.DownArrow))
            {
                _inputContainer.RightThumbPosition.SetValue(Vector2.down);
                temp.SetValue(true);

                vrInputCapture.TempEvent = (GameEvent)_inputContainer.RightClickEvents.Get("ThumbDown");
                vrInputCapture.TempEvent.Raise();

                // Touching event raise as well
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.RightTouchEvents.Get("ThumbStartTouching");
                vrInputCapture.TempEvent.Raise();
                _inputContainer.RightTouchBoolean.Get("ThumbIsTouching").SetValue(true);
            }
            else if (temp.Value && _inputContainer.RightThumbPosition.Value.Equals(Vector2.down) && Input.GetKeyUp(KeyCode.DownArrow))
            {
                _inputContainer.RightThumbPosition.SetValue(Vector2.zero);
                temp.SetValue(false);

                vrInputCapture.TempEvent = (GameEvent)_inputContainer.RightClickEvents.Get("ThumbUp");
                vrInputCapture.TempEvent.Raise();

                // Touching event raise as well
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.RightTouchEvents.Get("ThumbStopTouching");
                vrInputCapture.TempEvent.Raise();
                _inputContainer.RightTouchBoolean.Get("ThumbIsTouching").SetValue(false);
            }

            //GO RIGHT
            if (!temp.Value && Input.GetKeyDown(KeyCode.RightArrow))
            {
                _inputContainer.RightThumbPosition.SetValue(Vector2.right);
                temp.SetValue(true);

                vrInputCapture.TempEvent = (GameEvent)_inputContainer.RightClickEvents.Get("ThumbDown");
                vrInputCapture.TempEvent.Raise();

                // Touching event raise as well
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.RightTouchEvents.Get("ThumbStartTouching");
                vrInputCapture.TempEvent.Raise();
                _inputContainer.RightTouchBoolean.Get("ThumbIsTouching").SetValue(true);
            }
            else if (temp.Value && _inputContainer.RightThumbPosition.Value.Equals(Vector2.right) && Input.GetKeyUp(KeyCode.RightArrow))
            {
                _inputContainer.RightThumbPosition.SetValue(Vector2.zero);
                temp.SetValue(false);

                vrInputCapture.TempEvent = (GameEvent)_inputContainer.RightClickEvents.Get("ThumbUp");
                vrInputCapture.TempEvent.Raise();

                // Touching event raise as well
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.RightTouchEvents.Get("ThumbStopTouching");
                vrInputCapture.TempEvent.Raise();
                _inputContainer.RightTouchBoolean.Get("ThumbIsTouching").SetValue(false);
            }

            //GO LEFT
            if (!temp.Value && Input.GetKeyDown(KeyCode.LeftArrow))
            {
                _inputContainer.RightThumbPosition.SetValue(Vector2.left);
                temp.SetValue(true);

                vrInputCapture.TempEvent = (GameEvent)_inputContainer.RightClickEvents.Get("ThumbDown");
                vrInputCapture.TempEvent.Raise();

                // Touching event raise as well
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.RightTouchEvents.Get("ThumbStartTouching");
                vrInputCapture.TempEvent.Raise();
                _inputContainer.RightTouchBoolean.Get("ThumbIsTouching").SetValue(true);
            }
            else if (temp.Value && _inputContainer.RightThumbPosition.Value.Equals(Vector2.left) && Input.GetKeyUp(KeyCode.LeftArrow))
            {
                _inputContainer.RightThumbPosition.SetValue(Vector2.zero);
                temp.SetValue(false);

                vrInputCapture.TempEvent = (GameEvent)_inputContainer.RightClickEvents.Get("ThumbUp");
                vrInputCapture.TempEvent.Raise();

                // Touching event raise as well
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.RightTouchEvents.Get("ThumbStopTouching");
                vrInputCapture.TempEvent.Raise();
                _inputContainer.RightTouchBoolean.Get("ThumbIsTouching").SetValue(false);
            }
            #endregion THUMB


            //Right Shift
            #region GRIP
            temp = _inputContainer.RightClickBoolean.Get("GripIsDown");

            if (!temp.Value && Input.GetKeyDown(KeyCode.RightShift))
            {
                temp.SetValue(true);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.RightClickEvents.Get("GripDown");
                vrInputCapture.TempEvent.Raise();
            }
            else if (temp.Value && Input.GetKeyUp(KeyCode.RightShift))
            {
                temp.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.RightClickEvents.Get("GripUp");
                vrInputCapture.TempEvent.Raise();
            }
            #endregion GRIP


            #region VIVE_PARTICULARITY

            //Right Control
            #region MENU
            temp = _inputContainer.RightClickBoolean.Get("MenuIsDown");

            if (!temp.Value && Input.GetKeyDown(KeyCode.RightControl))
            {
                temp.SetValue(true);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.RightClickEvents.Get("MenuDown");
                vrInputCapture.TempEvent.Raise();
            }
            else if (temp.Value && Input.GetKeyUp(KeyCode.RightControl))
            {
                temp.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.RightClickEvents.Get("MenuUp");
                vrInputCapture.TempEvent.Raise();
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
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.RightClickEvents.Get("AButtonDown");
                vrInputCapture.TempEvent.Raise();
            }
            else if (temp.Value && Input.GetKeyUp(KeyCode.L))
            {
                temp.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.RightClickEvents.Get("AButtonUp");
                vrInputCapture.TempEvent.Raise();
            }
            #endregion A BUTTON

            //O
            #region B BUTTON
            temp = _inputContainer.RightClickBoolean.Get("BButtonIsDown");

            if (!temp.Value && Input.GetKeyDown(KeyCode.O))
            {
                temp.SetValue(true);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.RightClickEvents.Get("BButtonDown");
                vrInputCapture.TempEvent.Raise();
            }
            else if (temp.Value && Input.GetKeyUp(KeyCode.O))
            {
                temp.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.RightClickEvents.Get("BButtonUp");
                vrInputCapture.TempEvent.Raise();
            }
            #endregion B BUTTON

            #endregion OCULUS_PARTICULARITIES
        }
        #endregion
    }
}