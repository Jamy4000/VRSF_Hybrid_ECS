using UnityEngine;

namespace VRSF.Gaze.Inputs
{
    public class ViveGazeInputCaptureComponent : MonoBehaviour
    {
        [HideInInspector] public bool CheckGazeInteractions;
        [HideInInspector] public bool GazeReferencesSetup = false;
    }
}