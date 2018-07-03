using System.Collections.Generic;
using System.Globalization;
using ScriptableFramework.Events;
using ScriptableFramework.Util;
using UnityEditor;
using UnityEngine;

namespace ScriptableFramework.UI.Editor
{
    public class EventInspector : EditorWindow
    {
        private static EventInspectorStandIn _standIn;

        Vector2 _scrollPos = Vector2.zero;
        private List<GUILayoutOption[]> _layouts;
        private List<GUIStyle> _styles;
        private Texture2D _oddBackground;
        private GUIStyle _oddRow;
        private GUIStyle _evenRow;
        private float _queueSize;
        private Dictionary<string, Object> _cachedTypes = new Dictionary<string, Object>();

        public bool TraceEvents;
        [Tooltip("This is what actually makes the tracing slow, since we create stacktrace for each call.")]
        public bool TraceEventCaller;
        public bool LogTrace { get; set; }

        [MenuItem("Window/Event Inspector")]
        public static void ShowWindow()
        {
            var window = GetWindow(typeof(EventInspector));
            window.titleContent = new GUIContent("Event Inspector");
            
            window.ShowTab();
        }

        void OnEnable()
        {
            CreateBGTexture();
            _queueSize = 100;
        }

        void CreateBGTexture()
        {
            _oddBackground = new Texture2D(4, 4);
            for (var y = 0; y < _oddBackground.height; ++y)
            {
                for (var x = 0; x < _oddBackground.width; ++x)
                {
                    var color = new Color(128, 128, 128);
                    _oddBackground.SetPixel(x, y, color);
                }
            }
            _oddBackground.Apply();
        }

        void InitStandIn()
        {
            _standIn = FindObjectOfType<EventInspectorStandIn>();
            
            if (_standIn== null)
            {
                var go = Instantiate(UnityEngine.Resources.Load<GameObject>("Prefabs/EventInspectorSystem"));
                _standIn = go.GetComponent<EventInspectorStandIn>();
            }
            
            if (_standIn == null)
            {
                return;
            }
            _standIn.OnEvent = OnEvent;
        }

        GameEventBase GetEvent(EventDescription eventDescription)
        {
            return eventDescription.Event;
        }

        Object GetCaller(EventDescription eventDescription)
        {
            return GetScriptByName(eventDescription.CallerObject);
        }

        Object GetCallee(EventDescription eventDescription, int index)
        {
            return eventDescription.Response.GetPersistentTarget(index);
        }

        string GetParameter(EventDescription eventDescription)
        {
            return eventDescription.Parameter == null
                ? "NULL"
                : string.Format("{0} ", eventDescription.Parameter.ToString());
        }

        Object GetScriptByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            
            var parts = name.Split('.');
            name = parts[parts.Length - 1];
            if (_cachedTypes.ContainsKey(name))
            {
                return _cachedTypes[name];
            }
            foreach (var o in UnityEngine.Resources.FindObjectsOfTypeAll<Object>())
            {
                if (o.name == name)
                {
                    _cachedTypes.Add(name, o);
                    return o;
                }
            }
            return null;
        }

        void DrawEventDescription(EventDescription eventDescription, GUIStyle guiStyle)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(eventDescription.Time.ToString(CultureInfo.InvariantCulture), _layouts[0]);
            EditorGUILayout.ObjectField("", GetEvent(eventDescription), typeof(GameEventBase), true, _layouts[1]);
            var caller = GetCaller(eventDescription);
            if (caller != null)
            {
                EditorGUILayout.ObjectField("", caller, caller.GetType(), true, _layouts[2]);
            }
            else
            {
                EditorGUILayout.ObjectField("", null, null, true, _layouts[2]);
            }
            
            EditorGUILayout.BeginVertical(_layouts[3]);
            for (int i = 0; i < eventDescription.Response.GetPersistentEventCount(); i++)
            {
                var callee = GetCallee(eventDescription, i);

                EditorGUILayout.ObjectField("", callee, callee.GetType(), true, _layouts[3]);
            }
            EditorGUILayout.EndVertical();
            // EditorGUILayout.LabelField(GetParameter(eventDescription), _layouts[4]);
            var param = eventDescription.Parameter as GameObject;
            if (param != null)
            {
                EditorGUILayout.ObjectField("", param, param.GetType(), true, _layouts[4]);
            }
            else
            {
                EditorGUILayout.LabelField(GetParameter(eventDescription), _layouts[4]);
            }
            
            EditorGUILayout.EndHorizontal();
        }

        void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Time", _layouts[0]);
            EditorGUILayout.LabelField("Event name", _layouts[1]);
            EditorGUILayout.LabelField("Caller", _layouts[2]);
            EditorGUILayout.LabelField("Callee", _layouts[3]);
            EditorGUILayout.LabelField("Parameter", _layouts[4]);
            EditorGUILayout.EndHorizontal();
        }

        void CheckTraceEvents()
        {
            EditorGUILayout.BeginHorizontal();
            TraceEvents = EditorGUILayout.Toggle("Trace Events", TraceEvents);
            TraceEventCaller = EditorGUILayout.Toggle("Trace Event Callers", TraceEventCaller);
            LogTrace = EditorGUILayout.Toggle("Log Trace", LogTrace);
            EditorGUILayout.EndHorizontal();
        }

        void OnGUI()
        {
            if (_standIn == null)
            {
                InitStandIn();
                CreateRowStyles();
            }
            
            SetLayout();

            EditorGUILayout.BeginVertical();
            CheckTraceEvents();
            _queueSize = EditorGUILayout.Slider("Queue size", _queueSize, 10, 1000);
            EditorGUILayout.EndVertical();

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            EditorGUILayout.BeginVertical();
            DrawHeader();

            if (_standIn.Events.Count > _queueSize)
            {
                _standIn.Events.RemoveRange(0, _standIn.Events.Count - (int)_queueSize);
            }

            for (var index = 0; index < _standIn.Events.Count; index++)
            {
                var e = _standIn.Events[index];
                DrawEventDescription(e, index % 2 == 0 ? _evenRow : _oddRow);
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
            
        }

        private void CreateRowStyles()
        {
            _oddRow = new GUIStyle
            {
                normal = new GUIStyleState
                {
                    background = _oddBackground
                }
            };
            _evenRow = new GUIStyle();
        }

        private void SetLayout()
        {
            _layouts = new List<GUILayoutOption[]>
            {
                new [] {GUILayout.Width(50)},
                new [] {GUILayout.Width(200)},
                new [] {GUILayout.Width((position.width - 500) / 2) },
                new [] {GUILayout.Width(200) },
                new [] {GUILayout.Width((position.width - 500) / 2) }};
        }

        public void OnEvent()
        {
            if (LogTrace)
            {
                Log.Write(_standIn.LastEventDescription.ToString(), false);
            }
            Repaint();
        }
    }
}



//void SetLayout()
//{
////var maxEvent = 0f;
////var maxCaller = 0f;
////var maxCallee = 0f;
////var maxParameter = 0f;
////foreach (var e in _standIn.Events)
////{
////    var curWidth = GetColWidth(GetEventName(e));
////    maxEvent = curWidth > maxEvent ? curWidth : maxEvent;

////    curWidth = GetColWidth(GetCaller(e));
////    maxCaller = curWidth > maxCaller ? curWidth : maxCaller;

////    for (int i = 0; i < e.Response.GetPersistentEventCount(); i++)
////    {
////        curWidth = GetColWidth(GetCallee(e, i));
////        maxCallee = curWidth > maxCallee ? curWidth : maxCallee;
////    }

////    curWidth = GetColWidth(GetParameter(e));
////    maxParameter = curWidth > maxParameter ? curWidth : maxParameter;
////}
////_layouts = new List<GUILayoutOption[]>
////{
////    new [] {GUILayout.Width(maxEvent * 2)},
////    new [] {GUILayout.Width(maxCaller)},
////    new [] {GUILayout.Width(maxCallee * 2)},
////    new [] {GUILayout.Width(maxParameter)}
////};

//_layouts = new List<GUILayoutOption[]>
//{
//    new [] {GUILayout.Width(200)},
//    new [] {GUILayout.Width(200)},
//    new [] {GUILayout.Width(200)},
//    new [] {GUILayout.Width(200)}
//};
//}

////float GetColWidth(string s)
////{
////    GUIContent content = new GUIContent(s);

////    GUIStyle style = GUI.skin.box;
////    style.alignment = TextAnchor.MiddleCenter;

////    return style.CalcSize(content).x;
////}