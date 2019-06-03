using ScriptableFramework.Variables;
using UnityEngine;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Required to have the references to the controllers parameters and capture the inputs for the GearVR and the Oculus Go
    /// </summary>
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity))]
    public class GoAndGearVRControllersInputCaptureComponent : MonoBehaviour
    {
        [Header("Is the user Right Handed")]
        public bool IsRightHanded = true;

        /// <summary>
        /// Set in the Script Inspector (not the instance, but the script itself in the Assets Folder)
        /// </summary>
        [HideInInspector] public BoolVariable BackButtonClick;
    }
}