using UnityEngine;

namespace ScriptableFramework.Events
{
    [CreateAssetMenu(menuName = "Events/Event(Transform)")]
    public class GameEventTransform : GameEventBase
    {
        public void Raise(Transform value)
        {
            Fired();
            for (int i = Listeners.Count - 1; i >= 0; i--)
            {
                if ((GameEventListenerTransform)Listeners[i] != null)
                    ((GameEventListenerTransform)Listeners[i]).OnEventRaised(value);
            }
        }
        public void RegisterListener(GameEventListenerTransform listener)
        {
            Listeners.Add(listener);
        }

        public void UnregisterListener(GameEventListenerTransform listener)
        {
            Listeners.Remove(listener);
        }
    }
}
