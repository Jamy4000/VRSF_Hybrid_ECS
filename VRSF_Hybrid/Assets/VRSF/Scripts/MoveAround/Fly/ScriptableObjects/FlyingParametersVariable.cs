using ScriptableFramework.Util;
using UnityEngine;

namespace VRSF.MoveAround
{
    /// <summary>
    /// Contain all the parameters for the Flying Mode
    /// </summary>
    public class FlyingParametersVariable : ScriptableSingleton<FlyingParametersVariable>
    {
        #region PUBLIC_VARIABLES

        [Multiline(10)]
        public string DeveloperDescription = "";

        [Tooltip("The General speed factor.")]
        public float FlyingSpeed = 1.0f;

        [Tooltip("Whether the speed of the user should be faster if he has a higher position (Y axis).")]
        public bool ChangeSpeedDependingOnHeight = true;

        [Tooltip("Whether the speed of the user should be faster if he has a higher scale.")]
        public bool ChangeSpeedDependingOnScale = true;

        [Tooltip("Wheter you wanna use boundaries for the flying mode or not.")]
        public bool UseBoundaries = false;

        [Tooltip("The minimun local position at which the user can go.")]
        public float MinAvatarYPosition = 0.0f;

        [Tooltip("The maximum local position at which the user can go.")]
        public float MaxAvatarYPosition = 2000.0f;

        [Tooltip("The minimun local position at which the user can go.")]
        public Vector3 MinAvatarPosition = new Vector3(-10.0f, -1.0f, -10.0f);

        [Tooltip("The maximum local position at which the user can go.")]
        public Vector3 MaxAvatarPosition = new Vector3(10.0f, 10.0f, 10.0f);

        [Tooltip("The color of the flying boundaries.")]
        public Color FlyingBoundariesColor = Color.green;

        [Tooltip("If you want to have an acceleration and deceleration effect when starting and stopping to fly.")]
        public bool AccelerationDecelerationEffect = true;

        [Tooltip("The factor for the acceleration effect. Set to 0 to remove this effect.")]
        public float AccelerationEffectFactor = 1.0f;

        [Tooltip("The factor for the deceleration effect. Set to 0 to remove this effect.")]
        public float DecelerationEffectFactor = 1.0f;
        #endregion PUBLIC_VARIABLES


        #region PUBLIC_METHODS
        /// <summary>
        /// Get basic vertical speed (0.025) and multiply it by the flying speed factor
        /// </summary>
        /// <returns>The new vertical axis speed</returns>
        public float GetSpeed()
        {
            return 0.3f * FlyingSpeed;
        }

        /// <summary>
        /// Reset All parameters to there default values
        /// </summary>
        public void ResetParameters()
        {
            AccelerationDecelerationEffect = true;

            FlyingSpeed = 1.0f;
            MinAvatarPosition = new Vector3(-10.0f, -1.0f, -10.0f);
            MaxAvatarPosition = new Vector3(10.0f, 10.0f, 10.0f);
            AccelerationEffectFactor = 5.0f;
            DecelerationEffectFactor = 5.0f;
            UseBoundaries = false;
            MinAvatarPosition = new Vector3(-10.0f, -1.0f, -10.0f);
            MaxAvatarPosition = new Vector3(10.0f, 10.0f, 10.0f);
            FlyingBoundariesColor = Color.green;
        }
        #endregion PUBLIC_METHOD
    }
}