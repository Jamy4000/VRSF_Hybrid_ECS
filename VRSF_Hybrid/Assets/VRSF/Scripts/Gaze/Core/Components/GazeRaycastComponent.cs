using UnityEngine;

namespace VRSF.Gaze
{
    /// <summary>
    /// Component used to handle the Raycasting of the gaze on objects
    /// </summary>
    public class GazeRaycastComponent : MonoBehaviour
    {
        [HideInInspector] public Camera _VRCamera;
    }
}