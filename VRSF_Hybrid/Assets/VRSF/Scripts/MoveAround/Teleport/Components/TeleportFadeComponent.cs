using UnityEngine;

namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// Component handling the fading effects. Need to be placed a canvas with an Image in front of the user's eyes
    /// </summary>
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity), typeof(UnityEngine.UI.Image))]
    public class TeleportFadeComponent : MonoBehaviour
    {
        /// How long, in seconds, the fade-in/fade-out animation should take
        [Tooltip("Duration of the \"blink\" animation (fading in and out upon teleport) in seconds.")]
        public int TeleportFadeSpeed = 1;

        [HideInInspector] public UnityEngine.UI.Image _FadingImage;

        [HideInInspector] public bool _IsFadingIn;
    }
} 