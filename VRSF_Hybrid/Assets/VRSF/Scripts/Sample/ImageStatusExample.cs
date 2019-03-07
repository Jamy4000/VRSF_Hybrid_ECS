using UnityEngine;
using UnityEngine.UI;
using VRSF.Core.Events;

namespace VRSF.UI.Example
{
	public class ImageStatusExample : MonoBehaviour 
	{
        #region PRIVATE_VARIABLES
        Image _image;
        #endregion PRIVATE_VARIABLES

        
        #region MONOBEHAVIOUR_METHODS
        private void Start()
        {
            _image = GetComponent<Image>();
            ObjectWasHoveredEvent.RegisterListener(CheckRayCastGaze);
        }

        private void OnApplicationQuit()
        {
            ObjectWasHoveredEvent.UnregisterListener(CheckRayCastGaze);
        }
        #endregion MONOBEHAVIOUR_METHODS


        #region PRIVATE_METHODS
        public void CheckRayCastGaze(ObjectWasHoveredEvent value)
        {
            _image.enabled = (value.ObjectHovered == transform);
        }
        #endregion PRIVATE_METHODS
    }
}