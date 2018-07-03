//using System.Collections.Generic;
//using System.Globalization;
//using Framework.Events;
//using Framework.Utils;
//using UnityEditor;
//using UnityEngine;
//using UnityEngine.WSA;

//namespace Framework.UI.Editor
//{
//    public class TableBase : ScriptableObject
//    {
//        private static EventInspectorStandIn _standIn;

//        Vector2 _scrollPos = Vector2.zero;
//        private List<GUILayoutOption[]> _layouts;
//        private List<GUIStyle> _styles;
//        private Texture2D _oddBackground;
//        private GUIStyle _oddRow;
//        private GUIStyle _evenRow;
//        private float _queueSize;
//        private Dictionary<string, Object> _cachedTypes = new Dictionary<string, Object>();
//        private List<GUILayoutOption[]> _headerLayouts;
//        private List<GUIStyle> _headerStyles;
//        private List<GUILayoutOption[]> _oddRowLayoutOptions;
//        private List<GUIStyle> _oddRowHeaderStyles;
//        private List<GUILayoutOption[]> _evenLayoutOptions;
//        private List<GUIStyle> _oddEvenHeaderStyles;

//        [MenuItem("Window/Event Inspector")]
//        public static void ShowWindow()
//        {
//           // var window = GetWindow(typeof(EventInspector));
//            //window.titleContent = new GUIContent("Event Inspector");
            
//            //window.ShowTab();
//        }

//        public void SetHeaderLayoutOptions(List<GUILayoutOption[]> layoutOptions)
//        {
//            _headerLayouts = layoutOptions;
//        }

//        public void SetHeaderStyles(List<GUIStyle> styles)
//        {
//            _headerStyles = styles;
//        }

//        public void SetOddRowLayoutOptions(List<GUILayoutOption[]> layoutOptions)
//        {
//            _oddRowLayoutOptions = layoutOptions;
//        }

//        public void SetOddRowStyles(List<GUIStyle> styles)
//        {
//            _oddRowHeaderStyles = styles;
//        }
//        public void SetEvenRowLayoutOptions(List<GUILayoutOption[]> layoutOptions)
//        {
//            _evenLayoutOptions = layoutOptions;
//        }

//        public void SetEvenRowStyles(List<GUIStyle> styles)
//        {
//            _oddEvenHeaderStyles = styles;
//        }


//        void OnEnable()
//        {
//            _queueSize = 100;
//        }
        
//        //void DrawLine(EventDescription eventDescription, GUIStyle guiStyle)
//        //{
//        //    EditorGUILayout.BeginHorizontal();
//        //    EditorGUILayout.LabelField(eventDescription.Time.ToString(CultureInfo.InvariantCulture), _layouts[0]);
//        //    EditorGUILayout.ObjectField("", GetEvent(eventDescription), typeof(GameEventBase), true, _layouts[1]);
//        //    var caller = GetCaller(eventDescription);
//        //    EditorGUILayout.ObjectField("", caller, caller.GetType(), true, _layouts[2]);
//        //    EditorGUILayout.BeginVertical(_layouts[3]);
//        //    for (int i = 0; i < eventDescription.Response.GetPersistentEventCount(); i++)
//        //    {
//        //        var callee = GetCallee(eventDescription, i);

//        //        EditorGUILayout.ObjectField("", callee, callee.GetType(), true, _layouts[3]);
//        //    }
//        //    EditorGUILayout.EndVertical();
//        //    // EditorGUILayout.LabelField(GetParameter(eventDescription), _layouts[4]);
//        //    var param = eventDescription.Parameter as GameObject;
//        //    if (param != null)
//        //    {
//        //        EditorGUILayout.ObjectField("", param, caller.GetType(), true, _layouts[4]);
//        //    }
//        //    else
//        //    {
//        //        EditorGUILayout.LabelField(GetParameter(eventDescription), _layouts[4]);
//        //    }
            
//        //    EditorGUILayout.EndHorizontal();
//        //}

//        void DrawHeader()
//        {
//            EditorGUILayout.BeginHorizontal();
//            EditorGUILayout.LabelField("Time", _layouts[0]);
//            EditorGUILayout.LabelField("Event name", _layouts[1]);
//            EditorGUILayout.LabelField("Caller", _layouts[2]);
//            EditorGUILayout.LabelField("Callee", _layouts[3]);
//            EditorGUILayout.LabelField("Parameter", _layouts[4]);
//            EditorGUILayout.EndHorizontal();
//        }

//        void CheckTraceEvents()
//        {
//            EditorGUILayout.BeginHorizontal();
//            GlobalConfig.GetInstance().TraceEvents = EditorGUILayout.Toggle("Trace Events", GlobalConfig.GetInstance().TraceEvents);
//            GlobalConfig.GetInstance().TraceEventCaller = EditorGUILayout.Toggle("Trace Event Callers", GlobalConfig.GetInstance().TraceEventCaller);
//            GlobalConfig.GetInstance().LogTrace = EditorGUILayout.Toggle("Log Trace", GlobalConfig.GetInstance().LogTrace);
//            EditorGUILayout.EndHorizontal();
//        }

//        void LogTrace()
//        {
            
//        }

//        void OnGUI()
//        {
//            if (_standIn == null)
//            {
//                CreateRowStyles();
//            }
            
////            SetLayout();

//            EditorGUILayout.BeginVertical();
//            CheckTraceEvents();
//            _queueSize = EditorGUILayout.Slider("Queue size", _queueSize, 10, 1000);
//            EditorGUILayout.EndVertical();

//            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
//            EditorGUILayout.BeginVertical();
//            DrawHeader();

//            if (_standIn.Events.Count > _queueSize)
//            {
//                _standIn.Events.RemoveRange(0, _standIn.Events.Count - (int)_queueSize);
//            }

//            for (var index = 0; index < _standIn.Events.Count; index++)
//            {
//                var e = _standIn.Events[index];
//       //         DrawEventDescription(e, index % 2 == 0 ? _evenRow : _oddRow);
//            }
//            EditorGUILayout.EndVertical();
//            EditorGUILayout.EndScrollView();
            
//        }

//        private void CreateRowStyles()
//        {
//            _oddRow = new GUIStyle
//            {
//                normal = new GUIStyleState
//                {
//                    background = _oddBackground
//                }
//            };
//            _evenRow = new GUIStyle();
//        }

//        //private void SetLayout()
//        //{
//        //    _layouts = new List<GUILayoutOption[]>
//        //    {
//        //        new [] {GUILayout.Width(50)},
//        //        new [] {GUILayout.Width(200)},
//        //        new [] {GUILayout.Width((position.width - 500) / 2) },
//        //        new [] {GUILayout.Width(200) },
//        //        new [] {GUILayout.Width((position.width - 500) / 2) }};
//        //}

//        public void OnEvent()
//        {
//            if (GlobalConfig.GetInstance().LogTrace)
//            {
//                Log.Write(_standIn.LastEventDescription.ToString(), false);
//            }
//            //Repaint();
//        }
//    }
//}



////void SetLayout()
////{
//////var maxEvent = 0f;
//////var maxCaller = 0f;
//////var maxCallee = 0f;
//////var maxParameter = 0f;
//////foreach (var e in _standIn.Events)
//////{
//////    var curWidth = GetColWidth(GetEventName(e));
//////    maxEvent = curWidth > maxEvent ? curWidth : maxEvent;

//////    curWidth = GetColWidth(GetCaller(e));
//////    maxCaller = curWidth > maxCaller ? curWidth : maxCaller;

//////    for (int i = 0; i < e.Response.GetPersistentEventCount(); i++)
//////    {
//////        curWidth = GetColWidth(GetCallee(e, i));
//////        maxCallee = curWidth > maxCallee ? curWidth : maxCallee;
//////    }

//////    curWidth = GetColWidth(GetParameter(e));
//////    maxParameter = curWidth > maxParameter ? curWidth : maxParameter;
//////}
//////_layouts = new List<GUILayoutOption[]>
//////{
//////    new [] {GUILayout.Width(maxEvent * 2)},
//////    new [] {GUILayout.Width(maxCaller)},
//////    new [] {GUILayout.Width(maxCallee * 2)},
//////    new [] {GUILayout.Width(maxParameter)}
//////};

////_layouts = new List<GUILayoutOption[]>
////{
////    new [] {GUILayout.Width(200)},
////    new [] {GUILayout.Width(200)},
////    new [] {GUILayout.Width(200)},
////    new [] {GUILayout.Width(200)}
////};
////}

//////float GetColWidth(string s)
//////{
//////    GUIContent content = new GUIContent(s);

//////    GUIStyle style = GUI.skin.box;
//////    style.alignment = TextAnchor.MiddleCenter;

//////    return style.CalcSize(content).x;
//////}