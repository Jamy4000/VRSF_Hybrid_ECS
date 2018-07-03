using ScriptableFramework.Events;
using ScriptableFramework.Variables;
using UnityEngine;
using VRSF.Inputs.Gaze;

namespace VRSF.Inputs
{
    /// <summary>
    /// Script attached to the SimulatorSDK Prefab.
    /// Set the GameEvent depending on the Keyboard and Mouse Inputs.
    /// You can find a layout of the current mapping in the Wiki of the Repository.
    /// </summary>
    public class SimulatorInputCapture : VRInputCapture
    {
        // EMPTY
        #region PUBLIC_VARIABLES
        #endregion


        #region PRIVATE_VARIABLES
        private GameEvent _leftEvent;
        private GameEventBool _leftEventBool;

        private GameEvent _rightEvent;
        private GameEventBool _rightEventBool;
        #endregion


        #region MONOBEHAVIOURS
        public override void Start()
        {
            base.Start();
            CheckGazeClickButton();
        }

        // Update is called once per frame
        void Update()
        {
            CheckLeftControllerInput();
            CheckRightControllerInput();

            if (CheckGazeInteractions && GazeParameters.UseGaze)
            {
                CheckGazeInputs();
            }

            CheckWheelClick();
        }
        #endregion


        //EMPTY
        #region PUBLIC_METHODS
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Handle the Left Controller input and put them in the Events
        /// </summary>
        void CheckLeftControllerInput()
        {
            BoolVariable temp;

            //Left Click
            #region TRIGGER
            temp = InputContainer.LeftClickBoolean.Get("TriggerIsDown");

            if (!temp.Value && Input.GetMouseButtonDown(0))
            {
                temp.SetValue(true);
                _leftEvent = (GameEvent)InputContainer.LeftClickEvents.Get("TriggerDown");
                _leftEvent.Raise();
            }
            else if (temp.Value && Input.GetMouseButtonUp(0))
            {
                temp.SetValue(false);
                _leftEvent = (GameEvent)InputContainer.LeftClickEvents.Get("TriggerUp");
                _leftEvent.Raise();
            }
            #endregion TRIGGERa


            //W, A, S and D
            #region THUMB
            temp = InputContainer.LeftClickBoolean.Get("ThumbIsDown");

            //GO UP
            if (!temp.Value && Input.GetKeyDown(KeyCode.W))
            {
                temp.SetValue(true);
                InputContainer.LeftThumbPosition.SetValue(Vector2.up);

                _leftEvent = (GameEvent)InputContainer.LeftClickEvents.Get("ThumbDown");
                _leftEvent.Raise();

                // Touching event raise as well
                _leftEvent = (GameEvent)InputContainer.LeftTouchEvents.Get("ThumbStartTouching");
                _leftEvent.Raise();
                InputContainer.LeftTouchBoolean.Get("ThumbIsTouching").SetValue(true);
            }
            else if (temp.Value && InputContainer.LeftThumbPosition.Value.Equals(Vector2.up) && Input.GetKeyUp(KeyCode.W))
            {
                temp.SetValue(false);
                InputContainer.LeftThumbPosition.SetValue(Vector2.zero);
                _leftEvent = (GameEvent)InputContainer.LeftClickEvents.Get("ThumbUp");
                _leftEvent.Raise();

                // Touching event raise as well
                _leftEvent = (GameEvent)InputContainer.LeftTouchEvents.Get("ThumbStopTouching");
                _leftEvent.Raise();
                InputContainer.LeftTouchBoolean.Get("ThumbIsTouching").SetValue(false);
            }

            // GO DOWN
            if (!temp.Value && Input.GetKeyDown(KeyCode.S))
            {
                temp.SetValue(true);
                InputContainer.LeftThumbPosition.SetValue(Vector2.down);

                _leftEvent = (GameEvent)InputContainer.LeftClickEvents.Get("ThumbDown");
                _leftEvent.Raise();

                // Touching event raise as well
                _leftEvent = (GameEvent)InputContainer.LeftTouchEvents.Get("ThumbStartTouching");
                _leftEvent.Raise();
                InputContainer.LeftTouchBoolean.Get("ThumbIsTouching").SetValue(true);
            }
            else if (temp.Value && InputContainer.LeftThumbPosition.Value.Equals(Vector2.down) && Input.GetKeyUp(KeyCode.S))
            {
                temp.SetValue(false);
                InputContainer.LeftThumbPosition.SetValue(Vector2.zero);
                _leftEvent = (GameEvent)InputContainer.LeftClickEvents.Get("ThumbUp");
                _leftEvent.Raise();

                // Touching event raise as well
                _leftEvent = (GameEvent)InputContainer.LeftTouchEvents.Get("ThumbStopTouching");
                _leftEvent.Raise();
                InputContainer.LeftTouchBoolean.Get("ThumbIsTouching").SetValue(false);
            }

            //GO RIGHT
            if (!temp.Value && Input.GetKeyDown(KeyCode.D))
            {
                temp.SetValue(true);
                InputContainer.LeftThumbPosition.SetValue(Vector2.right);

                _leftEvent = (GameEvent)InputContainer.LeftClickEvents.Get("ThumbDown");
                _leftEvent.Raise();

                // Touching event raise as well
                _leftEvent = (GameEvent)InputContainer.LeftTouchEvents.Get("ThumbStartTouching");
                _leftEvent.Raise();
                InputContainer.LeftTouchBoolean.Get("ThumbIsTouching").SetValue(true);
            }
            else if (temp.Value && InputContainer.LeftThumbPosition.Value.Equals(Vector2.right) && Input.GetKeyUp(KeyCode.D))
            {
                temp.SetValue(false);
                InputContainer.LeftThumbPosition.SetValue(Vector2.zero);
                _leftEvent = (GameEvent)InputContainer.LeftClickEvents.Get("ThumbUp");
                _leftEvent.Raise();

                // Touching event raise as well
                _leftEvent = (GameEvent)InputContainer.LeftTouchEvents.Get("ThumbStopTouching");
                _leftEvent.Raise();
                InputContainer.LeftTouchBoolean.Get("ThumbIsTouching").SetValue(false);
            }

            //GO LEFT
            if (!temp.Value && Input.GetKeyDown(KeyCode.A))
            {
                temp.SetValue(true);
                InputContainer.LeftThumbPosition.SetValue(Vector2.left);

                _leftEvent = (GameEvent)InputContainer.LeftClickEvents.Get("ThumbDown");
                _leftEvent.Raise();

                // Touching event raise as well
                _leftEvent = (GameEvent)InputContainer.LeftTouchEvents.Get("ThumbStartTouching");
                _leftEvent.Raise();
                InputContainer.LeftTouchBoolean.Get("ThumbIsTouching").SetValue(true);
            }
            else if (temp.Value && InputContainer.LeftThumbPosition.Value.Equals(Vector2.left) && Input.GetKeyUp(KeyCode.A))
            {
                temp.SetValue(false);
                InputContainer.LeftThumbPosition.SetValue(Vector2.zero);
                _leftEvent = (GameEvent)InputContainer.LeftClickEvents.Get("ThumbUp");
                _leftEvent.Raise();

                // Touching event raise as well
                _leftEvent = (GameEvent)InputContainer.LeftTouchEvents.Get("ThumbStopTouching");
                _leftEvent.Raise();
                InputContainer.LeftTouchBoolean.Get("ThumbIsTouching").SetValue(false);
            }
            #endregion THUMB
        

            //Left Shift
            #region GRIP
            temp = InputContainer.LeftClickBoolean.Get("GripIsDown");

            if (!temp.Value && Input.GetKeyDown(KeyCode.LeftShift))
            {
                temp.SetValue(true);
                _leftEvent = (GameEvent)InputContainer.LeftClickEvents.Get("GripDown");
                _leftEvent.Raise();
            }
            else if (temp.Value && Input.GetKeyUp(KeyCode.LeftShift))
            {
                temp.SetValue(false);
                _leftEvent = (GameEvent)InputContainer.LeftClickEvents.Get("GripUp");
                _leftEvent.Raise();
            }
            #endregion GRIP


            //Left Control
            #region MENU
            temp = InputContainer.LeftClickBoolean.Get("MenuIsDown");

            if (!temp.Value && Input.GetKeyDown(KeyCode.LeftControl))
            {
                temp.SetValue(true);
                _leftEvent = (GameEvent)InputContainer.LeftClickEvents.Get("MenuDown");
                _leftEvent.Raise();
            }
            else if (temp.Value && Input.GetKeyUp(KeyCode.LeftControl))
            {
                temp.SetValue(false);
                _leftEvent = (GameEvent)InputContainer.LeftClickEvents.Get("MenuUp");
                _leftEvent.Raise();
            }
            #endregion MENU


            #region OCULUS_PARTICULARITIES
            
            //F
            #region X BUTTON
            temp = InputContainer.LeftClickBoolean.Get("XButtonIsDown");

            if (!temp.Value && Input.GetKeyDown(KeyCode.F))
            {
                temp.SetValue(true);
                _leftEvent = (GameEvent)InputContainer.LeftClickEvents.Get("XButtonDown");
                _leftEvent.Raise();
            }
            else if (temp.Value && Input.GetKeyUp(KeyCode.F))
            {
                temp.SetValue(false);
                _leftEvent = (GameEvent)InputContainer.LeftClickEvents.Get("XButtonUp");
                _leftEvent.Raise();
            }
            #endregion X BUTTON


            //R
            #region Y BUTTON
            temp = InputContainer.LeftClickBoolean.Get("YButtonIsDown");

            if (!temp.Value && Input.GetKeyDown(KeyCode.R))
            {
                temp.SetValue(true);
                _leftEvent = (GameEvent)InputContainer.LeftClickEvents.Get("YButtonDown");
                _leftEvent.Raise();
            }
            else if (temp.Value && Input.GetKeyUp(KeyCode.R))
            {
                temp.SetValue(false);
                _leftEvent = (GameEvent)InputContainer.LeftClickEvents.Get("YButtonUp");
                _leftEvent.Raise();
            }
            #endregion Y BUTTON

            #endregion OCULUS_PARTICULARITIES
        }

        /// <summary>
        /// Handle the Right Controller input and put them in the Events
        /// </summary>
        void CheckRightControllerInput()
        {
            BoolVariable temp;

            //Right Click
            #region TRIGGER
            temp = InputContainer.RightClickBoolean.Get("TriggerIsDown");

            if (!temp.Value && Input.GetMouseButtonDown(1))
            {
                temp.SetValue(true);
                _rightEvent = (GameEvent)InputContainer.RightClickEvents.Get("TriggerDown");
                _rightEvent.Raise();
            }
            else if (temp.Value && Input.GetMouseButtonUp(1))
            {
                temp.SetValue(false);
                _rightEvent = (GameEvent)InputContainer.RightClickEvents.Get("TriggerUp");
                _rightEvent.Raise();
            }
            #endregion TRIGGER
        

            //Up, Down, Left and Right Arrows
            #region THUMB
            temp = InputContainer.RightClickBoolean.Get("ThumbIsDown");

            //GO UP
            if (!temp.Value && Input.GetKeyDown(KeyCode.UpArrow))
            {
                InputContainer.RightThumbPosition.SetValue(Vector2.up);
                temp.SetValue(true);

                _rightEvent = (GameEvent)InputContainer.RightClickEvents.Get("ThumbDown");
                _rightEvent.Raise();

                // Touching event raise as well
                _rightEvent = (GameEvent)InputContainer.RightTouchEvents.Get("ThumbStartTouching");
                _rightEvent.Raise();
                InputContainer.RightTouchBoolean.Get("ThumbIsTouching").SetValue(true);
            }
            else if (temp.Value && InputContainer.RightThumbPosition.Value.Equals(Vector2.up) && Input.GetKeyUp(KeyCode.UpArrow))
            {
                InputContainer.RightThumbPosition.SetValue(Vector2.zero);
                temp.SetValue(false);

                _rightEvent = (GameEvent)InputContainer.RightClickEvents.Get("ThumbUp");
                _rightEvent.Raise();

                // Touching event raise as well
                _rightEvent = (GameEvent)InputContainer.RightTouchEvents.Get("ThumbStopTouching");
                _rightEvent.Raise();
                InputContainer.RightTouchBoolean.Get("ThumbIsTouching").SetValue(false);
            }

            //GO DOWN
            if (!temp.Value && Input.GetKeyDown(KeyCode.DownArrow))
            {
                InputContainer.RightThumbPosition.SetValue(Vector2.down);
                temp.SetValue(true);

                _rightEvent = (GameEvent)InputContainer.RightClickEvents.Get("ThumbDown");
                _rightEvent.Raise();

                // Touching event raise as well
                _rightEvent = (GameEvent)InputContainer.RightTouchEvents.Get("ThumbStartTouching");
                _rightEvent.Raise();
                InputContainer.RightTouchBoolean.Get("ThumbIsTouching").SetValue(true);
            }
            else if (temp.Value && InputContainer.RightThumbPosition.Value.Equals(Vector2.down) && Input.GetKeyUp(KeyCode.DownArrow))
            {
                InputContainer.RightThumbPosition.SetValue(Vector2.zero);
                temp.SetValue(false);

                _rightEvent = (GameEvent)InputContainer.RightClickEvents.Get("ThumbUp");
                _rightEvent.Raise();

                // Touching event raise as well
                _rightEvent = (GameEvent)InputContainer.RightTouchEvents.Get("ThumbStopTouching");
                _rightEvent.Raise();
                InputContainer.RightTouchBoolean.Get("ThumbIsTouching").SetValue(false);
            }

            //GO RIGHT
            if (!temp.Value && Input.GetKeyDown(KeyCode.RightArrow))
            {
                InputContainer.RightThumbPosition.SetValue(Vector2.right);
                temp.SetValue(true);

                _rightEvent = (GameEvent)InputContainer.RightClickEvents.Get("ThumbDown");
                _rightEvent.Raise();

                // Touching event raise as well
                _rightEvent = (GameEvent)InputContainer.RightTouchEvents.Get("ThumbStartTouching");
                _rightEvent.Raise();
                InputContainer.RightTouchBoolean.Get("ThumbIsTouching").SetValue(true);
            }
            else if (temp.Value && InputContainer.RightThumbPosition.Value.Equals(Vector2.right) && Input.GetKeyUp(KeyCode.RightArrow))
            {
                InputContainer.RightThumbPosition.SetValue(Vector2.zero);
                temp.SetValue(false);

                _rightEvent = (GameEvent)InputContainer.RightClickEvents.Get("ThumbUp");
                _rightEvent.Raise();

                // Touching event raise as well
                _rightEvent = (GameEvent)InputContainer.RightTouchEvents.Get("ThumbStopTouching");
                _rightEvent.Raise();
                InputContainer.RightTouchBoolean.Get("ThumbIsTouching").SetValue(false);
            }

            //GO LEFT
            if (!temp.Value && Input.GetKeyDown(KeyCode.LeftArrow))
            {
                InputContainer.RightThumbPosition.SetValue(Vector2.left);
                temp.SetValue(true);

                _rightEvent = (GameEvent)InputContainer.RightClickEvents.Get("ThumbDown");
                _rightEvent.Raise();

                // Touching event raise as well
                _rightEvent = (GameEvent)InputContainer.RightTouchEvents.Get("ThumbStartTouching");
                _rightEvent.Raise();
                InputContainer.RightTouchBoolean.Get("ThumbIsTouching").SetValue(true);
            }
            else if (temp.Value && InputContainer.RightThumbPosition.Value.Equals(Vector2.left) && Input.GetKeyUp(KeyCode.LeftArrow))
            {
                InputContainer.RightThumbPosition.SetValue(Vector2.zero);
                temp.SetValue(false);

                _rightEvent = (GameEvent)InputContainer.RightClickEvents.Get("ThumbUp");
                _rightEvent.Raise();

                // Touching event raise as well
                _rightEvent = (GameEvent)InputContainer.RightTouchEvents.Get("ThumbStopTouching");
                _rightEvent.Raise();
                InputContainer.RightTouchBoolean.Get("ThumbIsTouching").SetValue(false);
            }
            #endregion THUMB


            //Right Shift
            #region GRIP
            temp = InputContainer.RightClickBoolean.Get("GripIsDown");

            if (!temp.Value && Input.GetKeyDown(KeyCode.RightShift))
            {
                temp.SetValue(true);
                _rightEvent = (GameEvent)InputContainer.RightClickEvents.Get("GripDown");
                _rightEvent.Raise();
            }
            else if (temp.Value && Input.GetKeyUp(KeyCode.RightShift))
            {
                temp.SetValue(false);
                _rightEvent = (GameEvent)InputContainer.RightClickEvents.Get("GripUp");
                _rightEvent.Raise();
            }
            #endregion GRIP


            #region VIVE_PARTICULARITY

            //Right Control
            #region MENU
            temp = InputContainer.RightClickBoolean.Get("MenuIsDown");

            if (!temp.Value && Input.GetKeyDown(KeyCode.RightControl))
            {
                temp.SetValue(true);
                _rightEvent = (GameEvent)InputContainer.RightClickEvents.Get("MenuDown");
                _rightEvent.Raise();
            }
            else if (temp.Value && Input.GetKeyUp(KeyCode.RightControl))
            {
                temp.SetValue(false);
                _rightEvent = (GameEvent)InputContainer.RightClickEvents.Get("MenuUp");
                _rightEvent.Raise();
            }
            #endregion MENU

            #endregion VIVE_PARTICULARITY


            #region OCULUS_PARTICULARITIES

            //L
            #region A BUTTON
            temp = InputContainer.RightClickBoolean.Get("AButtonIsDown");

            if (!temp.Value && Input.GetKeyDown(KeyCode.L))
            {
                temp.SetValue(true);
                _leftEvent = (GameEvent)InputContainer.RightClickEvents.Get("AButtonDown");
                _leftEvent.Raise();
            }
            else if (temp.Value && Input.GetKeyUp(KeyCode.L))
            {
                temp.SetValue(false);
                _leftEvent = (GameEvent)InputContainer.RightClickEvents.Get("AButtonUp");
                _leftEvent.Raise();
            }
            #endregion A BUTTON

            //O
            #region B BUTTON
            temp = InputContainer.RightClickBoolean.Get("BButtonIsDown");

            if (!temp.Value && Input.GetKeyDown(KeyCode.O))
            {
                temp.SetValue(true);
                _leftEvent = (GameEvent)InputContainer.RightClickEvents.Get("BButtonDown");
                _leftEvent.Raise();
            }
            else if (temp.Value && Input.GetKeyUp(KeyCode.O))
            {
                temp.SetValue(false);
                _leftEvent = (GameEvent)InputContainer.RightClickEvents.Get("BButtonUp");
                _leftEvent.Raise();
            }
            #endregion B BUTTON

            #endregion OCULUS_PARTICULARITIES
        }

        /// <summary>
        /// Handle the Gaze click and Touch button 
        /// </summary>
        void CheckGazeInputs()
        {
            if (!InputContainer.GazeIsCliking.Value && Input.GetKeyDown(GazeInteractionDictionarySimulator.GazeToKeyCode[GazeParameters.GazeButtonSimulator]))
            {
                InputContainer.GazeIsCliking.SetValue(true);
                InputContainer.GazeIsTouching.SetValue(true);
                InputContainer.GazeClickDown.Raise();
                InputContainer.GazeStartTouching.Raise();
            }
            else if (InputContainer.GazeIsCliking.Value && Input.GetKeyUp(GazeInteractionDictionarySimulator.GazeToKeyCode[GazeParameters.GazeButtonSimulator]))
            {
                InputContainer.GazeIsCliking.SetValue(false);
                InputContainer.GazeIsTouching.SetValue(false);
                InputContainer.GazeClickUp.Raise();
                InputContainer.GazeStopTouching.Raise();
            }
        }

        /// <summary>
        /// Handle the click on the Wheel button of the Mouse
        /// </summary>
        private void CheckWheelClick()
        {
            if (!InputContainer.WheelIsClicking.Value && Input.GetKeyDown(KeyCode.Mouse2))
            {
                InputContainer.WheelIsClicking.SetValue(true);
                InputContainer.WheelClickDown.Raise();
            }
            else if (InputContainer.WheelIsClicking.Value && Input.GetKeyUp(KeyCode.Mouse2))
            {
                InputContainer.WheelIsClicking.SetValue(false);
                InputContainer.WheelClickUp.Raise();
            }
        }

        /// <summary>
        /// Check that the specified Gaze Button in the Gaze Parameters Window was set correctly
        /// </summary>
        private void CheckGazeClickButton()
        {
            if (GazeParameters.GazeButtonSimulator == EControllersInput.NONE)
            {
                CheckGazeInteractions = false;
            }
            else if (GazeParameters.GazeButtonSimulator == EControllersInput.WHEEL_BUTTON)
            {
                Debug.LogWarning("VRSF : The Gaze Button for the Simulator is currently set to Wheel_Button." +
                    "\nTake care to not use the wheel button in another script, or to use instead the GazeClick Events and variable to avoid any conflict.");
            }
        }
        #endregion
    }
}