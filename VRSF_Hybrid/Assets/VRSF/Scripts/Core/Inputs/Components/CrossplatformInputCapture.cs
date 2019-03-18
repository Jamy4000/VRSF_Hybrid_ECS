using UnityEngine;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Required to have the references to the controllers parameters and capture the inputs for all platform
    /// </summary>
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity))]
    public class CrossplatformInputCapture : MonoBehaviour
    {
        /// <summary>
        /// To handle the setup with the Vive
        /// </summary>
        [HideInInspector] public bool ControllersParametersSetup = false;

        /// <summary>
        /// The two parameters struct for the controllers
        /// </summary>
        [HideInInspector] public InputParameters RightParameters;
        [HideInInspector] public InputParameters LeftParameters;
    }
}