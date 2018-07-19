using UnityEditor;
using VRSF.MoveAround.Teleport.Components;

namespace VRSF.MoveAround.Teleport.Editor
{
    /// <summary>
    /// Just add a message to the user to let him know where the parameters are set.
    /// </summary>
    [CustomEditor(typeof(StepByStepComponent))]
    public class StepByStepTeleportEditor : UnityEditor.Editor
    {
        private StepByStepComponent _sbsComp;

        private void OnEnable()
        {
            _sbsComp = (StepByStepComponent)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("You only need to set the parameters for Component on this entity to use the Step by Step feature.\n" +
                "The response to the button is automatically handled in the scripts.", MessageType.Info);
        }
    }
}