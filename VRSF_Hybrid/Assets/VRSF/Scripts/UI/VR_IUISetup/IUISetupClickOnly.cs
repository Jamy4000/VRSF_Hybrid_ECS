using ScriptableFramework.Events;
using System.Collections.Generic;
using UnityEngine;

namespace VRSF.UI
{
    /// <summary>
    /// Interface that groups all method used to setup the UIElements of the Framework that only use the Click Events
    /// </summary>
	public interface IUISetupClickOnly
    {
        /// <summary>
        /// Check if the button gameobject contain a child named GameEventListenersButton
        /// </summary>
        /// <param name="gameEventListenersContainer">The GameObject containing the listeners</param>
        /// <param name="ListenersDictionary">The Dictionary of Listeners to feed, here only for the clicks</param>
        /// <param name="t">The Transform of the UI Element</param>
        /// <returns>return false if there's no child named GameEventListenersButton</returns>
        bool CheckGameEventListenerChild(ref GameObject gameEventListenersContainer, ref Dictionary<string, GameEventListenerTransform> ListenersDictionary, Transform t);


        /// <summary>
        /// Called if the GameEventListener Container already exist
        /// </summary>
        /// <param name="gameEventListenersContainer">The GameObject containing the listeners</param>
        /// <param name="ListenersDictionary">The Listeners Dictionnary to modify</param>
        /// <returns>The Listeners Dictionnary with updated values</returns>
        Dictionary<string, GameEventListenerTransform> SetListenerToPresentComponent(GameObject gameEventListenersContainer, Dictionary<string, GameEventListenerTransform> ListenersDictionary);
    }
}