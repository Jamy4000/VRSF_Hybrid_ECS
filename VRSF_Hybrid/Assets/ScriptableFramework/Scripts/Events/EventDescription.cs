using UnityEngine.Events;

namespace ScriptableFramework.Events
{
    public class EventDescription
    {
        public GameEventBase Event;
        public UnityEventBase Response;
        public object Parameter;
        public string CallerObject { get; set; }
        public string CallerMethod { get; set; }
        public float Time { get; set; }
        public override string ToString()
        {
            return string.Format("{0} {1} {2}.{3} ({4})", Time, Event.name, CallerObject, CallerMethod,
                Parameter.GetType().Name);
        }
    }
}
