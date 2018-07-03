using ScriptableFramework.Events.UnityEvents;
using UnityEngine;
using UnityEngine.Events;

namespace ScriptableFramework.Events
{
    public class GameEventListenerVector3 : GameEventListenerBase
    {
        public GameEventVector3 Event;
        public EventVector3 Response;
        public Vector3 vectorValue;

        private void OnEnable()
        {
            if (Event != null)
            {
                Event.RegisterListener(this);
            }
        }

        private void OnDisable()
        {
            if (Event != null)
            {
                Event.UnregisterListener(this);
            }
        }

        public void OnEventRaised(Vector3 value)
        {
            vectorValue = value;
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