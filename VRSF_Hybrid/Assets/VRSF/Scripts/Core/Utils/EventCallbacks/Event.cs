namespace EventCallbacks
{
    /// <summary>
    /// Base class for the event callback system. Extend this class to create a new event to raise and listen.
    /// For a simple example, check the Github Repository at this adress :
    /// https://github.com/Jamy4000/UnityCallbackAndEventTutorial
    /// </summary>
    /// <typeparam name="T">The new Event you've created and that extend this class</typeparam>
    public class Event<T> where T : Event<T>
    {
        /// <summary>
        /// The Description of the Event, what it's supposed to do
        /// </summary>
        public string Description;

        /// <summary>
        /// The delegate for the Event Listeners, create as _listeners variable
        /// </summary>
        /// <param name="info">Allow us to have the informations on the event that was fired</param>
        public delegate void EventListener(T info);

        /// <summary>
        /// The event, working kind of like a list, that contains all the methods to call when the event is fired
        /// </summary>
        public static event EventListener Listeners;

        /// <summary>
        /// Base constructor for the Event class
        /// </summary>
        /// <param name="description">The description of this event, copied in the Description variable</param>
        public Event(string description)
        {
            Description = description;
        }

        /// <summary>
        /// Static method to register a method that listen to this event.
        /// You need to add the type of the Event as a parameter of your method to be able to register it as a listener.
        /// </summary>
        /// <param name="listener">The method that need to be added to the listeners</param>
        public static void RegisterListener(EventListener listener)
        {
            Listeners += listener;
        }

        /// <summary>
        /// Static method to unregister a method that was listening to this event.
        /// You need to add the type of the Event as a parameter of your method to be able to register it as a listener.
        /// </summary>
        /// <param name="listener">The method that need to be removed from the listeners</param>
        public static void UnregisterListener(EventListener listener)
        {
            Listeners -= listener;
        }

        /// <summary>
        /// Fire the event. You can put it in the constructor of your Event, or simply call it when you need it.
        /// </summary>
        /// <param name="info">The reference to the event. Can be FireEvent(this) if you call it from the Constructor of your event.</param>
        public void FireEvent(T info)
        {
            Listeners?.Invoke(info);
        }
    }
}