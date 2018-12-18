using UnityEngine;
using VRSF.Controllers;
using VRSF.Gaze;
using VRSF.Inputs;
using VRSF.Interactions;

namespace VRSF.Utils.Components
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