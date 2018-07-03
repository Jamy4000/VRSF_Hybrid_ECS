using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;

namespace ScriptableFramework.Events
{
    /// <summary>
    /// WIP!
    /// </summary>
    public class GameEventListenerBase : MonoBehaviour
    {
        protected float LastFired = float.NegativeInfinity;
        private string _triggeredByObject;
        private string _triggeredByMethod;
        protected int FrameNum = 3;

        private EventInspectorStandIn _gameEventObject;

        public virtual GameEventBase GetEvent()
        {
            return null;
        }

        public virtual UnityEventBase GetResponse()
        {
            return null;
        }

        void SetEventSystem()
        {
            if (_gameEventObject == null)
            {
                _gameEventObject = FindObjectOfType<EventInspectorStandIn>();
            }
        }

        EventDescription GetEventDescription(object parameter)
        {
            SetEventSystem();
            
            return new EventDescription
            {
                Time = Time.time,
                Event = GetEvent(),
                Response = GetResponse(),
                Parameter = parameter,
                CallerObject = _triggeredByObject,
                CallerMethod = _triggeredByMethod
            };
        }

        protected void OnEvent(object value)
        {
            if (GetEvent().name == "OnEvent")
            {
                return;
            }
            SetEventSystem();
            _gameEventObject.OnEventListener(GetEventDescription(value));
        }

        protected void SetTriggered()
        {
            var mb = new StackTrace().GetFrame(FrameNum).GetMethod();
            _triggeredByMethod = mb.Name;

            if (mb.DeclaringType != null)
            {
                _triggeredByObject = mb.DeclaringType.ToString();
            }
            else
            {
                UnityEngine.Debug.LogWarning("No type " + mb);
            }
        }
    }
}
