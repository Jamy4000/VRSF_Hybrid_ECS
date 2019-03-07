using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.Inputs;
using VRSF.Core.Gaze;
using VRSF.Interactions;

namespace VRSF.Core.SetupVR
{
    public class ScriptableSingletonsComponent : MonoBehaviour
    {
        [HideInInspector] public GazeParametersVariable GazeParameters;
        [HideInInspector] public ControllersParametersVariable ControllersParameters;
        [HideInInspector] public InteractionVariableContainer InteractionsContainer;
        [HideInInspector] public InputVariableContainer InputsContainer;

        [HideInInspector] public bool _IsSetup;
    }
}