
using System.Collections.Generic;
using UnityEngine;

namespace VRSF.UI
{
    /// <summary>
    /// Method that implements the IUISetupClickOnly and the IUISetupNotScrollable for the VRInputField
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class VRInputFieldSetup : IUISetupClickOnly
    {
        // EMPTY
        #region PUBLIC_VARIABLES
        #endregion PUBLIC_VARIABLES


        // EMPTY
        #region PRIVATE_VARIABLES
        #endregion PRIVATE_VARIABLES


        #region PUBLIC_METHODS
        /// <summary>
        /// Called if the GameEventListener Container already exist
        /// </summary>
        /// <param name="gameEventListenersContainer">The GameObject containing the listeners</param>
        /// <param name="ListenersDictionary">The Listeners Dictionnary to modify</param>
        /// <returns>The Listeners Dictionnary with updated values</returns>
        public Dictionary<string, GameEventListenerTransform> SetListenerToPresentComponent(GameObject gameEventListenersContainer, Dictionary<string, GameEventListenerTransform> ListenersDictionary)
        {
            foreach (GameEventListenerTransform l in gameEventListenersContainer.GetComponents<GameEventListenerTransform>())
            {
                if (l.GetEvent().name.ToLower().Contains("right"))
                    ListenersDictionary["Right"] = l;

                else if (l.GetEvent().name.ToLower().Contains("left"))
                    ListenersDictionary["Left"] = l;

                else if (l.GetEvent().name.ToLower().Contains("gaze"))
                    ListenersDictionary["Gaze"] = l;

                else
                    Debug.LogError("Couldn't find the GameEventListenerTransform that correspond");
            }
            return ListenersDictionary;
        }

        /// <summary>
        /// Check if the button gameobject contain a child named GameEventListenersButton
        /// </summary>
        /// <param name="gameEventListenersContainer">The GameObject containing the listeners</param>
        /// <param name="ListenersDictionary">The Dictionary of Listeners to feed, here only for the clicks</param>
        /// <param name="t">The Transform of the UI Element</param>
        /// <returns>return false if there's no child named GameEventListenersButton</returns>
        public bool CheckGameEventListenerChild(ref GameObject gameEventListenersContainer, ref Dictionary<string, GameEventListenerTransform> ListenersDictionary, Transform t)
        {
            for (int i = 0; i < t.childCount; i++)
            {
                if (t.GetChild(i).name.Contains("GameEventListeners"))
                {
                    gameEventListenersContainer = t.GetChild(i).gameObject;
                    ListenersDictionary = SetListenerToPresentComponent(gameEventListenersContainer, ListenersDictionary);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Set the size of the BoxCollider to fit the UI Element
        /// </summary>
        /// <param name="box">The box collider to set</param>
        /// <param name="rect">the rect Transform of the object attached to the BoxCollider</param>
        /// <return>The Box Colldier updated</return>
        public BoxCollider CheckBoxColliderSize(BoxCollider box, RectTransform rect)
        {
            box.size = new Vector3(rect.rect.width, rect.rect.height, 0.0001f);
            box.center = new Vector3(0, rect.rect.height / 2, 0);
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