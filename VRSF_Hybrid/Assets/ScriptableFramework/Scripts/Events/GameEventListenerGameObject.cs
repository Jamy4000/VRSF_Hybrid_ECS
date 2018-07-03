using ScriptableFramework.Events.UnityEvents;
using UnityEngine;
using UnityEngine.Events;

namespace ScriptableFramework.Events
{
    public class GameEventListenerGameObject : GameEventListenerBase
    {
        public GameEventGameObject Event;
        public EventGameObject Response;
        
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

        public void OnEventRaised(GameObject value)
        {
            LastFired = Time.time;
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