using System.Collections.Generic;
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
            _boundsComp = target as TeleportBoundariesComponent;
        }

        public void OnSceneGUI()
        {
            if (_boundsComp.UseBoundaries() && _boundsComp.Boundaries().Count > 0)
            {
                for (int i = 0; i < _boundsComp.Boundaries().Count; i++)
                {
                    DrawCube(i);
                    DrawCubeName(_boundsComp.Boundaries()[i], i);
                }
            }
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
                EditorGUILayout.BeginHorizontal();

                var oldLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 200f;

                EditorGUILayout.PrefixLabel("Boundaries Color (Editor Only)");
                _boundsComp._BoundariesColor = EditorGUILayout.ColorField("", _boundsComp._BoundariesColor, GUILayout.MaxWidth(75f));

                EditorGUIUtility.labelWidth = oldLabelWidth;

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();

                EditorGUILayout.HelpBox("The Values for the Center Field of the Bounds are in World Position.", MessageType.Info);

                EditorGUILayout.Space();

                for (int i = 0; i < _boundsComp.Boundaries().Count; i++)
                {
                    EditorGUILayout.LabelField("Bound " + (i + 1) + " Parameters", EditorStyles.boldLabel);
                    _boundsComp._Boundaries[i] = EditorGUILayout.BoundsField("", _boundsComp._Boundaries[i]);
                    
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

        private void DrawCube(int i)
        {
            var bound = _boundsComp.Boundaries()[i];
            Vector3 minPos = bound.min;
            Vector3 maxPos = bound.max;
            Vector3 center = bound.center;

            // List of points/vertices, wit the position calculated with the rotation factor
            Vector3[] vertices = new Vector3[8]
            {
                new Vector3(minPos.x, minPos.y, minPos.z),
                new Vector3(minPos.x, maxPos.y, minPos.z),
                new Vector3(minPos.x, maxPos.y, maxPos.z),
                new Vector3(minPos.x, minPos.y, maxPos.z),
                new Vector3(maxPos.x, minPos.y, minPos.z),
                new Vector3(maxPos.x, maxPos.y, minPos.z),
                new Vector3(maxPos.x, maxPos.y, maxPos.z),
                new Vector3(maxPos.x, minPos.y, maxPos.z),
            };

            // List of points/vertices
            List<Vector3[]> polygones = GetPolygones(vertices);

            // List of indices/Vector2 between which a line must be made
            Vector2[] indices = new Vector2[12]
            {
                new Vector2(0, 1),
                new Vector2(1, 2),
                new Vector2(2, 3),
                new Vector2(3, 0),

                new Vector2(4, 5),
                new Vector2(5, 6),
                new Vector2(6, 7),
                new Vector2(7, 4),

                new Vector2(0, 4),
                new Vector2(1, 5),
                new Vector2(2, 6),
                new Vector2(3, 7),
            };

            Handles.color = _boundsComp._BoundariesColor;

            // Loop through each indices to draw polygones between the corners
            foreach (Vector3[] polygone in polygones)
            {
                Handles.DrawAAConvexPolygon(polygone);
            }

            var color = Handles.color;
            color.a = 1f;
            Handles.color = color;

            // Loop through each indices to connect the points together
            foreach (Vector2 indice in indices)
            {
                Vector3 start = new Vector3(vertices[(int)indice.x].x, vertices[(int)indice.x].y, vertices[(int)indice.x].z);
                Vector3 stop = new Vector3(vertices[(int)indice.y].x, vertices[(int)indice.y].y, vertices[(int)indice.y].z);
                Handles.DrawDottedLine(start, stop, 4f);
            }
        }

        private List<Vector3[]> GetPolygones(Vector3[] vertices)
        {
            Vector3[] firstFace = new Vector3[]
            {
                vertices[0],
                vertices[1],
                vertices[2],
                vertices[3],
            };

            Vector3[] secondFace = new Vector3[]
            {
                vertices[4],
                vertices[5],
                vertices[6],
                vertices[7],
            };

            Vector3[] thirdFace = new Vector3[]
            {
                vertices[0],
                vertices[1],
                vertices[5],
                vertices[4],
            };

            Vector3[] fourthFace = new Vector3[]
            {
                vertices[2],
                vertices[3],
                vertices[7],
                vertices[6],
            };

            Vector3[] fifthFace = new Vector3[]
            {
                vertices[1],
                vertices[2],
                vertices[6],
                vertices[5],
            };

            Vector3[] sixthFace = new Vector3[]
            {
                vertices[0],
                vertices[3],
                vertices[7],
                vertices[4],
            };

            return new List<Vector3[]>
            {
                firstFace,
                secondFace,
                thirdFace,
                fourthFace,
                fifthFace,
                sixthFace
            };
        }

        private void DrawCubeName(Bounds bound, int i)
        {
            Vector3 labelPos = bound.max;

            GUIStyle style = new GUIStyle();
            var newColor = _boundsComp._BoundariesColor;
            newColor.a = 1f;
            style.normal.textColor = newColor;

            Handles.Label(labelPos, _boundsComp.transform.name + " Boundaries " + (i + 1), style);
        }
    }
}