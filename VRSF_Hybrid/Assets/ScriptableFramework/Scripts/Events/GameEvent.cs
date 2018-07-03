using UnityEngine;

namespace ScriptableFramework.Events
{
    [CreateAssetMenu(menuName = "Events/Event()")]
    public class GameEvent : GameEventBase
    {
        public void Raise()
        {
            Fired();
            for (int i = Listeners.Count - 1; i >= 0; i--)
            {
                ((GameEventListener)Listeners[i]).OnEventRaised();
            }
        }

        public void RegisterListener(GameEventListener listener)
        {
            Listeners.Add(listener);
        }

        public void UnregisterListener(GameEventListener listener)
        {
            Listeners.Remove(listener);
        }
    }
}
