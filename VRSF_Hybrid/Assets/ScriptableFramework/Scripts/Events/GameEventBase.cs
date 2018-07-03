using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ScriptableFramework.Events
{
    public class GameEventBase : ScriptableObject
    {
        protected List<Object> Listeners = new List<Object>();

        public List<Object> GetListeners()
        {
            return Listeners;
        }

        protected void Fired()
        {
        }
    }
}
