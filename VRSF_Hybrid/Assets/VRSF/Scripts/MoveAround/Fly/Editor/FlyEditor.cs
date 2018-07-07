using UnityEditor;
using VRSF.MoveAround.Components;
using VRSF.Utils.Editor;

namespace VRSF.MoveAround.Editor
{
    /// <summary>
    /// Just add a message to the user to let him know where the parameters are set.
    /// </summary>
    [CustomEditor(typeof(FlyComponent))]
    public class FlyEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("To use the Fly System, you only need to set the parameters of the ButtonActionChoserComponents. The rest is already handle in the script.", MessageType.Info);
            EditorGUILayout.HelpBox("The Flying Parameters are set via Scriptable Objects. To modify them, please open Window/VRSF/VR Flying Parameters.", MessageType.Info);
        }
    }
}