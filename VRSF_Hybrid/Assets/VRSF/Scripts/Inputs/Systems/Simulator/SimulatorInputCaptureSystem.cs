using ScriptableFramework.Events;
using ScriptableFramework.Variables;
using Unity.Entities;
using UnityEngine;
using VRSF.Controllers;
using VRSF.Gaze;
using VRSF.Inputs.Components;
using VRSF.Inputs.Gaze;

namespace VRSF.Inputs
{
    /// <summary>
    /// Script attached to the SimulatorSDK Prefab.
    /// Set the GameEvent depending on the Keyboard and Mouse Inputs.
    /// You can find a layout of the current mapping in the Wiki of the Repository.
    /// </summary>
    public class SimulatorInputCaptureSystem : ComponentSystem
    {
        /// <summary>
        /// The filter for the entity component.
        /// </summary>
        struct Filter
        {
            public VRInputCaptureComponent VRInputCapture;
        }

        #region PRIVATE_VARIABLES
        // VRSF Parameters references
        private GazeParametersVariable _gazeParameters;
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

            _gazeParameters = GazeParametersVariable.Instance;
            _inputContainer = InputVariableContainer.Instance;

            foreach (var entity in GetEntities<Filter>())
            {
                CheckGazeClickButton(entity.VRInputCapture);
            }
        }

        protected override void OnUpdate()
        {
            // If we doesn't use the controllers, we don't check for the inputs.
            if (ControllersParametersVariable.Instance.UseControllers)
            {
                foreach (var entity in GetEntities<Filter>())
                {
                    CheckLeftControllerInput(entity.VRInputCapture);
                    CheckRightControllerInput(entity.VRInputCapture);

                    // If we want to check the gaze interactions
                    if (entity.VRInputCapture.CheckGazeInteractions)
                        CheckGazeInputs(entity.VRInputCapture);

                    CheckWheelClick();
                }
            }
        }
        #endregion


        //EMPTY
        #region PUBLIC_METHODS
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Handle the Left Controller input and put them in the Events
        /// </summary>
        void CheckLeftControllerInput(VRInputCaptureComponent vrInputCapture)
        {
            BoolVariable temp;

            //Left Click
            #region TRIGGER
            temp = _inputContainer.LeftClickBoolean.Get("TriggerIsDown");

            if (!temp.Value && Input.GetMouseButtonDown(0))
            {
                temp.SetValue(true);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("TriggerDown");
                vrInputCapture.TempEvent.Raise();
            }
            else if (temp.Value && Input.GetMouseButtonUp(0))
            {
                temp.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("TriggerUp");
                vrInputCapture.TempEvent.Raise();
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

                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("ThumbDown");
                vrInputCapture.TempEvent.Raise();

                // Touching event raise as well
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftTouchEvents.Get("ThumbStartTouching");
                vrInputCapture.TempEvent.Raise();
                _inputContainer.LeftTouchBoolean.Get("ThumbIsTouching").SetValue(true);
            }
            else if (temp.Value && _inputContainer.LeftThumbPosition.Value.Equals(Vector2.up) && Input.GetKeyUp(KeyCode.W))
            {
                temp.SetValue(false);
                _inputContainer.LeftThumbPosition.SetValue(Vector2.zero);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("ThumbUp");
                vrInputCapture.TempEvent.Raise();

                // Touching event raise as well
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftTouchEvents.Get("ThumbStopTouching");
                vrInputCapture.TempEvent.Raise();
                _inputContainer.LeftTouchBoolean.Get("ThumbIsTouching").SetValue(false);
            }

            // GO DOWN
            if (!temp.Value && Input.GetKeyDown(KeyCode.S))
            {
                temp.SetValue(true);
                _inputContainer.LeftThumbPosition.SetValue(Vector2.down);

                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("ThumbDown");
                vrInputCapture.TempEvent.Raise();

                // Touching event raise as well
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftTouchEvents.Get("ThumbStartTouching");
                vrInputCapture.TempEvent.Raise();
                _inputContainer.LeftTouchBoolean.Get("ThumbIsTouching").SetValue(true);
            }
            else if (temp.Value && _inputContainer.LeftThumbPosition.Value.Equals(Vector2.down) && Input.GetKeyUp(KeyCode.S))
            {
                temp.SetValue(false);
                _inputContainer.LeftThumbPosition.SetValue(Vector2.zero);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("ThumbUp");
                vrInputCapture.TempEvent.Raise();

                // Touching event raise as well
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftTouchEvents.Get("ThumbStopTouching");
                vrInputCapture.TempEvent.Raise();
                _inputContainer.LeftTouchBoolean.Get("ThumbIsTouching").SetValue(false);
            }

            //GO RIGHT
            if (!temp.Value && Input.GetKeyDown(KeyCode.D))
            {
                temp.SetValue(true);
                _inputContainer.LeftThumbPosition.SetValue(Vector2.right);

                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("ThumbDown");
                vrInputCapture.TempEvent.Raise();

                // Touching event raise as well
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftTouchEvents.Get("ThumbStartTouching");
                vrInputCapture.TempEvent.Raise();
                _inputContainer.LeftTouchBoolean.Get("ThumbIsTouching").SetValue(true);
            }
            else if (temp.Value && _inputContainer.LeftThumbPosition.Value.Equals(Vector2.right) && Input.GetKeyUp(KeyCode.D))
            {
                temp.SetValue(false);
                _inputContainer.LeftThumbPosition.SetValue(Vector2.zero);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("ThumbUp");
                vrInputCapture.TempEvent.Raise();

                // Touching event raise as well
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftTouchEvents.Get("ThumbStopTouching");
                vrInputCapture.TempEvent.Raise();
                _inputContainer.LeftTouchBoolean.Get("ThumbIsTouching").SetValue(false);
            }

            //GO LEFT
            if (!temp.Value && Input.GetKeyDown(KeyCode.A))
            {
                temp.SetValue(true);
                _inputContainer.LeftThumbPosition.SetValue(Vector2.left);

                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("ThumbDown");
                vrInputCapture.TempEvent.Raise();

                // Touching event raise as well
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftTouchEvents.Get("ThumbStartTouching");
                vrInputCapture.TempEvent.Raise();
                _inputContainer.LeftTouchBoolean.Get("ThumbIsTouching").SetValue(true);
            }
            else if (temp.Value && _inputContainer.LeftThumbPosition.Value.Equals(Vector2.left) && Input.GetKeyUp(KeyCode.A))
            {
                temp.SetValue(false);
                _inputContainer.LeftThumbPosition.SetValue(Vector2.zero);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("ThumbUp");
                vrInputCapture.TempEvent.Raise();

                // Touching event raise as well
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftTouchEvents.Get("ThumbStopTouching");
                vrInputCapture.TempEvent.Raise();
                _inputContainer.LeftTouchBoolean.Get("ThumbIsTouching").SetValue(false);
            }
            #endregion THUMB
        

            //Left Shift
            #region GRIP
            temp = _inputContainer.LeftClickBoolean.Get("GripIsDown");

            if (!temp.Value && Input.GetKeyDown(KeyCode.LeftShift))
            {
                temp.SetValue(true);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("GripDown");
                vrInputCapture.TempEvent.Raise();
            }
            else if (temp.Value && Input.GetKeyUp(KeyCode.LeftShift))
            {
                temp.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("GripUp");
                vrInputCapture.TempEvent.Raise();
            }
            #endregion GRIP


            //Left Control
            #region MENU
            temp = _inputContainer.LeftClickBoolean.Get("MenuIsDown");

            if (!temp.Value && Input.GetKeyDown(KeyCode.LeftControl))
            {
                temp.SetValue(true);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("MenuDown");
                vrInputCapture.TempEvent.Raise();
            }
            else if (temp.Value && Input.GetKeyUp(KeyCode.LeftControl))
            {
                temp.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("MenuUp");
                vrInputCapture.TempEvent.Raise();
            }
            #endregion MENU


            #region OCULUS_PARTICULARITIES
            
            //F
            #region X BUTTON
            temp = _inputContainer.LeftClickBoolean.Get("XButtonIsDown");

            if (!temp.Value && Input.GetKeyDown(KeyCode.F))
            {
                temp.SetValue(true);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("XButtonDown");
                vrInputCapture.TempEvent.Raise();
            }
            else if (temp.Value && Input.GetKeyUp(KeyCode.F))
            {
                temp.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("XButtonUp");
                vrInputCapture.TempEvent.Raise();
            }
            #endregion X BUTTON


            //R
            #region Y BUTTON
            temp = _inputContainer.LeftClickBoolean.Get("YButtonIsDown");

            if (!temp.Value && Input.GetKeyDown(KeyCode.R))
            {
                temp.SetValue(true);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("YButtonDown");
                vrInputCapture.TempEvent.Raise();
            }
            else if (temp.Value && Input.GetKeyUp(KeyCode.R))
            {
                temp.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)_inputContainer.LeftClickEvents.Get("YButtonUp");
                vrInputCapture.TempEvent.Raise();
            }
            #endregion Y BUTTON

            #endregion OCULUS_PARTICULARITIES
        }

        /// <summary>
        /// Handle the Right Controller input and put them in the Events
        /// </summary>
        void CheckRightControllerInput(VRInputCaptureComponent vrInputCapture)
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

        /// <summary>
        /// Handle the Gaze click and Touch button 
        /// </summary>
        void CheckGazeInputs(VRInputCaptureComponent vrInputCapture)
        {
            // If the gaze boolVariable is at false but the gaze button is clicking
            if (!_inputContainer.GazeIsCliking.Value && 
                Input.GetKeyDown(GazeInteractionSimulator.Dictionnary[_gazeParameters.GazeButtonSimulator]))
            {
                _inputContainer.GazeIsCliking.SetValue(true);
                _inputContainer.GazeIsTouching.SetValue(true);
                _inputContainer.GazeClickDown.Raise();
                _inputContainer.GazeStartTouching.Raise();
            }
            // If the gaze boolVariable is at true but the gaze button is not clicking
            else if (_inputContainer.GazeIsCliking.Value && 
                Input.GetKeyUp(GazeInteractionSimulator.Dictionnary[_gazeParameters.GazeButtonSimulator]))
            {
                _inputContainer.GazeIsCliking.SetValue(false);
                _inputContainer.GazeIsTouching.SetValue(false);
                _inputContainer.GazeClickUp.Raise();
                _inputContainer.GazeStopTouching.Raise();
            }
        }

        /// <summary>
        /// Handle the click on the Wheel button of the Mouse
        /// </summary>
        private void CheckWheelClick()
        {
            var inputContainer = InputVariableContainer.Instance;

            // If the boolVariable for the wheel is clicking is at false but the user is pressing it
            if (!inputContainer.WheelIsClicking.Value && Input.GetKeyDown(KeyCode.Mouse2))
            {
                inputContainer.WheelIsClicking.SetValue(true);
                inputContainer.WheelClickDown.Raise();
            }
            // If the boolVariable for the wheel is clicking is at true but the user is not pressing it
            else if (inputContainer.WheelIsClicking.Value && Input.GetKeyUp(KeyCode.Mouse2))
            {
                inputContainer.WheelIsClicking.SetValue(false);
                inputContainer.WheelClickUp.Raise();
            }
        }

        /// <summary>
        /// Check that the specified Gaze Button in the Gaze Parameters Window was set correctly
        /// </summary>
        private void CheckGazeClickButton(VRInputCaptureComponent vrInputCapture)
        {
            if (_gazeParameters.GazeButtonSimulator == EControllersInput.NONE)
            {
                vrInputCapture.CheckGazeInteractions = false;
            }
        }
        #endregion
    }
}