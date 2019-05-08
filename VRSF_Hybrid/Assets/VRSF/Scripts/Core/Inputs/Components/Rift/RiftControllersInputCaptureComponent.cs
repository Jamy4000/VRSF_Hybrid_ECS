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
        public BoolVariable LeftMenuClick;

        public BoolVariable AButtonClick;
        public BoolVariable AButtonTouch;

        public BoolVariable BButtonClick;
        public BoolVariable BButtonTouch;

        public BoolVariable XButtonClick;
        public BoolVariable XButtonTouch;

        public BoolVariable YButtonClick;
        public BoolVariable YButtonTouch;

        public BoolVariable RightThumbrestTouch;
        public BoolVariable LeftThumbrestTouch;
    }
}