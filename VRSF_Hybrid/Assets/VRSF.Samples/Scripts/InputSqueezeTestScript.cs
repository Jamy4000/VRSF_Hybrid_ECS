using ScriptableFramework.Variables;
using UnityEngine;
using UnityEngine.UI;

namespace VRSF.Example
{
    public class InputSqueezeTestScript : MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private Text _sliderValue;
        [SerializeField] private FloatVariable _axisVariable;

        private void Update()
        {
            _slider.value = _axisVariable.Value;
        }

        public void ChangeValueDisplayed(float newValue)
        {
            _sliderValue.text = "Value : " + newValue;
        }
    }
}