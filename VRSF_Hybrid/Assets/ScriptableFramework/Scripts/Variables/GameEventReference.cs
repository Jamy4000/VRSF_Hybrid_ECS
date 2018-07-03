using System;
using ScriptableFramework.Events;

namespace ScriptableFramework.Variables
{
    [Serializable]
    public class GameEventReference
    {
        public bool UseConstant = true;
        public GameEvent ConstantValue;
        public GameEventVariable Variable;

        public GameEventReference()
        { }

        public GameEventReference(GameEvent value)
        {
            UseConstant = true;
            ConstantValue = value;
        }

        public GameEvent Value
        {
            get { return UseConstant ? ConstantValue : Variable.Value; }
        }
    }
}