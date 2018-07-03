using ScriptableFramework.Events.UnityEvents;
using UnityEngine;
using UnityEngine.Events;

namespace ScriptableFramework.Events
{
    public class GameEventListenerTransform : GameEventListenerBase
    {
        public GameEventTransform Event;
        public EventTransform Response;

        public Transform Value;

        public void OnEnable()
        {
            if (Event != null)
            {
                Event.RegisterListener(this);
            }
        }

        public void OnDisable()
        {
            if (Event != null)
            {
                Event.UnregisterListener(this);
            }
        }

        public void OnEventRaised(Transform value)
        {
            LastFired = Time.time;
            Value = value;

            SetTriggered();
            OnEvent(value);
            Response.Invoke(value);
        }

        public override GameEventBase GetEvent()
        {
            return Event;
        }

        public override UnityEventBase GetResponse()
        {
            return Response;
        }
    }
}