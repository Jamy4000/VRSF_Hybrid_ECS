using UnityEngine;
using VRSF.Utils.Components;

namespace VRSF.MoveAround.Components
{
    /// <summary>
    /// Contains all references to the fly variables that are not in the FlyingParametersVariable already.
    /// </summary>
    [RequireComponent(typeof(ButtonActionChoserComponents), typeof(Unity.Entities.GameObjectEntity))]
    public class FlyComponent : MonoBehaviour
    {
        #region Parameters_Set_By_User

        #region GeneralParameters
        [Tooltip("The General speed factor.")]
        [HideInInspector] public float FlyingSpeed = 1.0f;

        [Tooltip("Whether the speed of the user should be faster if he has a higher position (Y axis).")]
        [HideInInspector] public bool ChangeSpeedDependingOnHeight = true;

        [Tooltip("Whether the speed of the user should be faster if he has a higher scale.")]
        [HideInInspector] public bool ChangeSpeedDependingOnScale = true;
        #endregion GeneralParameters

        #region Boundaries
        [Tooltip("Wheter you wanna use boundaries for the flying mode or not.")]
        [HideInInspector] public bool UseHorizontalBoundaries = false;

        [Tooltip("The minimun vertical position at which the user can go if UseHorizontalBoundaries is at false.")]
        [HideInInspector] public float MinAvatarYPosition = 0.0f;

        [Tooltip("The maximum vertical position at which the user can go if UseHorizontalBoundaries is at false.")]
        [HideInInspector] public float MaxAvatarYPosition = 2000.0f;

        [Tooltip("The minimun local position at which the user can go if UseHorizontalBoundaries is at true.")]
        [HideInInspector] public Vector3 MinAvatarPosition = new Vector3(-10.0f, -1.0f, -10.0f);

        [Tooltip("The maximum local position at which the user can go if UseHorizontalBoundaries is at true.")]
        [HideInInspector] public Vector3 MaxAvatarPosition = new Vector3(10.0f, 10.0f, 10.0f);
        #endregion Boundaries

        #region SlidingEffect
        [Tooltip("If you want to have an acceleration and deceleration effect when starting and stopping to fly.")]
        [HideInInspector] public bool AccelerationDecelerationEffect = true;

        [Tooltip("The factor for the acceleration effect. Set to 0 to remove this effect.")]
        [HideInInspector] public float AccelerationEffectFactor = 1.0f;

        [Tooltip("The factor for the deceleration effect. Set to 0 to remove this effect.")]
        [HideInInspector] public float DecelerationEffectFactor = 1.0f;
        #endregion SlidingEffect

        #endregion Parameters_Set_By_User


        #region Parameters_Handled_By_Script
        [HideInInspector] public bool IsSetup = false;

        [HideInInspector] public bool WantToFly = false;
        [HideInInspector] public bool FlyForward = true;
        [HideInInspector] public bool IsSlowingDown = false;

        [HideInInspector] public float FlightDirection = 0.0f;
        [HideInInspector] public float CurrentFlightVelocity = 0.0f;

        [HideInInspector] public Vector3 NormalizedDir = new Vector3();
        [HideInInspector] public Vector3 FinalDirection = new Vector3();

        [HideInInspector] public float TimeSinceStartFlying = 0.0f;
        [HideInInspector] public float SlowDownTimer = 0.0f;
        #endregion Parameters_Handled_By_Script
    }
}