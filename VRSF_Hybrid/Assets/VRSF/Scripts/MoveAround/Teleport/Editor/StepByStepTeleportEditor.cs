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

            _sbsComp._UseBoundaries = EditorGUILayout.ToggleLeft("Use Boundaries for Teleport", _sbsComp._UseBoundaries);

            if (_sbsComp._UseBoundaries)
            {
                _sbsComp._MinUserPosition = EditorGUILayout.Vector3Field("Minimum Position in Scene", _sbsComp._MinUserPosition);
                _sbsComp._MaxUserPosition = EditorGUILayout.Vector3Field("Maximum Position in Scene", _sbsComp._MaxUserPosition);
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("You only need to set the parameters for the StepByStepComponent and the buttonActionChoserComponents to be able to use the feature.\n" +
                "The response to the button is automatically handled in the scripts.", MessageType.Info);
        }
    }
}