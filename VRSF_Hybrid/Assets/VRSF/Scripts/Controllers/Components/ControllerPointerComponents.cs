using UnityEngine;
using UnityEngine.UI;
using VRSF.Gaze;
using VRSF.Interactions;

namespace VRSF.Controllers.Components
{
    /// <summary>
    /// Contains all the variable for the ControllerPointer Systems
    /// </summary>
    public class ControllerPointerComponents : MonoBehaviour
    {
        // VRSF Parameters and ScriptableSingletons
        [HideInInspector] public ControllersParametersVariable ControllersParameters;
        [HideInInspector] public GazeParametersVariable GazeParameters;
        [HideInInspector] public InteractionVariableContainer InteractionContainer;

        // Wheter we need to change the gaze state or not
        [HideInInspector] public bool CheckGazeStates = false;

        // OPTIONAL : Gaze Reticle Sprites and Script
        [HideInInspector] public Image GazeBackground;
        [HideInInspector] public Image GazeTarget;
        [HideInInspector] public Gaze.Gaze GazeScript;

        // LineRenderer attached to the right and left controllers
        [HideInInspector] public LineRenderer RightHandPointer;
        [HideInInspector] public LineRenderer LeftHandPointer;

        [HideInInspector] public bool IsSetup = false;

        [HideInInspector] public Transform CameraRigTransform;
    }
}