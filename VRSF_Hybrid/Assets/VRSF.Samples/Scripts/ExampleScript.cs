using ScriptableFramework.Variables;
using UnityEngine;

namespace VRSF.Example
{
    /// <summary>
    /// Example script displaying messages when the user press a button
    /// </summary>
    public class ExampleScript : MonoBehaviour
    {
        #region PUBLIC_VARIABLE
        [Header("Thumb orientation on the controller to which this script is attached to")]
        public Vector2Variable ThumbOrientation;
        #endregion PUBLIC_VARIABLE


        //EMPTY
        #region PRIVATE_VARIABLE
        #endregion PRIVATE_VARIABLE


        #region MONOBEHAVIOUR_METHODS
        private void Update()
        {
            if (ThumbOrientation.Value != Vector2.zero)
            {
                Debug.Log("One Thumb is moving the joystick. The value is : " +
                    ThumbOrientation.Value.ToString());
            }
        }
        #endregion MONOBEHAVIOUR_METHODS


        #region PUBLIC_METHODS
        /// <summary>
        /// Called by the GameEventListeners when a button is clicked
        /// </summary>
        /// <param name="ButtonName">the name of the button that was clicked</param>
        public void ClickButton(string ButtonName)
        {
            Debug.Log(ButtonName + " Was Clicked !");
        }

        /// <summary>
        /// Called by the GameEventListeners when a button is released
        /// </summary>
        /// <param name="ButtonName">the name of the button that was released</param>
        public void ReleaseButton(string ButtonName)
        {
            Debug.Log(ButtonName + " Was Released !");
        }

        /// <summary>
        /// Called by the GameEventListeners when a button is touched
        /// </summary>
        /// <param name="ButtonName">the name of the button that was touched</param>
        public void ThumbTouch(string ButtonName)
        {
            Debug.Log(ButtonName + " Was Touched !");
        }
        #endregion PUBLIC_METHODS


        //EMPTY
        #region PRIVATE_METHODS
        #endregion PRIVATE_METHODS
    }
}