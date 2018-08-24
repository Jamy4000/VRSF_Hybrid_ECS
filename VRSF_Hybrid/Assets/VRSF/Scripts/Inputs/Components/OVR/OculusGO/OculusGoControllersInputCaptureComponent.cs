using ScriptableFramework.Events;
using UnityEngine;

namespace VRSF.Inputs.Components
{
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity))]
    public class OculusGoControllersInputCaptureComponent : MonoBehaviour
    {
        // A Temp GameEvent to raise in the InputCaptureSystems
        [HideInInspector] public GameEvent TempEvent;

        // To know at rntime if the user was using the pointer of the hand that is not using the contorler
        [HideInInspector] public bool _UsingOtherHandPointer;
    }
}