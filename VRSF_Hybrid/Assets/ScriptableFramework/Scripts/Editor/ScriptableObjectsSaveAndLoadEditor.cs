using ScriptableFramework.Util.Components;
using UnityEditor;

[CustomEditor(typeof(ScriptableObjectSaveAndLoadComponent))]
public class ScriptableObjectsSaveAndLoadEditor : Editor
{
    private ScriptableObjectSaveAndLoadComponent _comp;

    public void OnEnable()
    {
        _comp = (ScriptableObjectSaveAndLoadComponent)target;
    }

    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();

        _comp.LoadScriptableFromJSONOnAppStart = EditorGUILayout.ToggleLeft("Load Scriptable Objects from JSON on App Start", _comp.LoadScriptableFromJSONOnAppStart);

        _comp.SaveScriptableInJSONOnAppQuit = EditorGUILayout.ToggleLeft("Save Scriptable Objects in JSON on App Quit", _comp.LoadScriptableFromJSONOnAppStart);

        _comp.RemoveUnusedJSONFiles = EditorGUILayout.ToggleLeft("Remove Unused JSON Files", _comp.LoadScriptableFromJSONOnAppStart);
    }
}
