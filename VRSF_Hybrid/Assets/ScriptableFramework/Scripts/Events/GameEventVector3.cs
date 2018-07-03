using UnityEngine;

namespace ScriptableFramework.Events
{
    [CreateAssetMenu(menuName = "Events/Event(Vector3)")]
    public class GameEventVector3 : GameEventBase
    {
        public void Raise(Vector3 value)
        {
            Fired();
            for (int i = Listeners.Count - 1; i >= 0; i--)
            {
                ((GameEventListenerVector3)Listeners[i]).OnEventRaised(value);
            }
        }

        public void RegisterListener(GameEventListenerVector3 listener)
        {
            Listeners.Add(listener);
        }

        public void UnregisterListener(GameEventListenerVector3 listener)
        {
            Listeners.Remove(listener);
        }
    }
}
