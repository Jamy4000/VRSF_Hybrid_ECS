using System;
using UnityEngine;
using UnityEngine.Events;

namespace ScriptableFramework.Events.UnityEvents
{
    [Serializable]
    public class EventTransform: UnityEvent<Transform>
    {
    }
}
