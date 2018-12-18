using UnityEngine;

public class ViveGazeInputCaptureComponent : MonoBehaviour
{
    [Header("The References to the SteamVR Devices")]
    [HideInInspector] public SteamVR_Controller.Device GazeController;

    [HideInInspector] public bool CheckGazeInteractions;
    [HideInInspector] public bool GazeReferencesSetup = false;
}
