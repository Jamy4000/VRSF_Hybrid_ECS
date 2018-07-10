using ScriptableFramework.Util.Components;
using UnityEditor;

namespace ScriptableFramework.Util.Editor
{
    [CustomEditor(typeof(ScriptableObjectLoadComponent))]
    public class ScriptableObjectsLoadEditor : UnityEditor.Editor
    {
        private ScriptableObjectLoadComponent _comp;

        public void OnEnable()
        {
            _comp = (ScriptableObjectLoadComponent)target;
        }

        public override void OnInspectorGUI()
        {
            base.DrawDefaultInspector();

            _comp.LoadScriptableFromJSONOnAppStart = EditorGUILayout.ToggleLeft("Load Scriptable Objects from JSON on App Start", _comp.LoadScriptableFromJSONOnAppStart);
        }
    }
}