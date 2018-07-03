using ScriptableFramework.Events;
using UnityEditor;
using UnityEngine;

namespace ScriptableFramework.UI.Editor
{
    [CustomEditor(typeof(GameEventVector3))]
    public class GameEventVector3Editor : GameEventEditorBase
    {
        private Vector3? vSource;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (vSource != null)
            {
                vSource = EditorGUILayout.Vector3Field("Position to send", vSource.Value);
            }
            else
            {
                vSource = EditorGUILayout.Vector3Field("Position to send", Vector3.zero);
            }
            var e = target as GameEventVector3;

            if (GUILayout.Button("Raise") && vSource != null)
            {
                e.Raise(vSource.Value);
            }
            
            DrawListeners();
        }
    }
}