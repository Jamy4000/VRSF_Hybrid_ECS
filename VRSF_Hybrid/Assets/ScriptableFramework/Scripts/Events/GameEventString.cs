using UnityEngine;

namespace ScriptableFramework.Events
{
    [CreateAssetMenu(menuName = "Events/Event(String)")]
    public class GameEventString: GameEventBase
    {
        public void Raise(string value)
        {
            Fired();
            for (int i = Listeners.Count - 1; i >= 0; i--)
            {
                ((GameEventListenerString)Listeners[i]).OnEventRaised(value);
            }
        }
        public void RegisterListener(GameEventListenerString listener)
        {
            Listeners.Add(listener);
        }

        public void UnregisterListener(GameEventListenerString listener)
        {
            Listeners.Remove(listener);
        }
    }
}
