using ScriptableFramework.Variables;
using Unity.Entities;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
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
            public CrossplatformInputCapture CrossplatformInput;
            public SimulatorInputCaptureComponent ControllersInputCapture;
        }


        #region ComponentSystem_Methods
        /// <summary>
        /// Called after the scene was loaded, setup the entities variables
        /// </summary>
        protected override void OnCreateManager()
        {
            OnSetupVRReady.Listeners += CheckDevice;
            base.OnCreateManager();
        }

        protected override void OnUpdate()
        {
            // If we doesn't use the controllers, we don't check for the inputs.
            if (ControllersParametersVariable.Instance.UseControllers)
            {
                foreach (var e in GetEntities<Filter>())
                {
                    if (e.CrossplatformInput.IsSetup)
                        CheckLeftControllerInput(e.CrossplatformInput);
                }
            }
        }

        protected override void OnDestroyManager()
        {
            OnSetupVRReady.Listeners -= CheckDevice;
            base.OnDestroyManager();
        }
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Handle the Left Controller input and put them in the Events
        /// </summary>
        void CheckLeftControllerInput(CrossplatformInputCapture inputCapture)
        {
            //Left Click
            #region TRIGGER
            BoolVariable tempClick = inputCapture.LeftParameters.ClickBools.Get("TriggerIsDown");
            BoolVariable tempTouch = inputCapture.LeftParameters.TouchBools.Get("TriggerIsTouching");

            if (Input.GetMouseButtonDown(0))
            {
                tempClick.SetValue(true);
                new ButtonClickEvent(EHand.LEFT, EControllersButton.TRIGGER);

                tempTouch.SetValue(true);
                new ButtonTouchEvent(EHand.LEFT, EControllersButton.TRIGGER);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                tempClick.SetValue(false);
                new ButtonUnclickEvent(EHand.LEFT, EControllersButton.TRIGGER);

                tempTouch.SetValue(false);
                new ButtonUntouchEvent(EHand.LEFT, EControllersButton.TRIGGER);
            }
            #endregion TRIGGERa


            //W, A, S and D
            #region THUMB
            tempClick = inputCapture.LeftParameters.ClickBools.Get("ThumbIsDown");
            tempTouch = inputCapture.LeftParameters.TouchBools.Get("ThumbIsTouching");
            Vector2 finalDirection = inputCapture.LeftParameters.ThumbPosition.Value;

            //GO UP
            if (Input.GetKeyDown(KeyCode.W))
            {
                finalDirection.y = 1.0f;
                tempClick.SetValue(true);
                new ButtonClickEvent(EHand.LEFT, EControllersButton.THUMBSTICK);

                // Touching event raise as well
                new ButtonTouchEvent(EHand.LEFT, EControllersButton.THUMBSTICK);
                tempTouch.SetValue(true);
            }
            else if (Input.GetKeyUp(KeyCode.W))
            {
                finalDirection.y = 0.0f;
                tempClick.SetValue(false);
                new ButtonUnclickEvent(EHand.LEFT, EControllersButton.THUMBSTICK);

                // Touching event raise as well
                new ButtonUntouchEvent(EHand.LEFT, EControllersButton.THUMBSTICK);
                tempTouch.SetValue(false);
            }

            // GO DOWN
            if (Input.GetKeyDown(KeyCode.S))
            {
                finalDirection.y = -1.0f;
                tempClick.SetValue(true);
                new ButtonClickEvent(EHand.LEFT, EControllersButton.THUMBSTICK);

                // Touching event raise as well
                new ButtonTouchEvent(EHand.LEFT, EControllersButton.THUMBSTICK);
                tempTouch.SetValue(true);
            }
            else if (Input.GetKeyUp(KeyCode.S))
            {
                finalDirection.y = 0.0f;
                tempClick.SetValue(false);
                new ButtonUnclickEvent(EHand.LEFT, EControllersButton.THUMBSTICK);

                // Touching event raise as well
                new ButtonUntouchEvent(EHand.LEFT, EControllersButton.THUMBSTICK);
                tempTouch.SetValue(false);
            }

            //GO RIGHT
            if (Input.GetKeyDown(KeyCode.D))
            {
                finalDirection.x = 1.0f;
                tempClick.SetValue(true);
                new ButtonClickEvent(EHand.LEFT, EControllersButton.THUMBSTICK);

                // Touching event raise as well
                new ButtonTouchEvent(EHand.LEFT, EControllersButton.THUMBSTICK);
                tempTouch.SetValue(true);
            }
            else if (Input.GetKeyUp(KeyCode.D))
            {
                finalDirection.x = 0.0f;
                tempClick.SetValue(false);
                new ButtonUnclickEvent(EHand.LEFT, EControllersButton.THUMBSTICK);

                // Touching event raise as well
                new ButtonUntouchEvent(EHand.LEFT, EControllersButton.THUMBSTICK);
                tempTouch.SetValue(false);
            }

            //GO LEFT
            if (Input.GetKeyDown(KeyCode.A))
            {
                finalDirection.x = -1.0f;
                tempClick.SetValue(true);
                new ButtonClickEvent(EHand.LEFT, EControllersButton.THUMBSTICK);

                // Touching event raise as well
                new ButtonTouchEvent(EHand.LEFT, EControllersButton.THUMBSTICK);
                tempTouch.SetValue(true);
            }
            else if (Input.GetKeyUp(KeyCode.A))
            {
                finalDirection.x = 0.0f;
                tempClick.SetValue(false);
                new ButtonUnclickEvent(EHand.LEFT, EControllersButton.THUMBSTICK);

                // Touching event raise as well
                new ButtonUntouchEvent(EHand.LEFT, EControllersButton.THUMBSTICK);
                tempTouch.SetValue(false);
            }

            inputCapture.LeftParameters.ThumbPosition.SetValue(finalDirection);
            #endregion THUMB


            //Left Shift
            #region GRIP
            tempClick = inputCapture.LeftParameters.ClickBools.Get("GripIsDown");

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                tempClick.SetValue(true);
                new ButtonClickEvent(EHand.LEFT, EControllersButton.GRIP);
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                tempClick.SetValue(false);
                new ButtonUnclickEvent(EHand.LEFT, EControllersButton.GRIP);
            }
            #endregion GRIP


            //Left Control
            #region MENU
            tempClick = inputCapture.LeftParameters.ClickBools.Get("MenuIsDown");

            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                tempClick.SetValue(true);
                new ButtonClickEvent(EHand.LEFT, EControllersButton.MENU);
            }
            else if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                tempClick.SetValue(false);
                new ButtonUnclickEvent(EHand.LEFT, EControllersButton.MENU);
            }
            #endregion MENU


            #region OCULUS_PARTICULARITIES
            
            //F
            #region X BUTTON
            tempClick = inputCapture.LeftParameters.ClickBools.Get("XButtonIsDown");
            tempTouch = inputCapture.LeftParameters.TouchBools.Get("XButtonIsTouching");

            if (Input.GetKeyDown(KeyCode.F))
            {
                tempClick.SetValue(true);
                new ButtonClickEvent(EHand.LEFT, EControllersButton.X_BUTTON);

                tempTouch.SetValue(true);
                new ButtonTouchEvent(EHand.LEFT, EControllersButton.X_BUTTON);
            }
            else if (Input.GetKeyUp(KeyCode.F))
            {
                tempClick.SetValue(false);
                new ButtonUnclickEvent(EHand.LEFT, EControllersButton.X_BUTTON);

                tempTouch.SetValue(false);
                new ButtonUntouchEvent(EHand.LEFT, EControllersButton.X_BUTTON);
            }
            #endregion X BUTTON


            //R
            #region Y BUTTON
            tempClick = inputCapture.LeftParameters.ClickBools.Get("YButtonIsDown");
            tempTouch = inputCapture.LeftParameters.TouchBools.Get("YButtonIsTouching");

            if (Input.GetKeyDown(KeyCode.R))
            {
                tempClick.SetValue(true);
                new ButtonClickEvent(EHand.LEFT, EControllersButton.Y_BUTTON);

                tempTouch.SetValue(true);
                new ButtonTouchEvent(EHand.LEFT, EControllersButton.Y_BUTTON);
            }
            else if (Input.GetKeyUp(KeyCode.R))
            {
                tempClick.SetValue(false);
                new ButtonUnclickEvent(EHand.LEFT, EControllersButton.Y_BUTTON);

                tempTouch.SetValue(false);
                new ButtonUntouchEvent(EHand.LEFT, EControllersButton.Y_BUTTON);
            }
            #endregion Y BUTTON

            #endregion OCULUS_PARTICULARITIES
        }

        private void CheckDevice(OnSetupVRReady info)
        {
            this.Enabled = VRSF_Components.DeviceLoaded == EDevice.SIMULATOR;
        }
        #endregion
    }
}