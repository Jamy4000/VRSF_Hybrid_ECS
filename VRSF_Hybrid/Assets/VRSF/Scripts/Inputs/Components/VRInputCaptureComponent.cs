using ScriptableFramework.Events;
using UnityEngine;

namespace VRSF.Inputs.Components
{
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity))]
    public class VRInputCaptureComponent : MonoBehaviour
    {
        // A Temp GameEvent to raise in the InputCaptureSystems
        [HideInInspector] public GameEvent TempEvent;

        // Wheter we check the Gaze Button or not
        [HideInInspector] public bool CheckGazeInteractions;
    }
}