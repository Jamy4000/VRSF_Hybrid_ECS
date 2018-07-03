using UnityEngine;

namespace ScriptableFramework.Variables
{
    [CreateAssetMenu(menuName = "Variables/GameObject")]
    public class GameObjectVariable : ScriptableObject
    {
        [Multiline]
        public string DeveloperDescription = "";

        public GameObject Value;

        public void SetValue(GameObject value)
        {
            Value = value;
        }

        public void SetValue(VariableBase<GameObject> value)
        {
            Value = value.Value;
        }
    }
}