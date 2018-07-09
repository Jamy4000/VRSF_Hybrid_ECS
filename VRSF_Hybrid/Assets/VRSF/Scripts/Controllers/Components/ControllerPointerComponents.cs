using UnityEngine;
using UnityEngine.UI;

namespace VRSF.Controllers.Components
{
    /// <summary>
    /// Contains all the variable for the ControllerPointer Systems
    /// </summary>
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity))]
    public class ControllerPointerComponents : MonoBehaviour
    {
        // Wheter we need to change the gaze state or not
        [HideInInspector] public bool CheckGazeStates = false;

        // OPTIONAL : Gaze Reticle Sprites and Script
        [HideInInspector] public Image GazeBackground;
        [HideInInspector] public Image GazeTarget;
        [HideInInspector] public Gaze.Gaze GazeScript;

        // LineRenderer attached to the right and left controllers
        [HideInInspector] public LineRenderer RightHandPointer;
        [HideInInspector] public LineRenderer LeftHandPointer;

        [HideInInspector] public Transform CameraRigTransform;

        [HideInInspector] public bool IsSetup = false;
    }
}