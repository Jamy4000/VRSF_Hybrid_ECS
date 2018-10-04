using UnityEngine;
using VRSF.Utils.Components.ButtonActionChoser;

namespace VRSF.MoveAround.Components
{
    /// <summary>
    /// Contains all references to the fly variables that are not in the FlyingParametersVariable already.
    /// </summary>
    [RequireComponent(typeof(BACGeneralVariablesComponents), typeof(FlyVelocityComponent), typeof(FlyDirectionComponent))]
    public class FlyParametersComponent : MonoBehaviour
    {
        #region Parameters_Set_By_User
        
        [Tooltip("The General speed factor.")]
        [HideInInspector] public float FlyingSpeed = 1.0f;

        [Tooltip("Whether the speed of the user should be faster if he has a higher position (Y axis).")]
        [HideInInspector] public bool ChangeSpeedDependingOnHeight = true;

        [Tooltip("Whether the speed of the user should be faster if he has a higher scale.")]
        [HideInInspector] public bool ChangeSpeedDependingOnScale = true;

        #endregion Parameters_Set_By_User


        #region Parameters_Handled_By_Script
        [HideInInspector] public bool _IsInteracting = false;
        [HideInInspector] public bool _WantToFly = false;
        [HideInInspector] public bool _FlyForward = true;
        [HideInInspector] public bool _IsSlowingDown = false;

        [HideInInspector] public float _TimeSinceStartFlying = 0.0f;
        [HideInInspector] public float _SlowDownTimer = 0.0f;
        #endregion Parameters_Handled_By_Script
    }
}