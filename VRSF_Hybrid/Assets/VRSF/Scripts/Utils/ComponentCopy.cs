using UnityEngine;

namespace VRSF.Utils
{
    /// <summary>
    /// Static class letting us copy components form a GameObject to another
    /// </summary>
	public static class ComponentCopy
	{
        #region PUBLIC_METHODS
        /// <summary>
        /// Copy a component to a destination GameObject 
        /// </summary>
        /// <param name="original">The component to copy</param>
        /// <param name="destination">The GameObject in which we need to paste the Component</param>
        /// <returns>The Copied component</returns>
        public static Component CopyComponent(Component original, ref GameObject destination)
        {
            System.Type type = original.GetType();

            Component copy = null;
            if (!destination.GetComponent(type))
            {
                copy = destination.AddComponent(type);
            }

            // Copied fields, can be restricted with BindingFlags
            System.Reflection.FieldInfo[] fields = type.GetFields();
            foreach (System.Reflection.FieldInfo field in fields)
            {
                field.SetValue(copy, field.GetValue(original));
            }

            return copy;
        }


        /// <summary>
        /// Copy a Component in a more Generic way, using a Type instead of a Component
        /// </summary>
        /// <typeparam name="T">The type to copy</typeparam>
        /// <param name="original">The original Type to copy</param>
        /// <param name="destination">The GameObject that will contains the copied Type</param>
        /// <returns>The new copied Type as Component</returns>
        public static T CopyComponent<T>(T original, ref GameObject destination) where T : Component
        {
            System.Type type = original.GetType();

            Component copy = null;
            if (!destination.GetComponent(type))
            {
                copy = destination.AddComponent(type);
            }

            // Copied fields, can be restricted with BindingFlags
            System.Reflection.FieldInfo[] fields = type.GetFields();
            foreach (System.Reflection.FieldInfo field in fields)
            {
                field.SetValue(copy, field.GetValue(original));
            }

            // Copied Properties
            var props = type.GetProperties();
            foreach (var prop in props)
            {
                if (!prop.CanWrite) continue;
                prop.SetValue(copy, prop.GetValue(original, null), null);
            }

            return copy as T;
        }
        #endregion
    }
}