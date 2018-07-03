using UnityEngine;
using UnityEngine.Events;

namespace ScriptableFramework.Events
{
    public class GameEventListener : GameEventListenerBase
    {
        public GameEvent Event;
        public UnityEvent Response;

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

        public void OnEventRaised()
        {
            LastFired = Time.time;
            SetTriggered();
            OnEvent(null);
            Response.Invoke();
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