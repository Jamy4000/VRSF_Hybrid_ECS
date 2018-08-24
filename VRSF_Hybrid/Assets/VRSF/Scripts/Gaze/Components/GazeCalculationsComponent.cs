using UnityEngine;

namespace VRSF.Gaze.Components
{
    /// <summary>
    /// Contains all the variable to calculate the Gaze size, rotation and position.
    /// </summary>
    public class GazeCalculationsComponent : MonoBehaviour
    {
        [HideInInspector] public Vector3 _OriginalScale;        // Since the scale of the reticle changes, the original scale needs to be stored.
        [HideInInspector] public Quaternion _OriginalRotation;  // Used to store the original rotation of the reticle.
        [HideInInspector] public Transform _VRCamera;

        [HideInInspector] public bool _IsSetup;
    }
}