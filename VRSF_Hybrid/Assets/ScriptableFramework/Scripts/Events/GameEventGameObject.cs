using UnityEngine;

namespace ScriptableFramework.Events
{
    [CreateAssetMenu(menuName = "Events/Event(GameObject)")]
    public class GameEventGameObject : GameEventBase
    {
        public void Raise(GameObject value)
        {
            Fired();
            for (int i = Listeners.Count - 1; i >= 0; i--)
            {
                ((GameEventListenerGameObject)Listeners[i]).OnEventRaised(value);
            }
        }

        public void RegisterListener(GameEventListenerGameObject listener)
        {
            Listeners.Add(listener);
        }

        public void UnregisterListener(GameEventListenerGameObject listener)
        {
            Listeners.Remove(listener);
        }
    }
}
