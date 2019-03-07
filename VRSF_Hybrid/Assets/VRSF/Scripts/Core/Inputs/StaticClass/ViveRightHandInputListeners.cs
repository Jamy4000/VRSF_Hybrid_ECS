using UnityEngine;
using Valve.VR;
using VRSF.Core.Controllers;

namespace VRSF.Core.Inputs
{
    public static class ViveRightHandInputListeners
    {
        public static ViveInputParameters RightInputParam;

        #region TRIGGER
        public static void OnRightTriggerDown(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
        {
            RightInputParam.ClickBools.Get("TriggerIsDown").SetValue(true);
            new ButtonClickEvent(EHand.RIGHT, EControllersButton.TRIGGER);
        }

        public static void OnRightTriggerUp(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
        {
            RightInputParam.ClickBools.Get("TriggerIsDown").SetValue(false);
            new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.TRIGGER);
        }
        #endregion TRIGGER

        #region GRIP
        public static void OnRightGripDown(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
        {
            RightInputParam.ClickBools.Get("GripIsDown").SetValue(true);
            new ButtonClickEvent(EHand.RIGHT, EControllersButton.GRIP);
        }

        public static void OnRightGripUp(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
        {
            RightInputParam.ClickBools.Get("GripIsDown").SetValue(false);
            new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.GRIP);
        }
        #endregion GRIP

        #region TOUCHPAD
        public static void OnRightTouchpadDown(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
        {
            RightInputParam.ClickBools.Get("ThumbIsDown").SetValue(true);
            RightInputParam.TouchBools.Get("ThumbIsTouching").SetValue(false);
            new ButtonClickEvent(EHand.RIGHT, EControllersButton.THUMBSTICK);
        }

        public static void OnRightTouchpadUp(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
        {
            RightInputParam.ClickBools.Get("ThumbIsDown").SetValue(false);
            new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.THUMBSTICK);
        }

        public static void OnRightTouchpadTouch(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
        {
            RightInputParam.TouchBools.Get("ThumbIsTouching").SetValue(true);
            new ButtonTouchEvent(EHand.RIGHT, EControllersButton.THUMBSTICK);
        }

        public static void OnRightTouchpadUntouch(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
        {
            RightInputParam.TouchBools.Get("ThumbIsTouching").SetValue(false);
            new ButtonUntouchEvent(EHand.RIGHT, EControllersButton.THUMBSTICK);
        }

        public static void OnRightTouchpadAxisChanged(SteamVR_Action_Vector2 action, SteamVR_Input_Sources source, Vector2 newAxis, Vector2 newDelta)
        {
            RightInputParam.ThumbPosition.SetValue(newAxis);
        }
        #endregion TOUCHPAD

        #region MENU
        public static void OnRightMenuDown(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
        {
            RightInputParam.ClickBools.Get("GripIsDown").SetValue(true);
            new ButtonClickEvent(EHand.RIGHT, EControllersButton.GRIP);
        }

        public static void OnRightMenuUp(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
        {
            RightInputParam.ClickBools.Get("GripIsDown").SetValue(false);
            new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.GRIP);
        }
        #endregion MENU
    }
}
