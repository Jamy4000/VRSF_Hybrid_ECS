using ScriptableFramework.Variables;
using UnityEngine;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Required to have the references to the controllers parameters and capture the inputs for the Rift
    /// </summary>
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity))]
    public class RiftControllersInputCaptureComponent : MonoBehaviour
    {
        [HideInInspector] public BoolVariable LeftMenuClick;

        [HideInInspector] public BoolVariable AButtonClick;
        [HideInInspector] public BoolVariable AButtonTouch;

        [HideInInspector] public BoolVariable BButtonClick;
        [HideInInspector] public BoolVariable BButtonTouch;

        [HideInInspector] public BoolVariable XButtonClick;
        [HideInInspector] public BoolVariable XButtonTouch;

        [HideInInspector] public BoolVariable YButtonClick;
        [HideInInspector] public BoolVariable YButtonTouch;

        [HideInInspector] public BoolVariable RightThumbrestTouch;
        [HideInInspector] public BoolVariable LeftThumbrestTouch;
    }
}