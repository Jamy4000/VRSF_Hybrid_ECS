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
    }
}