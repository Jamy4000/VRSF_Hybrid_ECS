using UnityEngine;

namespace ScriptableFramework.Events
{
    [CreateAssetMenu(menuName = "Events/Event(Bool)")]
    public class GameEventBool : GameEventBase
    {
        public void Raise(bool value)
        {
            Fired();
            for (int i = Listeners.Count - 1; i >= 0; i--)
            {
                ((GameEventListenerBool)Listeners[i]).OnEventRaised(value);
            }
        }

        public void RegisterListener(GameEventListenerBool listener)
        {
            Listeners.Add(listener);
        }

        public void UnregisterListener(GameEventListenerBool listener)
        {
            Listeners.Remove(listener);
        }
    }
}
