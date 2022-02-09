using UnityEngine;

namespace Common
{
    /// <summary>
    /// расширение для удобства, чтобы можно было искать компоненты и сразу получить bool
    /// </summary>
    public static class ComponentExtensions
    {
        public static bool TryGetComponentInChildren<T>(this Component parentComponent, out T component, bool includeInactive = false)
            where T : class
        {
            return TryGetComponentInChildren(parentComponent.gameObject, out component, includeInactive);
        }

        public static bool TryGetComponentsInChildren<T>(this Component parentComponent, out T[] components, bool includeInactive = false)
            where T : class
        {
            return TryGetComponentsInChildren(parentComponent.gameObject, out components, includeInactive);
        }

        public static bool TryGetComponentInChildren<T>(this GameObject gameObject, out T component, bool includeInactive = false)
            where T : class
        {
            if (gameObject != null)
            {
                component = gameObject.GetComponentInChildren<T>(includeInactive);
                return component != null;
            }

            component = null;
            return false;
        }

        public static bool TryGetComponentsInChildren<T>(this GameObject gameObject, out T[] components, bool includeInactive = false)
            where T : class
        {
            if (gameObject != null)
            {
                components = gameObject.GetComponentsInChildren<T>(includeInactive);
                return components.Length != 0;
            }

            components = null;
            return false;
        }
    }
}