using UnityEngine;

namespace ScriptableFramework.Events
{
    [CreateAssetMenu(menuName = "Events/Event(object)")]
    public class GameEventObject : GameEventBase
    {
        public void Raise(object value)
        {
            Fired();
            for (int i = Listeners.Count - 1; i >= 0; i--)
            {
                ((GameEventListenerObject)Listeners[i]).OnEventRaised(value);
            }
        }

        public void RegisterListener(GameEventListenerObject listener)
        {
            Listeners.Add(listener);
        }

        public void UnregisterListener(GameEventListenerObject listener)
        {
            Listeners.Remove(listener);
        }
    }
}
