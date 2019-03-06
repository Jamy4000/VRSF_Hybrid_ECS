using UnityEngine;

namespace VRSF.Core.Inputs
{
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity))]
    public class SimulatorGazeInputCaptureComponent : MonoBehaviour
    {
        // Wheter we check the Gaze Button or not
        [HideInInspector] public bool CheckGazeInteractions;
    }
}