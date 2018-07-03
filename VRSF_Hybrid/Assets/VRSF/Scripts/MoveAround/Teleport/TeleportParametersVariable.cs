using ScriptableFramework.Util;
using UnityEngine;

namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// Contain all the parameters for the Teleport Script
    /// </summary>
    public class TeleportParametersVariable : ScriptableSingleton<TeleportParametersVariable>
    {
        [Multiline(10)]
        public string DeveloperDescription = "";
        
        [Header("Teleport Far Away Parameters")]
        [Tooltip("The loading time for the TeleportFarAway feature.")]
        public float TimerBeforeTeleport = 1.0f;
        [Tooltip("If you want to adjust the height to the point that was hit.")]
        public bool AdjustHeight = true;
        [Tooltip("The height at which the user is teleported above the ground.")]
        public float HeightAboveGround = 1.8f;



        [Header("Teleport Step by Step Parameters")]
        [Tooltip("The distance to move the Camera (in Meters) for the step by step feature.")]
        public float DistanceStepByStep = 1.0f;
        [Tooltip("If you want to move on the vertical axis with the step by step feature.")]
        public bool MoveOnVerticalAxis = false;



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



        [Header("Teleport Boundaries Parameters")]
        [Tooltip("Wheter you wanna use boundaries for the Teleporting mode or not.")]
        public bool UseBoundaries = false;
        [Tooltip("The minimun local position at which the user can go.")]
        public Vector3 MinAvatarPosition = new Vector3(-100.0f, -1.0f, -100.0f);
        [Tooltip("The maximum local position at which the user can go.")]
        public Vector3 MaxAvatarPosition = new Vector3(100.0f, 100.0f, 100.0f);
        [Tooltip("The color of the Teleporting boundaries.")]
        public Color TeleportingBoundariesColor = Color.blue;


        public void ResetParameters()
        {
            TimerBeforeTeleport = 1.0f;
            AdjustHeight = true;
            HeightAboveGround = 1.8f;
            
            DistanceStepByStep = 1.0f;
            MoveOnVerticalAxis = true;

            BezierAngle = 45f;
            BezierStrength = 10f;
            ColorBezierNotHitting = Color.red;
            ColorBezierHitting = Color.green;

            UseBoundaries = false;
            MinAvatarPosition = new Vector3(-100.0f, -1.0f, -100.0f);
            MaxAvatarPosition = new Vector3(100.0f, 100.0f, 100.0f);
            TeleportingBoundariesColor = Color.blue;
        }
    }
}