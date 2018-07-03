using ScriptableFramework.Events;
using UnityEngine;

namespace ScriptableFramework.Variables
{
    [CreateAssetMenu(menuName = "Variables/GameEventListener<Object>")]
    public class GameEventListenerObjectVariable : ScriptableObject
    {
        [SerializeField]
        private GameEventListenerObject _value;

        public GameEventListenerObject Value
        {
            get { return _value; }
            set { _value = value; }
        }
    }
}