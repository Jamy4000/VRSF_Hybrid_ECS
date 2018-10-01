namespace EventCallbacks
{
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
        private static event EventListener _listeners;

        /// <summary>
        /// Security to avoid an event to be raised multiple times.
        /// If you want to raise the same event several times, you need to call the constructor each time
        /// the event must be raised.
        /// </summary>
        private bool _hasFired;

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
            _listeners += listener;
        }

        /// <summary>
        /// Static method to unregister a method that was listening to this event.
        /// You need to add the type of the Event as a parameter of your method to be able to register it as a listener.
        /// </summary>
        /// <param name="listener">The method that need to be removed from the listeners</param>
        public static void UnregisterListener(EventListener listener)
        {
            _listeners -= listener;
        }

        /// <summary>
        /// Fire the event. You can put it in the constructor of your Event, or simply call it when you need it.
        /// </summary>
        /// <param name="info">The reference to the event. Can be FireEvent(this) if you call it from the Constructor of your event.</param>
        public void FireEvent(T info)
        {
            if (_hasFired)
            {
                throw new EventException($"The event { this.GetType().ToString() } has already fired, to prevent infinite loops you can't refire an event");
            }
            else
            {
                _hasFired = true;
                _listeners?.Invoke(info);
            }
        }
    }
}