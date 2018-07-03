using System;
using UnityEngine.Events;

namespace ScriptableFramework.Events.UnityEvents
{
    [Serializable]
    public class EventBase<T> : UnityEvent<T>
    {
    }
}
