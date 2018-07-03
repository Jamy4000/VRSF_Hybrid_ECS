namespace ScriptableFramework.Variables
{
    public class ReferenceBase<TTReal, TTBoxed> where TTBoxed : VariableBase<TTReal>
    {
        public bool UseConstant = false;
        public TTReal ConstantValue;
        public TTBoxed Variable;

        public ReferenceBase()
        { }

        public ReferenceBase(TTReal value)
        {
            UseConstant = true;
            ConstantValue = value;
        }

        public TTReal Value
        {
            get
            {
                return UseConstant ? ConstantValue : Variable.Value;
            }
        }

        public static implicit operator TTReal(ReferenceBase<TTReal, TTBoxed> reference)
        {
            return reference.Value;
        }
    }
}
