using ScriptableFramework.Events;
using UnityEngine;
using VRSF.Controllers;
using VRSF.Gaze;

namespace VRSF.Inputs.Components
{
    public class VRInputCaptureComponent : MonoBehaviour
    {
        // The Scriptable Singletons
        [HideInInspector] public ControllersParametersVariable ControllersParameters;
        [HideInInspector] public GazeParametersVariable GazeParameters;
        [HideInInspector] public InputVariableContainer InputContainer;

        // A Temp GameEvent to raise in the InputCaptureSystems
        [HideInInspector] public GameEvent TempEvent;

        // Wheter we check the Gaze Button or not
        [HideInInspector] public bool CheckGazeInteractions;
    }
}