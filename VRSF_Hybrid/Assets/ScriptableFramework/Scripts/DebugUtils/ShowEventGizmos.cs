using System.Collections.Generic;
using ScriptableFramework.Events;
using UnityEngine;

namespace ScriptableFramework.DebugUtils
{
    public class ShowEventGizmos : MonoBehaviour {

        void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying)
            {
                return;
            }
            var listeners = FindObjectsOfType<GameEventListenerBase>();

            foreach (var listener in listeners)
            {
                var curEvent = listener.GetEvent();
                var curResponse = listener.GetResponse();
                var from = new List<Vector3>();
                var to = new List<Vector3>();

                if (curEvent == null)
                {
                    return;
                }

                foreach (var curListener in curEvent.GetListeners())
                {
                    var target = curListener as GameEventListenerBase;

                    if (target != null)
                    {
                        from.Add(target.transform.position);
                    }
                    else
                    {
                        UnityEngine.Debug.LogError(listener.GetType().Name);
                    }
                }
                if (curResponse == null)
                {
                    return;
                }
                for (var i = 0; i < curResponse.GetPersistentEventCount(); i++)
                {
                    var target  = curResponse.GetPersistentTarget(i) as MonoBehaviour;
                    if (target != null)
                    {
                        to.Add(target.transform.position);
                    }
                    else
                    {
                        var go = curResponse.GetPersistentTarget(i) as GameObject;
                        if (go != null)
                        {
                            UnityEngine.Debug.Log(go.name);
                        }
                        else
                        {
                            UnityEngine.Debug.Log(curResponse.GetPersistentTarget(i).GetType().Name);
                        }
                    }
                }
                foreach (var vFrom in from)
                {
                    foreach (var vTo in to)
                    {
                        Gizmos.DrawCube(vFrom, Vector3.one);
                        Gizmos.DrawCube(vTo, Vector3.one);
                        Gizmos.DrawLine(vFrom, vTo);
                    }
                }
            }
        }
    }
}
