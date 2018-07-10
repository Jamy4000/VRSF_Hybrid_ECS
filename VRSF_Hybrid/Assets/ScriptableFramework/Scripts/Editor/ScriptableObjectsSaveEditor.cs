using ScriptableFramework.Util.Components;
using UnityEditor;

namespace ScriptableFramework.Util.Editor
{
    [CustomEditor(typeof(ScriptableObjectSaveComponent))]
    public class ScriptableObjectsSaveEditor : UnityEditor.Editor
    {
        private ScriptableObjectSaveComponent _comp;

        public void OnEnable()
        {
            _comp = (ScriptableObjectSaveComponent)target;
        }

        public override void OnInspectorGUI()
        {
            base.DrawDefaultInspector();

            _comp.SaveScriptableInJSONOnAppQuit = EditorGUILayout.ToggleLeft("Save Scriptable Objects in JSON on App Quit", _comp.SaveScriptableInJSONOnAppQuit);

            _comp.RemoveUnusedJSONFiles = EditorGUILayout.ToggleLeft("Remove Unused JSON Files", _comp.RemoveUnusedJSONFiles);
        }
    }
}