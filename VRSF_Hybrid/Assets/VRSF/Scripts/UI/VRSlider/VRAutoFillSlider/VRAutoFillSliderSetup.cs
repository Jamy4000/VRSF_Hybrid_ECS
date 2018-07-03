using System.Collections.Generic;
using ScriptableFramework.Events;
using UnityEngine;

namespace VRSF.UI
{
    /// <summary>
    /// Method that implements the IUISetupClickAndOver for the VRAutoFillSlider
    /// This one, compared to the ClickOnly, does have a constructor to set the CheckObjectOver delegate
    /// </summary>
	public class VRAutoFillSliderSetup : IUISetupClickAndOver
    {
        // EMPTY
        #region PUBLIC_VARIABLES
        #endregion


        // EMPTY
        #region PRIVATE_VARIABLES
        #endregion


        #region PUBLIC_METHODS
        /// <summary>
        /// Check if the button gameobject contain a child named GameEventListenersButton
        /// </summary>
        /// <param name="gameEventListenersContainer">The GameObject containing the listeners</param>
        /// <param name="ListenersDictionary">The Dictionary of Listeners to feed, here only for the clicks</param>
        /// <param name="t">The Transform of the UI Element</param>
        /// <returns>return false if there's no child named GameEventListenersButton</returns>
        public bool CheckGameEventListenerChild(ref GameObject gameEventListenersContainer, ref Dictionary<string, GameEventListenerTransform> ListenersDictionary, Transform t, EUIInputType inputType)
        {
            for (int i = 0; i < t.childCount; i++)
            {
                if (t.GetChild(i).name.Contains("GameEventListeners"))
                {
                    gameEventListenersContainer = t.GetChild(i).gameObject;
                    ListenersDictionary = SetListenerToPresentComponent(gameEventListenersContainer, ListenersDictionary, inputType);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Called if the GameEventListener Container already exist
        /// </summary>
        /// <param name="gameEventListenersContainer">The GameObject containing the listeners</param>
        /// <param name="ListenersDictionary">The Listeners Dictionnary to modify</param>
        /// <param name="inputType">click or over</param>
        /// <returns>The Listeners Dictionnary with updated values</returns>
        public Dictionary<string, GameEventListenerTransform> SetListenerToPresentComponent(GameObject gameEventListenersContainer, Dictionary<string, GameEventListenerTransform> ListenersDictionary, EUIInputType inputType)
        {
            foreach (GameEventListenerTransform l in gameEventListenersContainer.GetComponents<GameEventListenerTransform>())
            {
                string eventName = l.GetEvent().name.ToLower();
                
                if (eventName.Contains(inputType.ToString().ToLower()))
                {
                    if (eventName.Contains("right"))
                        ListenersDictionary["Right"] = l;

                    else if (eventName.Contains("left"))
                        ListenersDictionary["Left"] = l;

                    else if (eventName.Contains("gaze"))
                        ListenersDictionary["Gaze"] = l;

                    else
                        Debug.LogError("Couldn't find the GameEventListenerTransform that correspond");
                }
            }
            return ListenersDictionary;
        }

        /// <summary>
        /// Check the BoxCollider size and make it fit the Scrollable UI Element according to its direction
        /// </summary>
        /// <param name="box">The box collider to set</param>
        /// <param name="rt">The rect transform of the ui element</param>
        /// <param name="direction">The direction of the scrollable, given as a UIDirection</param>
        /// <returns>The new BoxCOllider after being set</returns>
        public BoxCollider CheckBoxColliderSize(BoxCollider box, RectTransform rt, EUIDirection direction)
        {
            switch (direction)
            {
                case (EUIDirection.TopToBottom):
                    box.size = new Vector3(rt.rect.width, rt.rect.height, 0.0001f);
                    box.center = new Vector3(0.0f, rt.rect.height / 2, 0.0f);
                    break;
                case (EUIDirection.BottomToTop):
                    box.size = new Vector3(rt.rect.width, rt.rect.height, 0.0001f);
                    box.center = new Vector3(0.0f, -rt.rect.height / 2, 0.0f);
                    break;
                case (EUIDirection.RightToLeft):
                    box.size = new Vector3(rt.rect.width, rt.rect.height, 0.0001f);
                    box.center = new Vector3(0.0f, 0, 0.0f);
                    break;
                case (EUIDirection.LeftToRight):
                    box.size = new Vector3(rt.rect.width, rt.rect.height, 0.0001f);
                    box.center = new Vector3(0.0f, 0, 0.0f);
                    break;
            }
            return box;
        }
        #endregion PUBLIC_METHODS


        // EMPTY
        #region PRIVATE_METHODS

        #endregion


        // EMPTY
        #region GETTERS_SETTERS

        #endregion
    }
}