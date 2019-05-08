using ScriptableFramework.Variables;
using UnityEngine;
using UnityEngine.UI;

namespace VRSF.Example
{
    public class InputTouchpadTestScript : MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private Text _sliderValue;
        [SerializeField] private Vector2Variable _axisVariable;
        [SerializeField] private bool _isXAxis;

        private void Update()
        {
            _slider.value = _isXAxis ? _axisVariable.Value.x : _axisVariable.Value.y;
        }

        public void ChangeValueDisplayed(float newValue)
        {
            _sliderValue.text = "Value : " + newValue;
        }
    }
}