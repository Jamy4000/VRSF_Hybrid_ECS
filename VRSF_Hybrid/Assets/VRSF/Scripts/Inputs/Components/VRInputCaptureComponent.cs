using ScriptableFramework.Events;
using UnityEngine;
using VRSF.Controllers;
using VRSF.Gaze;

namespace VRSF.Inputs.Components
{
    public class VRInputCaptureComponent : MonoBehaviour
    {
        [HideInInspector] public ControllersParametersVariable ControllersParameters;
        [HideInInspector] public GazeParametersVariable GazeParameters;
        [HideInInspector] public InputVariableContainer InputContainer;

        [HideInInspector] public GameEvent TempEvent;

        [HideInInspector] public bool CheckGazeInteractions;
    }
}