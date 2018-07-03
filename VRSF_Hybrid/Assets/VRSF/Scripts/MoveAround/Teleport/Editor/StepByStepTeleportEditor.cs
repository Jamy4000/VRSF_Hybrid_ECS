using UnityEditor;
using VRSF.Utils.Editor;

namespace VRSF.MoveAround.Teleport.Editor
{
    /// <summary>
    /// Just add a message to the user to let him know where the parameters are set.
    /// </summary>
    [CustomEditor(typeof(StepByStepTeleport))]
    public class StepByStepTeleportEditor : ButtonActionChoserEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("The parameters of the Step by Step Teleport Scripts are set via Scriptable Objects. To modify them, please open Window/VRSF/Teleport Parameters.", MessageType.Info);
        }
    }
}