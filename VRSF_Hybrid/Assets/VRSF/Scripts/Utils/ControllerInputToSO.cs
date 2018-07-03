using VRSF.Inputs;
using UnityEngine;

public static class ControllerInputToSO
{
    public static string GetDownGameEventFor(EControllersInput input)
    {
        switch (input)
        {
            case (EControllersInput.LEFT_TRIGGER):
            case (EControllersInput.RIGHT_TRIGGER):
                return "TriggerDown";

            case (EControllersInput.LEFT_GRIP):
            case (EControllersInput.RIGHT_GRIP):
                return "GripDown";

            case (EControllersInput.LEFT_MENU):
            case (EControllersInput.RIGHT_MENU):
                return "MenuDown";

            case (EControllersInput.LEFT_THUMBSTICK):
            case (EControllersInput.RIGHT_THUMBSTICK):
                return "ThumbDown";

            case (EControllersInput.A_BUTTON):
                return "AButtonDown";
            case (EControllersInput.B_BUTTON):
                return "BButtonDown";
            case (EControllersInput.X_BUTTON):
                return "XButtonDown";
            case (EControllersInput.Y_BUTTON):
                return "YButtonDown";

            default:
                Debug.LogError("The EControllersInput provided is null.");
                return null;
        }
    }

    public static string GetUpGameEventFor(EControllersInput input)
    {
        switch (input)
        {
            case (EControllersInput.LEFT_TRIGGER):
            case (EControllersInput.RIGHT_TRIGGER):
                return "TriggerUp";

            case (EControllersInput.LEFT_GRIP):
            case (EControllersInput.RIGHT_GRIP):
                return "GripUp";

            case (EControllersInput.LEFT_MENU):
            case (EControllersInput.RIGHT_MENU):
                return "MenuUp";

            case (EControllersInput.LEFT_THUMBSTICK):
            case (EControllersInput.RIGHT_THUMBSTICK):
                return "ThumbUp";

            case (EControllersInput.A_BUTTON):
                return "AButtonUp";
            case (EControllersInput.B_BUTTON):
                return "BButtonUp";
            case (EControllersInput.X_BUTTON):
                return "XButtonUp";
            case (EControllersInput.Y_BUTTON):
                return "YButtonUp";

            default:
                Debug.LogError("The EControllersInput provided is null.");
                return null;
        }
    }


    public static string GetClickVariableFor(EControllersInput input)
    {
        switch (input)
        {
            case (EControllersInput.LEFT_TRIGGER):
            case (EControllersInput.RIGHT_TRIGGER):
                return "TriggerIsDown";

            case (EControllersInput.LEFT_GRIP):
            case (EControllersInput.RIGHT_GRIP):
                return "GripIsDown";

            case (EControllersInput.LEFT_MENU):
            case (EControllersInput.RIGHT_MENU):
                return "MenuIsDown";

            case (EControllersInput.LEFT_THUMBSTICK):
            case (EControllersInput.RIGHT_THUMBSTICK):
                return "ThumbIsDown";

            case (EControllersInput.A_BUTTON):
                return "AButtonIsDown";
            case (EControllersInput.B_BUTTON):
                return "BButtonIsDown";
            case (EControllersInput.X_BUTTON):
                return "XButtonIsDown";
            case (EControllersInput.Y_BUTTON):
                return "YButtonIsDown";

            default:
                Debug.LogError("The EControllersInput provided is null.");
                return null;
        }
    }


    public static string GetTouchGameEventFor(EControllersInput input)
    {
        switch (input)
        {
            case (EControllersInput.LEFT_TRIGGER):
            case (EControllersInput.RIGHT_TRIGGER):
                return "TriggerStartTouching";

            case (EControllersInput.LEFT_THUMBSTICK):
            case (EControllersInput.RIGHT_THUMBSTICK):
                return "ThumbStartTouching";

            case (EControllersInput.A_BUTTON):
                return "AButtonStartTouching";
            case (EControllersInput.B_BUTTON):
                return "BButtonStartTouching";
            case (EControllersInput.X_BUTTON):
                return "XButtonStartTouching";
            case (EControllersInput.Y_BUTTON):
                return "YButtonStartTouching";
                
            case (EControllersInput.LEFT_THUMBREST):
            case (EControllersInput.RIGHT_THUMBREST):
                return "ThumbrestStartTouching";

            default:
                Debug.LogError("The EControllersInput provided is null.");
                return null;
        }
    }


    public static string GetReleasedGameEventFor(EControllersInput input)
    {
        switch (input)
        {
            case (EControllersInput.LEFT_TRIGGER):
            case (EControllersInput.RIGHT_TRIGGER):
                return "TriggerStopTouching";

            case (EControllersInput.LEFT_THUMBSTICK):
            case (EControllersInput.RIGHT_THUMBSTICK):
                return "ThumbStopTouching";

            case (EControllersInput.A_BUTTON):
                return "AButtonStopTouching";
            case (EControllersInput.B_BUTTON):
                return "BButtonStopTouching";
            case (EControllersInput.X_BUTTON):
                return "XButtonStopTouching";
            case (EControllersInput.Y_BUTTON):
                return "YButtonStopTouching";

            case (EControllersInput.LEFT_THUMBREST):
            case (EControllersInput.RIGHT_THUMBREST):
                return "ThumbrestStopTouching";

            default:
                Debug.LogError("The EControllersInput provided is null.");
                return null;
        }
    }


    public static string GetTouchVariableFor(EControllersInput input)
    {
        switch (input)
        {
            case (EControllersInput.LEFT_TRIGGER):
            case (EControllersInput.RIGHT_TRIGGER):
                return "TriggerIsTouching";

            case (EControllersInput.LEFT_THUMBSTICK):
            case (EControllersInput.RIGHT_THUMBSTICK):
                return "ThumbIsTouching";

            case (EControllersInput.A_BUTTON):
                return "AButtonIsTouching";
            case (EControllersInput.B_BUTTON):
                return "BButtonIsTouching";
            case (EControllersInput.X_BUTTON):
                return "XButtonIsTouching";
            case (EControllersInput.Y_BUTTON):
                return "YButtonIsTouching";

            case (EControllersInput.LEFT_THUMBREST):
            case (EControllersInput.RIGHT_THUMBREST):
                return "ThumbrestIsTouching";

            default:
                Debug.LogError("The EControllersInput provided is null.");
                return null;
        }
    }
}
