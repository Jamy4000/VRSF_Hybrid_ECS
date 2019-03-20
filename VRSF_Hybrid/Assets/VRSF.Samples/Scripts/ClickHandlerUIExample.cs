using VRSF.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace VRSF.UI.Example
{
    /// <summary>
    /// Script attached to the UIExample Prefabs.
    /// It's a quick example on how to interact with the Scriptable UI System in VR.
    /// </summary>
    public class ClickHandlerUIExample : MonoBehaviour
    {
        #region PUBLIC_VARIABLES
        [Header("The VRKeyboard in the Scene")]
        public GameObject VRKeyboard;
        #endregion PUBLIC_VARIABLES


        // EMPTY
        #region PRIVATE_VARIABLES
        #endregion PRIVATE_VARIABLES


        // EMPTY
        #region MONOBEHAVIOUR_METHODS
        #endregion MONOBEHAVIOUR_METHODS


        #region PUBLIC_METHODS
        /// <summary>
        /// Handle the button when it's clicked
        /// </summary>
        /// <param name="t">The text under the button</param>
        public void HandleButton(Text t)
        {
            t.fontSize = 12;
            t.text = "You're a wonderfull person and I love you";
        }

        public void Test()
        {
            Debug.Log("Test method called !");
        }

        /// <summary>
        /// Handle the toggle example when it's clicked
        /// </summary>
        public void HandleToggle()
        {
            Debug.Log("toggle clicked");
            VRKeyboard.SetActive(!VRKeyboard.activeSelf);
        }
        #endregion PUBLIC_METHODS


        #region PRIVATE_METHODS
        /// <summary>
        /// Handle the inputField when it's clicked
        /// </summary>
        /// <param name="inputFieldHit">The gameObject that was hit</param>
        void HandleInputField(GameObject inputFieldHit)
        {
            var inputField = inputFieldHit.GetComponent<InputField>();
            foreach (Text t in inputField.GetComponentsInChildren<Text>())
            {
                t.text = "";
            }
            inputField.ActivateInputField();

            if (VRKeyboard != null)
                VRKeyboard.GetComponent<VRKeyboard>().InputField = inputField;
        }
        #endregion PRIVATE_METHODS
    }
}