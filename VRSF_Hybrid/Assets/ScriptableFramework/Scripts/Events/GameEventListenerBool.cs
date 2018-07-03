using ScriptableFramework.Events.UnityEvents;
using UnityEngine;
using UnityEngine.Events;

namespace ScriptableFramework.Events
{
    public class GameEventListenerBool : GameEventListenerBase
    {
        public GameEventBool Event;
        public EventBool Response;
        
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

        public void OnEventRaised(bool value)
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