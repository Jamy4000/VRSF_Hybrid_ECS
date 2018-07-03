using System.Collections.Generic;
using ScriptableFramework.Events;
using UnityEngine;

namespace VRSF.UI
{
    /// <summary>
    /// This class is called in every UI Elements to setup the gameEvents and there Listeners and add the response to them.
    /// </summary>
	public class VRUISetup
    {
        #region CONSTRUCTOR
        /// <summary>
        /// Delegate to pass the CHeckObjectHit method in each UIElement
        /// </summary>
        /// <param name="t">The transform that was hit</param>
        public delegate void CheckObjectDelegate(Transform t);

        public VRUISetup(CheckObjectDelegate checkObjectClicked, CheckObjectDelegate checkObjectOverred = null)
        {
            this._CheckObjectClick = checkObjectClicked;

            if (checkObjectOverred != null)
                this._CheckObjectOver = checkObjectOverred;
        }
        #endregion


        // EMPTY
        #region PUBLIC_VARIABLES
        #endregion


        #region PRIVATE_VARIABLES
        CheckObjectDelegate _CheckObjectClick;
        CheckObjectDelegate _CheckObjectOver;
        #endregion


        #region PUBLIC_METHODS
        /// <summary>
        /// Called when the user stop the app, to unregister the listeners
        /// </summary>
        /// <param name="listenersDictionary">The Listeners Dictionnary to modify</param>
        /// <param name="EventsDictionary">The GameEvents dictionary with the events to unregister</param>
        /// <returns>The Listeners Dictionnary with updated values</returns>
        public Dictionary<string, GameEventListenerTransform> EndApp(Dictionary<string, GameEventListenerTransform> listenersDictionary, Dictionary<string, GameEventTransform> EventsDictionary)
        {
            listenersDictionary["Right"] = DeactivateListener(listenersDictionary["Right"], EventsDictionary["Right"]);
            listenersDictionary["Left"] = DeactivateListener(listenersDictionary["Left"], EventsDictionary["Left"]);
            listenersDictionary["Gaze"] = DeactivateListener(listenersDictionary["Gaze"], EventsDictionary["Gaze"]);
            return listenersDictionary;
        }

        /// <summary>
        /// Check if the listener passed as reference exist, disable and unregister it if it's the case.
        /// </summary>
        /// <param name="listener">The listener to check and disable</param>
        /// <param name="gameEvent">the GameEvent that Registered the listener at runtime</param>
        /// <returns>The Listener after being disabled</returns>
        public GameEventListenerTransform DeactivateListener(GameEventListenerTransform listener, GameEventTransform gameEvent)
        {
            if (listener)
            {
                gameEvent.UnregisterListener(listener);
                listener.OnDisable();
            }
            return listener;
        }

        /// <summary>
        /// Create the container for the listeners
        /// </summary>
        /// <param name="gameEventListenersContainer">The GameObject containing the listeners</param>
        /// <param name="componentTransform">The transform to which we attach the new GameObject</param>
        public void CreateGameEventListenerChild(ref GameObject gameEventListenersContainer, Transform componentTransform)
        {
            gameEventListenersContainer = new GameObject();
            gameEventListenersContainer.transform.SetParent(componentTransform);
            gameEventListenersContainer.transform.name = "GameEventListeners";
            gameEventListenersContainer.transform.localPosition = Vector3.zero;
            gameEventListenersContainer.transform.localRotation = Quaternion.Euler(Vector3.zero);
            gameEventListenersContainer.transform.localScale = Vector3.zero;
        }

        /// <summary>
        /// Check if the GameEventListenersContainer contains the amount of listeners required
        /// </summary>
        /// <param name="gameEventListenersContainer">The GameObject containing the listeners</param>
        /// <param name="listenersDictionary">The Listeners Dictionnary to modify</param>
        /// <param name="amountListener">Define how many listeners ,ust be in the gameEventListenerContainer. the minimum is three, for the gaze and Hands clicks</param>
        /// <returns>The Listeners Dictionnary with updated values</returns>
        public Dictionary<string, GameEventListenerTransform> CheckGameEventListenersPresence(GameObject gameEventListenersContainer, Dictionary<string, GameEventListenerTransform> listenersDictionary, int amoutListener = 3)
        {
            // If the GameEventListenerContainer does not contain the listeners
            if (gameEventListenersContainer.GetComponents<GameEventListenerTransform>().Length != amoutListener)
            {
                listenersDictionary["Right"] = gameEventListenersContainer.AddComponent<GameEventListenerTransform>();
                listenersDictionary["Left"] = gameEventListenersContainer.AddComponent<GameEventListenerTransform>();
                listenersDictionary["Gaze"] = gameEventListenersContainer.AddComponent<GameEventListenerTransform>();
            }
            return listenersDictionary;
        }

        /// <summary>
        /// Method called to set all the GameEventListeners references and Responses
        /// </summary>
        /// <param name="ListenersDictionary">The Listeners Dictionnary to modify</param>
        /// <param name="EventsDictionary">The Events Dictionnary to modify</param>
        /// <param name="useGaze">Wheter we check the gaze events or not</param>
        /// <returns>The Listeners Dictionnary with updated values</returns>
        public Dictionary<string, GameEventListenerTransform> SetGameEventListeners(Dictionary<string, GameEventListenerTransform> ListenersDictionary, Dictionary<string, GameEventTransform> EventsDictionary, bool useGaze)
        {
            ListenersDictionary["Right"] = SetGameEventClickListener(EventsDictionary["Right"], ListenersDictionary["Right"]);
            ListenersDictionary["Left"] = SetGameEventClickListener(EventsDictionary["Left"], ListenersDictionary["Left"]);

            if (useGaze)
                ListenersDictionary["Gaze"] = SetGameEventClickListener(EventsDictionary["Gaze"], ListenersDictionary["Gaze"]);

            return ListenersDictionary;
        }

        /// <summary>
        /// Set the Event and response of the GameEventListeners passed in parameter, and register it int he gameEvent corresponding
        /// A GameEventListenerTransform is used as we check the transfrom of the object that was clicked
        /// </summary>
        /// <param name="gameEvent">The game Event to listen to</param>
        /// <param name="listener">The listener of the gameEvent that need to be set</param>
        /// <returns>The new version of the GameEventListenerTransform</returns>
        public GameEventListenerTransform SetGameEventClickListener(GameEventTransform gameEvent, GameEventListenerTransform listener)
        {
            listener.Event = gameEvent;
            listener.Response = new ScriptableFramework.Events.UnityEvents.EventTransform();

            // Add the listener to the gameEvent if it wasn't there yet
            if (!gameEvent.GetListeners().Contains(listener))
                gameEvent.RegisterListener(listener);

            if (listener.Event.name.ToLower().Contains("click"))
            {
                listener.Response.AddListener(delegate { _CheckObjectClick(listener.Value); });
            }
            else
            {
                listener.Response.AddListener(delegate { _CheckObjectOver(listener.Value); });
            }

            return listener;
        }

        /// <summary>
        /// Set the size of the BoxCollider to fit the UI Element
        /// </summary>
        /// <param name="box">The box collider to set</param>
        /// <param name="rect">the rect Transform of the object attached to the BoxCollider</param>
        /// <return>The Box Colldier updated</return>
        public BoxCollider CheckBoxColliderSize(BoxCollider box, RectTransform rect)
        {
            // Set box size
            box.size = new Vector3(rect.rect.width, rect.rect.height, 0.001f);

            // Set box center
            float x = GetBoxCenter(rect.pivot.x, box.size.x);
            float y = GetBoxCenter(rect.pivot.y, box.size.y);
            box.center = new Vector3(x, y, 0.0f);

            return box;
        }
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Check one of the pivot axis of the UIElement to return one of the axis of the BoxCollider center
        /// </summary>
        /// <param name="pivotAxis">The x or y value of the UIElement's pivot</param>
        /// <param name="boxSizeAxis">The x or y value of the BoxCollider size</param>
        /// <returns>One of the axis of the BoxCollider center</returns>
        private float GetBoxCenter(float pivotAxis, float boxSizeAxis)
        {
            if (pivotAxis > 0.5f)
            {
                return -boxSizeAxis / 2;
            }
            else if (pivotAxis < 0.5f)
            {
                return boxSizeAxis / 2;
            }
            else    // pivotAxis == 0.0f
            {
                return 0.0f;
            }
        }
        #endregion


        // EMPTY
        #region GETTERS_SETTERS
        #endregion
    }
}