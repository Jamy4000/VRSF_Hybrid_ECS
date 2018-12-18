using UnityEngine;

namespace EventCallbacks
{
    /// <summary>
    /// Extension of the System.Exception class, allowing us to 
    /// catch this specific type of exception more easily and clearly. 
    /// </summary>
    public class EventException : System.Exception
    {
        /// <summary>
        /// Exception specific for the Event Callback System. 
        /// </summary>
        public EventException(string message) : base(message) { }
    }
}