using VRSF.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        }
        #endregion MONOBEHAVIOUR_METHODS


        // EMPTY
        #region PUBLIC_METHODS

        #endregion PUBLIC_METHODS


        #region PRIVATE_METHODS
        public void CheckRayCastGaze(Transform value)
        {
            image.enabled = (value == transform);
        }
        #endregion PRIVATE_METHODS


        // EMPTY
        #region GETTERS_SETTERS
        #endregion GETTERS_SETTERS
    }
}