using UnityEngine;
namespace VRSF.MoveAround.Teleport.Components
{
    /// <summary>
    /// Contains all variable that can be set by the user in the editor.
    /// </summary>
    [RequireComponent(typeof(TeleportGeneralComponent))]
    public class BezierTeleportParametersComponent : MonoBehaviour
    {
        [Header("Bezier Curve Parameters")]
        
        [Tooltip("The value corresponding to the Arc take off angle.")]
        public float BezierAngle = 45f;
        [Tooltip("The value corresponding to the overall arc length.\n" +
            "Increasing this value will increase overall arc length")]
        public float BezierStrength = 10f;

        [Tooltip("The color of the bézier ray when it's not hitting anything.")]
        public Color ColorBezierNotHitting = Color.red;
        [Tooltip("The color of the bézier ray when it's hitting something.")]
        public Color ColorBezierHitting = Color.green;


        [Header("Teleport Target")]
        [Tooltip("Specify the GameObject corresponding to the Target of the Bezier Curve.\n" +
            "It will be used to display the ground position.")]
        public GameObject TargetMarker;
    }
}