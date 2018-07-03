using UnityEngine;

namespace ScriptableFramework.Variables
{
    [CreateAssetMenu(menuName = "Variables/RaycastHit")]
    public class RaycastHitVariable : VariableBase<RaycastHit>
    {
        public bool isNull = true;
    }
}