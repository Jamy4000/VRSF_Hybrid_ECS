using ScriptableFramework.Variables;
using Unity.Entities;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.Inputs;
using VRSF.Core.SetupVR;

public class WMRControllersInputCaptureSystem : ComponentSystem
{
    private struct Filter
    {
        public WMRControllersInputCaptureComponent WMRControllersInput;
    }

    protected override void OnCreateManager()
    {
        OnSetupVRReady.Listeners += CheckDevice;
        base.OnCreateManager();
    }

    protected override void OnStartRunning()
    {
        foreach (var e in GetEntities<Filter>())
        {
            // We give the references to the Scriptable variable containers in the Left Parameters variable
            e.WMRControllersInput.LeftParameters = new InputParameters
            {
                ClickBools = InputVariableContainer.Instance.LeftClickBoolean,
                TouchBools = InputVariableContainer.Instance.LeftTouchBoolean,
                ThumbPosition = InputVariableContainer.Instance.LeftThumbPosition
            };

            // We give the references to the Scriptable variable containers in the Right Parameters variable
            e.WMRControllersInput.RightParameters = new InputParameters
            {
                ClickBools = InputVariableContainer.Instance.RightClickBoolean,
                TouchBools = InputVariableContainer.Instance.RightTouchBoolean,
                ThumbPosition = InputVariableContainer.Instance.RightThumbPosition
            };

            e.WMRControllersInput.IsSetup = true;
        }
        base.OnStartRunning();
    }

    protected override void OnUpdate()
    {
        // If we doesn't use the controllers, we don't check for the inputs.
        if (ControllersParametersVariable.Instance.UseControllers )
        {
            foreach (var e in GetEntities<Filter>())
            {
                if (e.WMRControllersInput.IsSetup)
                {
                    // We check the Input for the Right controller
                    CheckRightControllerInput(e.WMRControllersInput);

                    // We check the Input for the Left controller
                    CheckLeftControllerInput(e.WMRControllersInput);
                }
            }
        }
    }
    
    protected override void OnDestroyManager()
    {
        OnSetupVRReady.Listeners -= CheckDevice;
        base.OnDestroyManager();
    }

    /// <summary>
    /// Handle the Right Controller input and put them in the Events
    /// </summary>
    private void CheckRightControllerInput(WMRControllersInputCaptureComponent inputCapture)
    {

        BoolVariable menuClick = inputCapture.RightParameters.ClickBools.Get("WMRRightMenu");
        BoolVariable thumbClick = inputCapture.RightParameters.ClickBools.Get("ThumbIsDown");
        BoolVariable triggerClick = inputCapture.RightParameters.ClickBools.Get("TriggerIsDown");
        BoolVariable gripClick = inputCapture.RightParameters.ClickBools.Get("GripIsDown");
        
        BoolVariable thumbTouch = inputCapture.RightParameters.TouchBools.Get("ThumbIsTouching");

        inputCapture.RightParameters.ThumbPosition.SetValue(new Vector2(Input.GetAxis("WMRTouchpadHorizontalRight"), Input.GetAxis("WMRTouchpadVerticalRight")));

        // Trigger right
        if (Input.GetButtonDown("WMRTriggerClickRight"))
        {
            triggerClick.SetValue(true);
            new ButtonClickEvent(EHand.RIGHT, EControllersButton.TRIGGER);
            Debug.Log("WMRTriggerClickRight");
        }
        else if(Input.GetButtonUp("WMRTriggerClickRight"))
        {
            triggerClick.SetValue(false);
            new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.TRIGGER);
        }

        // Grip right
        if (Input.GetButtonDown("WMRGripClickRight"))
        {
            gripClick.SetValue(true);
            new ButtonClickEvent(EHand.RIGHT, EControllersButton.GRIP);
            Debug.Log("WMRGripClickRight");
        }
        else if (Input.GetButtonUp("WMRGripClickRight"))
        {
            gripClick.SetValue(false);
            new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.GRIP);
        }

        // Menu right
        if (Input.GetButtonDown("WMRMenuRight"))
        {
            menuClick.SetValue(true);
            new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.MENU);
            Debug.Log("WMRMenuRight");
        }
        else if (Input.GetButtonUp("WMRGripClickRight"))
        {
            menuClick.SetValue(false);
            new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.MENU);
        }

        // Touchpad
        if (Input.GetButtonDown("WMRTouchpadClickRight"))
        {
            thumbClick.SetValue(true);
            thumbTouch.SetValue(false);
            new ButtonClickEvent(EHand.RIGHT, EControllersButton.THUMBREST);
            Debug.Log("WMRTouchpadClickRight");
        }
        else if (Input.GetButtonUp("WMRTouchpadClickRight"))
        {
            thumbClick.SetValue(false);
            new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.THUMBREST);
        }
        // Check Touch Events if user is not clicking
        else if (!thumbClick.Value && Input.GetButtonDown("WMRTouchpadTouchRight"))
        {
            thumbTouch.SetValue(true);
            new ButtonTouchEvent(EHand.RIGHT, EControllersButton.THUMBREST);
            Debug.Log("WMRthumbrestClickRight");
        }
        else if (Input.GetButtonUp("WMRTouchpadTouchRight"))
        {
            thumbTouch.SetValue(false);
            new ButtonUntouchEvent(EHand.RIGHT, EControllersButton.THUMBREST);
        }
        
        inputCapture.RightParameters.ThumbPosition.SetValue(new Vector2(Input.GetAxis("WMRThumbstickHorizontalRight"), Input.GetAxis("WMRThumbstickVerticalRight")));
        var touchpadTouch = inputCapture.RightParameters.TouchBools.Get("ThumbrestIsTouching");
        var touchpadthumClick = inputCapture.RightParameters.ClickBools.Get("ThumbIsDown");

        if (!thumbTouch.Value && Input.GetButton("WMRThumbstickClickRight"))
        {
            thumbTouch.SetValue(true);
            new ButtonTouchEvent(EHand.RIGHT, EControllersButton.THUMBREST);
            Debug.Log("WMRThumbstickClickRight");
        }
        else if (Input.GetButtonUp("WMRThumbstickClickRight"))
        {
            thumbTouch.SetValue(false);
            new ButtonUntouchEvent(EHand.RIGHT, EControllersButton.THUMBREST);
        }

        if (Input.GetButtonDown("WMRThumbstickClickRight"))
        {
            thumbClick.SetValue(true);
            new ButtonClickEvent(EHand.RIGHT, EControllersButton.THUMBREST);
            Debug.Log("WMRThumbstickClickRight");
        }
        else if (Input.GetButtonUp("WMRThumbstickClickRight"))
        {
            thumbClick.SetValue(false);
            new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.THUMBREST);
        }
    }

    /// <summary>
    /// Handle the Left Controller input and put them in the Events
    /// </summary>
    private void CheckLeftControllerInput(WMRControllersInputCaptureComponent inputCapture)
    {
        BoolVariable menuClick = inputCapture.LeftParameters.ClickBools.Get("WMRLeftMenu");
        BoolVariable thumbClick = inputCapture.LeftParameters.ClickBools.Get("ThumbIsDown");
        BoolVariable triggerClick = inputCapture.LeftParameters.ClickBools.Get("TriggerIsDown");
        BoolVariable gripClick = inputCapture.LeftParameters.ClickBools.Get("GripIsDown");

        BoolVariable thumbTouch = inputCapture.LeftParameters.TouchBools.Get("ThumbIsTouching");

        inputCapture.LeftParameters.ThumbPosition.SetValue(new Vector2(Input.GetAxis("WMRTouchpadHorizontalLeft"), Input.GetAxis("WMRTouchpadVerticalLeft")));

        // Trigger Left
        if (Input.GetButtonDown("WMRTriggerClickLeft"))
        {
            triggerClick.SetValue(true);
            new ButtonClickEvent(EHand.LEFT, EControllersButton.TRIGGER);
            Debug.Log("WMRTriggerClickLeft");
        }
        else if (Input.GetButtonUp("WMRTriggerClickLeft"))
        {
            triggerClick.SetValue(false);
            new ButtonUnclickEvent(EHand.LEFT, EControllersButton.TRIGGER);
        }

        // Grip Left
        if (Input.GetButtonDown("WMRGripClickLeft"))
        {
            gripClick.SetValue(true);
            new ButtonClickEvent(EHand.LEFT, EControllersButton.GRIP);
            Debug.Log("WMRGripClickLeft");
        }
        else if (Input.GetButtonUp("WMRGripClickLeft"))
        {
            gripClick.SetValue(false);
            new ButtonUnclickEvent(EHand.LEFT, EControllersButton.GRIP);
        }

        // Menu Left
        if (Input.GetButtonDown("WMRMenuLeft"))
        {
            menuClick.SetValue(true);
            new ButtonUnclickEvent(EHand.LEFT, EControllersButton.MENU);
            Debug.Log("WMRMenuLeft");
        }
        else if (Input.GetButtonUp("WMRGripClickLeft"))
        {
            menuClick.SetValue(false);
            new ButtonUnclickEvent(EHand.LEFT, EControllersButton.MENU);
        }

        // Touchpad
        if (Input.GetButtonDown("WMRTouchpadClickLeft"))
        {
            thumbClick.SetValue(true);
            thumbTouch.SetValue(false);
            new ButtonClickEvent(EHand.LEFT, EControllersButton.THUMBREST);
            Debug.Log("WMRTouchpadClickLeft");
        }
        else if (Input.GetButtonUp("WMRTouchpadClickLeft"))
        {
            thumbClick.SetValue(false);
            new ButtonUnclickEvent(EHand.LEFT, EControllersButton.THUMBREST);
        }
        // Check Touch Events if user is not clicking
        else if (!thumbClick.Value && Input.GetButtonDown("WMRTouchpadTouchLeft"))
        {
            thumbTouch.SetValue(true);
            new ButtonTouchEvent(EHand.LEFT, EControllersButton.THUMBREST);
            Debug.Log("WMRTouchpadTouchLeft");
        }
        else if (Input.GetButtonUp("WMRTouchpadTouchLeft"))
        {
            thumbTouch.SetValue(false);
            new ButtonUntouchEvent(EHand.LEFT, EControllersButton.THUMBREST);
        }

        inputCapture.LeftParameters.ThumbPosition.SetValue(new Vector2(Input.GetAxis("WMRThumbstickHorizontalLeft"), Input.GetAxis("WMRThumbstickVerticalLeft")));
        var touchpadTouch = inputCapture.LeftParameters.TouchBools.Get("ThumbrestIsTouching");
        var touchpadthumClick = inputCapture.LeftParameters.ClickBools.Get("ThumbIsDown");

        if (!touchpadTouch.Value && Input.GetButton("WMRThumbstickClickLeft"))
        {
            touchpadTouch.SetValue(true);
            new ButtonTouchEvent(EHand.LEFT, EControllersButton.THUMBREST);
            Debug.Log("WMRThumbstickTouchLeft");
        }
        else if (Input.GetButtonUp("WMRThumbstickClickLeft"))
        {
            touchpadTouch.SetValue(false);
            new ButtonUntouchEvent(EHand.LEFT, EControllersButton.THUMBREST);
        }
        if (Input.GetButtonDown("WMRThumbstickClickLeft"))
        {
            touchpadthumClick.SetValue(true);
            new ButtonClickEvent(EHand.LEFT, EControllersButton.THUMBREST);

            Debug.Log("WMRThumbstickClickLeft");
        }
        else if (Input.GetButtonUp("WMRThumbstickClickLeft"))
        {
            touchpadthumClick.SetValue(false);
            new ButtonUnclickEvent(EHand.LEFT, EControllersButton.THUMBREST);
        }
    }

    private void CheckDevice(OnSetupVRReady info)
    {
        this.Enabled = VRSF_Components.DeviceLoaded == EDevice.WMR;
    }
}