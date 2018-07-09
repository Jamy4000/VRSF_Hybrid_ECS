using ScriptableFramework.Variables;
using UnityEngine;
using VRSF.Controllers;
using VRSF.Gaze;
using VRSF.Inputs;

namespace VRSF.Interactions.Components
{
    public class OnColliderClickComponents : MonoBehaviour
    {
        [Tooltip("The Controllers and Gaze Parameters as ScriptableSingletons")]
        [HideInInspector] public ControllersParametersVariable ControllersParameters;
        [HideInInspector] public GazeParametersVariable GazeParameters;

        [Tooltip("The Interactions and Input Container, as ScriptableSingletons, for the VRSF Scriptable Objects")]
        [HideInInspector] public InputVariableContainer InputsContainer;
        [HideInInspector] public InteractionVariableContainer InteractionsContainer;

        [Tooltip("Reference to the Left and Right Click Variables, meaning the Trigger")]
        [HideInInspector] public BoolVariable LeftClickBool;
        [HideInInspector] public BoolVariable RightClickBool;

        [Tooltip("Wheter we check if the Raycast is over the objects, set at runtime by checking if we use the controllers or the gaze")]
        [HideInInspector] public bool CheckRaycast = true;
    }

}