namespace VRSF.Inputs
{
    /// <summary>
    /// The type of movement the user is doing on the button. Down : is interacting or start interacting, Up : Stop Interacting
    /// </summary>
    public enum EFingerMovement
    {
        NONE = 0,
        DOWN = 1 << 0,
        UP = 1 << 1
    }
}