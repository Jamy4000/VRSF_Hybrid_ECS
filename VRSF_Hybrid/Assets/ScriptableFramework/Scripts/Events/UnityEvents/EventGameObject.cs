using System;
using UnityEngine;
using UnityEngine.Events;

namespace ScriptableFramework.Events.UnityEvents
{
    [Serializable]
    public class EventGameObject: UnityEvent<GameObject>
    {
    }
}
