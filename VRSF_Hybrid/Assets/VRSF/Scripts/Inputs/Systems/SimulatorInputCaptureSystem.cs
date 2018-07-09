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


        #region ComponentSystem_Methods
        /// <summary>
        /// Called after the scene was loaded, setup the entities variables
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnStartRunning()
        {
            foreach (var entity in GetEntities<Filter>())
            {
                entity.VRInputCapture.ControllersParameters = ControllersParametersVariable.Instance;
                entity.VRInputCapture.GazeParameters = GazeParametersVariable.Instance;
                entity.VRInputCapture.InputContainer = InputVariableContainer.Instance;

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
            temp = vrInputCapture.InputContainer.LeftClickBoolean.Get("TriggerIsDown");

            if (!temp.Value && Input.GetMouseButtonDown(0))
            {
                temp.SetValue(true);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.LeftClickEvents.Get("TriggerDown");
                vrInputCapture.TempEvent.Raise();
            }
            else if (temp.Value && Input.GetMouseButtonUp(0))
            {
                temp.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.LeftClickEvents.Get("TriggerUp");
                vrInputCapture.TempEvent.Raise();
            }
            #endregion TRIGGERa


            //W, A, S and D
            #region THUMB
            temp = vrInputCapture.InputContainer.LeftClickBoolean.Get("ThumbIsDown");

            //GO UP
            if (!temp.Value && Input.GetKeyDown(KeyCode.W))
            {
                temp.SetValue(true);
                vrInputCapture.InputContainer.LeftThumbPosition.SetValue(Vector2.up);

                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.LeftClickEvents.Get("ThumbDown");
                vrInputCapture.TempEvent.Raise();

                // Touching event raise as well
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.LeftTouchEvents.Get("ThumbStartTouching");
                vrInputCapture.TempEvent.Raise();
                vrInputCapture.InputContainer.LeftTouchBoolean.Get("ThumbIsTouching").SetValue(true);
            }
            else if (temp.Value && vrInputCapture.InputContainer.LeftThumbPosition.Value.Equals(Vector2.up) && Input.GetKeyUp(KeyCode.W))
            {
                temp.SetValue(false);
                vrInputCapture.InputContainer.LeftThumbPosition.SetValue(Vector2.zero);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.LeftClickEvents.Get("ThumbUp");
                vrInputCapture.TempEvent.Raise();

                // Touching event raise as well
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.LeftTouchEvents.Get("ThumbStopTouching");
                vrInputCapture.TempEvent.Raise();
                vrInputCapture.InputContainer.LeftTouchBoolean.Get("ThumbIsTouching").SetValue(false);
            }

            // GO DOWN
            if (!temp.Value && Input.GetKeyDown(KeyCode.S))
            {
                temp.SetValue(true);
                vrInputCapture.InputContainer.LeftThumbPosition.SetValue(Vector2.down);

                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.LeftClickEvents.Get("ThumbDown");
                vrInputCapture.TempEvent.Raise();

                // Touching event raise as well
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.LeftTouchEvents.Get("ThumbStartTouching");
                vrInputCapture.TempEvent.Raise();
                vrInputCapture.InputContainer.LeftTouchBoolean.Get("ThumbIsTouching").SetValue(true);
            }
            else if (temp.Value && vrInputCapture.InputContainer.LeftThumbPosition.Value.Equals(Vector2.down) && Input.GetKeyUp(KeyCode.S))
            {
                temp.SetValue(false);
                vrInputCapture.InputContainer.LeftThumbPosition.SetValue(Vector2.zero);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.LeftClickEvents.Get("ThumbUp");
                vrInputCapture.TempEvent.Raise();

                // Touching event raise as well
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.LeftTouchEvents.Get("ThumbStopTouching");
                vrInputCapture.TempEvent.Raise();
                vrInputCapture.InputContainer.LeftTouchBoolean.Get("ThumbIsTouching").SetValue(false);
            }

            //GO RIGHT
            if (!temp.Value && Input.GetKeyDown(KeyCode.D))
            {
                temp.SetValue(true);
                vrInputCapture.InputContainer.LeftThumbPosition.SetValue(Vector2.right);

                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.LeftClickEvents.Get("ThumbDown");
                vrInputCapture.TempEvent.Raise();

                // Touching event raise as well
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.LeftTouchEvents.Get("ThumbStartTouching");
                vrInputCapture.TempEvent.Raise();
                vrInputCapture.InputContainer.LeftTouchBoolean.Get("ThumbIsTouching").SetValue(true);
            }
            else if (temp.Value && vrInputCapture.InputContainer.LeftThumbPosition.Value.Equals(Vector2.right) && Input.GetKeyUp(KeyCode.D))
            {
                temp.SetValue(false);
                vrInputCapture.InputContainer.LeftThumbPosition.SetValue(Vector2.zero);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.LeftClickEvents.Get("ThumbUp");
                vrInputCapture.TempEvent.Raise();

                // Touching event raise as well
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.LeftTouchEvents.Get("ThumbStopTouching");
                vrInputCapture.TempEvent.Raise();
                vrInputCapture.InputContainer.LeftTouchBoolean.Get("ThumbIsTouching").SetValue(false);
            }

            //GO LEFT
            if (!temp.Value && Input.GetKeyDown(KeyCode.A))
            {
                temp.SetValue(true);
                vrInputCapture.InputContainer.LeftThumbPosition.SetValue(Vector2.left);

                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.LeftClickEvents.Get("ThumbDown");
                vrInputCapture.TempEvent.Raise();

                // Touching event raise as well
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.LeftTouchEvents.Get("ThumbStartTouching");
                vrInputCapture.TempEvent.Raise();
                vrInputCapture.InputContainer.LeftTouchBoolean.Get("ThumbIsTouching").SetValue(true);
            }
            else if (temp.Value && vrInputCapture.InputContainer.LeftThumbPosition.Value.Equals(Vector2.left) && Input.GetKeyUp(KeyCode.A))
            {
                temp.SetValue(false);
                vrInputCapture.InputContainer.LeftThumbPosition.SetValue(Vector2.zero);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.LeftClickEvents.Get("ThumbUp");
                vrInputCapture.TempEvent.Raise();

                // Touching event raise as well
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.LeftTouchEvents.Get("ThumbStopTouching");
                vrInputCapture.TempEvent.Raise();
                vrInputCapture.InputContainer.LeftTouchBoolean.Get("ThumbIsTouching").SetValue(false);
            }
            #endregion THUMB
        

            //Left Shift
            #region GRIP
            temp = vrInputCapture.InputContainer.LeftClickBoolean.Get("GripIsDown");

            if (!temp.Value && Input.GetKeyDown(KeyCode.LeftShift))
            {
                temp.SetValue(true);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.LeftClickEvents.Get("GripDown");
                vrInputCapture.TempEvent.Raise();
            }
            else if (temp.Value && Input.GetKeyUp(KeyCode.LeftShift))
            {
                temp.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.LeftClickEvents.Get("GripUp");
                vrInputCapture.TempEvent.Raise();
            }
            #endregion GRIP


            //Left Control
            #region MENU
            temp = vrInputCapture.InputContainer.LeftClickBoolean.Get("MenuIsDown");

            if (!temp.Value && Input.GetKeyDown(KeyCode.LeftControl))
            {
                temp.SetValue(true);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.LeftClickEvents.Get("MenuDown");
                vrInputCapture.TempEvent.Raise();
            }
            else if (temp.Value && Input.GetKeyUp(KeyCode.LeftControl))
            {
                temp.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.LeftClickEvents.Get("MenuUp");
                vrInputCapture.TempEvent.Raise();
            }
            #endregion MENU


            #region OCULUS_PARTICULARITIES
            
            //F
            #region X BUTTON
            temp = vrInputCapture.InputContainer.LeftClickBoolean.Get("XButtonIsDown");

            if (!temp.Value && Input.GetKeyDown(KeyCode.F))
            {
                temp.SetValue(true);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.LeftClickEvents.Get("XButtonDown");
                vrInputCapture.TempEvent.Raise();
            }
            else if (temp.Value && Input.GetKeyUp(KeyCode.F))
            {
                temp.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.LeftClickEvents.Get("XButtonUp");
                vrInputCapture.TempEvent.Raise();
            }
            #endregion X BUTTON


            //R
            #region Y BUTTON
            temp = vrInputCapture.InputContainer.LeftClickBoolean.Get("YButtonIsDown");

            if (!temp.Value && Input.GetKeyDown(KeyCode.R))
            {
                temp.SetValue(true);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.LeftClickEvents.Get("YButtonDown");
                vrInputCapture.TempEvent.Raise();
            }
            else if (temp.Value && Input.GetKeyUp(KeyCode.R))
            {
                temp.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.LeftClickEvents.Get("YButtonUp");
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
            temp = vrInputCapture.InputContainer.RightClickBoolean.Get("TriggerIsDown");

            if (!temp.Value && Input.GetMouseButtonDown(1))
            {
                temp.SetValue(true);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.RightClickEvents.Get("TriggerDown");
                vrInputCapture.TempEvent.Raise();
            }
            else if (temp.Value && Input.GetMouseButtonUp(1))
            {
                temp.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.RightClickEvents.Get("TriggerUp");
                vrInputCapture.TempEvent.Raise();
            }
            #endregion TRIGGER
        

            //Up, Down, Left and Right Arrows
            #region THUMB
            temp = vrInputCapture.InputContainer.RightClickBoolean.Get("ThumbIsDown");

            //GO UP
            if (!temp.Value && Input.GetKeyDown(KeyCode.UpArrow))
            {
                vrInputCapture.InputContainer.RightThumbPosition.SetValue(Vector2.up);
                temp.SetValue(true);

                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.RightClickEvents.Get("ThumbDown");
                vrInputCapture.TempEvent.Raise();

                // Touching event raise as well
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.RightTouchEvents.Get("ThumbStartTouching");
                vrInputCapture.TempEvent.Raise();
                vrInputCapture.InputContainer.RightTouchBoolean.Get("ThumbIsTouching").SetValue(true);
            }
            else if (temp.Value && vrInputCapture.InputContainer.RightThumbPosition.Value.Equals(Vector2.up) && Input.GetKeyUp(KeyCode.UpArrow))
            {
                vrInputCapture.InputContainer.RightThumbPosition.SetValue(Vector2.zero);
                temp.SetValue(false);

                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.RightClickEvents.Get("ThumbUp");
                vrInputCapture.TempEvent.Raise();

                // Touching event raise as well
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.RightTouchEvents.Get("ThumbStopTouching");
                vrInputCapture.TempEvent.Raise();
                vrInputCapture.InputContainer.RightTouchBoolean.Get("ThumbIsTouching").SetValue(false);
            }

            //GO DOWN
            if (!temp.Value && Input.GetKeyDown(KeyCode.DownArrow))
            {
                vrInputCapture.InputContainer.RightThumbPosition.SetValue(Vector2.down);
                temp.SetValue(true);

                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.RightClickEvents.Get("ThumbDown");
                vrInputCapture.TempEvent.Raise();

                // Touching event raise as well
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.RightTouchEvents.Get("ThumbStartTouching");
                vrInputCapture.TempEvent.Raise();
                vrInputCapture.InputContainer.RightTouchBoolean.Get("ThumbIsTouching").SetValue(true);
            }
            else if (temp.Value && vrInputCapture.InputContainer.RightThumbPosition.Value.Equals(Vector2.down) && Input.GetKeyUp(KeyCode.DownArrow))
            {
                vrInputCapture.InputContainer.RightThumbPosition.SetValue(Vector2.zero);
                temp.SetValue(false);

                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.RightClickEvents.Get("ThumbUp");
                vrInputCapture.TempEvent.Raise();

                // Touching event raise as well
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.RightTouchEvents.Get("ThumbStopTouching");
                vrInputCapture.TempEvent.Raise();
                vrInputCapture.InputContainer.RightTouchBoolean.Get("ThumbIsTouching").SetValue(false);
            }

            //GO RIGHT
            if (!temp.Value && Input.GetKeyDown(KeyCode.RightArrow))
            {
                vrInputCapture.InputContainer.RightThumbPosition.SetValue(Vector2.right);
                temp.SetValue(true);

                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.RightClickEvents.Get("ThumbDown");
                vrInputCapture.TempEvent.Raise();

                // Touching event raise as well
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.RightTouchEvents.Get("ThumbStartTouching");
                vrInputCapture.TempEvent.Raise();
                vrInputCapture.InputContainer.RightTouchBoolean.Get("ThumbIsTouching").SetValue(true);
            }
            else if (temp.Value && vrInputCapture.InputContainer.RightThumbPosition.Value.Equals(Vector2.right) && Input.GetKeyUp(KeyCode.RightArrow))
            {
                vrInputCapture.InputContainer.RightThumbPosition.SetValue(Vector2.zero);
                temp.SetValue(false);

                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.RightClickEvents.Get("ThumbUp");
                vrInputCapture.TempEvent.Raise();

                // Touching event raise as well
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.RightTouchEvents.Get("ThumbStopTouching");
                vrInputCapture.TempEvent.Raise();
                vrInputCapture.InputContainer.RightTouchBoolean.Get("ThumbIsTouching").SetValue(false);
            }

            //GO LEFT
            if (!temp.Value && Input.GetKeyDown(KeyCode.LeftArrow))
            {
                vrInputCapture.InputContainer.RightThumbPosition.SetValue(Vector2.left);
                temp.SetValue(true);

                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.RightClickEvents.Get("ThumbDown");
                vrInputCapture.TempEvent.Raise();

                // Touching event raise as well
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.RightTouchEvents.Get("ThumbStartTouching");
                vrInputCapture.TempEvent.Raise();
                vrInputCapture.InputContainer.RightTouchBoolean.Get("ThumbIsTouching").SetValue(true);
            }
            else if (temp.Value && vrInputCapture.InputContainer.RightThumbPosition.Value.Equals(Vector2.left) && Input.GetKeyUp(KeyCode.LeftArrow))
            {
                vrInputCapture.InputContainer.RightThumbPosition.SetValue(Vector2.zero);
                temp.SetValue(false);

                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.RightClickEvents.Get("ThumbUp");
                vrInputCapture.TempEvent.Raise();

                // Touching event raise as well
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.RightTouchEvents.Get("ThumbStopTouching");
                vrInputCapture.TempEvent.Raise();
                vrInputCapture.InputContainer.RightTouchBoolean.Get("ThumbIsTouching").SetValue(false);
            }
            #endregion THUMB


            //Right Shift
            #region GRIP
            temp = vrInputCapture.InputContainer.RightClickBoolean.Get("GripIsDown");

            if (!temp.Value && Input.GetKeyDown(KeyCode.RightShift))
            {
                temp.SetValue(true);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.RightClickEvents.Get("GripDown");
                vrInputCapture.TempEvent.Raise();
            }
            else if (temp.Value && Input.GetKeyUp(KeyCode.RightShift))
            {
                temp.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.RightClickEvents.Get("GripUp");
                vrInputCapture.TempEvent.Raise();
            }
            #endregion GRIP


            #region VIVE_PARTICULARITY

            //Right Control
            #region MENU
            temp = vrInputCapture.InputContainer.RightClickBoolean.Get("MenuIsDown");

            if (!temp.Value && Input.GetKeyDown(KeyCode.RightControl))
            {
                temp.SetValue(true);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.RightClickEvents.Get("MenuDown");
                vrInputCapture.TempEvent.Raise();
            }
            else if (temp.Value && Input.GetKeyUp(KeyCode.RightControl))
            {
                temp.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.RightClickEvents.Get("MenuUp");
                vrInputCapture.TempEvent.Raise();
            }
            #endregion MENU

            #endregion VIVE_PARTICULARITY


            #region OCULUS_PARTICULARITIES

            //L
            #region A BUTTON
            temp = vrInputCapture.InputContainer.RightClickBoolean.Get("AButtonIsDown");

            if (!temp.Value && Input.GetKeyDown(KeyCode.L))
            {
                temp.SetValue(true);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.RightClickEvents.Get("AButtonDown");
                vrInputCapture.TempEvent.Raise();
            }
            else if (temp.Value && Input.GetKeyUp(KeyCode.L))
            {
                temp.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.RightClickEvents.Get("AButtonUp");
                vrInputCapture.TempEvent.Raise();
            }
            #endregion A BUTTON

            //O
            #region B BUTTON
            temp = vrInputCapture.InputContainer.RightClickBoolean.Get("BButtonIsDown");

            if (!temp.Value && Input.GetKeyDown(KeyCode.O))
            {
                temp.SetValue(true);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.RightClickEvents.Get("BButtonDown");
                vrInputCapture.TempEvent.Raise();
            }
            else if (temp.Value && Input.GetKeyUp(KeyCode.O))
            {
                temp.SetValue(false);
                vrInputCapture.TempEvent = (GameEvent)vrInputCapture.InputContainer.RightClickEvents.Get("BButtonUp");
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
            if (!vrInputCapture.InputContainer.GazeIsCliking.Value && 
                Input.GetKeyDown(GazeInteractionSimulator.Dictionnary[vrInputCapture.GazeParameters.GazeButtonSimulator]))
            {
                vrInputCapture.InputContainer.GazeIsCliking.SetValue(true);
                vrInputCapture.InputContainer.GazeIsTouching.SetValue(true);
                vrInputCapture.InputContainer.GazeClickDown.Raise();
                vrInputCapture.InputContainer.GazeStartTouching.Raise();
            }
            // If the gaze boolVariable is at true but the gaze button is not clicking
            else if (vrInputCapture.InputContainer.GazeIsCliking.Value && 
                Input.GetKeyUp(GazeInteractionSimulator.Dictionnary[vrInputCapture.GazeParameters.GazeButtonSimulator]))
            {
                vrInputCapture.InputContainer.GazeIsCliking.SetValue(false);
                vrInputCapture.InputContainer.GazeIsTouching.SetValue(false);
                vrInputCapture.InputContainer.GazeClickUp.Raise();
                vrInputCapture.InputContainer.GazeStopTouching.Raise();
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
            if (vrInputCapture.GazeParameters.GazeButtonSimulator == EControllersInput.NONE)
            {
                vrInputCapture.CheckGazeInteractions = false;
            }
        }
        #endregion
    }
}