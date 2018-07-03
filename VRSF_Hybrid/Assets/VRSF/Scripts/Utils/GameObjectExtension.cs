using System;
using UnityEngine;

namespace ScriptableFramework.Util
{
    /// <summary>
    /// Extension for the GameObject class to add a new Component on Init.
    /// </summary>
    public static class GameObjectExtension
    {
        /// <summary>
        /// Deactivate a gameObject
        /// </summary>
        /// <param name="gameObject">The GameObject to Deactivate on Init</param>
        /// <returns></returns>
        public static IDisposable Deactivate(this GameObject gameObject)
        {
            return new GameObjectDeactivateSection(gameObject);
        }

        /// <summary>
        /// Add a component on Init
        /// </summary>
        /// <typeparam name="T">The type of Component you want to add</typeparam>
        /// <param name="gameObject">The GameObject to which we wanna add the Component</param>
        /// <param name="onInit">Action to do on Init</param>
        /// <returns></returns>
        public static T AddComponentWithInit<T>(this GameObject gameObject, Action<T> onInit) where T : Component
        {
            using (gameObject.Deactivate())
            {
                T component = gameObject.AddComponent<T>();

                if (onInit != null)
                    onInit(component);

                return component;
            }
        }
    }

    /// <summary>
    /// Extension for the GameObject class to Deactivate and Dispose a GameObject
    /// </summary>
    public class GameObjectDeactivateSection : IDisposable
    {
        GameObject go;
        bool oldState;

        /// <summary>
        /// Dactivate a gameObject and keep it's previous state
        /// </summary>
        /// <param name="aGo"></param>
        public GameObjectDeactivateSection(GameObject aGo)
        {
            go = aGo;
            oldState = go.activeSelf;
            go.SetActive(false);
        }

        /// <summary>
        /// Set back the GameObject to its old State
        /// </summary>
        public void Dispose()
        {
            go.SetActive(oldState);
        }
    }
}