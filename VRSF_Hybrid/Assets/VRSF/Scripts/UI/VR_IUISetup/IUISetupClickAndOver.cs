using ScriptableFramework.Events;
using System.Collections.Generic;
using UnityEngine;

namespace VRSF.UI
{
    /// <summary>
    /// Interface that groups all method used to setup the UIElements of the Framework that use the Over and Click events
    /// </summary>
    public interface IUISetupClickAndOver
    {
        /// <summary>
        /// Set the size of the BoxCollider to fit the UI Element
        /// </summary>
        /// <param name="box">The box collider to set</param>
        /// <param name="rect">The rect Transform of the object attached to the BoxCollider</param>
        /// <param name="direction">The UIDirection of the UI Element</param>
        /// <return>The Box Colldier updated</return>
        BoxCollider CheckBoxColliderSize(BoxCollider box, RectTransform rt, EUIDirection direction);


        /// <summary>
        /// Check if the button gameobject contain a child named GameEventListenersButton
        /// </summary>
        /// <param name="gameEventListenersContainer">The GameObject containing the listeners</param>
        /// <param name="ListenersDictionary">The Dictionary of Listeners to feed, here only for the clicks</param>
        /// <param name="t">The Transform of the UI Element</param>
        /// <param name="inputType">The UIInputType of the GameEventsListeners to check (click or over)</param>
        /// <returns>return false if there's no child named GameEventListenersButton</returns>
        bool CheckGameEventListenerChild(ref GameObject gameEventListenersContainer, ref Dictionary<string, GameEventListenerTransform> ListenersDictionary, Transform t, EUIInputType inputType);


        /// <summary>
        /// Called if the GameEventListener Container already exist
        /// </summary>
        /// <param name="gameEventListenersContainer">The GameObject containing the listeners</param>
        /// <param name="ListenersDictionary">The Listeners Dictionnary to modify</param>
        /// <param name="inputType">click or over</param>
        /// <returns>The Listeners Dictionnary with updated values</returns>
        Dictionary<string, GameEventListenerTransform> SetListenerToPresentComponent(GameObject gameEventListenersContainer, Dictionary<string, GameEventListenerTransform> ListenersDictionary, EUIInputType inputType);
    }
}