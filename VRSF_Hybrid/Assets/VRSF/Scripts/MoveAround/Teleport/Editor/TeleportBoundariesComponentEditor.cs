using UnityEditor;
using UnityEngine;
using VRSF.MoveAround.Teleport.Components;

namespace VRSF.MoveAround.Teleport.Editor
{
    [CustomEditor(typeof(TeleportBoundariesComponent))]
    public class TeleportBoundariesComponentEditor : UnityEditor.Editor
    {
        private TeleportBoundariesComponent _boundsComp;

        private void OnEnable()
        {
            _boundsComp = (TeleportBoundariesComponent)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            base.OnInspectorGUI();
            
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            _boundsComp._UseBoundaries = EditorGUILayout.ToggleLeft("Use Boundaries for Teleport", _boundsComp._UseBoundaries);

            if (_boundsComp._UseBoundaries)
            {
                EditorGUILayout.Space();

                for (int i = 0; i < _boundsComp.Boundaries().Count; i++)
                {
                    _boundsComp._Boundaries[i] = EditorGUILayout.BoundsField("Bound " + i, _boundsComp._Boundaries[i]);
                    EditorGUILayout.Space();
                }

                if (GUILayout.Button("Add New Bounds"))
                {
                    Bounds bound = new Bounds
                    {
                        center = Vector3.zero,
                        size = Vector3.one,
                    };
                    _boundsComp._Boundaries.Add(bound);
                }

                EditorGUILayout.Space();

                if (_boundsComp.Boundaries().Count > 0)
                {
                    if (GUILayout.Button("Remove Last Bounds"))
                    {
                        Bounds bound = new Bounds
                        {
                            center = Vector3.zero,
                            size = Vector3.zero,
                            min = Vector3.zero,
                            max = Vector3.zero,
                            extents = Vector3.zero
                        };

                        _boundsComp._Boundaries.RemoveAt(_boundsComp.Boundaries().Count - 1);
                    }
                }

                Undo.RecordObject(this, "boundariesComp");
            }

            EditorUtility.SetDirty(_boundsComp);

            serializedObject.ApplyModifiedProperties();
        }
    }
}