using UnityEngine;
using Valve.VR;
using VRSF.Core.Controllers;

namespace VRSF.Core.Inputs
{
    public static class ViveLeftHandInputListeners
    {
        public static ViveInputParameters LeftInputParam;

        private static bool _triggerIsTouching;

        #region TRIGGER
        public static void OnLeftTriggerDown(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
        {
            LeftInputParam.ClickBools.Get("TriggerIsDown").SetValue(true);
            new ButtonClickEvent(EHand.LEFT, EControllersButton.TRIGGER);
        }

        public static void OnLeftTriggerUp(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
        {
            LeftInputParam.ClickBools.Get("TriggerIsDown").SetValue(false);
            new ButtonUnclickEvent(EHand.LEFT, EControllersButton.TRIGGER);
        }
        #endregion TRIGGER

        #region GRIP
        public static void OnLeftGripDown(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
        {
            LeftInputParam.ClickBools.Get("GripIsDown").SetValue(true);
            new ButtonClickEvent(EHand.LEFT, EControllersButton.GRIP);
        }

        public static void OnLeftGripUp(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
        {
            LeftInputParam.ClickBools.Get("GripIsDown").SetValue(false);
            new ButtonUnclickEvent(EHand.LEFT, EControllersButton.GRIP);
        }
        #endregion GRIP

        #region TOUCHPAD
        public static void OnLeftTouchpadDown(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
        {
            LeftInputParam.ClickBools.Get("ThumbIsDown").SetValue(true);
            LeftInputParam.TouchBools.Get("ThumbIsTouching").SetValue(false);
            new ButtonClickEvent(EHand.LEFT, EControllersButton.THUMBSTICK);
        }

        public static void OnLeftTouchpadUp(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
        {
            LeftInputParam.ClickBools.Get("ThumbIsDown").SetValue(false);
            new ButtonUnclickEvent(EHand.LEFT, EControllersButton.THUMBSTICK);
        }

        public static void OnLeftTouchpadTouch(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
        {
            LeftInputParam.TouchBools.Get("ThumbIsTouching").SetValue(true);
            new ButtonTouchEvent(EHand.LEFT, EControllersButton.THUMBSTICK);
        }

        public static void OnLeftTouchpadUntouch(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
        {
            LeftInputParam.TouchBools.Get("ThumbIsTouching").SetValue(false);
            new ButtonUntouchEvent(EHand.LEFT, EControllersButton.THUMBSTICK);
        }

        public static void OnLeftTouchpadAxisChanged(SteamVR_Action_Vector2 action, SteamVR_Input_Sources source, Vector2 newAxis, Vector2 newDelta)
        {
            LeftInputParam.ThumbPosition.SetValue(newAxis);
        }
        #endregion TOUCHPAD

        #region MENU
        public static void OnLeftMenuDown(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
        {
            LeftInputParam.ClickBools.Get("GripIsDown").SetValue(true);
            new ButtonClickEvent(EHand.LEFT, EControllersButton.GRIP);
        }

        public static void OnLeftMenuUp(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
        {
            LeftInputParam.ClickBools.Get("GripIsDown").SetValue(false);
            new ButtonUnclickEvent(EHand.LEFT, EControllersButton.GRIP);
        }
        #endregion MENU
    }
}
