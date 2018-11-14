using UnityEngine;

namespace ScriptableFramework.Variables
{
    [CreateAssetMenu(menuName = "Variables/RaycastHit")]
    public class RaycastHitVariable : VariableBase<RaycastHit>
    {
        public bool IsNull = true;

        public void SetIsNull(bool value)
        {
            IsNull = value;
        }
    }
}