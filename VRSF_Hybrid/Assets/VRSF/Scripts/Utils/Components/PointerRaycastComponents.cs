using UnityEngine;
using VRSF.Controllers;
using VRSF.Gaze;
using VRSF.Interactions;

namespace VRSF.Utils.Components
{
    /// <summary>
    /// Contains the variables for the PointerRaycastSystems
    /// </summary>
    public class PointerRaycastComponents : MonoBehaviour
    {
        [HideInInspector] public ControllersParametersVariable ControllersParameters;
        [HideInInspector] public GazeParametersVariable GazeParameters;
        [HideInInspector] public InteractionVariableContainer InteractionsContainer;

        // Wheter we check the raycast, set at runtime by checking if we use the controllers or the gaze
        [HideInInspector] public bool CheckRaycast = true;
    }
}