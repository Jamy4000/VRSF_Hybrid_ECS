namespace VRSF.Controllers
{
    /// <summary>
    /// List the possible "Hand" with which the user can click or over on things.
    /// There's three of them, the left and right hand and the gaze, plus a null property
    /// </summary>
	public enum EHand
    {
        NONE = 0,
        LEFT = 1 << 0,
        RIGHT = 1 << 1,
        GAZE = 1 << 2
    }
}