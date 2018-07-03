namespace VRSF.Inputs
{
    /// <summary>
    /// Contain all possible click type for Oculus and Vive controllers
    /// If you're using the two SDKs, make sure the button are corresponding.
    /// </summary>
    public enum EControllersInput
    {
        NONE = 0,
        LEFT_TRIGGER = 1 << 0,
        RIGHT_TRIGGER = 1 << 1,
        LEFT_GRIP = 1 << 2,
        RIGHT_GRIP = 1 << 3,
        LEFT_THUMBSTICK = 1 << 4,
        RIGHT_THUMBSTICK = 1 << 5,
        LEFT_MENU = 1 << 6,

        // VIVE PARTICULARITY
        RIGHT_MENU = 1 << 7,

        // OCULUS PARTICULARITIES
        A_BUTTON = 1 << 8,
        B_BUTTON = 1 << 9,
        X_BUTTON = 1 << 10,
        Y_BUTTON = 1 << 11,
        LEFT_THUMBREST = 1 << 12,
        RIGHT_THUMBREST = 1 << 13,

        // SIMULATOR PARTICULARITY
        WHEEL_BUTTON = 1 << 14
    }

}
