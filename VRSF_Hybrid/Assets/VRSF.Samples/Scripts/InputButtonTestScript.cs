using UnityEngine;
using UnityEngine.UI;
using VRSF.Core.Controllers;
using VRSF.Core.Inputs;

namespace VRSF.Example
{
    public class InputButtonTestScript : MonoBehaviour
    {
        [SerializeField] private Image _statusPointForClick;
        [SerializeField] private Image _statusPointForTouch;

        [SerializeField] private EHand _buttonsHand;
        [SerializeField] private EControllersButton _buttonsToCheck;

        private void Awake()
        {
            if (_statusPointForClick != null)
            {
                ButtonClickEvent.Listeners += ChangeButtonStatusToClick;
                ButtonUnclickEvent.Listeners += ChangeButtonStatusToUnclick;
            }
            if (_statusPointForTouch != null)
            {
                ButtonTouchEvent.Listeners += ChangeButtonStatusToTouch;
                ButtonUntouchEvent.Listeners += ChangeButtonStatusToUntouch;
            }
        }

        private void OnDestroy()
        {
            ButtonClickEvent.Listeners -= ChangeButtonStatusToClick;
            ButtonUnclickEvent.Listeners -= ChangeButtonStatusToUnclick;
            ButtonTouchEvent.Listeners -= ChangeButtonStatusToTouch;
            ButtonUntouchEvent.Listeners -= ChangeButtonStatusToUntouch;
        }

        private void ChangeButtonStatusToClick(ButtonClickEvent info)
        {
            if (info.HandInteracting == _buttonsHand && info.ButtonInteracting == _buttonsToCheck)
                _statusPointForClick.color = Color.green;
        }

        private void ChangeButtonStatusToUnclick(ButtonUnclickEvent info)
        {
            if (info.HandInteracting == _buttonsHand && info.ButtonInteracting == _buttonsToCheck)
                _statusPointForClick.color = Color.red;
        }
        
        private void ChangeButtonStatusToTouch(ButtonTouchEvent info)
        {
            if (info.HandInteracting == _buttonsHand && info.ButtonInteracting == _buttonsToCheck)
                _statusPointForTouch.color = Color.green;
        }

        private void ChangeButtonStatusToUntouch(ButtonUntouchEvent info)
        {
            if (info.HandInteracting == _buttonsHand && info.ButtonInteracting == _buttonsToCheck)
                _statusPointForTouch.color = Color.red;
        }
    }
}