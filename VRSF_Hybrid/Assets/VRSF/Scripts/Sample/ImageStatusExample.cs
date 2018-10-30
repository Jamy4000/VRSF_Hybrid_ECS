using UnityEngine;
using UnityEngine.UI;
using VRSF.Utils.Events;

namespace VRSF.UI.Example
{
	public class ImageStatusExample : MonoBehaviour 
	{
        // EMPTY
        #region PUBLIC_VARIABLES
        #endregion PUBLIC_VARIABLES


        // EMPTY
        #region PRIVATE_VARIABLES
        Image image;
        #endregion PRIVATE_VARIABLES

        
        #region MONOBEHAVIOUR_METHODS
        private void Start()
        {
            image = GetComponent<Image>();
            ObjectWasHoveredEvent.RegisterListener(CheckRayCastGaze);
        }

        private void OnApplicationQuit()
        {
            ObjectWasHoveredEvent.UnregisterListener(CheckRayCastGaze);
        }
        #endregion MONOBEHAVIOUR_METHODS


        // EMPTY
        #region PUBLIC_METHODS

        #endregion PUBLIC_METHODS


        #region PRIVATE_METHODS
        public void CheckRayCastGaze(ObjectWasHoveredEvent value)
        {
            image.enabled = (value.ObjectHovered == transform);
        }
        #endregion PRIVATE_METHODS


        // EMPTY
        #region GETTERS_SETTERS
        #endregion GETTERS_SETTERS
    }
}