using System.Collections.Generic;
using System.Reflection;
using ScriptableFramework.Variables;
using UnityEditor;
using UnityEngine;

namespace ScriptableFramework.UI.Editor
{
    public class VariableSubscriber
    {
        public GameObject ConsumerObject;
        public MonoBehaviour Script;
        public object Variable;
        public PrefabType PrefabType;
    }
    public class VariableEditorBase : UnityEditor.Editor
    {
        private List<VariableSubscriber> _subscribers;

        public virtual VariableSubscriber GetVariableSubscriber(FieldInfo memberInfo, MonoBehaviour var, PrefabType prefabType)
        {
            return null;
        }
        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField(this.GetType().Name);

            DrawDefaultInspector();

            _subscribers = new List<VariableSubscriber>();
            foreach (var var in UnityEngine.Resources.FindObjectsOfTypeAll<MonoBehaviour>())
            {
                var prefabType = PrefabUtility.GetPrefabType(var);
                
                foreach (var fieldInfo in var.GetType().GetFields())
                {
                    var vs = GetVariableSubscriber(fieldInfo, var, prefabType);
                    if (vs != null)
                    {
                        _subscribers.Add(vs);
                    }
                }
            }

            var width = Screen.width;
            var gop = new[]
            {
                GUILayout.MaxWidth(width / 2f)
            };
            EditorGUILayout.BeginVertical();
            foreach (var subscriber in _subscribers)
            {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(subscriber.Variable.ToString().Split(' ')[0] + " (" + subscriber.PrefabType + ")");
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField("", subscriber.ConsumerObject, subscriber.ConsumerObject.GetType(), true, gop);
                EditorGUILayout.ObjectField("", subscriber.Script, typeof(MonoBehaviour), true, gop);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
        }
    }

    [CustomEditor(typeof(FloatVariable))]
    public class VariableFloatEditor : VariableEditorBase
    {
        public override VariableSubscriber GetVariableSubscriber(FieldInfo memberInfo, MonoBehaviour var, PrefabType prefabType)
        {
            var fr = memberInfo.GetValue(var) as FloatReference;
            
            if (fr != null && fr.Variable != null && fr.Variable.name == target.name)
            {
                var varSubscriber = new VariableSubscriber
                {
                    ConsumerObject = var.gameObject,
                    Script = var,
                    Variable = fr.Variable,
                    PrefabType = prefabType
                };
                return varSubscriber;
            }
            return null;
        }
    }
    [CustomEditor(typeof(BoolVariable))]
    public class VariableBoolEditor : VariableEditorBase
    {
        public override VariableSubscriber GetVariableSubscriber(FieldInfo memberInfo, MonoBehaviour var, PrefabType prefabType)
        {
            var fr = memberInfo.GetValue(var) as BoolReference;

            if (fr != null && fr.Variable != null && fr.Variable.name == target.name)
            {
                var varSubscriber = new VariableSubscriber
                {
                    ConsumerObject = var.gameObject,
                    Script = var,
                    Variable = fr.Variable,
                    PrefabType = prefabType
                };
                return varSubscriber;
            }
            return null;
        }
    }

    [CustomEditor(typeof(StringVariable))]
    public class VariableStringEditor : VariableEditorBase
    {
        public override VariableSubscriber GetVariableSubscriber(FieldInfo memberInfo, MonoBehaviour var, PrefabType prefabType)
        {
            var fr = memberInfo.GetValue(var) as StringReference;

            if (fr != null && fr.Variable != null && fr.Variable.name == target.name)
            {
                var varSubscriber = new VariableSubscriber
                {
                    ConsumerObject = var.gameObject,
                    Script = var,
                    Variable = fr.Variable,
                    PrefabType = prefabType
                };
                return varSubscriber;
            }
            return null;
        }
    }
    [CustomEditor(typeof(GameEventVariable))]
    public class VariableGameEventEditor : VariableEditorBase
    {
        public override VariableSubscriber GetVariableSubscriber(FieldInfo memberInfo, MonoBehaviour var, PrefabType prefabType)
        {
            var fr = memberInfo.GetValue(var) as GameEventReference;

            if (fr != null && fr.Variable != null && fr.Variable.name == target.name)
            {
                var varSubscriber = new VariableSubscriber
                {
                    ConsumerObject = var.gameObject,
                    Script = var,
                    Variable = fr.Variable,
                    PrefabType = prefabType
                };
                return varSubscriber;
            }
            return null;
        }
    }
    [CustomEditor(typeof(GameEventObjectVariable))]
    public class VariableGameEventObjectEditor : VariableEditorBase
    {
        public override VariableSubscriber GetVariableSubscriber(FieldInfo memberInfo, MonoBehaviour var, PrefabType prefabType)
        {
            var fr = memberInfo.GetValue(var) as GameEventObjectReference;

            if (fr != null && fr.Variable != null && fr.Variable.name == target.name)
            {
                var varSubscriber = new VariableSubscriber
                {
                    ConsumerObject = var.gameObject,
                    Script = var,
                    Variable = fr.Variable,
                    PrefabType = prefabType
                };
                return varSubscriber;
            }
            return null;
        }
    }
    [CustomEditor(typeof(TransformVariable))]
    public class VariableTransformEditor : VariableEditorBase
    {
        public override VariableSubscriber GetVariableSubscriber(FieldInfo memberInfo, MonoBehaviour var, PrefabType prefabType)
        {
            var fr = memberInfo.GetValue(var) as TransformReference;

            if (fr != null && fr.Variable != null && fr.Variable.name == target.name)
            {
                var varSubscriber = new VariableSubscriber
                {
                    ConsumerObject = var.gameObject,
                    Script = var,
                    Variable = fr.Variable,
                    PrefabType = prefabType
                };
                return varSubscriber;
            }
            return null;
        }
    }
    [CustomEditor(typeof(Vector3Variable))]
    public class VariableVector3Editor : VariableEditorBase
    {
        public override VariableSubscriber GetVariableSubscriber(FieldInfo memberInfo, MonoBehaviour var, PrefabType prefabType)
        {
            var fr = memberInfo.GetValue(var) as Vector3Reference;

            if (fr != null && fr.Variable != null && fr.Variable.name == target.name)
            {
                var varSubscriber = new VariableSubscriber
                {
                    ConsumerObject = var.gameObject,
                    Script = var,
                    Variable = fr.Variable,
                    PrefabType = prefabType
                };
                return varSubscriber;
            }
            return null;
        }
    }
    [CustomEditor(typeof(GameObjectVariable))]
    public class VariableGameObjectEditor : VariableEditorBase
    {
        public override VariableSubscriber GetVariableSubscriber(FieldInfo memberInfo, MonoBehaviour var, PrefabType prefabType)
        {
            var fr = memberInfo.GetValue(var) as GameObjectReference;

            if (fr != null && fr.Variable != null && fr.Variable.name == target.name)
            {
                var varSubscriber = new VariableSubscriber
                {
                    ConsumerObject = var.gameObject,
                    Script = var,
                    Variable = fr.Variable,
                    PrefabType = prefabType
                };
                return varSubscriber;
            }
            return null;
        }
    }

}
