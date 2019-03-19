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
    public class SimulatorRightControllersInputCaptureSystem : ComponentSystem
    {
        /// <summary>
        /// The filter for the entity component.
        /// </summary>
        struct Filter
        {
            public CrossplatformInputCapture CrossplatformInput;
            public SimulatorInputCaptureComponent ControllersInputCapture;
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
        protected override void OnCreateManager()
        {
            OnSetupVRReady.Listeners += CheckDevice;
            _inputContainer = InputVariableContainer.Instance;
            base.OnCreateManager();
        }

        protected override void OnUpdate()
        {
            // If we doesn't use the controllers, we don't check for the inputs.
            if (ControllersParametersVariable.Instance.UseControllers)
            {
                foreach (var entity in GetEntities<Filter>())
                {
                    CheckRightControllerInput(entity.CrossplatformInput);
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
        /// Handle the Right Controller input and put them in the Events
        /// </summary>
        void CheckRightControllerInput(CrossplatformInputCapture inputCapture)
        {
            //Right Click
            #region TRIGGER
            BoolVariable tempClick = inputCapture.RightParameters.ClickBools.Get("TriggerIsDown");
            BoolVariable tempTouch = inputCapture.RightParameters.TouchBools.Get("TriggerIsTouching");

            if (Input.GetMouseButtonDown(1))
            {
                tempClick.SetValue(true);
                new ButtonClickEvent(EHand.RIGHT, EControllersButton.TRIGGER);

                tempTouch.SetValue(true);
                new ButtonTouchEvent(EHand.RIGHT, EControllersButton.TRIGGER);
            }
            else if (Input.GetMouseButtonUp(1))
            {
                tempClick.SetValue(false);
                new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.TRIGGER);

                tempTouch.SetValue(false);
                new ButtonUntouchEvent(EHand.RIGHT, EControllersButton.TRIGGER);
            }
            #endregion TRIGGER


            //Up, Down, Left and Right Arrows
            #region THUMB
            tempClick = inputCapture.RightParameters.ClickBools.Get("ThumbIsDown");
            tempTouch = inputCapture.RightParameters.TouchBools.Get("ThumbIsTouching");
            Vector2 finalDirection = inputCapture.RightParameters.ThumbPosition.Value;

            //GO UP
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                finalDirection.y = 1.0f;
                tempClick.SetValue(true);
                new ButtonClickEvent(EHand.RIGHT, EControllersButton.THUMBSTICK);

                // Touching event raise as well
                new ButtonTouchEvent(EHand.RIGHT, EControllersButton.THUMBSTICK);
                tempTouch.SetValue(true);
            }
            else if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                finalDirection.y = 0.0f;
                tempClick.SetValue(false);
                new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.THUMBSTICK);

                // Touching event raise as well
                new ButtonUntouchEvent(EHand.RIGHT, EControllersButton.THUMBSTICK);
                tempTouch.SetValue(false);
            }

            // GO DOWN
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                finalDirection.y = -1.0f;
                tempClick.SetValue(true);
                new ButtonClickEvent(EHand.RIGHT, EControllersButton.THUMBSTICK);

                // Touching event raise as well
                new ButtonTouchEvent(EHand.RIGHT, EControllersButton.THUMBSTICK);
                tempTouch.SetValue(true);
            }
            else if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                finalDirection.y = 0.0f;
                tempClick.SetValue(false);
                new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.THUMBSTICK);

                // Touching event raise as well
                new ButtonUntouchEvent(EHand.RIGHT, EControllersButton.THUMBSTICK);
                tempTouch.SetValue(false);
            }

            //GO RIGHT
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                finalDirection.x = 1.0f;
                tempClick.SetValue(true);
                new ButtonClickEvent(EHand.RIGHT, EControllersButton.THUMBSTICK);

                // Touching event raise as well
                new ButtonTouchEvent(EHand.RIGHT, EControllersButton.THUMBSTICK);
                tempTouch.SetValue(true);
            }
            else if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                finalDirection.x = 0.0f;
                tempClick.SetValue(false);
                new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.THUMBSTICK);

                // Touching event raise as well
                new ButtonUntouchEvent(EHand.RIGHT, EControllersButton.THUMBSTICK);
                tempTouch.SetValue(false);
            }

            //GO Right
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                finalDirection.x = -1.0f;
                tempClick.SetValue(true);
                new ButtonClickEvent(EHand.RIGHT, EControllersButton.THUMBSTICK);

                // Touching event raise as well
                new ButtonTouchEvent(EHand.RIGHT, EControllersButton.THUMBSTICK);
                tempTouch.SetValue(true);
            }
            else if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                finalDirection.x = 0.0f;
                tempClick.SetValue(false);
                new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.THUMBSTICK);

                // Touching event raise as well
                new ButtonUntouchEvent(EHand.RIGHT, EControllersButton.THUMBSTICK);
                tempTouch.SetValue(false);
            }

            inputCapture.RightParameters.ThumbPosition.SetValue(finalDirection);
            #endregion THUMB


            //Right Shift
            #region GRIP
            tempClick = inputCapture.RightParameters.ClickBools.Get("GripIsDown");

            if (Input.GetKeyDown(KeyCode.RightShift))
            {
                tempClick.SetValue(true);
                new ButtonClickEvent(EHand.RIGHT, EControllersButton.GRIP);
            }
            else if (Input.GetKeyUp(KeyCode.RightShift))
            {
                tempClick.SetValue(false);
                new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.GRIP);
            }
            #endregion GRIP


            #region VIVE_PARTICULARITY
            
            //Right Control
            #region MENU
            tempClick = inputCapture.RightParameters.ClickBools.Get("MenuIsDown");

            if (Input.GetKeyDown(KeyCode.RightControl))
            {
                tempClick.SetValue(true);
                new ButtonClickEvent(EHand.RIGHT, EControllersButton.MENU);
            }
            else if (Input.GetKeyUp(KeyCode.RightControl))
            {
                tempClick.SetValue(false);
                new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.MENU);
            }
            #endregion MENU

            #endregion VIVE_PARTICULARITY

            #region OCULUS_PARTICULARITIES

            //L
            #region A BUTTON
            tempClick = inputCapture.RightParameters.ClickBools.Get("AButtonIsDown");
            tempTouch = inputCapture.RightParameters.TouchBools.Get("AButtonIsTouching");

            if (Input.GetKeyDown(KeyCode.L))
            {
                tempClick.SetValue(true);
                new ButtonClickEvent(EHand.RIGHT, EControllersButton.A_BUTTON);

                tempTouch.SetValue(true);
                new ButtonTouchEvent(EHand.RIGHT, EControllersButton.A_BUTTON);
            }
            else if (Input.GetKeyUp(KeyCode.L))
            {
                tempClick.SetValue(false);
                new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.A_BUTTON);

                tempTouch.SetValue(false);
                new ButtonUntouchEvent(EHand.RIGHT, EControllersButton.A_BUTTON);
            }
            #endregion A BUTTON


            //O
            #region B BUTTON
            tempClick = inputCapture.RightParameters.ClickBools.Get("BButtonIsDown");
            tempTouch = inputCapture.RightParameters.TouchBools.Get("BButtonIsTouching");

            if (Input.GetKeyDown(KeyCode.O))
            {
                tempClick.SetValue(true);
                new ButtonClickEvent(EHand.RIGHT, EControllersButton.B_BUTTON);

                tempTouch.SetValue(true);
                new ButtonTouchEvent(EHand.RIGHT, EControllersButton.B_BUTTON);
            }
            else if (Input.GetKeyUp(KeyCode.O))
            {
                tempClick.SetValue(false);
                new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.B_BUTTON);

                tempTouch.SetValue(false);
                new ButtonUntouchEvent(EHand.RIGHT, EControllersButton.B_BUTTON);
            }
            #endregion B BUTTON

            #endregion OCULUS_PARTICULARITIES
        }

        private void CheckDevice(OnSetupVRReady info)
        {
            this.Enabled = VRSF_Components.DeviceLoaded == EDevice.SIMULATOR;
        }
        #endregion
    }
}