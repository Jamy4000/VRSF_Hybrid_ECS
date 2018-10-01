using VRSF.Inputs;
using UnityEngine;

public static class ControllerInputToVariable
{
    public static string GetClickVariableFor(EControllersInput input)
    {
        switch (input)
        {
            case (EControllersInput.TRIGGER):
                return "TriggerIsDown";

            case (EControllersInput.GRIP):
                return "GripIsDown";

            case (EControllersInput.MENU):
                return "MenuIsDown";

            case (EControllersInput.THUMBSTICK):
                return "ThumbIsDown";

            case (EControllersInput.A_BUTTON):
                return "AButtonIsDown";
            case (EControllersInput.B_BUTTON):
                return "BButtonIsDown";
            case (EControllersInput.X_BUTTON):
                return "XButtonIsDown";
            case (EControllersInput.Y_BUTTON):
                return "YButtonIsDown";

            case (EControllersInput.BACK_BUTTON):
                return "BackButtonIsDown";

            default:
                Debug.LogError("The EControllersInput provided is null.");
                return null;
        }
    }

    
    public static string GetTouchVariableFor(EControllersInput input)
    {
        switch (input)
        {
            case (EControllersInput.TRIGGER):
                return "TriggerIsTouching";

            case (EControllersInput.THUMBSTICK):
                return "ThumbIsTouching";

            case (EControllersInput.A_BUTTON):
                return "AButtonIsTouching";
            case (EControllersInput.B_BUTTON):
                return "BButtonIsTouching";
            case (EControllersInput.X_BUTTON):
                return "XButtonIsTouching";
            case (EControllersInput.Y_BUTTON):
                return "YButtonIsTouching";

            case (EControllersInput.THUMBREST):
                return "ThumbrestIsTouching";

            default:
                Debug.LogError("The EControllersInput provided is null.");
                return null;
        }
    }
}
