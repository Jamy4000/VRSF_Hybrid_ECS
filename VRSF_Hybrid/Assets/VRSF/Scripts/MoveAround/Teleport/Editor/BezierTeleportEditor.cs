using UnityEditor;
using VRSF.Utils.Editor;
using UnityEngine.UI;

namespace VRSF.MoveAround.Teleport.Editor
{
    /// <summary>
    /// Just add a message to the user to let him know where the parameters are set.
    /// </summary>
    [CustomEditor(typeof(BezierTeleporter))]
    public class BezierTeleportEditor : ButtonActionChoserEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("The parameters of the Bezier Teleporter Scripts are set via Scriptable Objects. To modify them, please open Window/VRSF/VR Teleport Parameters.", MessageType.Info);
        }
    }
}