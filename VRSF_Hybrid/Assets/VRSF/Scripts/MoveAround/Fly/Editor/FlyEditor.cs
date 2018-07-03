using UnityEditor;
using VRSF.Utils.Editor;

namespace VRSF.MoveAround.Editor
{
    /// <summary>
    /// Just add a message to the user to let him know where the parameters are set.
    /// </summary>
    [CustomEditor(typeof(Fly))]
    public class FlyEditor : ButtonActionChoserEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("The Flying Parameters are set via Scriptable Objects. To modify them, please open Window/VRSF/VR Flying Parameters.", MessageType.Info);
        }
    }
}