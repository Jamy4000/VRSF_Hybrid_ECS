using UnityEngine;
using VRSF.Controllers;
using VRSF.Gaze;

namespace VRSF.Interactions.Components
{
    /// <summary>
    /// Contains the variables for the OnColliderOverSystems
    /// </summary>
    public class OnColliderOverComponents : MonoBehaviour
    {
        [Tooltip("The Controllers and Gaze Parameters as ScriptableSingletons")]
        [HideInInspector] public ControllersParametersVariable ControllersParameters;
        [HideInInspector] public GazeParametersVariable GazeParameters;

        [Tooltip("The Interactions Container as ScriptableSingletons for the VRSF Scriptable Objects")]
        [HideInInspector] public InteractionVariableContainer InteractionsContainer;
        
        [Tooltip("Wheter we check if the Raycast is over the objects, set at runtime by checking if we use the controllers or the gaze")]
        [HideInInspector] public bool CheckRaycast = true;
    }
}
