
using UnityEngine;

namespace VRSF.Inputs.Components
{
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity))]
    public class SimulatorControllersInputCaptureComponent : MonoBehaviour
    {
        // A Temp GameEvent to raise in the InputCaptureSystems
        [HideInInspector] public GameEvent TempEvent;
    }
}