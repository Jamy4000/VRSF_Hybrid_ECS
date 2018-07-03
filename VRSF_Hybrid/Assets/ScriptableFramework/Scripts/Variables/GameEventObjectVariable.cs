using ScriptableFramework.Events;
using UnityEngine;

namespace ScriptableFramework.Variables
{
    [CreateAssetMenu(menuName = "Variables/GameEvent<Object>")]
    public class GameEventObjectVariable : ScriptableObject
    {
        [SerializeField]
        private GameEventObject value;

        public GameEventObject Value
        {
            get { return value; }
            set { this.value = value; }
        }
    }
}